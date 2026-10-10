using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UECP;

namespace UecpSharpTests
{
	// Expected messages are the examples of SPB 490 wherever it gives one,
	// with the data set and programme service numbers the encoder uses (0 and 0)
	[TestClass]
	public class EncoderTests
	{
		private RecordingEndpoint _endpoint;
		private UECPEncoder _encoder;

		[TestInitialize]
		public void Initialize()
		{
			_endpoint = new RecordingEndpoint();
			_encoder = new UECPEncoder(_endpoint);
		}

		[TestMethod]
		public void SetPITest()
		{
			_encoder.SetPI(0xC201);

			Hex.AssertEqual("01 00 00 C2 01", sentMessage());
		}

		[TestMethod]
		public void SetPSTest()
		{
			_encoder.SetPS("RADIO 1");

			Hex.AssertEqual("02 00 00 52 41 44 49 4F 20 31 20", sentMessage());
		}

		[TestMethod]
		[DataRow("", "20 20 20 20 20 20 20 20")]
		[DataRow("ABCDEFGH", "41 42 43 44 45 46 47 48")]
		[DataRow("ABCDEFGHIJ", "41 42 43 44 45 46 47 48")]
		[DataRow("Café", "43 61 66 82 20 20 20 20")]
		[DataRow("Météo FM", "4D 82 74 82 6F 20 46 4D")] // accents as combining characters: 8 characters, not 10
		public void SetPSLengthTest(string ps, string expected)
		{
			_encoder.SetPS(ps);

			Hex.AssertEqual("02 00 00 " + expected, sentMessage());
		}

		[TestMethod]
		public void SetPSInvalidUnicodeTest()
		{
			// Half of a surrogate pair, as left by a text cut in the middle of an emoji: sent as a space
			_encoder.SetPS("AB\uD83D");

			Hex.AssertEqual("02 00 00 41 42 20 20 20 20 20 20", sentMessage());
		}

		[TestMethod]
		public void SetRadioTextTest()
		{
			_encoder.SetRadioText("RDS");

			// Configuration 00: buffer flushed, transmitted indefinitely, A/B flag not toggled
			Hex.AssertEqual("0A 00 00 04 00 52 44 53", sentMessage());
		}

		[TestMethod]
		public void SetRadioTextEmptyTest()
		{
			_encoder.SetRadioText("");

			Hex.AssertEqual("0A 00 00 01 00", sentMessage());
		}

		[TestMethod]
		[DataRow(64)]
		[DataRow(70)]
		public void SetRadioTextMaxLengthTest(int length)
		{
			// Only the first 64 characters are sent
			_encoder.SetRadioText(new string('A', 64) + new string('B', length - 64));

			byte[] expected = Hex.Parse("0A 00 00 41 00").Concat(Enumerable.Repeat((byte)0x41, 64)).ToArray();
			CollectionAssert.AreEqual(expected, sentMessage());
		}

		[TestMethod]
		public void SetRadioTextDecomposedAccentsTest()
		{
			// "Café" with the accent as a combining character
			_encoder.SetRadioText("Café");

			Hex.AssertEqual("0A 00 00 05 00 43 61 66 82", sentMessage());
		}

		[TestMethod]
		public void SetRadioTextCarriageReturnTest()
		{
			// EN 50067, 3.1.5.3: a text shorter than 64 characters should be ended by 0D
			_encoder.SetRadioText("RDS\r");

			Hex.AssertEqual("0A 00 00 05 00 52 44 53 0D", sentMessage());
		}

		[TestMethod]
		public void SetRadioTextLineFeedTest()
		{
			// EN 50067, 3.1.5.3: 0A may be inserted to indicate a preferred line break
			_encoder.SetRadioText("Line 1\nLine 2");

			Hex.AssertEqual("0A 00 00 0E 00 4C 69 6E 65 20 31 0A 4C 69 6E 65 20 32", sentMessage());
		}

		[TestMethod]
		public void SetRadioTextCarriageReturnAfterMaxLengthTest()
		{
			// A text of 64 characters needs no end mark, and has no room for it
			_encoder.SetRadioText(new string('A', 64) + "\r");

			byte[] expected = Hex.Parse("0A 00 00 41 00").Concat(Enumerable.Repeat((byte)0x41, 64)).ToArray();
			CollectionAssert.AreEqual(expected, sentMessage());
		}

		[TestMethod]
		[DataRow(0x09)]
		[DataRow(0x0B)]
		[DataRow(0x1F)]
		public void SetRadioTextOtherControlCodesTest(int code)
		{
			// Carriage return and line feed are the only control codes let through
			_encoder.SetRadioText("A" + (char)code + "B");

			Hex.AssertEqual("0A 00 00 04 00 41 20 42", sentMessage());
		}

