using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UECP;

namespace UecpSharpTests
{
	[TestClass]
	public class ProtocolTests
	{
		[TestMethod]
		public void ReferenceCrcCheckValueTest()
		{
			// Published check value for this CRC variant (CRC-16/GENIBUS)
			Assert.AreEqual((ushort)0xD64E, Crc16.Compute(Encoding.ASCII.GetBytes("123456789")));
		}

		[TestMethod]
		public void ReferenceCrcSpecExampleTest()
		{
			// SPB 490, appendix 1, example
			byte[] data = Encoding.ASCII.GetBytes("2D111234010105ABCD123F0XXXX11069212491000320066");

			Assert.AreEqual((ushort)0x9723, Crc16.Compute(data));
		}

		[TestMethod]
		public void FrameGoldenTest()
		{
			// PI example of SPB 490 3.3.1, sent to all encoders with sequence number 1
			UECPFrame frame = new UECPFrame();
			frame.SequenceCounter = 1;
			frame.MessageElements.Add(new MessageElement(MEC.RDS_PI, Hex.Parse("C2 01")));

			Hex.AssertEqual("FE 00 00 01 05 01 00 00 C2 01 D6 AB FF", frame.GetBytes());
		}

		[TestMethod]
		public void FrameEmptyTest()
		{
			// A frame without message is valid, it indicates an idle line
			UECPFrame frame = new UECPFrame();

			Hex.AssertEqual("FE 00 00 00 00 7B 3F FF", frame.GetBytes());
		}

		[TestMethod]
		public void FrameMultipleElementsTest()
		{
			UECPFrame frame = new UECPFrame();
			frame.SequenceCounter = 7;
			frame.MessageElements.Add(new MessageElement(MEC.RDS_PI, Hex.Parse("C2 01")));
			frame.MessageElements.Add(new MessageElement(MEC.RDS_PS, Encoding.ASCII.GetBytes("RADIO 1 ")));

			Hex.AssertEqual(
				"FE 00 00 07 10 01 00 00 C2 01 02 00 00 52 41 44 49 4F 20 31 20 53 5F FF",
				frame.GetBytes()
			);
		}

		[TestMethod]
		public void FrameSequenceCounterDefaultTest()
		{
			// 00 means that the sequence counter is not used
			Assert.AreEqual((byte)0x00, new UECPFrame().SequenceCounter);
		}

		[TestMethod]
		public void FrameSequenceCounterTest()
		{
			UECPFrame frame = new UECPFrame();
			frame.SequenceCounter = 0x2A;

			Assert.AreEqual((byte)0x2A, DecodedFrame.Parse(frame.GetBytes()).SequenceCounter);
		}

		[TestMethod]
		public void FrameGetBytesTwiceTest()
		{
			UECPFrame frame = new UECPFrame();
			frame.SequenceCounter = 0x2A;
			frame.MessageElements.Add(new MessageElement(MEC.RDS_PTY, Hex.Parse("08")));

			CollectionAssert.AreEqual(frame.GetBytes(), frame.GetBytes());
		}

		[TestMethod]
		public void FrameAddressTest()
		{
			// Site address in the 10 most significant bits, encoder address in the 6 least significant ones
			UECPFrame frame = new UECPFrame();
			frame.SiteAddress = 0x123;
			frame.EncoderAddress = 0x05;

			byte[] wire = frame.GetBytes();
			DecodedFrame decoded = DecodedFrame.Parse(wire);

			Hex.AssertEqual("FE 48 C5", wire.Take(3).ToArray());
			Assert.AreEqual(0x123, decoded.SiteAddress);
			Assert.AreEqual(0x05, decoded.EncoderAddress);
		}

