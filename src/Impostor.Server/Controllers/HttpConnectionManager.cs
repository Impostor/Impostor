using System.Collections.Generic;

namespace Impostor.Server.Controllers;

public class HttpConnectionManager
{
    public List<IExtensionConnection> Connections { get; } = [];
}
