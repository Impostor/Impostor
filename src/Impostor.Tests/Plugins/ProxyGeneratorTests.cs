using System;
using System.Threading.Tasks;
using Impostor.Api.Events;
using Impostor.Api.Events.Managers;
using Impostor.Server.Plugins;
using Impostor.Server.Plugins.Services;
using Xunit;

namespace Impostor.Tests.Plugins
{
    public class ProxyGeneratorTests
    {
        [Fact]
        public void CreateProxyService()
        {
            var instance = new TestService();
            var proxy = ProxyGenerator.GetServiceProxy<ITestService>(instance);

            proxy.Value = 1;
            Assert.Equal(1, instance.Value);
            Assert.Equal(1, proxy.Value);

            proxy.ResetValue();
            Assert.Equal(0, instance.Value);
            Assert.Equal(0, proxy.Value);

            var value = proxy.ReturnSelf(instance);
            Assert.Equal(instance, value);
        }


        [Fact]
        public void CreateEventManager()
        {
            ProxyGenerator.GetServiceProxyType(typeof(IEventManager));
        }

        public interface ITestService
        {
            int Value { get; set; }

            void ResetValue();

            T ReturnSelf<T>(T value) where T : ITestService;
        }

        public class TestService : ITestService
        {
            public int Value { get; set; }

            public void ResetValue()
            {
                Value = 0;
            }

            public T ReturnSelf<T>(T value) where T : ITestService
                => value;
        }
    }
}
