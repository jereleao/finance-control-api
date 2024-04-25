namespace FinTech.Application.Data;

/// <summary>
/// The response type for the "/manage/info" endpoints.
/// </summary>
public sealed class InfoResponse
{
    /// <summary>
    /// The user name associated with the authenticated user.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The email address associated with the authenticated user.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Indicates whether or not the <see cref="Email"/> has been confirmed yet.
    /// </summary>
    public required bool IsEmailConfirmed { get; init; }
}