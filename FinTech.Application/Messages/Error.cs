using Microsoft.AspNetCore.Identity;

namespace FinTech.Application.Messages;

/// <summary>
/// Encapsulates an error for the API.
/// </summary>
public class Error
{
    /// <summary>
    /// Gets or sets the code for this error.
    /// </summary>
    /// <value>
    /// The code for this error.
    /// </value>
    public string Code { get; set; } = default!;

    /// <summary>
    /// Gets or sets the description for this error.
    /// </summary>
    /// <value>
    /// The description for this error.
    /// </value>
    public string Description { get; set; } = default!;
}

public class ErrorDescriber
{
    /// <summary>
    /// Returns an <see cref="Error"/>
    /// </summary>
    /// <returns>An <see cref="Error"/></returns>
    public static Error OldPasswordRequired()
    {
        return new Error
        {
            Code = nameof(OldPasswordRequired),
            Description = "The old password is required to set a new password. If the old password is forgotten, use /resetPassword."
        };
    }

    /// <summary>
    /// Returns an <see cref="Error"/>
    /// </summary>
    /// <returns>An <see cref="Error"/></returns>
    public static Error CannotResetSharedKeyAndEnable()
    {
        return new Error
        {
            Code = nameof(CannotResetSharedKeyAndEnable),
            Description = "Resetting the 2fa shared key must disable 2fa until a 2fa token based on the new shared key is validated."
        };
    }

    /// <summary>
    /// Returns an <see cref="Error"/>
    /// </summary>
    /// <returns>An <see cref="Error"/></returns>
    public static Error RequiresTwoFactor()
    {
        return new Error
        {
            Code = nameof(RequiresTwoFactor),
            Description = "No 2fa token was provided by the request. A valid 2fa token is required to enable 2fa."
        };
    }

    /// <summary>
    /// Returns an <see cref="Error"/>
    /// </summary>
    /// <returns>An <see cref="Error"/></returns>
    public static Error InvalidTwoFactorCode()
    {
        return new Error
        {
            Code = nameof(InvalidTwoFactorCode),
            Description = "The 2fa token provided by the request was invalid. A valid 2fa token is required to enable 2fa."
        };
    }
}