using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EeshaProperty.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using EeshaProperty.Areas.Identity.Data;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using EeshaProperty.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using EeshaProperty.Services;
using Microsoft.AspNetCore.Http;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace EeshaProperty.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMailService _emailSender;

        public HomeController(ILogger<HomeController> logger,
                            UserManager<ApplicationUser> userManager,
                            SignInManager<ApplicationUser> signInManager,
                            IWebHostEnvironment webHostEnvironment,
                            ApplicationDbContext db,
                            IMailService emailSender)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _db = db;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            ApplicationUserViewModel model = new ApplicationUserViewModel();

            var user = await _userManager.GetUserAsync(User);

            if (user != null)
                model.User = user;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    if (model.Avatar != null)
                    {
                        string filePath = user.ProfileImage == null ? null : Path.Combine(_webHostEnvironment.WebRootPath, "profileimages/", user.ProfileImage);

                        if (System.IO.File.Exists(filePath))
                            System.IO.File.Delete(filePath);

                        user.ProfileImage = UploadedFile(model);
                    }

                    user.FirstName = model.FirstName;
                    user.MiddleName = model.MiddleName;
                    user.LastName = model.LastName;
                    user.Gender = model.Gender;
                    user.DateOfBirth = model.DateOfBirth;
                    user.FatherName = model.FatherName;
                    user.Address = model.Address;
                    user.PhoneNumber = model.PhoneNumber;
                    user.IsActive = true;

                    if (user.PAN != model.PAN)
                    {
                        user.PAN = model.PAN;
                    }

                    if (user.AadhaarNo != model.AadhaarNo)
                    {
                        user.AadhaarNo = model.AadhaarNo;
                    }

                    if (user.HighestQualification != model.HighestQualification)
                    {
                        user.HighestQualification = model.HighestQualification;
                    }

                    await _db.SaveChangesAsync();

                    CookieOptions option = new CookieOptions();
                    string path = Path.Combine(_webHostEnvironment.WebRootPath, "/profileimages/");

                    if (user.ProfileImage != null)
                        Response.Cookies.Append("userProf", path + user.ProfileImage, option);
                    else
                        Response.Cookies.Append("userProf", "", option);

                    if (user.Gender.ToLower() == "male")
                    {
                        Response.Cookies.Append("gender", "0", option);
                    }
                    else if (user.Gender.ToLower() == "female")
                    {
                        Response.Cookies.Append("gender", "1", option);
                    }
                    else
                    {
                        Response.Cookies.Append("gender", "1", option);
                    }
                }

                return RedirectToAction("Index");
            }

            return View(user);
        }

        private async Task<bool> SaveDocument(Documents doc, IFormFile file, string empCode)
        {
            if (file == null)
            {
                return false;
            }
            string docName = "";

            switch (doc)
            {
                case Documents.PANCARD:
                    docName = "PANCARD";
                    break;
                case Documents.AADHAARCARD:
                    docName = "AADHAARCARD";
                    break;
                case Documents.HIGHESTQUALIFICATION:
                    docName = "HIGHESTQUALIFICATION";
                    break;
                default:
                    docName = "";
                    break;

            }

            if (docName == "")
                return false;

            string uniqueFiles = UploadedDocumentFile(file);

            MyDocument docs = new MyDocument
            {
                CreatedBy = User.Identity.Name,
                CreatedDate = DateTime.Now,
                EmployeeCode = empCode,
                DocumentName = docName,

                URL = uniqueFiles
            };

            _db.MyDocuments.Add(docs);
            await _db.SaveChangesAsync();
            return true;
        }
        private string UploadedDocumentFile(IFormFile file)
        {
            string uniqueFileName = null;

            if (file != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "mydocuments/");

                if (!System.IO.Directory.Exists(uploadsFolder))
                    System.IO.Directory.CreateDirectory(uploadsFolder);

                uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult ManageEmployee()
        {
            var users = _userManager.Users;
            return View(users);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EditEmployee(string id = null)
        {
            if (id == null)
            {
                return NoContent();
            }

            var user = await _userManager.FindByIdAsync(id);
            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EditEmployee(ApplicationUser model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            var user = await _userManager.FindByIdAsync(model.Id);

            if (ModelState.IsValid)
            {
                if (model.Email == "" || model.Email == null)
                {
                    ModelState.AddModelError(string.Empty, "Please enter email address.");
                    return View(model);
                }

                if (user != null)
                {
                    //var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    //await _userManager.ResetPasswordAsync(user, token, "Abc123#$%");

                    if (user.Email != model.Email)
                    {
                        user.Email = model.Email;
                        user.EmailConfirmed = true;
                    }

                    if (user.UserName != model.UserName)
                    {
                        var setUserNameResult = await _userManager.SetUserNameAsync(user, model.UserName);
                        if (!setUserNameResult.Succeeded)
                        {
                            return View(model);
                        }
                    }

                    user.EmployeeCode = model.EmployeeCode;

                    user.PhoneNumber = model.PhoneNumber;
                    user.Designation = model.Designation;
                    user.DateofJoining = model.DateofJoining;
                    user.IsActive = model.IsActive;
                    user.Gender = model.Gender;
                    user.FirstName = model.FirstName;
                    user.MiddleName = model.MiddleName;
                    user.LastName = model.LastName;


                    await _db.SaveChangesAsync();
                }

                return RedirectToAction("ManageEmployee", _userManager.Users);
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SendEmailVerificationCode(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                //var code = await _userManager.GenerateChangeEmailTokenAsync(user, user.Email);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = code },
                protocol: Request.Scheme);

                string htmlBody = "<p>Dear " + user.FullName + $@"</p>
                                    <p>Your Password is : Abc123#$%</p><p>Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.</<p>";

                _emailSender.SendEmail(
                    Mail.DNR,
                   user.Email,
                   user.FullName,
                   "Confirm your email", htmlBody);
                //$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            }

            TempData["StatusMessage"] = "Email has been sent";
            var users = _userManager.Users;
            return RedirectToAction("ManageEmployee", users);
        }


        public async Task<string> ChangeEmail(ApplicationUser oldUser, ApplicationUser model)
        {
            //var user = await _userManager.GetUserAsync(User);
            if (model == null)
            {
                return $"Unable to load user with ID '{model.Id}'.";
            }

            var email = await _userManager.GetEmailAsync(oldUser);
            if (model.Email != email)
            {
                var userId = await _userManager.GetUserIdAsync(model);
                var code = await _userManager.GenerateChangeEmailTokenAsync(model, model.Email);

                var callbackUrl = Url.Page(
                    "~/Identity/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { userId = userId, email = model.Email, code = code },
                    protocol: Request.Scheme);

                _emailSender.SendEmail(
                    Mail.DNR,
                   model.Email,
                   model.Email,
                   "Confirm your email",
                   $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return "mail sent";
            }

            return "Your email is unchanged.";
        }

        private string UploadedFile(ApplicationUser model)
        {
            string uniqueFileName = null;

            if (model.Avatar != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "profileimages");

                if (!System.IO.Directory.Exists(uploadsFolder))
                    System.IO.Directory.CreateDirectory(uploadsFolder);

                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Avatar.FileName;

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Avatar.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        public IActionResult Privacy(string email, string name, string subject, string htmlBody)
        {
            _emailSender.SendEmail(Mail.DNR, email, name, subject, htmlBody);
            return View();
        }

        [Authorize(Roles = "admin")]
        public IActionResult NewEmployee()
        {
            return Redirect("/Identity/Account/Register");
        }

        [HttpGet, ActionName("DeleteEmployee")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteEmployee(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{user.Id}'.");
            }

            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user with ID '{userId}'.");
            }

            return RedirectToAction("ManageEmployee");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
