using System.Threading.Tasks;
using Impostor.Hazel.Dtls;
using Impostor.Hazel.Udp;

namespace Impostor.Server.Net;

public class AuNetListeners
{
    public DtlsConnectionListener? _dtlsConnectionListener;

    public DtlsConnectionListener? _onlineConnectionListener;
    public UdpConnectionListener? _udpConnectionListener;

    public async Task StartAsync()
    {
        if (_udpConnectionListener != null)
        {
            await _udpConnectionListener.StartAsync();
        }

        if (_dtlsConnectionListener != null)
        {
            await _dtlsConnectionListener.StartAsync();
        }

        if (_onlineConnectionListener != null)
        {
            await _onlineConnectionListener.StartAsync();
        }
    }

    public async Task StopAsync()
    {
        if (_udpConnectionListener != null)
        {
            await _udpConnectionListener.DisposeAsync();
        }

        if (_dtlsConnectionListener != null)
        {
            await _dtlsConnectionListener.DisposeAsync();
        }

        if (_onlineConnectionListener != null)
        {
            await _onlineConnectionListener.DisposeAsync();
        }
    }
}
