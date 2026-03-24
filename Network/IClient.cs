namespace Lightspeed.Network;

/// <summary>
/// Discovery client interface
/// </summary>
public interface IClient
{
    /// <summary>
    /// Discovery event
    /// </summary>
    event EventHandler<DiscoveryEventArgs> Discovery;

    /// <summary>
    /// Start the client
    /// </summary>
    void Start();

    /// <summary>
    /// Stop the client
    /// </summary>
    void Stop();
}
