namespace FinTech.Application.Data;

/// <summary>
/// The request type for the "/register".
/// </summary>
public sealed class RegisterRequest
{
    /// <summary>
    /// The user's name
    /// </summary>
    public required string UserName { get; init; }

    /// <summary>
    /// The user's email address
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// The user's password.
    /// </summary>
    public required string Password { get; init; }
}
