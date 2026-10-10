using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UECP;

namespace UecpSharpTests
{
	[TestClass]
	public class TextEncodingTests
	{
		// ETSI TS 101 756 V1.6.1, annex C, table C.1: one line per row, from code 20 to code FF.
		// Written with the unicode numbers of the table, many of these characters have look-alikes.
		// \0 stands for the two codes without display character, 7F and FF.
		private static readonly string[] ebuLatinTable =
		{
			" !\"#¤%&'()*+,-./",
			"0123456789:;<=>?",
			"@ABCDEFGHIJKLMNO",
			"PQRSTUVWXYZ[\\]―_",
			"║abcdefghijklmno",
			"pqrstuvwxyz{|}¯\0",
			"áàéèíìóòúùÑÇŞß¡Ĳ",
			"âäêëîïôöûüñçşğıĳ",
			"ªα©‰Ğěňőπ€£$←↑→↓",
			"º¹²³±İńűµ¿÷°¼½¾§",
			"ÁÀÉÈÍÌÓÒÚÙŘČŠŽÐĿ",
			"ÂÄÊËÎÏÔÖÛÜřčšžđŀ",
			"ÃÅÆŒŷÝÕØÞŊŔĆŚŹŦð",
			"ãåæœŵýõøþŋŕćśźŧ\0"
		};

		[TestMethod]
		public void E1SimpleTest()
		{
			string text = "test";
			byte[] expectedBytes = { 0x74, 0x65, 0x73, 0x74 };

			testEncoding(text, expectedBytes);
		}

		[TestMethod]
		public void E1SpecialCharsTest()
		{
			string text = "éàçüö#$£";
			byte[] expectedBytes = { 0x82, 0x81, 0x9B, 0x99, 0x97, 0x23, 0xAB, 0xAA };

			testEncoding(text, expectedBytes);
		}

		[TestMethod]
		public void E1UnknownCharsTest()
		{
			byte[] sourceBytes = { 0x68, 0x65, 0x6C, 0x6C, 0x6F, 0x7F };
			string text = Encoding.UTF8.GetString(sourceBytes);
			byte[] expectedBytes = { 0x68, 0x65, 0x6C, 0x6C, 0x6F, 0x20 }; // charactère inconnu remplacé par espace

			testEncoding(text, expectedBytes);
		}

		[TestMethod]
		public void E1EmptyTextTest()
		{
			string text = "";
			byte[] expectedBytes = { };

			testEncoding(text, expectedBytes);
		}

		[TestMethod]
		public void E1EmptyBytesTest()
		{
			Encoding e1 = new EBULatinEncoding();
			string actualText = e1.GetString(new byte[0]);

			Assert.AreEqual("", actualText);
		}

		[TestMethod]
		public void E1FullTableEncodeTest()
		{
			Encoding e1 = new EBULatinEncoding();
			List<string> errors = new List<string>();

			for (int code = 0x20; code <= 0xFF; code++)
			{
				char c = tableChar(code);
				if (c == '\0')
					continue;

				byte actualCode = e1.GetBytes(c.ToString())[0];
				if (actualCode != code)
					errors.Add($"U+{(int)c:X4} gives {actualCode:X2} instead of {code:X2}");
			}

			Assert.AreEqual("", string.Join(", ", errors));
		}

		[TestMethod]
		public void E1FullTableDecodeTest()
		{
			Encoding e1 = new EBULatinEncoding();
			List<string> errors = new List<string>();

			for (int code = 0x20; code <= 0xFF; code++)
			{
				char c = tableChar(code);
				if (c == '\0')
					c = ' ';

				char actualChar = e1.GetString(new[] { (byte)code })[0];
				if (actualChar != c)
					errors.Add($"{code:X2} gives U+{(int)actualChar:X4} instead of U+{(int)c:X4}");
			}

			Assert.AreEqual("", string.Join(", ", errors));
		}

		[TestMethod]
		public void E1ThornTest()
		{
			// Icelandic thorn, upper and lower case
			testEncoding("Þþ", new byte[] { 0xE8, 0xF8 });
		}

