namespace Lightspeed.Network;

/// <summary>
/// UDP discovery provider
/// </summary>
/// <remarks>
/// Initializes a new instance of the UdpProvider
/// </remarks>
/// <param name="port">Discovery port</param>
public sealed class UdpProvider(int port) : IProvider
{
    /// <summary>
    /// Discovery port
    /// </summary>
    private readonly int _port = port;

    /// <summary>
    /// Dispose of this provider
    /// </summary>
    public void Dispose()
    {
    }

    /// <summary>
    /// Create discovery client
    /// </summary>
    /// <returns>New discovery client</returns>
    public IClient CreateClient() => new DiscoveryClient(_port);

    /// <summary>
    /// Create discovery server
    /// </summary>
    /// <returns>New discovery server</returns>
    public IServer CreateServer() => new DiscoveryServer(_port);

    /// <summary>
    /// Start this provider
    /// </summary>
    public void Start()
    {
    }

    /// <summary>
    /// Stop this provider
    /// </summary>
    public void Stop()
    {
    }
}
