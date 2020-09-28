using Impostor.Server.Data;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Impostor.Server.Net.Redirector
{
    public class NodeLocatorUDPSockets : INodeLocator, IDisposable
    {
        readonly UdpClient client;
        private bool disposedValue;

        private class AvailableNode { 
            public IPEndPoint Endpoint { get; set; }
            public DateTime LastUpdated { get; set; }
            public bool Expired => LastUpdated < DateTime.Now.AddHours(-1);
        }

        private Dictionary<string, AvailableNode> AvailableNodes;

        public NodeLocatorUDPSockets(IOptions<ServerRedirectorConfig> config)
        {
            if (!Uri.TryCreate(config.Value.UDPMasterEndpoint, UriKind.Absolute, out Uri ServerAddress) || ServerAddress.Scheme.ToLower() != "udp")
            {
                throw new Exception($"UDPMasterEndpoint has an invalid value of '{config.Value.UDPMasterEndpoint}'. Expected udp://ip:port/ schema.");
            }

            if (config.Value.Master)
            {
                if (!IPAddress.TryParse(ServerAddress.Host, out var LocalIPBinding)) {
                    throw new Exception("$UDPMasterEndpoint must use an IP address rather than a hostname when acting as a master.");
                }

                client = new UdpClient(new IPEndPoint(LocalIPBinding, ServerAddress.Port));
                AvailableNodes = new Dictionary<string, AvailableNode>();
                Task.Run(HandleClientPackets);
            }
            else
            {
                client = new UdpClient(ServerAddress.Host, ServerAddress.Port);
            }
        }

        public IPEndPoint Find(string gameCode)
        {
            lock (AvailableNodes)
            {
                return AvailableNodes.ContainsKey(gameCode) ? AvailableNodes[gameCode].Endpoint : null;
            }
        }

        private void HandleClientPackets()
        {
            try
            {
                while (true)
                {
                    var data = client.ReceiveAsync().GetAwaiter().GetResult();
                    //TODO: Log that we got an update from the given remote EP.
                    string message = Encoding.UTF8.GetString(data.Buffer);
                    var parts = message.Split(',', 2);
                    if (parts.Length != 2) { continue; }

                    if (!IPEndPoint.TryParse(parts[1], out var Endpoint)) { continue; }

                    HandleUpdate(parts[0], Endpoint);
                }
            }
            catch (ObjectDisposedException)
            {
                return; //Bail out and don't try anything else.
            }
            catch
            {
                //Something else went wrong but we're not shutting down, give up and try again.
                if (!disposedValue) { Task.Run(HandleClientPackets); }
            }
        }

        private void HandleUpdate(string gameCode, IPEndPoint endpoint)
        {
            lock (AvailableNodes)
            {
                var node = AvailableNodes.ContainsKey(gameCode) ? AvailableNodes[gameCode] : new AvailableNode();
                node.Endpoint = endpoint;
                node.LastUpdated = DateTime.Now;
                AvailableNodes[gameCode] = node;

                var KeysToRemove = AvailableNodes.Where(kvp => kvp.Value.Expired).Select(kvp => kvp.Key).ToList();
                KeysToRemove.ForEach(n => AvailableNodes.Remove(n));
            }
        }

        public void Remove(string gameCode)
        {
            lock (AvailableNodes)
            {
                AvailableNodes.Remove(gameCode);
            }
        }

        public void Save(string gameCode, IPEndPoint endPoint)
        {
            byte[] data = Encoding.UTF8.GetBytes($"{endPoint},{gameCode}");
            client.Send(data, data.Length);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    client.Close();
                    client.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
