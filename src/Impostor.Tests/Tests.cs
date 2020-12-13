using System;
using System.Collections.Generic;
using Impostor.Api.Net;
using Impostor.Server.Config;
using Impostor.Server.Net;
using Impostor.Server.Net.Factories;
using Impostor.Server.Net.Manager;
using Impostor.Server.Recorder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Impostor.Tests
{
    public class Tests
    {
        [Fact]
        public void TestNextId()
        {
            var mockSP = new Mock<IServiceProvider>();

            var clientFactory = new ClientFactory<Client>(mockSP.Object);
            var clientManager = new ClientManager(null, clientFactory);

            int id = clientManager.NextId();

            Assert.True(id == 1);
            id = clientManager.NextId();
            Assert.True(id == 2);
            id = clientManager.NextId();
            id = clientManager.NextId();
            Assert.True(id == 4);
        }

        [Fact]
        public void TestValide()
        {
            var mockSP = new Mock<IServiceProvider>();
            var mockAC = new Mock<IOptions<AntiCheatConfig>>();
            var clientFactory = new ClientFactory<Client>(mockSP.Object);
            var clientManager = new ClientManager(null, clientFactory);

            IOptions<AntiCheatConfig> anticheatCfg = mockAC.Object;
            IClient client = new ClientRecorder(null, anticheatCfg, clientManager, null, "TEST", null, null);

            bool validate = clientManager.Validate(client);
            Assert.False(validate);

        }

        [Fact]
        public void TestClientFactroryCreate()
        {
            var Ip = Impostor.Server.Utils.IpUtils("127.0.0.1");

            Assert.NotNull(Ip);
        }




    }
}
