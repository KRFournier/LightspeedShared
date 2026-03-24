using System.Net;

namespace Lightspeed.Network;

/// <summary>
/// Discovery event arguments
/// </summary>
/// <remarks>
/// Initializes a new instance of the DiscoveryEventArgs class
/// </remarks>
/// <param name="address">Server address</param>
/// <param name="identity">Server identity</param>
public sealed class DiscoveryEventArgs(IPAddress address, string identity)
{

    /// <summary>
    /// Gets the server address
    /// </summary>
    public IPAddress Address { get; } = address;

    /// <summary>
    /// Gets the server identity
    /// </summary>
    public string Identity { get; } = identity;
}
