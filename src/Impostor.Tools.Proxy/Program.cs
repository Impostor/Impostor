using System;
using System.Collections.Generic;
using System.Linq;
using Impostor.Hazel.Abstractions;
using Impostor.Hazel;
using Impostor.Hazel.Extensions;
using Impostor.Hazel.Udp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using PcapDotNet.Core;
using PcapDotNet.Packets;

namespace Impostor.Tools.Proxy
{
    internal static class Program
    {
        private const string DeviceName = "Intel(R) I211 Gigabit Network Connection";

        private static readonly Dictionary<byte, string> TagMap = new Dictionary<byte, string>
        {
            { 0, "HostGame" },
            { 1, "JoinGame" },
            { 2, "StartGame" },
            { 3, "RemoveGame" },
            { 4, "RemovePlayer" },
            { 5, "GameData" },
            { 6, "GameDataTo" },
            { 7, "JoinedGame" },
            { 8, "EndGame" },
            { 9, "GetGameList" },
            { 10, "AlterGame" },
            { 11, "KickPlayer" },
            { 12, "WaitForHost" },
            { 13, "Redirect" },
            { 14, "ReselectServer" },
            { 16, "GetGameListV2" },
        };

        private static IServiceProvider _serviceProvider;
        private static ObjectPool<MessageReader> _readerPool;

        private static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddHazel();

            _serviceProvider = services.BuildServiceProvider();
            _readerPool = _serviceProvider.GetRequiredService<ObjectPool<MessageReader>>();

            var devices = LivePacketDevice.AllLocalMachine;
            if (devices.Count == 0)
            {
                Console.WriteLine("No interfaces found! Make sure WinPcap is installed.");
                return;
            }

            var device = devices.FirstOrDefault(x => x.Description.Contains(DeviceName));
            if (device == null)
            {
                Console.WriteLine("Unable to find configured device.");
                return;
            }

            using (var communicator = device.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
            {
                using (var filter = communicator.CreateFilter("udp and port 22023"))
                {
                    communicator.SetFilter(filter);
                }

                communicator.ReceivePackets(0, PacketHandler);
            }
        }

        private static void PacketHandler(Packet packet)
        {
            var ip = packet.Ethernet.IpV4;
            var ipSrc = ip.Source.ToString();
            var udp = ip.Udp;

            // True if this is our own packet.
            using (var stream = udp.Payload.ToMemoryStream())
            {
                using var reader = _readerPool.Get();

                reader.Update(stream.ToArray());

                var option = reader.Buffer[0];
                if (option == (byte)MessageType.Reliable)
                {
                    reader.Seek(reader.Position + 3);
                }
                else if (option == (byte)UdpSendOption.Acknowledgement ||
                         option == (byte)UdpSendOption.Ping ||
                         option == (byte)UdpSendOption.Hello ||
                         option == (byte)UdpSendOption.Disconnect)
                {
                    return;
                }
                else
                {
                    reader.Seek(reader.Position + 1);
                }

                var isSent = ipSrc.StartsWith("192.");

                while (true)
                {
                    if (reader.Position >= reader.Length)
                    {
                        break;
                    }

                    using var message = reader.ReadMessage();
                    if (isSent)
                    {
                        HandleToServer(ipSrc, message);
                    }
                    else
                    {
                        HandleToClient(ipSrc, message);
                    }

                    if (message.Position < message.Length)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("- Did not consume all bytes.");
                    }
                }
            }
        }

        private static void HandleToClient(string source, IMessageReader packet)
        {
            var tagName = TagMap.ContainsKey(packet.Tag) ? TagMap[packet.Tag] : "Unknown";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{source,-15} Client received: {packet.Tag,-2} {tagName}");

            switch (packet.Tag)
            {
                case 14:
                case 13:
                    // packet.Position = packet.Length;
                    break;
                case 0:
                    Console.WriteLine("- GameCode        " + packet.ReadInt32());
                    break;
                case 5:
                case 6:
                    Console.WriteLine(HexUtils.HexDump(packet.Buffer.ToArray().Take(packet.Length).ToArray()));
                    // packet.Position = packet.Length;
                    break;
                case 7:
                    Console.WriteLine("- GameCode        " + packet.ReadInt32());
                    Console.WriteLine("- PlayerId        " + packet.ReadInt32());
                    Console.WriteLine("- Host            " + packet.ReadInt32());
                    var playerCount = packet.ReadPackedInt32();
                    Console.WriteLine("- PlayerCount     " + playerCount);
                    for (var i = 0; i < playerCount; i++)
                    {
                        Console.WriteLine("-     PlayerId    " + packet.ReadPackedInt32());
                    }

                    break;
                case 10:
                    Console.WriteLine("- GameCode        " + packet.ReadInt32());
                    Console.WriteLine("- Flag            " + packet.ReadSByte());
                    Console.WriteLine("- Value           " + packet.ReadBoolean());
                    break;
            }
        }

        private static void HandleToServer(string source, IMessageReader packet)
        {
            var tagName = TagMap.ContainsKey(packet.Tag) ? TagMap[packet.Tag] : "Unknown";
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{source,-15} Server received: {packet.Tag,-2} {tagName}");

            switch (packet.Tag)
            {
                case 0:
                    Console.WriteLine("- GameInfo length " + packet.ReadBytesAndSize().Length);
                    break;
                case 1:
                    Console.WriteLine("- GameCode        " + packet.ReadInt32());
                    Console.WriteLine("- Unknown         " + packet.ReadByte());
                    break;
                case 5:
                case 6:
                    Console.WriteLine("- GameCode        " + packet.ReadInt32());
                    Console.WriteLine(HexUtils.HexDump(packet.Buffer.ToArray().Take(packet.Length).ToArray()));
                    // packet.Position = packet.Length;
                    break;
            }
        }
    }
}
