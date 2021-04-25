using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using EeshaProperty.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EeshaProperty.Utilities
{

    public class UserManagerExtension : UserManager<ApplicationUser>
    {
        public UserManagerExtension(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {

        }

        public async Task<string> GetNameAsync(ClaimsPrincipal principal)
        {
            var user = await GetUserAsync(principal);
            return user.UserName;
        }

    }

}