using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UECP;

namespace UecpSharpTests
{
	[TestClass]
	public class TextEncodingTests
	{
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

		private void testEncoding(string sourceText, byte[] expectedBytes)
		{
			Encoding e1 = new RdsE1Encoding();
			byte[] actualBytes = e1.GetBytes(sourceText);

			Assert.AreEqual(actualBytes.Length, expectedBytes.Length);
			Assert.AreNotSame(actualBytes, expectedBytes);
			for (int i = 0; i < actualBytes.Length; i++)
			{
				Assert.AreEqual(actualBytes[i], expectedBytes[i]);
			}
		}
	}
}
