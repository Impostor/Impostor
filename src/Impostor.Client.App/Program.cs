using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Api.Net.Messages.C2S;
using Impostor.Hazel;
using Impostor.Hazel.Abstractions;
using Impostor.Hazel.Udp;
using Serilog;

namespace Impostor.Client.App
{
    internal static class Program
    {
        private static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);

        private static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            var writeHandshake = MessageWriter.Get(MessageType.Reliable);

            writeHandshake.Write(50516550);
            writeHandshake.Write("AeonLucid");

            var writeGameCreate = MessageWriter.Get(MessageType.Reliable);

            Message00HostGameC2S.Serialize(writeGameCreate, new LegacyGameOptionsData
            {
                MaxPlayers = 4,
                NumImpostors = 2,
            }, CrossplayFlags.All, GameFilterOptions.CreateDefault());

            // TODO: ObjectPool for MessageReaders
            using (var connection = new UdpClientConnection(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 22023), null))
            {
                var e = new ManualResetEvent(false);

                // Register events.
                connection.DataReceived = DataReceived;
                connection.Disconnected = Disconnected;

                // Connect and send handshake.
                await connection.ConnectAsync(writeHandshake.ToByteArray(false));
                Log.Information("Connected.");

                // Create a game.
                await connection.SendAsync(writeGameCreate);
                Log.Information("Requested game creation.");

                // Recycle.
                writeHandshake.Recycle();
                writeGameCreate.Recycle();

                e.WaitOne();
            }
        }

        private static ValueTask DataReceived(DataReceivedEventArgs e)
        {
            Log.Information("Received data.");
            return default;
        }

        private static ValueTask Disconnected(DisconnectedEventArgs e)
        {
            Log.Information("Disconnected: " + e.Reason);
            QuitEvent.Set();
            return default;
        }
    }
}