		[TestMethod]
		public void E1RoundTripTest()
		{
			Encoding e1 = new EBULatinEncoding();
			byte[] codes = Enumerable.Range(0x20, 0xE0)
				.Where(code => code != 0x7F && code != 0xFF)
				.Select(code => (byte)code)
				.ToArray();

			CollectionAssert.AreEqual(codes, e1.GetBytes(e1.GetString(codes)));
		}

		[TestMethod]
		public void E1DollarTest()
		{
			// The dollar sign is not where ASCII has it: its code is taken by the currency sign
			testEncoding("$¤", new byte[] { 0xAB, 0x24 });
		}

		[TestMethod]
		[DataRow("^")]
		[DataRow("`")]
		[DataRow("~")]
		[DataRow("Ω")]
		[DataRow("中")]
		[DataRow("😀")]
		public void E1UnknownCharsReplacedTest(string text)
		{
			// One space per UTF-16 code unit, including for the ASCII characters that E.1 does not have
			testEncoding(text, Enumerable.Repeat((byte)0x20, text.Length).ToArray());
		}

		[TestMethod]
		[DataRow(0x00)]
		[DataRow(0x7F)]
		[DataRow(0xFF)]
		public void E1UnknownBytesTest(int code)
		{
			Encoding e1 = new EBULatinEncoding();

			Assert.AreEqual(" ", e1.GetString(new[] { (byte)code }));
		}

		[TestMethod]
		public void E1CountsTest()
		{
			Encoding e1 = new EBULatinEncoding();

			// One byte per character
			Assert.AreEqual(5, e1.GetByteCount("héllo"));
			Assert.AreEqual(3, e1.GetByteCount("hello".ToCharArray(), 1, 3));
			Assert.AreEqual(5, e1.GetCharCount(new byte[5]));
			Assert.AreEqual(3, e1.GetCharCount(new byte[5], 1, 3));
			Assert.AreEqual(8, e1.GetMaxByteCount(8));
			Assert.AreEqual(8, e1.GetMaxCharCount(8));
		}

		[TestMethod]
		public void E1IdentityTest()
		{
			Encoding e1 = new EBULatinEncoding();

			Assert.AreEqual("ebulatin", e1.WebName);
			Assert.AreEqual("EBU Latin", e1.EncodingName);
			Assert.AreEqual(65221, e1.CodePage);
		}

		[TestMethod]
		public void E1GetBytesRangeTest()
		{
			Encoding e1 = new EBULatinEncoding();

			byte[] actualBytes = e1.GetBytes("hello".ToCharArray(), 1, 3);

			CollectionAssert.AreEqual(new byte[] { 0x65, 0x6C, 0x6C }, actualBytes);
		}

		[TestMethod]
		public void E1GetBytesAtOffsetTest()
		{
			Encoding e1 = new EBULatinEncoding();
			byte[] actualBytes = { 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE };

			int count = e1.GetBytes("hello", 1, 3, actualBytes, 2);

			Assert.AreEqual(3, count);
			CollectionAssert.AreEqual(new byte[] { 0xEE, 0xEE, 0x65, 0x6C, 0x6C, 0xEE }, actualBytes);
		}

		[TestMethod]
		public void E1GetStringRangeTest()
		{
			Encoding e1 = new EBULatinEncoding();
			byte[] sourceBytes = { 0x68, 0x65, 0x6C, 0x6C, 0x6F, 0x21 };

			Assert.AreEqual("llo", e1.GetString(sourceBytes, 2, 3));
		}

		[TestMethod]
		public void E1GetCharsAtOffsetTest()
		{
			Encoding e1 = new EBULatinEncoding();
			byte[] sourceBytes = { 0x68, 0x65, 0x6C, 0x6C, 0x6F, 0x21 };
			char[] actualChars = "......".ToCharArray();

			int count = e1.GetChars(sourceBytes, 4, 2, actualChars, 3);

			Assert.AreEqual(2, count);
			Assert.AreEqual("...o!.", new string(actualChars));
		}

		[TestMethod]
		public void E1SpanTest()
		{
			Encoding e1 = new EBULatinEncoding();
			byte[] actualBytes = new byte[8];

			int count = e1.GetBytes("héllo".AsSpan(), actualBytes);

			Assert.AreEqual(5, count);
			Assert.AreEqual("héllo", e1.GetString(new ReadOnlySpan<byte>(actualBytes, 0, count)));
		}