		[TestMethod]
		public void SetPSControlCodesTest()
		{
			// Unlike a Radiotext, a PS only takes displayable characters
			_encoder.SetPS("AB\r\nCD");

			Hex.AssertEqual("02 00 00 41 42 20 20 43 44 20 20", sentMessage());
		}

		[TestMethod]
		public void SetPTYNControlCodesTest()
		{
			_encoder.SetPTYN("AB\r\nCD");

			Hex.AssertEqual("3E 00 00 41 42 20 20 43 44 20 20", sentMessage());
		}

		[TestMethod]
		[DataRow(false, false, "00")]
		[DataRow(true, false, "01")]
		[DataRow(false, true, "02")]
		[DataRow(true, true, "03")]
		public void SetTrafficTest(bool ta, bool tp, string expected)
		{
			_encoder.SetTraffic(ta, tp);

			Hex.AssertEqual("03 00 00 " + expected, sentMessage());
		}

		[TestMethod]
		[DataRow(PTY.None, "00")]
		[DataRow(PTY.Science, "08")]
		[DataRow(PTY.Alarm, "1F")]
		public void SetPTYTest(PTY pty, string expected)
		{
			_encoder.SetPTY(pty);

			Hex.AssertEqual("07 00 00 " + expected, sentMessage());
		}

		[TestMethod]
		[DataRow(-1)]
		[DataRow(32)]
		[DataRow(256)]
		public void SetPTYOutOfRangeTest(int pty)
		{
			// The programme type is 5 bits wide
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _encoder.SetPTY((PTY)pty));
			Assert.IsEmpty(_endpoint.Frames);
		}

		[TestMethod]
		[DataRow("Football", "46 6F 6F 74 62 61 6C 6C")]
		[DataRow("Jazz", "4A 61 7A 7A 20 20 20 20")]
		[DataRow("Programme", "50 72 6F 67 72 61 6D 6D")]
		[DataRow("Variétés", "56 61 72 69 82 74 82 73")] // accents as combining characters
		public void SetPTYNTest(string ptyn, string expected)
		{
			_encoder.SetPTYN(ptyn);

			Hex.AssertEqual("3E 00 00 " + expected, sentMessage());
		}

		[TestMethod]
		[DataRow(true, "01")]
		[DataRow(false, "00")]
		public void SetMusicSpeechTest(bool isMusic, string expected)
		{
			_encoder.SetMusicSpeech(isMusic);

			Hex.AssertEqual("05 00 00 " + expected, sentMessage());
		}

		[TestMethod]
		public void SendODAShortMessageTest()
		{
			// SPB 490 6.02, 3.1.19: 55 AA once only as short message for AID 0123
			_encoder.SendODAShortMessage(0x0123, 0x55AA);

			Hex.AssertEqual("46 05 01 23 40 55 AA", sentMessage());
		}

		[TestMethod]
		[DataRow(ODABufferConfig.TransmitOnce, "40")]
		[DataRow(ODABufferConfig.AddToCyclic, "42")]
		[DataRow(ODABufferConfig.ClearCyclic, "43")]
		public void SendODAShortMessageConfigurationTest(ODABufferConfig bufferConfig, string expected)
		{
			_encoder.SendODAShortMessage(0x1111, 0x2222, bufferConfig);

			Hex.AssertEqual("46 05 11 11 " + expected + " 22 22", sentMessage());
		}

		[TestMethod]
		public void SendODADataTest()
		{
			// SPB 490 6.02, 3.1.19: 1F 64 73 82 91 once only, normal mode and priority, for AID 0123
			_encoder.SendODAData(0x0123, 0x1F, 0x6473, 0x8291);

			Hex.AssertEqual("46 08 01 23 00 1F 64 73 82 91", sentMessage());
		}

		[TestMethod]
		public void SendODADataGroupBTest()
		{
			// No block 3 in type B groups
			_encoder.SendODAData(0x1111, 0x1F, 0x2222, 0x3333, isGroupB: true);

			Hex.AssertEqual("46 06 11 11 00 1F 33 33", sentMessage());
		}

		[TestMethod]
		public void SendODADataBlock2Test()
		{
			// Block 2 only has 5 bits of data
			_encoder.SendODAData(0x1111, 0xF2, 0x2222, 0x3333);

			Hex.AssertEqual("46 08 11 11 00 12 22 22 33 33", sentMessage());
		}

