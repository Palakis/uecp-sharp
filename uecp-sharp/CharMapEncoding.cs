using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UECP
{
	public class CharMapEncoding : Encoding
	{
		protected const byte BLANK = 0x20; // espace
		protected Dictionary<char, byte> FromUTF16;
		protected Dictionary<byte, char> ToUTF16;

		public CharMapEncoding(int codePage) : base(codePage)
		{
			FromUTF16 = new Dictionary<char, byte>();
			ToUTF16 = new Dictionary<byte, char>();
		}

		protected void SetCharMap(Dictionary<char, byte> map)
		{
			FromUTF16 = new Dictionary<char, byte>(map);
			ToUTF16 = FromUTF16.ToDictionary(kp => kp.Value, kp => kp.Key);
		}

		public override int GetByteCount(char[] chars, int index, int count)
		{
			if (chars == null)
				throw new ArgumentNullException();

			if (index < 0 || count < 0)
				throw new ArgumentOutOfRangeException();

			if ((index + count) > chars.Length || count > chars.Length)
				throw new ArgumentOutOfRangeException();

			return count;
		}

		public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			if (chars == null || bytes == null)
				throw new ArgumentNullException();

			if (charIndex < 0 || charCount < 0 || byteIndex < 0)
				throw new ArgumentOutOfRangeException();

			if ((charIndex + charCount) > chars.Length || charCount > chars.Length || byteIndex >= bytes.Length)
				throw new ArgumentOutOfRangeException();

			if (bytes.Length < chars.Length)
				throw new ArgumentException();

			int i;
			for (i = 0; i < charCount; i++)
			{
				char c = chars[charIndex + i];

				byte b = BLANK;
				if (FromUTF16.ContainsKey(c))
					b = FromUTF16[c];

				bytes[byteIndex + i] = b;
			}

			return i;
		}

		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			if (bytes == null)
				throw new ArgumentNullException();

			if (index < 0 || count < 0)
				throw new ArgumentOutOfRangeException();

			if ((index + count) > bytes.Length || count > bytes.Length)
				throw new ArgumentOutOfRangeException();

			return count;
		}

		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			if (bytes == null || chars == null)
				throw new ArgumentNullException();

			if (byteIndex < 0 || byteCount < 0 || charIndex < 0)
				throw new ArgumentOutOfRangeException();

			if ((byteIndex + byteCount) > chars.Length || byteCount > chars.Length || charIndex >= chars.Length)
				throw new ArgumentOutOfRangeException();

			if (chars.Length < bytes.Length)
				throw new ArgumentException();

			int i;
			for (i = 0; i < byteCount; i++)
			{
				byte b = bytes[byteIndex + i];

				char c = ' ';
				if (ToUTF16.ContainsKey(b))
					c = ToUTF16[b];

				chars[charIndex + i] = c;
			}

			return i;
		}

		public override int GetMaxByteCount(int charCount)
		{
			return charCount;
		}

		public override int GetMaxCharCount(int byteCount)
		{
			return byteCount;
		}
	}
}
