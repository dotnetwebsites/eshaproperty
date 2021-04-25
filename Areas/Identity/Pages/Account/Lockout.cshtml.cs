using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EeshaProperty.Areas.Identity.Pages.Account
{
    //[AllowAnonymous]
    [Authorize(Roles = "superadmin")]
    public class LockoutModel : PageModel
    {
        public void OnGet()
        {

        }
    }
}
