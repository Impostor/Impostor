using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using Agones;
using Impostor.Server.Data;
using Microsoft.Extensions.Options;
using k8s;
using k8s.Models;
using Microsoft.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Impostor.Server.Net.Redirector
{
    internal class NodeProviderAgones : INodeProvider
    {
        private class Port
        {
            public string name { get; set; }
            public int port { get; set; }
        }

        private class Status
        {
            public IList<Port> ports { get; set; }
            public string address { get; set; }
        }

        private class AgonesCreateServerResponse
        {
            public string kind { get; set; }
            public Status status { get; set; }
        }

        public NodeProviderAgones(IOptions<ServerRedirectorConfig> redirectorConfig)
        {
        }

        public IPEndPoint Get()
        {
            // curl -d '{"apiVersion":"allocation.agones.dev/v1","kind":"GameServerAllocation","spec":{"required":{"matchLabels":{"agones.dev/fleet":"simple-udp"}}}}' -H "Content-Type: application/json" -X POST http://localhost:8001/apis/allocation.agones.dev/v1/namespaces/default/gameserverallocations
            // Load from in-cluster configuration:
            var config = KubernetesClientConfiguration.BuildDefaultConfig();

            // Use the config object to create a client.
            var client = new Kubernetes(config);
            var body = JObject.Parse(
                @"{""apiVersion"":""allocation.agones.dev/v1"",""kind"":""GameServerAllocation"",""spec"":{""required"":{""matchLabels"":{""agones.dev/fleet"":""simple-udp""}}}}");
            string result = null;
            try
            {
                client.CreateNamespacedCustomObject(body, "allocation.agones.dev", "v1", "impostor",
                    "gameserverallocations");
            }
            catch (HttpOperationException e)
            {
                if (e.Message != "Operation returned an invalid status code 'OK'")
                {
                    Console.WriteLine(e.Response.Content);
                    throw;
                }

                result = e.Response.Content;
            }

            // client.Create
            var serverDetails = JsonConvert.DeserializeObject<AgonesCreateServerResponse>(result);
            var ipEndPoint = new IPEndPoint(IPAddress.Parse(serverDetails.status.address),
                serverDetails.status.ports.First().port);
            Console.WriteLine("IP " + ipEndPoint.Address + " Port " + ipEndPoint.Port);
            return ipEndPoint;
        }
    }
}
