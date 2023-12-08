using WebSocketMockServer.Reactions;

namespace WebSocketMockServer.Storage;

/// <summary>
/// Mock template model.
/// </summary>
public class MockTemplate
{
    /// <summary>
    /// Request text.
    /// </summary>
    public string Request { get; }

    /// <summary>
    /// Reactions for request.
    /// </summary>
    public IEnumerable<Reaction> Reactions { get; }

    /// <summary>
    /// Creates <see cref="MockTemplate"/>.
    /// </summary>
    /// <param name="request">Request text</param>
    /// <param name="reactions">Reactions on request</param>
    /// <exception cref="ArgumentNullException">Throws if request data or reactions is null.</exception>
    /// <exception cref="ArgumentException">Throws if request data or reactions is empty.</exception>
    public MockTemplate(string request, IEnumerable<Reaction> reactions)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request))
        {
            throw new ArgumentException("Request not set", nameof(request));
        }

        Request = request;

        ArgumentNullException.ThrowIfNull(reactions);

        if (!reactions.Any())
        {
            throw new ArgumentException("Reactions not set", nameof(reactions));
        }

        Reactions = reactions.ToList(); // Materialize
    }
}
