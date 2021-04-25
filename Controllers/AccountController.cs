using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EeshaProperty.Areas.Identity.Data;
using EeshaProperty.Data;
using EeshaProperty.Models;
using EeshaProperty.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace EeshaProperty.Controllers
{
    [Authorize(Roles = "admin")]
    public class AccountController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMailService _emailSender;
        public AccountController(ApplicationDbContext db,
                                IWebHostEnvironment webHostEnvironment,
                                UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager,
                                IMailService emailSender)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ChangeEmail(string id)
        {
            //var user = await _userManager.GetUserAsync(User);
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                //return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                return NotFound($"Unable to load user with ID '{id}'.");
            }

            var changeEmailModel = new ChangeEmailModel()
            {
                Id = user.Id,
                Email = await _userManager.GetEmailAsync(user),
                IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user)
            };

            //var email = await _userManager.GetEmailAsync(user);
            //IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            return View(changeEmailModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeEmail(ChangeEmailModel Input)
        {
            var user = await _userManager.FindByIdAsync(Input.Id);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{Input.Id}'.");
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.NewEmail != email)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);

                // var callbackUrl = Url.Page(
                //     "/Account/ConfirmEmailChange",
                //     pageHandler: null,
                //     values: new { userId = userId, email = Input.NewEmail, code = code },
                //     protocol: Request.Scheme);

                var callbackUrl = Url.Action("ConfirmEmail", "Account",
                    values: new { userId = userId, email = Input.NewEmail, code = code },
                    protocol: Request.Scheme);


                _emailSender.SendEmail(
                    Mail.DNR,
                    Input.NewEmail,
                    Input.NewEmail,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{user.Id}'.");
                }

                return View(Input);
            }

            return View(Input);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string email, string code)
        {
            if (userId == null || email == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            //code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ChangeEmailAsync(user, email, code);
            if (!result.Succeeded)
            {
                return View();
            }

            // In our UI email and user name are one and the same, so when we update the email
            // we need to update the user name.
            var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
            if (!setUserNameResult.Succeeded)
            {
                return View();
            }

            await _signInManager.RefreshSignInAsync(user);
            return View();
        }

        [HttpGet]
        public IActionResult SendEmailConfirmLink()
        {
            // if (User.Identity.Name != null)
            // {
            //     return Redirect("/");
            // }

            SendEmailConfirmLinkViewModel model = new SendEmailConfirmLinkViewModel();

            model.IsSuccess = false;
            ModelState.AddModelError(string.Empty, $"Your email address not confirmed yet!");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SendEmailConfirmLink(SendEmailConfirmLinkViewModel model)
        {
            if (ModelState.IsValid)
            {
                var findByEmail = await _userManager.FindByEmailAsync(model.Value);

                if (findByEmail != null)
                {
                    if (findByEmail.EmailConfirmed)
                    {
                        model.IsSuccess = true;
                        ModelState.AddModelError(string.Empty, $"Email already confirmed");
                        return View(model);
                    }

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(findByEmail);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = findByEmail.Id, code = code },
                    protocol: Request.Scheme);

                    //MailContent mail = new MailContent("Samyra Global", "Please confirm your Email address", HtmlEncoder.Default.Encode(callbackUrl), "Click here");

                    _emailSender.SendEmail(Mail.DNR, findByEmail.Email, findByEmail.FullName, "Confirm your email",
                    $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}' target='_blank'>Click here</a>");

                    //var result = await _repository.GenerateLogTokenAsync(TokenType.EMAIL, findByEmail.Id, findByEmail.Email, code);

                    model.Value = "";
                    model.IsSuccess = true;
                    ModelState.AddModelError(string.Empty, $"Confirmation link has been sent, please check your mailbox.");
                    return View(model);
                }
                else
                {
                    model.IsSuccess = false;
                    ModelState.AddModelError(string.Empty, $"The '{model.Value}' not found in database, please enter valid email address.");
                    return View(model);
                }

            }

            model.IsSuccess = false;
            ModelState.AddModelError(string.Empty, $"Your email address not confirmed yet!");
            return View(model);
        }

    }
}