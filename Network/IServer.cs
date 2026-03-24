namespace Lightspeed.Network;

/// <summary>
/// Discovery server interface
/// </summary>
public interface IServer
{
    /// <summary>
    /// Gets or sets the server identity
    /// </summary>
    string Identity { get; set; }

    /// <summary>
    /// Start the server
    /// </summary>
    void Start();

    /// <summary>
    /// Stop the server
    /// </summary>
    void Stop();
}