		[TestMethod]
		public void E1NullArgumentsTest()
		{
			Encoding e1 = new EBULatinEncoding();

			Assert.ThrowsExactly<ArgumentNullException>(() => e1.GetByteCount((char[])null, 0, 0));
			Assert.ThrowsExactly<ArgumentNullException>(() => e1.GetBytes((char[])null, 0, 0, new byte[1], 0));
			Assert.ThrowsExactly<ArgumentNullException>(() => e1.GetBytes(new char[1], 0, 0, null, 0));
			Assert.ThrowsExactly<ArgumentNullException>(() => e1.GetCharCount(null, 0, 0));
			Assert.ThrowsExactly<ArgumentNullException>(() => e1.GetChars(null, 0, 0, new char[1], 0));
			Assert.ThrowsExactly<ArgumentNullException>(() => e1.GetChars(new byte[1], 0, 0, null, 0));
		}

		[TestMethod]
		public void E1NegativeArgumentsTest()
		{
			Encoding e1 = new EBULatinEncoding();

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => e1.GetByteCount(new char[4], -1, 1));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => e1.GetBytes(new char[4], -1, 1, new byte[4], 0));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => e1.GetBytes(new char[4], 0, -1, new byte[4], 0));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => e1.GetBytes(new char[4], 0, 1, new byte[4], -1));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => e1.GetCharCount(new byte[4], 0, -1));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => e1.GetChars(new byte[4], -1, 1, new char[4], 0));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => e1.GetChars(new byte[4], 0, -1, new char[4], 0));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => e1.GetChars(new byte[4], 0, 1, new char[4], -1));
		}

		[TestMethod]
		public void E1GetBytesSourceOutOfRangeTest()
		{
			Encoding e1 = new EBULatinEncoding();

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => e1.GetByteCount(new char[4], 2, 3));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => e1.GetBytes(new char[4], 2, 3, new byte[8], 0));
		}

		[TestMethod]
		public void E1GetCharsSourceOutOfRangeTest()
		{
			Encoding e1 = new EBULatinEncoding();

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => e1.GetCharCount(new byte[4], 2, 3));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => e1.GetChars(new byte[4], 2, 3, new char[8], 0));
		}

		[TestMethod]
		public void E1GetBytesDestinationTooSmallTest()
		{
			Encoding e1 = new EBULatinEncoding();
			byte[] actualBytes = new byte[4];

			Assert.ThrowsExactly<ArgumentException>(() => e1.GetBytes("abcd".ToCharArray(), 0, 4, new byte[2], 0));
			Assert.ThrowsExactly<ArgumentException>(() => e1.GetBytes("abcd".ToCharArray(), 0, 4, actualBytes, 2));

			// Nothing is written when it does not fit
			CollectionAssert.AreEqual(new byte[4], actualBytes);
		}

		[TestMethod]
		public void E1GetCharsDestinationTooSmallTest()
		{
			Encoding e1 = new EBULatinEncoding();
			byte[] sourceBytes = { 0x61, 0x62, 0x63, 0x64 };
			char[] actualChars = new char[4];

			Assert.ThrowsExactly<ArgumentException>(() => e1.GetChars(sourceBytes, 0, 4, new char[2], 0));
			Assert.ThrowsExactly<ArgumentException>(() => e1.GetChars(sourceBytes, 0, 4, actualChars, 2));

			// Nothing is written when it does not fit
			CollectionAssert.AreEqual(new char[4], actualChars);
		}

		private char tableChar(int code)
		{
			return ebuLatinTable[(code >> 4) - 2][code & 0x0F];
		}

		private void testEncoding(string sourceText, byte[] expectedBytes)
		{
			Encoding e1 = new EBULatinEncoding();
			byte[] actualBytes = e1.GetBytes(sourceText);

			Assert.AreEqual(expectedBytes.Length, actualBytes.Length);
			Assert.AreNotSame(expectedBytes, actualBytes);
			for (int i = 0; i < actualBytes.Length; i++)
			{
				Assert.AreEqual(expectedBytes[i], actualBytes[i]);
			}
		}
	}
}
