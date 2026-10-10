using System;
using System.Globalization;
using System.Net;
using UECP;

namespace UecpTestClient
{
	static class Program
	{
		static int Main(string[] args)
		{
			if (args.Length != 4)
				return Usage();

			string host = args[0];
			string command = args[2].ToLowerInvariant();
			string value = args[3];

			if (!IPAddress.TryParse(host, out _))
				return Error("Host must be an IP address.");

			if (!int.TryParse(args[1], out int port) || port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
				return Error("Port must be a number between 0 and 65535.");

			switch (command)
			{
				case "pi":
					if (value.Length != 4 || !ushort.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out ushort pi))
						return Error("PI must be an hexadecimal number of exactly four characters.");

					Connect(host, port).SetPI(pi);
					break;

				case "ps":
					if (value.Length > 8)
						return Error("PS must be at most 8 characters long.");

					Connect(host, port).SetPS(value);
					break;

				case "rt":
					if (value.Length > 64)
						return Error("RadioText must be at most 64 characters long.");

					Connect(host, port).SetRadioText(value);
					break;

				default:
					return Usage();
			}

			return 0;
		}

		private static UECPEncoder Connect(string host, int port)
		{
			return new UECPEncoder(new UDPSimplexEndpoint(host, port));
		}

		private static int Error(string message)
		{
			Console.Error.WriteLine("Invalid input: " + message);
			return 1;
		}

		private static int Usage()
		{
			Console.Error.WriteLine("Usage: UecpTestClient <host> <port> <command> <value>");
			Console.Error.WriteLine();
			Console.Error.WriteLine("Commands:");
			Console.Error.WriteLine("  pi <hex>    Set the PI code (exactly four hexadecimal characters)");
			Console.Error.WriteLine("  ps <text>   Set the PS name (at most 8 characters)");
			Console.Error.WriteLine("  rt <text>   Set the RadioText (at most 64 characters)");
			return 1;
		}
	}
}
