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

namespace UECP
{
    public class UECPEncoder
    {
        Endpoint _endpoint;

        public UECPEncoder(Endpoint ep)
        {
            _endpoint = ep;
        }

        public void SetPI(UInt16 pi)
        {
            BuildAndSendMessage(0x01, BitConverter.GetBytes(pi));
        }

        public void SetPS(string ps)
        {
            byte[] psBytes = Encoding.ASCII.GetBytes(ps);
            byte[] psData = new byte[8];

            Buffer.BlockCopy(psBytes, 0, psData, 0, psBytes.Length);

            BuildAndSendMessage(0x02, psData);
        }

        public void SetRadioText(string rt)
        {
            BuildAndSendMessage(0x02, Encoding.ASCII.GetBytes(rt));
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

            byte[] msgData = new byte[1];
            msgData[0] = (byte)data;

            BuildAndSendMessage(0x03, msgData);
        }

        private void BuildAndSendMessage(byte elementCode, byte[] messageElementData)
        {
            UECPFrame uecpFrame = new UECPFrame();
            uecpFrame.AddMessageElement(new MessageElement(elementCode, messageElementData));

            _endpoint.SendData(uecpFrame.GetBytes());
        }
    }
}
