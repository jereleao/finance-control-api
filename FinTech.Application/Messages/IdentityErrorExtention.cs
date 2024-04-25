using Microsoft.AspNetCore.Identity;

namespace FinTech.Application.Messages;

public static class IdentityErrorExtention
{
    public static Error ToError (this IdentityError error)
    {
        return new Error
        {
            Code = $"IdentityError: {error.Code}",
            Description = error.Description,
        };
    }
}
