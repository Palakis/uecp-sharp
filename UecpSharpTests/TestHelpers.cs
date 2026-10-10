using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UECP;

namespace UecpSharpTests
{
	// Endpoint that keeps the frames instead of sending them
	class RecordingEndpoint : Endpoint
	{
		public List<byte[]> Frames = new List<byte[]>();

		public void SendData(byte[] data)
		{
			Frames.Add(data);
		}
	}

	// A frame read back the way an RDS encoder would read it off the wire
	class DecodedFrame
	{
		public int SiteAddress;
		public int EncoderAddress;
		public byte SequenceCounter;
		public byte[] Message;
		public bool CrcIsValid;

		public static DecodedFrame Parse(byte[] wire)
		{
			Assert.IsTrue(wire.Length >= 8, "Frame is too short");
			Assert.AreEqual((byte)0xFE, wire[0], "Start byte");
			Assert.AreEqual((byte)0xFF, wire[wire.Length - 1], "Stop byte");

			// Undo byte stuffing between start and stop
			List<byte> fields = new List<byte>();
			for (int i = 1; i < wire.Length - 1; i++)
			{
				byte b = wire[i];
				Assert.IsTrue(b < 0xFE, "Reserved byte inside the frame: " + Hex.Format(wire));

				if (b == 0xFD)
				{
					i++;
					Assert.IsTrue(i < wire.Length - 1 && wire[i] <= 0x02, "Bad stuffing: " + Hex.Format(wire));
					b = (byte)(0xFD + wire[i]);
				}

				fields.Add(b);
			}

			// ADD (2) + SQC + MFL + MSG + CRC (2)
			Assert.IsTrue(fields.Count >= 6, "Frame is too short");
			Assert.AreEqual(fields.Count - 6, fields[3], "MFL does not match the message length");

			int address = (fields[0] << 8) | fields[1];
			int crc = (fields[fields.Count - 2] << 8) | fields[fields.Count - 1];

			DecodedFrame frame = new DecodedFrame();
			frame.SiteAddress = address >> 6;
			frame.EncoderAddress = address & 0x3F;
			frame.SequenceCounter = fields[2];
			frame.Message = fields.Skip(4).Take(fields.Count - 6).ToArray();
			frame.CrcIsValid = crc == Crc16.Compute(fields.Take(fields.Count - 2).ToArray());
			return frame;
		}
	}

	// Bit by bit CRC-CCITT as specified for UECP (start at FFFF, result inverted).
	// Deliberately not the algorithm used by the library.
	static class Crc16
	{
		public static ushort Compute(byte[] data)
		{
			int crc = 0xFFFF;

			foreach (byte b in data)
			{
				crc ^= b << 8;
				for (int bit = 0; bit < 8; bit++)
				{
					crc <<= 1;
					if ((crc & 0x10000) != 0)
						crc ^= 0x11021;
				}
			}

			return (ushort)(crc ^ 0xFFFF);
		}
	}

	static class Hex
	{
		public static byte[] Parse(string hex)
		{
			return hex
				.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(b => Convert.ToByte(b, 16))
				.ToArray();
		}

		public static string Format(byte[] bytes)
		{
			return BitConverter.ToString(bytes).Replace('-', ' ');
		}

		// Compares as text so that a failure shows both byte sequences
		public static void AssertEqual(string expectedHex, byte[] actual)
		{
			Assert.AreEqual(Format(Parse(expectedHex)), Format(actual));
		}
	}
}
