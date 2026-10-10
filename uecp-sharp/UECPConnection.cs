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
using System.Net;
using System.Net.Sockets;

namespace UECP
{
	public interface Endpoint
	{
		void SendData(byte[] data);
	}

	public class UDPSimplexEndpoint : Endpoint, IDisposable
	{
		private UdpClient _udpClient;

		public UDPSimplexEndpoint(string encoderAddress, int encoderPort)
		{
			IPAddress address = IPAddress.Parse(encoderAddress);

			// The socket has to be of the same family as the address, IPv4 or IPv6
			_udpClient = new UdpClient(address.AddressFamily);

			try
			{
				_udpClient.Connect(address, encoderPort);
			}
			catch
			{
				// Nobody else can dispose the socket when the constructor fails
				_udpClient.Dispose();
				throw;
			}
		}

		public void SendData(byte[] data)
		{
			if (data == null)
				throw new ArgumentNullException(nameof(data));

			_udpClient.Send(data, data.Length);
		}

		public void Dispose()
		{
			_udpClient.Dispose();
		}
	}
}
