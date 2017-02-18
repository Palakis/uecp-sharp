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
using System.Threading.Tasks;
using System.Collections;

namespace UECP
{
    public class UECPFrame
    {
        public static byte SequenceCounter = 1;
        public UInt16 SiteAddress;
        public UInt16 EncoderAddress;
        public List<MessageElement> MessageElements;

        public UECPFrame()
        {
            SiteAddress = new UInt16();
            EncoderAddress = new UInt16();

            MessageElements = new List<MessageElement>();
        }

        public byte[] GetBytes()
        {
            // Gather all message elements into a single byte array
            List<byte> msgBytes = new List<byte>();
            foreach(MessageElement element in MessageElements)
            {
                msgBytes.Concat(element.GetBytes());
            }

            if (msgBytes.Count > 255)
                throw new Exception("Message too large");

            // Calculate the two-bytes ADD field
            byte[] addrBytes = BitConverter.GetBytes(AddressField());

            // Start building the UECP frame
            List<byte> frame = new List<byte>();
            frame.Add(addrBytes[0]); // ADD
            frame.Add(addrBytes[1]); // ADD
            frame.Add(SequenceCounter++); // SEQ
            frame.Add((byte)msgBytes.Count); // MEL = Message Element Length
            frame.Concat(msgBytes); // Message

            // Calculate CRC mid-way
            byte[] msgCRC = BitConverter.GetBytes(CRCField(frame));
            frame.Add(msgCRC[0]); // CRC
            frame.Add(msgCRC[1]); // CRC

            // Apply stuffing
            ApplyByteStuffing(frame);

            // Build the final frame
            List<byte> finalFrame = new List<byte>();
            finalFrame.Add(0xFE); // Start
            finalFrame.Concat(frame);
            finalFrame.Add(0xFF); // Stop

            // And voilà
            return finalFrame.ToArray();
        }

        private ushort CRCField(List<byte> data)
        {
            // CRC16-CCITT : x^16 + x^12 + x^5 + 1
            // Code based on http://sanity-free.org/133/crc_16_ccitt_in_csharp.html
            // I have no idea what i'm doing

            const ushort polynomial = 4129;

            ushort[] table = new ushort[256];
            for (int i = 0; i < table.Length; ++i)
            {
                ushort temp = 0;
                ushort a = (ushort)(i << 8);

                for (int j = 0; j < 8; ++j)
                {
                    if (((temp ^ a) & 0x8000) != 0)
                    {
                        temp = (ushort)((temp << 1) ^ polynomial);
                    }
                    else
                    {
                        temp <<= 1;
                    }

                    a <<= 1;
                }

                table[i] = temp;
            }

            ushort crc = 0;
            for (int i = 0; i < data.Count; ++i)
            {
                crc = (ushort)((crc << 8) ^ table[((crc >> 8) ^ (0xFF & data[i]))]);
            }

            return crc;
        }

        private UInt16 AddressField()
        {
            if (SiteAddress > 1023)
                throw new Exception("Invalid Site address");

            if (EncoderAddress > 64)
                throw new Exception("Invalid Encoder address");

            // Calculate the value of the ADD two-bytes field
            int address = 0;
            address = (SiteAddress & 0x3FF) << 6;
            address = address | (EncoderAddress & 0x3F);

            return (UInt16)address;
        }

        private void ApplyByteStuffing(List<byte> data)
        {
            for(int i = 0; i < data.Count; i++)
            {
                if (data[i] == 0xFD)
                {
                    data[i] = 0xFD;
                    data.Insert(i + 1, 0x00);
                }
                else if (data[i] == 0xFE)
                {
                    data[i] = 0xFD;
                    data.Insert(i + 1, 0x01);
                }
                else if (data[i] == 0xFF)
                {
                    data[i] = 0xFD;
                    data.Insert(i + 1, 0x02);
                }
            }
        }
    }

    public class MessageElement
    {
        public byte ElementCode;
        public byte DataSetNumber;
        public byte ProgramServiceNumber;
        public byte[] Data;

        public MessageElement()
        {
            ElementCode = 0;
            DataSetNumber = 0; // Current data set
            ProgramServiceNumber = 0; // Main service
            Data = new byte[0];
        }

        public MessageElement(byte elementCode, byte[] data)
        {
            ElementCode = elementCode;
            DataSetNumber = 0;
            ProgramServiceNumber = 0;
            Data = data;
        }

        public byte[] GetBytes()
        {
            if(Data.Length > 254)
                throw new Exception("Message Element Data too large");

            List<byte> msgData = new List<byte>();
            msgData.Add(ElementCode);
            msgData.Add(DataSetNumber);
            msgData.Add(ProgramServiceNumber);
            msgData.Add((byte)Data.Length);
            msgData.Concat(Data);

            return msgData.ToArray();
        }
    }
}
