namespace Impostor.Api.Config;

/// <summary>
/// Configuration for HttpServer.
/// </summary>
public class HttpServerConfig
{
    /// <summary>
    /// Gets the name of this config section.
    /// </summary>
    public const string Section = "HttpServer";

    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the IP address the HTTP Matchmaking server will listen on.
    /// </summary>
    /// Use "127.0.0.1" if you are running behind a reverse proxy or just testing locally.
    /// Use "0.0.0.0" if you are directly exposing this server to the internet (not recommended).
    public string ListenIp { get; set; } = "127.0.0.1";

    /// <summary>
    /// Gets or sets the port the HTTP Matchmaking server will listen on.
    /// </summary>
    /// For port forwarding purposes, this is a TCP port.
    public ushort ListenPort { get; set; } = 22023;

    /// <summary>
    /// Gets or sets a value indicating whether Https is enabled or not.
    /// </summary>
    /// If true, CertificatePath needs to be set to a SSL certificate.
    public bool UseHttps { get; set; }

    /// <summary>
    /// Gets or sets the path to the SSL certificate.
    /// </summary>
    /// This field is ignored if SSL is not used.
    public string CertificatePath { get; set; } = string.Empty;
}
