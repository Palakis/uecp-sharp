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
        Endpoint _endpoint;

        public UECPEncoder(Endpoint ep)
        {
            _endpoint = ep;
        }

        public void SetPI(UInt16 pi)
        {
            BuildAndSendMessage(MEC.RDS_PI, BitConverter.GetBytes(pi));
        }

        public void SetPS(string ps)
        {
            if (ps.Length > 8)
                ps = ps.Substring(0, 8);

            byte[] psBytes = Encoding.ASCII.GetBytes(ps);
            byte[] psData = new byte[8];

            Buffer.BlockCopy(psBytes, 0, psData, 0, psBytes.Length);

            BuildAndSendMessage(MEC.RDS_PS, psData);
        }

        public void SetRadioText(string radioText)
        {
            if(radioText.Length > 64)
                radioText = radioText.Substring(0, 64);

            // no A/B flag, infinite transmissions, buffer flushed when a new RT message arrives
            byte rtConfig = 0x00; 

            List<byte> rtData = new List<byte>();
            rtData.Add(rtConfig);
            rtData.AddRange(Encoding.ASCII.GetBytes(radioText));

            BuildAndSendMessage(MEC.RDS_RT, rtData.ToArray());
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

            BuildAndSendMessage(MEC.RDS_TA_TP, msgData);
        }

        public void SetPTY(PTY pty)
        {
            byte[] ptyData = new byte[1];
            ptyData[0] = (byte)pty;
            BuildAndSendMessage(MEC.RDS_PTY, ptyData);
        }

        public void SetPTYN(string ptyn)
        {
            if (ptyn.Length > 8)
                ptyn = ptyn.Substring(0, 8);

            byte[] ptynBytes = Encoding.ASCII.GetBytes(ptyn);
            byte[] ptynData = new byte[8];

            Buffer.BlockCopy(ptynBytes, 0, ptynData, 0, ptynBytes.Length);

            BuildAndSendMessage(MEC.RDS_PTYN, ptynData);
        }

        private void BuildAndSendMessage(MEC elementCode, byte[] messageElementData)
        {
            BuildAndSendMessage(new MessageElement((byte)elementCode, messageElementData));
        }

        private void BuildAndSendMessage(MessageElement messageElement)
        {
            UECPFrame uecpFrame = new UECPFrame();
            uecpFrame.MessageElements.Add(messageElement);

            _endpoint.SendData(uecpFrame.GetBytes());
        }
    }
}