		[TestMethod]
		[DataRow(ODABufferConfig.TransmitOnce, ODATransmitMode.Normal, 0, "00")]
		[DataRow(ODABufferConfig.AddToCyclic, ODATransmitMode.Normal, 0, "02")]
		[DataRow(ODABufferConfig.ClearCyclic, ODATransmitMode.Normal, 0, "03")]
		[DataRow(ODABufferConfig.TransmitOnce, ODATransmitMode.Burst, 0, "04")]
		[DataRow(ODABufferConfig.TransmitOnce, ODATransmitMode.SpinningWheel, 0, "08")]
		[DataRow(ODABufferConfig.TransmitOnce, ODATransmitMode.Normal, 1, "10")]
		[DataRow(ODABufferConfig.TransmitOnce, ODATransmitMode.Normal, 2, "20")]
		[DataRow(ODABufferConfig.TransmitOnce, ODATransmitMode.Normal, 3, "20")] // 3 is reserved
		[DataRow(ODABufferConfig.TransmitOnce, ODATransmitMode.Burst, 1, "14")]
		[DataRow(ODABufferConfig.AddToCyclic, ODATransmitMode.SpinningWheel, 2, "2A")]
		public void SendODADataConfigurationTest(ODABufferConfig bufferConfig, ODATransmitMode transmitMode, int priority, string expected)
		{
			_encoder.SendODAData(0x1111, 0x00, 0x2222, 0x3333, bufferConfig, transmitMode, (ushort)priority);

			Hex.AssertEqual("46 08 11 11 " + expected + " 00 22 22 33 33", sentMessage());
		}

		[TestMethod]
		public void SendODAFreeFormatDataTest()
		{
			// SPB 490 3.3.16: group 11A, once only in burst mode, extremely urgent
			_encoder.SendODAFreeFormatData(0x16, 0x12, 0x3456, 0x7890, ODABufferConfig.TransmitOnce, ODATransmitMode.Burst, 1);

			Hex.AssertEqual("42 16 14 12 34 56 78 90", sentMessage());
		}

		[TestMethod]
		public void SendODAFreeFormatDataBlock2Test()
		{
			_encoder.SendODAFreeFormatData(0x16, 0xF2, 0x1111, 0x2222);

			Hex.AssertEqual("42 16 00 12 11 11 22 22", sentMessage());
		}

		// The DAB elements come with SPB 490 version 7, which these tests could not be checked against:
		// they only describe what the encoder intends to send

		[TestMethod]
		public void SetDABDynamicLabelMessageTest()
		{
			_encoder.SetDABDynamicLabelMessage("Hello");

			Hex.AssertEqual("AA 06 00 48 65 6C 6C 6F", sentMessage());
		}

		[TestMethod]
		public void SetDABDynamicLabelMessageMaxLengthTest()
		{
			// Only the first 128 characters are sent
			_encoder.SetDABDynamicLabelMessage(new string('A', 128) + "BB");

			byte[] expected = Hex.Parse("AA 81 00").Concat(Enumerable.Repeat((byte)0x41, 128)).ToArray();
			CollectionAssert.AreEqual(expected, sentMessage());
		}

		[TestMethod]
		public void SetDABDynamicLabelMessageDecomposedAccentsTest()
		{
			_encoder.SetDABDynamicLabelMessage("Café");

			Hex.AssertEqual("AA 05 00 43 61 66 82", sentMessage());
		}

		[TestMethod]
		public void SendDABDynamicLabelCommandTest()
		{
			_encoder.SendDABDynamicLabelCommand(0x01, Hex.Parse("10 20 30"));

			Hex.AssertEqual("48 04 01 10 20 30", sentMessage());
		}

		[TestMethod]
		[DataRow(0x01, false, "01")]
		[DataRow(0x01, true, "81")]
		[DataRow(0xF5, false, "05")] // the command is 4 bits wide
		public void SendDABDynamicLabelCommandConfigurationTest(int command, bool temporary, string expected)
		{
			_encoder.SendDABDynamicLabelCommand((byte)command, new byte[0], temporary);

			Hex.AssertEqual("48 01 " + expected, sentMessage());
		}

		[TestMethod]
		[DataRow(128)]
		[DataRow(200)]
		[DataRow(300)]
		public void SendDABDynamicLabelCommandMaxLengthTest(int length)
		{
			byte[] commandBytes = Enumerable.Range(0, length).Select(i => (byte)i).ToArray();

			_encoder.SendDABDynamicLabelCommand(0x01, commandBytes);

			// Only the first 128 bytes are sent
			byte[] expected = Hex.Parse("48 81 01").Concat(commandBytes.Take(128)).ToArray();
			CollectionAssert.AreEqual(expected, sentMessage());
		}

