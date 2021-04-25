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
using EeshaProperty.Services;
using ExcelDataReader;
using System.Net;
using AutoMapper.Configuration;

namespace EeshaProperty.Controllers
{
    [Authorize]
    public class EnquiryController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMailService _emailSender;
        private static int count = 0;
        private static bool Inserted = false;
        public Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public EnquiryController(ILogger<HomeController> logger,
                                ApplicationDbContext repository,
                                IWebHostEnvironment webHostEnvironment,
                                UserManager<ApplicationUser> userManager,
                                IMailService emailSender,
                                Microsoft.Extensions.Configuration.IConfiguration config)
        {
            _logger = logger;
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = config;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userrole = await _userManager.IsInRoleAsync(user, "admin");

            if (userrole)
            {
                var Enquiries = await _repository.Enquiries.ToListAsync();
                return View(Enquiries);
            }
            else
            {
                var Enquiries = await _repository.Enquiries.Where(p => p.EmpCode == user.EmployeeCode).ToListAsync();
                return View(Enquiries);
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult EmailCampain()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult SMSCampain()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SendEmails(string subject, string mailContent)
        {
            var enquiries = await _repository.Enquiries.ToListAsync();

            foreach (var e in enquiries)
            {
                if (subject != null && mailContent != null)
                {
                    _emailSender.SendEmail(Mail.DNR, e.Email, e.FullName, subject, mailContent);
                }
            }

            return Json("mail has been sent");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SendSMS(string message)
        {
            var enquiries = await _repository.Enquiries.ToListAsync();

            foreach (var e in enquiries)
            {
                var PhoneNumber = e.PhoneNumber;
                var AlternateNo = e.AlternateNo;
                
                var sURL = _configuration["SmsUrl"].ToString() +
                "username=" + _configuration["SMSUser"].ToString() +
                "&message=" + message +
                "&sendername=" + _configuration["SenderName"].ToString() +
                "&smstype=" + _configuration["SmsType"].ToString() +
                "&numbers=" + PhoneNumber +
                "&apikey=" + _configuration["ApiKey"].ToString();

                try
                {
                    using (WebClient client = new WebClient())
                    {

                        string s = client.DownloadString(sURL);

                        var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(s);
                        int n = responseObject.Status;
                        // if (n == 3)
                        // {
                        //     Json("Message does not Send Successfully due to invalid credentials");
                        // }
                        // else
                        // {
                        //     Json("Message Send Successfully !");                        
                        // }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error in sending Message");
                    ex.ToString();
                }

            }

            return Json("sms has been sent");
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.StateId = new SelectList(_repository.States, "Id", "StateName");
            return View();
        }

        [HttpGet]
        public JsonResult LoadCities(int id)
        {
            var cities = _repository.Cities.Where(p => p.StateId == id).ToList();
            return Json(new SelectList(cities, "Id", "CityName"));
        }

        [HttpPost]
        public async Task<IActionResult> Create(Enquiry enquiry)
        {
            if (ModelState.IsValid)
            {
                // if (enquiry.CityId <= 0 || enquiry?.CityId == null)
                // {
                //     ModelState.AddModelError(string.Empty, "Please select city and try again.");
                //     ViewBag.StateId = new SelectList(_repository.States, "Id", "StateName", enquiry.StateId);
                //     ViewBag.CityId = new SelectList(_repository.Cities, "Id", "CityName", enquiry.CityId);
                //     return View(enquiry);
                // }

                var user = await _userManager.GetUserAsync(User);

                enquiry.CreatedBy = user.UserName;
                enquiry.Username = user.UserName;
                enquiry.CreatedDate = DateTime.Now;
                enquiry.EmpCode = user.EmployeeCode;

                _repository.Enquiries.Add(enquiry);
                await _repository.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewBag.StateId = new SelectList(_repository.States, "Id", "StateName", enquiry.StateId);
            ViewBag.CityId = new SelectList(_repository.Cities, "Id", "CityName", enquiry.CityId);
            return View(enquiry);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NoContent();
            }
            Enquiry enquiry = await _repository.Enquiries.FindAsync(id);
            return View(enquiry);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NoContent();
            }

            Enquiry enquiry = await _repository.Enquiries.FindAsync(id);

            ViewBag.StateId = new SelectList(_repository.States, "Id", "StateName", enquiry.StateId);
            ViewBag.CityId = new SelectList(_repository.Cities.Where(p => p.StateId == enquiry.StateId), "Id", "CityName", enquiry.CityId);

            return View(enquiry);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Enquiry enquiry)
        {
            if (ModelState.IsValid)
            {
                // if (enquiry.CityId <= 0 || enquiry?.CityId == null)
                // {
                //     ModelState.AddModelError(string.Empty, "Please select city and try again.");
                //     ViewBag.StateId = new SelectList(_repository.States, "Id", "StateName", enquiry.StateId);
                //     ViewBag.CityId = new SelectList(_repository.Cities, "Id", "CityName", enquiry.CityId);
                //     return View(enquiry);
                // }

                var user = await _userManager.GetUserAsync(User);

                enquiry.UpdatedBy = user.UserName;
                enquiry.UpdatedDate = DateTime.Now;

                _repository.Entry(enquiry).State = EntityState.Modified;
                await _repository.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.StateId = new SelectList(_repository.States, "Id", "StateName", enquiry.StateId);
            ViewBag.CityId = new SelectList(_repository.Cities, "Id", "CityName", enquiry.CityId);
            return View(enquiry);
        }

        //[Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult BulkUpload()
        {
            if (Inserted)
            {
                if (count <= 0)
                    ViewBag.RecordSaved = "No records has been inserted";
                else
                    ViewBag.RecordSaved = count + " Records successfully inserted";

                count = 0;
                Inserted = false;
            }

            return View();
        }

        //[Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> BulkUpload(EnquiryViewModel model)
        {
            if (ModelState.IsValid)
            {
                // if (model.Month == "" || model.Month == null)
                // {
                //     ModelState.AddModelError(string.Empty, "Please select month and try again.");
                //     return View(model);
                // }

                // if (model.Year < 2019)
                // {
                //     ModelState.AddModelError(string.Empty, "Please select year and try again.");
                //     return View(model);
                // }

                string directory = Path.Combine(_webHostEnvironment.WebRootPath, "files");

                if (!System.IO.Directory.Exists(directory))
                    System.IO.Directory.CreateDirectory(directory);

                string fileName = directory + "\\" + model.ExcelFile.FileName;

                using (FileStream fileStream = System.IO.File.Create(fileName))
                {
                    model.ExcelFile.CopyTo(fileStream);
                    fileStream.Flush();
                }

                int cnt = 0;

                var enquiries = await this.GetRecAsync(model);
                foreach (var enquiry in enquiries)
                {
                    var exists = _repository.Enquiries.Any(p =>
                     p.PhoneNumber == enquiry.PhoneNumber ||
                     p.Email == enquiry.Email);

                    if (!exists)
                    {
                        _repository.Enquiries.Add(enquiry);
                        await _repository.SaveChangesAsync();
                        cnt++;
                    }
                }

                Inserted = true;
                count = cnt;
                return RedirectToAction(nameof(BulkUpload));
            }

            return View(model);
        }

        public async Task<List<Enquiry>> GetRecAsync(EnquiryViewModel model)
        {
            List<Enquiry> enquiries = new List<Enquiry>();

            var user = await _userManager.GetUserAsync(User);

            var fileName = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\files"}" + "\\" + model.ExcelFile.FileName;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        if(reader.GetValue(0)!=null)
                        if (!reader.GetValue(0).ToString().Contains("SNO"))
                        {
                            if(reader.GetValue(4)==null)
                                continue;

                            bool exists = _repository.Enquiries.Any(p =>
                            p.PhoneNumber == reader.GetValue(4).ToString());

                            if (exists)
                                continue;

                            // if (!_repository.Cities.Any(p => p.CityName == reader.GetValue(6).ToString()))
                            //     continue;

                            var enquiry = new Enquiry();
                            
                            if(reader.GetValue(1)!=null)
                                enquiry.FirstName = reader.GetValue(1).ToString();
                            
                            if(reader.GetValue(2)!=null)
                                enquiry.LastName = reader.GetValue(2).ToString();
                            
                            if(reader.GetValue(3)!=null)
                                enquiry.Email = reader.GetValue(3).ToString();
                            
                            if(reader.GetValue(4)!=null)
                                enquiry.PhoneNumber = reader.GetValue(4).ToString();
                            
                            if(reader.GetValue(5)!=null)
                                enquiry.AlternateNo = reader.GetValue(5).ToString();
                            
                            if(reader.GetValue(6)!=null)
                                enquiry.Occupation = reader.GetValue(6).ToString();
                            
                            if(reader.GetValue(7)!=null)
                            {
                                string cityName = reader.GetValue(7).ToString().ToLower();
                                var city = await _repository.Cities.FirstOrDefaultAsync(p => p.CityName.ToLower() == cityName);
                                
                                if(city == null)
                                    continue;

                                enquiry.CityId = city.Id;
                                enquiry.StateId = city.StateId;
                            }                                
                                                                                    
                            enquiry.CreatedBy = user.UserName;
                            enquiry.CreatedDate = DateTime.Now;
                            enquiry.Username = user.UserName;
                            enquiry.EmpCode = user.EmployeeCode;
                            
                            enquiries.Add(enquiry);
                        }
                    }
                }
            }
            return enquiries;
        }


        [HttpGet, ActionName("Delete")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteConfirmedAssort(int id)
        {
            Enquiry enquiry = await _repository.Enquiries.FindAsync(id);

            _repository.Enquiries.Remove(enquiry);
            await _repository.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }

    public class Response
    {
        public string message_id { get; set; }
        public int message_count { get; set; }
        public double price { get; set; }
    }
    public class RootObject
    {
        public Response Response { get; set; }
        public string ErrorMessage { get; set; }
        public int Status { get; set; }
    }
}

