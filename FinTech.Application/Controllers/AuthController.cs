using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.WebUtilities;
using FinTech.Domain.Entities;
using FinTech.Application.Messages;
using System.Net;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Builder;
using System.Text.Encodings.Web;



namespace FinTech.Application.Controllers;

/**
 * Based on https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Core/src/IdentityApiEndpointRouteBuilderExtensions.cs
 */
[ApiController]
[Route("api/auth")]
public class AuthController(
    ILogger<AuthController> logger,
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    IUserStore<ApplicationUser> userStore,
    //IEmailSender<ApplicationUser> emailSender,
    LinkGenerator linkGenerator,
    IActionDescriptorCollectionProvider actionDescriptorCollectionProvider
    ) : ControllerBase
{
    private readonly ILogger<AuthController> _logger = logger;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUserStore<ApplicationUser> _userStore = userStore;
    //private readonly IEmailSender<ApplicationUser> _emailSender = emailSender;
    private readonly LinkGenerator _linkGenerator = linkGenerator;

    private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;

    private static readonly EmailAddressAttribute _emailAddressAttribute = new();

    private readonly string confirmEmailEndpointName = $"{nameof(AuthController)}-ConfirmEmaill";

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] Data.RegisterRequest registration)
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException($"{nameof(AuthController)} requires a user store with email support.");
        }

        var emailStore = (IUserEmailStore<ApplicationUser>)_userStore;
        var email = registration.Email;

        if (string.IsNullOrEmpty(email) || !_emailAddressAttribute.IsValid(email))
        {
            return BadRequest(_userManager.ErrorDescriber.InvalidEmail(email).ToError());
        }

        var user = new ApplicationUser();

        await _userStore.SetUserNameAsync(user, registration.UserName, CancellationToken.None);
        await emailStore.SetEmailAsync(user, email, CancellationToken.None);

        var result = await _userManager.CreateAsync(user, registration.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.ToString());
        }

        await SendConfirmationEmailAsync(user, Request.HttpContext, email);

        return Ok();
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] Data.LoginRequest login, [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies)
    {
        var useCookieScheme = (useCookies == true) || (useSessionCookies == true);
        var isPersistent = (useCookies == true) && (useSessionCookies != true);
        _signInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

        ApplicationUser? user;

        if (string.IsNullOrEmpty(login.Login) && _emailAddressAttribute.IsValid(login.Login))
            user = await _userManager.FindByEmailAsync(login.Login);
        else
            user = await _userManager.FindByNameAsync(login.Login);

        if (user is null) return Unauthorized();

        var result = await _signInManager.PasswordSignInAsync(user, login.Password, isPersistent, lockoutOnFailure: true);

        if (result.RequiresTwoFactor)
        {
            if (!string.IsNullOrEmpty(login.TwoFactorCode))
            {
                result = await _signInManager.TwoFactorAuthenticatorSignInAsync(login.TwoFactorCode, isPersistent, rememberClient: isPersistent);
            }
            else if (!string.IsNullOrEmpty(login.TwoFactorRecoveryCode))
            {
                result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(login.TwoFactorRecoveryCode);
            }
        }

        if (!result.Succeeded)
        {
            return Unauthorized(result.ToString());
        }

        // The signInManager already produced the needed response in the form of a cookie or bearer token.
        return NoContent();
    }

    //[HttpPost("refresh")]
    //[ProducesResponseType(StatusCodes.Status204NoContent)]
    //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
    //[AllowAnonymous]
    //public async Task<IActionResult> Refresh([FromBody] RefreshRequest refreshRequest)
    //{
    //    var refreshTokenProtector = bearerTokenOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
    //    var refreshTicket = refreshTokenProtector.Unprotect(refreshRequest.RefreshToken);

    //    // Reject the /refresh attempt with a 401 if the token expired or the security stamp validation fails
    //    if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc ||
    //        timeProvider.GetUtcNow() >= expiresUtc ||
    //        await _signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not ApplicationUser user)

    //    {
    //        return TypedResults.Challenge();
    //    }

    //    var newPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
    //    return TypedResults.SignIn(newPrincipal, authenticationScheme: IdentityConstants.BearerScheme);
    //}

    [HttpPost]
    [Route("confirmEmail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointName($"{nameof(AuthController)}-ConfirmEmaill")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string code, [FromQuery] string? changedEmail)
    {
        if (await _userManager.FindByIdAsync(userId) is not { } user)
        {
            // We could respond with a 404 instead of a 401 like Identity UI, but that feels like unnecessary information.
            return Unauthorized();
        }

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Unauthorized();
        }

        IdentityResult result;

        if (string.IsNullOrEmpty(changedEmail))
        {
            result = await _userManager.ConfirmEmailAsync(user, code);
        }
        else
        {
            // As with Identity UI, email and user name are one and the same. So when we update the email,
            // we need to update the user name.
            result = await _userManager.ChangeEmailAsync(user, changedEmail, code);

            if (result.Succeeded)
            {
                result = await _userManager.SetUserNameAsync(user, changedEmail);
            }
        }

        if (!result.Succeeded)
        {
            return Unauthorized();
        }

        return Ok();
    }

    [HttpPost("resendConfirmationEmail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [AllowAnonymous]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailRequest resendRequest)
    {
        if (await _userManager.FindByEmailAsync(resendRequest.Email) is not { } user)
        {
            return Ok();
        }

        await SendConfirmationEmailAsync(user, Request.HttpContext, resendRequest.Email);
        return Ok();
    }

    //[HttpPost("forgotPassword")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[AllowAnonymous]
    //public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest resetRequest)
    //{
    //    var user = await _userManager.FindByEmailAsync(resetRequest.Email);

    //    if (user is not null && await _userManager.IsEmailConfirmedAsync(user))
    //    {
    //        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
    //        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

    //        await _emailSender.SendPasswordResetCodeAsync(user, resetRequest.Email, HtmlEncoder.Default.Encode(code));
    //    }

    //    // Don't reveal that the user does not exist or is not confirmed, so don't return a 200 if we would have
    //    // returned a 400 for an invalid code given a valid user email.
    //    return Ok();
    //}

    [HttpPost("resetPassword")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest resetRequest)
    {
        var user = await _userManager.FindByEmailAsync(resetRequest.Email);

        if (user is null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
            // Don't reveal that the user does not exist or is not confirmed, so don't return a 200 if we would have
            // returned a 400 for an invalid code given a valid user email.
            return BadRequest(_userManager.ErrorDescriber.InvalidToken().ToError());
        }

        IdentityResult result;
        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetRequest.ResetCode));
            result = await _userManager.ResetPasswordAsync(user, code, resetRequest.NewPassword);
        }
        catch (FormatException)
        {
            return BadRequest(_userManager.ErrorDescriber.InvalidToken().ToError());
        }

        if (!result.Succeeded)
        {
            return BadRequest(result.ToString());
        }

        return Ok();
    }

    [HttpPost("manage/2fa")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<ActionResult<TwoFactorResponse>> Manage2fa([FromBody] TwoFactorRequest tfaRequest)
    {
        var userManager = _signInManager.UserManager;
        if (await userManager.GetUserAsync(User) is not { } user)
        {
            return NotFound();
        }

        if (tfaRequest.Enable == true)
        {
            if (tfaRequest.ResetSharedKey)
            {
                return BadRequest(ErrorDescriber.CannotResetSharedKeyAndEnable());
            }
            else if (string.IsNullOrEmpty(tfaRequest.TwoFactorCode))
            {
                return BadRequest(ErrorDescriber.RequiresTwoFactor());
            }
            else if (!await userManager.VerifyTwoFactorTokenAsync(user, userManager.Options.Tokens.AuthenticatorTokenProvider, tfaRequest.TwoFactorCode))
            {
                return BadRequest(ErrorDescriber.InvalidTwoFactorCode());
            }

            await userManager.SetTwoFactorEnabledAsync(user, true);
        }
        else if (tfaRequest.Enable == false || tfaRequest.ResetSharedKey)
        {
            await userManager.SetTwoFactorEnabledAsync(user, false);
        }

        if (tfaRequest.ResetSharedKey)
        {
            await userManager.ResetAuthenticatorKeyAsync(user);
        }

        string[]? recoveryCodes = null;
        if (tfaRequest.ResetRecoveryCodes || (tfaRequest.Enable == true && await userManager.CountRecoveryCodesAsync(user) == 0))
        {
            var recoveryCodesEnumerable = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            recoveryCodes = recoveryCodesEnumerable?.ToArray();
        }

        if (tfaRequest.ForgetMachine)
        {
            await _signInManager.ForgetTwoFactorClientAsync();
        }

        var key = await userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(key))
        {
            await userManager.ResetAuthenticatorKeyAsync(user);
            key = await userManager.GetAuthenticatorKeyAsync(user);

            if (string.IsNullOrEmpty(key))
            {
                throw new NotSupportedException("The user manager must produce an authenticator key after reset.");
            }
        }

        return Ok(new TwoFactorResponse
        {
            SharedKey = key,
            RecoveryCodes = recoveryCodes,
            RecoveryCodesLeft = recoveryCodes?.Length ?? await userManager.CountRecoveryCodesAsync(user),
            IsTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user),
            IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
        });
    }

    [HttpGet("manage/info")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<ActionResult<Data.InfoResponse>> ManageGetInfo()
    {
        if (await _userManager.GetUserAsync(User) is not { } user)
        {
            return NotFound();
        }

        return Ok(new
        {
            Name = await _userManager.GetUserNameAsync(user) ?? throw new NotSupportedException("Users must have a name."),
            Email = await _userManager.GetEmailAsync(user) ?? throw new NotSupportedException("Users must have an email."),
            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user),
        });
    }

    [HttpPost("manage/info")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<ActionResult<Data.InfoResponse>> ManagePostInfo([FromBody] InfoRequest infoRequest)
    {
        if (await _userManager.GetUserAsync(User) is not { } user)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(infoRequest.NewEmail) && !_emailAddressAttribute.IsValid(infoRequest.NewEmail))
        {
            return BadRequest(_userManager.ErrorDescriber.InvalidEmail(infoRequest.NewEmail).ToError());
        }

        if (!string.IsNullOrEmpty(infoRequest.NewPassword))
        {
            if (string.IsNullOrEmpty(infoRequest.OldPassword))
            {
                return BadRequest(ErrorDescriber.OldPasswordRequired());
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, infoRequest.OldPassword, infoRequest.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                return BadRequest(changePasswordResult);
            }
        }

        if (!string.IsNullOrEmpty(infoRequest.NewEmail))
        {
            var email = await _userManager.GetEmailAsync(user);

            if (email != infoRequest.NewEmail)
            {
                await SendConfirmationEmailAsync(user, Request.HttpContext, infoRequest.NewEmail, isChange: true);
            }
        }

        return Ok(new
        {
            Name = await _userManager.GetUserNameAsync(user) ?? throw new NotSupportedException("Users must have a name."),
            Email = await _userManager.GetEmailAsync(user) ?? throw new NotSupportedException("Users must have an email."),
            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user),
        });
    }

    async Task SendConfirmationEmailAsync(ApplicationUser user, HttpContext context, string email, bool isChange = false)
    {
        var code = isChange
            ? await _userManager.GenerateChangeEmailTokenAsync(user, email)
            : await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var userId = await _userManager.GetUserIdAsync(user);
        var routeValues = new RouteValueDictionary()
        {
            ["userId"] = userId,
            ["code"] = code,
        };

        if (isChange)
        {
            // This is validated by the /confirmEmail endpoint on change.
            routeValues.Add("changedEmail", email);
        }

        var confirmEmailUrl = _linkGenerator.GetUriByName(context, confirmEmailEndpointName, routeValues)
            ?? throw new NotSupportedException($"Could not find endpoint named '{confirmEmailEndpointName}'.");

        //await _emailSender.SendConfirmationLinkAsync(user, email, HtmlEncoder.Default.Encode(confirmEmailUrl));
    }
}