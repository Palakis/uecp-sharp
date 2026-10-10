using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UECP;

namespace UecpSharpTests
{
	[TestClass]
	public class EndpointTests
	{
		[TestMethod]
		public void UdpSendDataTest()
		{
			using (UdpClient listener = createListener(IPAddress.Loopback))
			using (UDPSimplexEndpoint endpoint = new UDPSimplexEndpoint("127.0.0.1", listenerPort(listener)))
			{
				byte[] data = { 0xFE, 0x00, 0x01, 0x02, 0xFF };

				endpoint.SendData(data);

				CollectionAssert.AreEqual(data, receive(listener));
			}
		}

		[TestMethod]
		public void UdpIPv6Test()
		{
			if (!Socket.OSSupportsIPv6)
				Assert.Inconclusive("IPv6 is not available on this machine");

			using (UdpClient listener = createListener(IPAddress.IPv6Loopback))
			using (UDPSimplexEndpoint endpoint = new UDPSimplexEndpoint("::1", listenerPort(listener)))
			{
				byte[] data = { 0xFE, 0x00, 0x01, 0x02, 0xFF };

				endpoint.SendData(data);

				CollectionAssert.AreEqual(data, receive(listener));
			}
		}

		[TestMethod]
		public void UdpEncoderTest()
		{
			using (UdpClient listener = createListener(IPAddress.Loopback))
			using (UDPSimplexEndpoint endpoint = new UDPSimplexEndpoint("127.0.0.1", listenerPort(listener)))
			{
				UECPEncoder encoder = new UECPEncoder(endpoint);

				encoder.SetPS("RADIO 1");

				// One datagram per frame
				DecodedFrame frame = DecodedFrame.Parse(receive(listener));
				Assert.IsTrue(frame.CrcIsValid);
				Hex.AssertEqual("02 00 00 52 41 44 49 4F 20 31 20", frame.Message);
			}
		}

		[TestMethod]
		public void UdpDisposeTest()
		{
			UDPSimplexEndpoint endpoint = new UDPSimplexEndpoint("127.0.0.1", 5001);

			endpoint.Dispose();

			// The socket is closed, nothing can be sent any more
			Assert.ThrowsExactly<ObjectDisposedException>(() => endpoint.SendData(new byte[] { 0xFE, 0xFF }));

			// Disposing twice is harmless
			endpoint.Dispose();
		}

		[TestMethod]
		public void UdpSendDataNullTest()
		{
			using (UDPSimplexEndpoint endpoint = new UDPSimplexEndpoint("127.0.0.1", 5001))
			{
				Assert.ThrowsExactly<ArgumentNullException>(() => endpoint.SendData(null));
			}
		}

		[TestMethod]
		public void UdpHostNameTest()
		{
			// Only IP addresses are accepted
			Assert.ThrowsExactly<FormatException>(() => new UDPSimplexEndpoint("localhost", 5001));
		}

		[TestMethod]
		public void UdpPortOutOfRangeTest()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new UDPSimplexEndpoint("127.0.0.1", 70000));
		}

		private UdpClient createListener(IPAddress address)
		{
			// Port 0: any free port
			UdpClient listener = new UdpClient(new IPEndPoint(address, 0));
			listener.Client.ReceiveTimeout = 2000;
			return listener;
		}

		private int listenerPort(UdpClient listener)
		{
			return ((IPEndPoint)listener.Client.LocalEndPoint).Port;
		}

		private byte[] receive(UdpClient listener)
		{
			IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
			return listener.Receive(ref sender);
		}
	}
}