		[TestMethod]
		public void FrameAddressMaxTest()
		{
			UECPFrame frame = new UECPFrame();
			frame.SiteAddress = 1023;
			frame.EncoderAddress = 63;

			byte[] wire = frame.GetBytes();
			DecodedFrame decoded = DecodedFrame.Parse(wire);

			// The address is FF FF, which has to be stuffed
			Hex.AssertEqual("FE FD 02 FD 02", wire.Take(5).ToArray());
			Assert.AreEqual(1023, decoded.SiteAddress);
			Assert.AreEqual(63, decoded.EncoderAddress);
		}

		[TestMethod]
		public void FrameSiteAddressOutOfRangeTest()
		{
			UECPFrame frame = new UECPFrame();
			frame.SiteAddress = 1024;

			Assert.ThrowsExactly<InvalidOperationException>(() => frame.GetBytes());
		}

		[TestMethod]
		public void FrameEncoderAddressOutOfRangeTest()
		{
			// The encoder address is 6 bits wide, 63 is the highest one
			UECPFrame frame = new UECPFrame();
			frame.EncoderAddress = 64;

			Assert.ThrowsExactly<InvalidOperationException>(() => frame.GetBytes());
		}

		[TestMethod]
		public void FrameStuffingTest()
		{
			UECPFrame frame = new UECPFrame();
			frame.MessageElements.Add(new MessageElement(MEC.FREE_FORMAT, Hex.Parse("FD FE FF 41")));

			byte[] wire = frame.GetBytes();

			// The message length is the one before stuffing
			Hex.AssertEqual("FE 00 00 00 05 24 FD 00 FD 01 FD 02 41", wire.Take(13).ToArray());
			Hex.AssertEqual("24 FD FE FF 41", DecodedFrame.Parse(wire).Message);
		}

		[TestMethod]
		[DataRow(0xFD, "FD 00")]
		[DataRow(0xFE, "FD 01")]
		[DataRow(0xFF, "FD 02")]
		public void FrameStuffingSequenceCounterTest(int sequenceCounter, string expected)
		{
			UECPFrame frame = new UECPFrame();
			frame.SequenceCounter = (byte)sequenceCounter;

			byte[] wire = frame.GetBytes();

			Hex.AssertEqual(expected, wire.Skip(3).Take(2).ToArray());
			Assert.AreEqual((byte)sequenceCounter, DecodedFrame.Parse(wire).SequenceCounter);
		}

		[TestMethod]
		[DataRow(253, "FD 00")]
		[DataRow(254, "FD 01")]
		[DataRow(255, "FD 02")]
		public void FrameStuffingMessageLengthTest(int messageLength, string expected)
		{
			UECPFrame frame = new UECPFrame();
			frame.MessageElements.Add(new MessageElement(MEC.FREE_FORMAT, new byte[messageLength - 1]));

			byte[] wire = frame.GetBytes();

			Hex.AssertEqual(expected, wire.Skip(4).Take(2).ToArray());
			Assert.HasCount(messageLength, DecodedFrame.Parse(wire).Message);
		}

		[TestMethod]
		public void FrameStuffingCrcTest()
		{
			// The CRC of this frame is 96 FE
			UECPFrame frame = new UECPFrame();
			frame.SequenceCounter = 1;
			frame.MessageElements.Add(new MessageElement(MEC.RDS_PI, Hex.Parse("00 02")));

			Hex.AssertEqual("FE 00 00 01 05 01 00 00 00 02 96 FD 01 FF", frame.GetBytes());
		}