		[TestMethod]
		public void SequenceCounterTest()
		{
			_encoder.SetPI(0xC201);
			_encoder.SetPS("RADIO 1");
			_encoder.SetRadioText("RDS");

			CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, sentSequenceCounters());
			Assert.AreEqual((byte)4, _encoder.SequenceCounter);
		}

		[TestMethod]
		public void SequenceCounterSetTest()
		{
			_encoder.SequenceCounter = 0x40;

			_encoder.SetPS("RADIO 1");
			_encoder.SetPS("RADIO 1");

			CollectionAssert.AreEqual(new byte[] { 0x40, 0x41 }, sentSequenceCounters());
		}

		[TestMethod]
		public void SequenceCounterWrapTest()
		{
			// Sequence numbers go from 01 to FF, 00 means that the counter is not used
			_encoder.SequenceCounter = 0xFE;

			_encoder.SetPS("RADIO 1");
			_encoder.SetPS("RADIO 1");
			_encoder.SetPS("RADIO 1");

			CollectionAssert.AreEqual(new byte[] { 0xFE, 0xFF, 0x01 }, sentSequenceCounters());
		}

		[TestMethod]
		public void SequenceCounterPerEncoderTest()
		{
			RecordingEndpoint otherEndpoint = new RecordingEndpoint();
			UECPEncoder otherEncoder = new UECPEncoder(otherEndpoint);

			_encoder.SetPS("RADIO 1");
			_encoder.SetPS("RADIO 1");
			otherEncoder.SetPS("RADIO 2");

			Assert.AreEqual((byte)1, DecodedFrame.Parse(otherEndpoint.Frames.Single()).SequenceCounter);
		}

		[TestMethod]
		public void SequenceCounterOffTest()
		{
			// 00 means that the sequence counter is not used, and it stays that way
			_encoder.SequenceCounter = 0x00;

			_encoder.SetPS("RADIO 1");
			_encoder.SetPS("RADIO 1");
			_encoder.SetPS("RADIO 1");

			CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00 }, sentSequenceCounters());
		}

		[TestMethod]
		public void ConcurrentCallsTest()
		{
			const int threads = 8;
			const int framesPerThread = 1000;

			Task[] tasks = Enumerable.Range(0, threads)
				.Select(t => Task.Run(() =>
				{
					for (int i = 0; i < framesPerThread; i++)
						_encoder.SetMusicSpeech(true);
				}))
				.ToArray();
			Task.WaitAll(tasks);

			// Frames leave one at a time, numbered in the order they are sent: 01 to FF, then 01 again
			byte[] expected = Enumerable.Range(0, threads * framesPerThread).Select(i => (byte)(i % 255 + 1)).ToArray();
			CollectionAssert.AreEqual(expected, sentSequenceCounters());
		}

		[TestMethod]
		public void NullArgumentsTest()
		{
			Assert.ThrowsExactly<ArgumentNullException>(() => new UECPEncoder(null));
			Assert.ThrowsExactly<ArgumentNullException>(() => _encoder.SetPS(null));
			Assert.ThrowsExactly<ArgumentNullException>(() => _encoder.SetRadioText(null));
			Assert.ThrowsExactly<ArgumentNullException>(() => _encoder.SetPTYN(null));
			Assert.ThrowsExactly<ArgumentNullException>(() => _encoder.SetDABDynamicLabelMessage(null));
			Assert.ThrowsExactly<ArgumentNullException>(() => _encoder.SendDABDynamicLabelCommand(0x01, null));

			// Nothing was sent and no sequence number was used
			Assert.IsEmpty(_endpoint.Frames);
			Assert.AreEqual((byte)1, _encoder.SequenceCounter);
		}

		[TestMethod]
		public void TextTableCharsTest()
		{
			// Every character of the E.1 table has to reach the encoder as its own code
			byte[] codes = Enumerable.Range(0x20, 0xDF).Where(code => code != 0x7F).Select(code => (byte)code).ToArray();
			string text = new EBULatinEncoding().GetString(codes);

			_encoder.SetDABDynamicLabelMessage(text.Substring(0, 128));
			_encoder.SetDABDynamicLabelMessage(text.Substring(128));

			// Without the element code, length and configuration bytes
			byte[] sentCodes = _endpoint.Frames.SelectMany(f => DecodedFrame.Parse(f).Message.Skip(3)).ToArray();
			CollectionAssert.AreEqual(codes, sentCodes);
		}

		// Message of the only frame sent, which has to be a valid one addressed to all encoders
		private byte[] sentMessage()
		{
			Assert.HasCount(1, _endpoint.Frames);

			DecodedFrame frame = DecodedFrame.Parse(_endpoint.Frames[0]);
			Assert.IsTrue(frame.CrcIsValid);
			Assert.AreEqual(0, frame.SiteAddress);
			Assert.AreEqual(0, frame.EncoderAddress);

			return frame.Message;
		}

		private byte[] sentSequenceCounters()
		{
			return _endpoint.Frames.Select(f => DecodedFrame.Parse(f).SequenceCounter).ToArray();
		}
	}
}
