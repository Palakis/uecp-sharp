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
                msgBytes.AddRange(element.GetBytes());
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
            frame.AddRange(msgBytes); // Message

            // Calculate CRC mid-way
            byte[] msgCRC = BitConverter.GetBytes(CRCField(frame));
            frame.Add(msgCRC[1]); // CRC
            frame.Add(msgCRC[0]); // CRC

            // Apply stuffing
            ApplyByteStuffing(frame);

            // Build the final frame
            List<byte> finalFrame = new List<byte>();
            finalFrame.Add(0xFE); // Start
            finalFrame.AddRange(frame);
            finalFrame.Add(0xFF); // Stop

            // And voilà
            return finalFrame.ToArray();
        }

        private ushort CRCField(List<byte> data)
        {
            // CRC16-CCITT : x^16 + x^12 + x^5 + 1
            // Code from UoC-Radio/rds-control on GitHub
            // Link : https://github.com/UoC-Radio/rds-control/blob/master/uecp.c#L50-L65
            // Because I have no idea what i'm doing

            int crc = 0xFFFF;

            for(int i = 0; i < data.Count; i++)
            {
                crc = (byte)(crc >> 8) | (crc << 8);
                crc ^= data[i];
                crc ^= (byte)(crc & 0xff) >> 4;
                crc ^= (crc << 8) << 4;
                crc ^= ((crc & 0xff) << 4) << 1;
            }

            return (ushort)((crc ^= 0xFFFF) & 0xFFFF);
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
        public MEC ElementCode;
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

        public MessageElement(MEC elementCode, byte[] data) : this()
        {
            ElementCode = elementCode;
            Data = data;
        }

        public byte[] GetBytes()
        {
            if(Data.Length > 254)
                throw new Exception("Message Element Data too large");

            List<byte> msgData = new List<byte>();
            msgData.Add((byte)ElementCode);

            if (MECRules.HasDSNPSN.Contains(ElementCode))
            {
                msgData.Add(DataSetNumber);

                if(!MECRules.ExcludePSN.Contains(ElementCode))
                    msgData.Add(ProgramServiceNumber);
            }

            if (MECRules.HasMEL.Contains(ElementCode))
            {
                msgData.Add((byte)Data.Length);
            }
            
            msgData.AddRange(Data);

            return msgData.ToArray();
        }
    }
}