		[TestMethod]
		public void FrameRandomRoundTripTest()
		{
			Random random = new Random(490);

			for (int i = 0; i < 500; i++)
			{
				byte[] data = new byte[random.Next(0, 255)];
				random.NextBytes(data);

				UECPFrame frame = new UECPFrame();
				frame.SiteAddress = (ushort)random.Next(0, 1024);
				frame.EncoderAddress = (ushort)random.Next(0, 64);
				frame.SequenceCounter = (byte)random.Next(0, 256);
				frame.MessageElements.Add(new MessageElement(MEC.FREE_FORMAT, data));

				// Parsing also checks start, stop, stuffing and message length
				DecodedFrame decoded = DecodedFrame.Parse(frame.GetBytes());

				Assert.IsTrue(decoded.CrcIsValid);
				Assert.AreEqual(frame.SiteAddress, decoded.SiteAddress);
				Assert.AreEqual(frame.EncoderAddress, decoded.EncoderAddress);
				Assert.AreEqual(frame.SequenceCounter, decoded.SequenceCounter);
				CollectionAssert.AreEqual(new byte[] { 0x24 }.Concat(data).ToArray(), decoded.Message);
			}
		}

		[TestMethod]
		public void FrameMessageTooLargeTest()
		{
			// Two elements of 128 bytes, one byte more than a frame can carry
			UECPFrame frame = new UECPFrame();
			frame.MessageElements.Add(new MessageElement(MEC.FREE_FORMAT, new byte[127]));
			frame.MessageElements.Add(new MessageElement(MEC.FREE_FORMAT, new byte[127]));

			Assert.ThrowsExactly<InvalidOperationException>(() => frame.GetBytes());
		}

		[TestMethod]
		public void ElementDataTooLargeTest()
		{
			MessageElement element = new MessageElement(MEC.FREE_FORMAT, new byte[255]);

			Assert.ThrowsExactly<InvalidOperationException>(() => element.GetBytes());
		}

		[TestMethod]
		public void ElementNullDataTest()
		{
			Assert.ThrowsExactly<ArgumentNullException>(() => new MessageElement(MEC.RDS_PI, null));

			// Data is a field, it can also be emptied afterwards
			MessageElement element = new MessageElement(MEC.RDS_PI, Hex.Parse("C2 01"));
			element.Data = null;

			Assert.ThrowsExactly<InvalidOperationException>(() => element.GetBytes());
		}

		[TestMethod]
		public void ElementDefaultsTest()
		{
			MessageElement element = new MessageElement();

			// Current data set, main programme service
			Assert.AreEqual((byte)0, element.DataSetNumber);
			Assert.AreEqual((byte)0, element.ProgramServiceNumber);
			Assert.IsEmpty(element.Data);
		}

