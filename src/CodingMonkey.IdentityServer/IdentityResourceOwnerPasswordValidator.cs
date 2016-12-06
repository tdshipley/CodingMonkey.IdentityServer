namespace CodingMonkey.IdentityServer
{
    using System.Threading.Tasks;
    using IdentityServer4.Validation;
    using Microsoft.AspNetCore.Identity;
    using static IdentityModel.OidcConstants;
    using IdentityServer4.Models;

    public class IdentityResourceOwnerPasswordValidator<TUser> : IResourceOwnerPasswordValidator where TUser : class
    {
        private readonly UserManager<TUser> _userManager;
        private readonly SignInManager<TUser> _signInManager;

        public IdentityResourceOwnerPasswordValidator(UserManager<TUser> userManager, SignInManager<TUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = await _userManager.FindByNameAsync(context.UserName);
            if (user != null)
            {
                if (await _signInManager.CanSignInAsync(user))
                {
                    if (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user))
                    {
                        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, $"The user: {context.UserName} is lockout");
                    }
                    else if (await _userManager.CheckPasswordAsync(user, context.Password))
                    {
                        if (_userManager.SupportsUserLockout)
                        {
                            await _userManager.ResetAccessFailedCountAsync(user);
                        }

                        var userId = await _userManager.GetUserIdAsync(user);
                        context.Result = new GrantValidationResult(userId, AuthenticationMethods.Password);
                    }
                    else
                    {
                        int code;
                        if (int.TryParse(context.Password, out code) && _userManager.SupportsUserPhoneNumber)
                        {
                            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                            if (await _userManager.VerifyChangePhoneNumberTokenAsync(user, context.Password, phoneNumber))
                            {
                                if (_userManager.SupportsUserLockout)
                                {
                                    await _userManager.ResetAccessFailedCountAsync(user);
                                }
                                var sub = await _userManager.GetUserIdAsync(user);

                                context.Result = new GrantValidationResult(sub, AuthenticationMethods.Password);
                            }
                        }
                        else if (_userManager.SupportsUserLockout)
                        {
                            await _userManager.AccessFailedAsync(user);
                        }
                    }
                }
            }
        }
    }
}
