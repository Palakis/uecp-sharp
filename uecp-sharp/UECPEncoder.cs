/*
MIT License

Copyright (c) 2017 Stéphane Lepin <stephane.lepin@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UECP
{
	public class UECPEncoder
	{
		protected Encoding _textEncoding;
		protected Endpoint _endpoint;

		public UECPEncoder(Endpoint ep)
		{
			_textEncoding = new EBULatinEncoding();
			_endpoint = ep;
		}

		public void SetPI(ushort pi)
		{
			BuildAndSendMessage(MEC.RDS_PI, BitConverter.GetBytes(pi));
		}

		public void SetPS(string ps)
		{
			if (ps.Length > 8)
				ps = ps.Substring(0, 8);

			List<byte> med = new List<byte>();
			med.AddRange(_textEncoding.GetBytes(ps));
			FillBytes(med, (byte)' ', 8); // the space char is the same in both ASCII and E.1 encoding

			BuildAndSendMessage(MEC.RDS_PS, med.ToArray());
		}

		public void SetRadioText(string radioText)
		{
			if(radioText.Length > 64)
				radioText = radioText.Substring(0, 64);

			// no A/B flag, infinite transmissions, buffer flushed when a new RT message arrives
			byte rtConfig = 0x00; 

			List<byte> med = new List<byte>();
			med.Add(rtConfig);
			med.AddRange(_textEncoding.GetBytes(radioText));

			BuildAndSendMessage(MEC.RDS_RT, med.ToArray());
		}

		public void SetTraffic(bool TA, bool TP)
		{
			byte taFlag = 0x01;
			byte tpFlag = 0x02;

			int data = 0;

			if (TA)
				data = data | taFlag;

			if (TP)
				data = data | tpFlag;

			var med = new byte[] { (byte)data };
			BuildAndSendMessage(MEC.RDS_TA_TP, med);
		}

		public void SetPTY(PTY pty)
		{
			var med = new byte[] { (byte)pty };
			BuildAndSendMessage(MEC.RDS_PTY, med);
		}

		public void SetPTYN(string ptyn)
		{
			if (ptyn.Length > 8)
				ptyn = ptyn.Substring(0, 8);

			List<byte> med = new List<byte>();
			med.AddRange(_textEncoding.GetBytes(ptyn));
			FillBytes(med, (byte)' ', 8);

			BuildAndSendMessage(MEC.RDS_PTYN, med.ToArray());
		}

		public void SetMusicSpeech(bool isMusic)
		{
			var med = new byte[] { (byte)(isMusic ? 0x01 : 0x00) };
			BuildAndSendMessage(MEC.RDS_MS, med);
		}

		public void SendODAShortMessage(
			ushort aid, ushort message,
			ODABufferConfig odaBufConfig = ODABufferConfig.TransmitOnce
		) {
			var med = new List<byte>();
			med.AddRange(BitConverter.GetBytes(aid));
			med.Add(
				getODAConfiguration(ODAConfigKind.ShortMessage, odaBufConfig)
			);
			med.AddRange(BitConverter.GetBytes(message));

			BuildAndSendMessage(MEC.ODA_DATA, med.ToArray());
		}

		public void SendODAData(
			ushort aid, byte block2, ushort block3, ushort block4,
			ODABufferConfig odaBufConfig = ODABufferConfig.TransmitOnce,
			ODATransmitMode odaTransmitMode = ODATransmitMode.Normal,
			ushort priority = 0,
			bool isGroupB = false
		) {
			// ODA block 2 is 5 bits only
			block2 &= 0x1F;

			var med = new List<byte>();
			med.AddRange(BitConverter.GetBytes(aid));
			med.Add(
				getODAConfiguration(ODAConfigKind.Data, odaBufConfig, odaTransmitMode, priority)
			);
			med.Add(block2);
			if (!isGroupB)
			{
				med.AddRange(BitConverter.GetBytes(block3));
			}
			med.AddRange(BitConverter.GetBytes(block4));

			BuildAndSendMessage(MEC.ODA_DATA, med.ToArray());
		}

		public void SendODAFreeFormatData(
			byte appGroupType, byte block2, ushort block3, ushort block4,
			ODABufferConfig odaBufConfig = ODABufferConfig.TransmitOnce,
			ODATransmitMode odaTransmitMode = ODATransmitMode.Normal,
			ushort priority = 0
		) {
			// ODA block 2 is 5 bits only
			block2 &= 0x1F;

			var med = new List<byte>();
			med.Add(appGroupType);
			med.Add(
				getODAConfiguration(ODAConfigKind.Data, odaBufConfig, odaTransmitMode, priority)
			);
			med.Add(block2);
			med.AddRange(BitConverter.GetBytes(block3));
			med.AddRange(BitConverter.GetBytes(block4));

			BuildAndSendMessage(MEC.ODA_FREE_FORMAT, med.ToArray());
		}

		public void SetDABDynamicLabelMessage(string dynamicLabel)
		{
			if (dynamicLabel.Length > 128)
				dynamicLabel = dynamicLabel.Substring(0, 128);

			var med = new List<byte>();
			med.Add(0x00); // EBU Latin encoding
			med.AddRange(_textEncoding.GetBytes(dynamicLabel));

			BuildAndSendMessage(MEC.DAB_DL_MESSAGE, med.ToArray());
		}

		public void SendDABDynamicLabelCommand(byte command, byte[] commandBytes, bool temporary = false)
		{
			byte config = 0x00;
			config |= (byte)((temporary ? 0b1 : 0b0) << 7);
			config |= (byte)(command & 0xF);

			int dataSize = Math.Min(128, commandBytes.Length);
			byte[] data = new byte[dataSize];
			Array.Copy(commandBytes, dataSize, data, 0, dataSize);

			var med = new List<byte>();
			med.Add(config);
			med.AddRange(data);

			BuildAndSendMessage(MEC.DAB_DL_COMMAND, med.ToArray());
		}

		private byte getODAConfiguration(
			ODAConfigKind kind,
			ODABufferConfig odaBufConfig,
			ODATransmitMode odaTransmitMode = ODATransmitMode.Normal,
			ushort priority = 0
		) {
			if (priority > 2)
				priority = 2;

			int transmitMode = 0b00;
			switch (odaTransmitMode)
			{
				case ODATransmitMode.Normal:
				default:
					transmitMode = 0b00;
					break;

				case ODATransmitMode.Burst:
					transmitMode = 0b01;
					break;

				case ODATransmitMode.SpinningWheel:
					transmitMode = 0b10;
					break;
			}

			int bufferConfig = 0b00;
			switch (odaBufConfig)
			{
				case ODABufferConfig.TransmitOnce:
				default:
					bufferConfig = 0b00;
					break;

				case ODABufferConfig.AddToCyclic:
					bufferConfig = 0b10;
					break;

				case ODABufferConfig.ClearCyclic:
					bufferConfig = 0b11;
					break;
			}

			byte config = 0x00;
			// bit 7 is always set to 0
			if (kind == ODAConfigKind.ShortMessage)
			{
				config |= (0b1 << 6); // bit 6 set to 1
			}
			if (kind == ODAConfigKind.Data)
			{
				config |= (byte)((priority & 0b11) << 4); // bits 4 and 5
				config |= (byte)((transmitMode & 0b11) << 2); // bits 2 and 3
			}
			config |= (byte)(bufferConfig & 0b11); // bits 0 and 1

			return config;
		}

		private void BuildAndSendMessage(MEC elementCode, byte[] messageElementData)
		{
			BuildAndSendMessage(new MessageElement(elementCode, messageElementData));
		}

		private void BuildAndSendMessage(MessageElement messageElement)
		{
			UECPFrame uecpFrame = new UECPFrame();
			uecpFrame.MessageElements.Add(messageElement);

			_endpoint.SendData(uecpFrame.GetBytes());
		}

		private void FillBytes(List<byte> data, byte fillWith, int desiredLength)
		{
			int fillBytes = desiredLength - data.Count;
			if (fillBytes > 0)
			{
				for (int i = 0; i < fillBytes; i++)
				{
					data.Add(fillWith);
				}
			}
		}
	}
}