		// Examples given for each message element by SPB 490 (version 5.1, and 6.02 for the ODA data ones).
		// 77 is passed as DSN or PSN to the elements that must not carry them.
		[TestMethod]
		[DataRow(MEC.RDS_PI, 0x00, 0x01, "C2 01", "01 00 01 C2 01")]
		[DataRow(MEC.RDS_PS, 0x00, 0x02, "52 41 44 49 4F 20 31 20", "02 00 02 52 41 44 49 4F 20 31 20")]
		[DataRow(MEC.RDS_PIN, 0x00, 0x06, "71 5E", "06 00 06 71 5E")]
		[DataRow(MEC.RDS_DI, 0x00, 0x03, "01", "04 00 03 01")]
		[DataRow(MEC.RDS_TA_TP, 0x00, 0x05, "02", "03 00 05 02")]
		[DataRow(MEC.RDS_MS, 0x00, 0x01, "01", "05 00 01 01")]
		[DataRow(MEC.RDS_PTY, 0x00, 0x05, "08", "07 00 05 08")]
		[DataRow(MEC.RDS_PTYN, 0x00, 0x02, "46 6F 6F 74 62 61 6C 6C", "3E 00 02 46 6F 6F 74 62 61 6C 6C")]
		[DataRow(MEC.RDS_RT, 0x00, 0x01, "0B 52 44 53", "0A 00 01 04 0B 52 44 53")]
		[DataRow(MEC.RDS_AF, 0x00, 0x01, "00 00 E2 15 27 CD 00", "13 00 01 07 00 00 E2 15 27 CD 00")]
		[DataRow(MEC.RDS_EON_AF, 0x00, 0x01, "00 00 05 15 19 05 18 10 00", "14 00 01 09 00 00 05 15 19 05 18 10 00")]
		[DataRow(MEC.RDS_SLOW_LABELING, 0x04, 0x77, "00 E2", "1A 04 00 E2")]
		[DataRow(MEC.RDS_LINKAGE_INFO, 0x02, 0x03, "81 23", "2E 02 03 81 23")]
		[DataRow(MEC.ODA_CONFIG, 0x77, 0x77, "16 12 34 02 AB CD 0A", "40 16 12 34 02 AB CD 0A")]
		[DataRow(MEC.ODA_IDENT, 0x00, 0x77, "16 16 16 12", "41 00 04 16 16 16 12")]
		[DataRow(MEC.ODA_FREE_FORMAT, 0x77, 0x77, "16 14 12 34 56 78 90", "42 16 14 12 34 56 78 90")]
		[DataRow(MEC.ODA_PRIORITY, 0x77, 0x77, "16 1F 1D", "43 03 16 1F 1D")]
		[DataRow(MEC.ODA_BURST_MODE, 0x77, 0x77, "16 10", "44 16 10")]
		[DataRow(MEC.ODA_SPIN_WHEEL, 0x77, 0x77, "16 0A 03 00", "45 16 0A 03 00")]
		[DataRow(MEC.ODA_DATA, 0x77, 0x77, "01 23 40 55 AA", "46 05 01 23 40 55 AA")]
		[DataRow(MEC.ODA_DATA, 0x77, 0x77, "01 23 00 1F 64 73 82 91", "46 08 01 23 00 1F 64 73 82 91")]
		[DataRow(MEC.ODA_DATA_ACL, 0x77, 0x77, "12 34 02 01", "47 12 34 02 01")]
		[DataRow(MEC.TDC, 0x77, 0x77, "00 01 45 42 55 20", "26 06 00 01 45 42 55 20")]
		[DataRow(MEC.EWS, 0x77, 0x77, "41 23 45 67 89", "2B 41 23 45 67 89")]
		[DataRow(MEC.IH, 0x77, 0x77, "01 60 00 00 00 00", "25 01 60 00 00 00 00")]
		[DataRow(MEC.TMC, 0x77, 0x77, "04 01 23 65 B0 E3 11 23 45 67 89", "30 0B 04 01 23 65 B0 E3 11 23 45 67 89")]
		[DataRow(MEC.FREE_FORMAT, 0x77, 0x77, "07 0C 00 00 AB DE", "24 07 0C 00 00 AB DE")]
		[DataRow(MEC.RTC, 0x77, 0x77, "5C 09 0C 0A 12 21 0F 02", "0D 5C 09 0C 0A 12 21 0F 02")]
		[DataRow(MEC.RTC_CORRECTION, 0x77, 0x77, "FF C6", "09 FF C6")]
		[DataRow(MEC.CT_ON_OFF, 0x77, 0x77, "01", "19 01")]
		[DataRow(MEC.RDS_ON_OFF, 0x77, 0x77, "00", "1E 00")]
		[DataRow(MEC.RDS_PHASE, 0x77, 0x77, "45 4C", "22 45 4C")]
		[DataRow(MEC.RDS_LEVEL, 0x77, 0x77, "A3 11", "0E A3 11")]
		[DataRow(MEC.UECP_ACK, 0x77, 0x77, "02 42", "18 02 42")]
		[DataRow(MEC.UECP_REQUEST, 0x77, 0x77, "1A 01", "17 02 1A 01")]
		public void ElementLayoutTest(MEC elementCode, int dataSetNumber, int programServiceNumber, string data, string expected)
		{
			MessageElement element = new MessageElement(elementCode, Hex.Parse(data));
			element.DataSetNumber = (byte)dataSetNumber;
			element.ProgramServiceNumber = (byte)programServiceNumber;

			Hex.AssertEqual(expected, element.GetBytes());
		}
	}
}
