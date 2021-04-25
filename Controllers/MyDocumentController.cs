using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EeshaProperty.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using EeshaProperty.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using EeshaProperty.Data;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EeshaProperty.Controllers
{
    [Authorize(Roles = "admin")]
    public class MyDocumentController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public MyDocumentController(ILogger<HomeController> logger,
                                ApplicationDbContext db,
                                IWebHostEnvironment webHostEnvironment,
                                UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (await _userManager.IsInRoleAsync(user, "admin"))
            {
                var myDocuments = _db.MyDocuments.ToList();
                return View(myDocuments);
            }
            else
            {
                var myDocuments = _db.MyDocuments.Where(p => p.EmployeeCode == user.EmployeeCode).ToList();
                return View(myDocuments);
            }
        }

        [HttpGet]
        public async Task<ActionResult> UserDownload(int? id)
        {
            //var user = await _userManager.GetUserAsync(User);
            if (id == null)
            {
                ModelState.AddModelError(string.Empty, "File not available");
                return View();
            }

            if (!_db.MyDocuments.Any(p => p.Id == id))
            {
                ModelState.AddModelError(string.Empty, "File not available");
                var myDocuments = _db.MyDocuments.ToList();

                return View("Index", myDocuments);
            }

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "mydocuments/");
            string fileName = (await _db.MyDocuments.FirstOrDefaultAsync(p => p.Id == id)).URL;
            if (fileName.ToLower().EndsWith(".pdf"))
            {
                return File(new FileStream(filePath + fileName, FileMode.Open), "application/pdf", "Document.pdf");
            }
            else if (fileName.ToLower().EndsWith(".jpeg") || fileName.ToLower().EndsWith(".jpg"))
            {
                return File(new FileStream(filePath + fileName, FileMode.Open), "image/jpeg", "Document.jpeg");
            }
            else if (fileName.ToLower().EndsWith(".png"))
            {
                return File(new FileStream(filePath + fileName, FileMode.Open), "image/png", "Document.png");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "File not available");
                var myDocuments = _db.MyDocuments.ToList();

                return View("Index", myDocuments);
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Upload()
        {

            ViewBag.EmployeeCode = new SelectList(_userManager.Users.OrderBy(p => p.EmployeeCode), "EmployeeCode", "EmployeeCode");
            ViewBag.DocumentName = new SelectList(_db.DocumentLists.OrderBy(p => p.DocumentName), "DocumentName", "DocumentName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Upload(MyDocument model)
        {
            if (ModelState.IsValid)
            {
                bool exists = _db.MyDocuments.Any(p => p.EmployeeCode == model.EmployeeCode && p.DocumentName == model.DocumentName);
                if (exists)
                {
                    ViewBag.EmployeeCode = new SelectList(_userManager.Users.OrderBy(p => p.EmployeeCode), "EmployeeCode", "EmployeeCode", model.EmployeeCode);
                    ViewBag.DocumentName = new SelectList(_db.DocumentLists.OrderBy(p => p.DocumentName), "DocumentName", "DocumentName", model.DocumentName);
                    ModelState.AddModelError(string.Empty, "Selected document already exists for this user.");
                    return View(model);
                }

                string uniqueFiles = UploadedFile(model);

                MyDocument docs = new MyDocument
                {
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    EmployeeCode = model.EmployeeCode,
                    DocumentName = model.DocumentName,

                    URL = uniqueFiles
                };

                _db.MyDocuments.Add(docs);

                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.EmployeeCode = new SelectList(_userManager.Users.OrderBy(p => p.EmployeeCode), "EmployeeCode", "EmployeeCode", model.EmployeeCode);
            ViewBag.DocumentName = new SelectList(_db.DocumentLists.OrderBy(p => p.DocumentName), "DocumentName", "DocumentName", model.DocumentName);

            return View(model);
        }

        private string UploadedFile(MyDocument model)
        {
            string uniqueFileName = null;

            if (model.DocumentFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "mydocuments/");

                if (!System.IO.Directory.Exists(uploadsFolder))
                    System.IO.Directory.CreateDirectory(uploadsFolder);

                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.DocumentFile.FileName;

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.DocumentFile.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Verification()
        {
            var model = await LoadRecords();
            return View(model);
        }

        public async Task<ActionResult> AllRecords()
        {
            var model = await LoadRecords(true);
            return View("Verification", model);
        }

        public async Task<List<UserDocumentViewModel>> LoadRecords(bool? IsAllRecords = null)
        {
            List<UserDocumentViewModel> model = new List<UserDocumentViewModel>();
            foreach (var user in _userManager.Users)
            {
                var docs = await _db.MyDocuments?.Where(p => p.EmployeeCode == user.EmployeeCode).ToListAsync();
                string path = Path.Combine(_webHostEnvironment.WebRootPath, "/mydocuments/");
                string prof = Path.Combine(_webHostEnvironment.WebRootPath, "/profileimages/");

                UserDocumentViewModel userDocument = new UserDocumentViewModel();
                if(user.ProfileImage!=null)
                user.ProfileImage = prof + user.ProfileImage;
                else
                user.ProfileImage =null;

                userDocument.Users = user;
                
                // if (docs.Any(p => p.DocumentName == Documents.PANCARD.ToString()))
                // {
                //     userDocument.PanUrl = path + docs.FirstOrDefault(p => p.DocumentName == Documents.PANCARD.ToString()).URL;
                //     isAnyDoc = true;
                // }

                // if (docs.Any(p => p.DocumentName == Documents.AADHAARCARD.ToString()))
                // {
                //     userDocument.AadhaarUrl = path + docs.FirstOrDefault(p => p.DocumentName == Documents.AADHAARCARD.ToString()).URL;
                //     isAnyDoc = true;
                // }

                // if (docs.Any(p => p.DocumentName == Documents.HIGHESTQUALIFICATION.ToString()))
                // {
                //     userDocument.DegreeUrl = path + docs.FirstOrDefault(p => p.DocumentName == Documents.HIGHESTQUALIFICATION.ToString()).URL;
                //     isAnyDoc = true;
                // }

                if(IsAllRecords == true)
                {
                    model.Add(userDocument);
                }
                    
            }

            return model;
        }

        [HttpGet, ActionName("Delete")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteConfirmedAssort(int id)
        {
            MyDocument myDocument = await _db.MyDocuments.FindAsync(id);

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "mydocuments/", myDocument.URL);

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            _db.MyDocuments.Remove(myDocument);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


    }
}
