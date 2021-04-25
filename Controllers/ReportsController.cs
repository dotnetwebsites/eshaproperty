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

namespace EeshaProperty.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMailService _emailSender;

        public ReportsController(ILogger<HomeController> logger,
                                ApplicationDbContext repository,
                                IWebHostEnvironment webHostEnvironment,
                                UserManager<ApplicationUser> userManager,
                                IMailService emailSender)
        {
            _logger = logger;
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // public async Task<IActionResult> Index()
        // {
        //     var enquiries = await _repository.Enquiries.ToListAsync();
        //     return View(enquiries);
        // }
        
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UserWise(string id = null)
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "admin");

            if (isAdmin)
            {
                if (id != null)
                {
                    ViewBag.EmployeeId = new SelectList(_userManager.Users.OrderBy(p => p.FirstName + " " + p.LastName), "EmployeeCode", "EmpCodeName", id);
                    return View(await _repository.Enquiries.OrderBy(p => p.EmpCode).Where(p => p.EmpCode == id).ToListAsync());
                }
                else
                {
                    ViewBag.EmployeeId = new SelectList(_userManager.Users.OrderBy(p => p.FirstName + " " + p.LastName), "EmployeeCode", "EmpCodeName");
                    return View(await _repository.Enquiries.OrderBy(p => p.EmpCode).ToListAsync());
                }
            }
            else
            {
                if (id != null)
                {
                    ViewBag.EmployeeId = new SelectList(_userManager.Users.Where(p => p.UserName == user.UserName).OrderBy(p => p.FirstName + " " + p.LastName), "EmployeeCode", "EmpCodeName", id);
                    return View(await _repository.Enquiries.Where(p => p.Username == User.Identity.Name).OrderBy(p => p.EmpCode).Where(p => p.EmpCode == id).ToListAsync());
                }
                else
                {
                    ViewBag.EmployeeId = new SelectList(_userManager.Users.Where(p => p.UserName == user.UserName), "EmployeeCode", "EmpCodeName");
                    return View(await _repository.Enquiries.Where(p => p.Username == user.UserName).OrderBy(p => p.EmpCode).ToListAsync());
                }
            }
        }

        public async Task<IActionResult> Yearly(int? id = null)
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "admin");

            if (isAdmin)
            {
                if (id != null)
                {
                    ViewBag.Year = new SelectList(_repository.Enquiries.GroupBy(p => p.CreatedDate.Year), "Id", "CreatedDate");
                    return View(await _repository.Enquiries.Where(p => p.CreatedDate.Year == id).OrderBy(p => p.EmpCode).ToListAsync());
                }
                else
                {
                    ViewBag.Year = new SelectList(_repository.Enquiries, "Id", "CreatedDate");
                    return View(await _repository.Enquiries.OrderBy(p => p.EmpCode).ToListAsync());
                }
            }
            else
            {
                if (id != null)
                {
                    ViewBag.Year = new SelectList(_repository.Enquiries, "Id", "CreatedDate");
                    return View(await _repository.Enquiries.Where(p => p.CreatedDate.Year == id && p.Username == User.Identity.Name).OrderBy(p => p.EmpCode).ToListAsync());
                }
                else
                {
                    ViewBag.Year = new SelectList(_repository.Enquiries, "Id", "CreatedDate");
                    return View(await _repository.Enquiries.Where(p => p.Username == User.Identity.Name).OrderBy(p => p.EmpCode).ToListAsync());
                }
            }

        }

        public async Task<IActionResult> Weekly(string id = null)
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "admin");

            var endDate = Convert.ToDateTime(id);

            var defStartDate = DateTime.Today.AddDays(-7);
            if (isAdmin)
            {
                if (id != null)
                {
                    var startDate = endDate.AddDays(-7);
                    ViewBag.EmployeeId = new SelectList(_userManager.Users.OrderBy(p => p.FirstName + " " + p.LastName), "EmployeeCode", "EmpCodeName", id);

                    var model = await _repository.Enquiries.Where(p => p.CreatedDate.Date >= startDate.Date && p.CreatedDate.Date <= endDate.Date).OrderBy(p => p.EmpCode).ToListAsync();

                    return View(model);
                }
                else
                {
                    ViewBag.EmployeeId = new SelectList(_userManager.Users, "EmployeeCode", "EmpCodeName");
                    return View(await _repository.Enquiries.Where(p => p.CreatedDate.Date >= defStartDate.Date && p.CreatedDate.Date <= endDate.Date).OrderBy(p => p.EmpCode).ToListAsync());
                }
            }
            else
            {
                if (id != null)
                {
                    var startDate = endDate.AddDays(-7);
                    ViewBag.EmployeeId = new SelectList(_userManager.Users.Where(p => p.UserName == user.UserName).OrderBy(p => p.FirstName + " " + p.LastName), "EmployeeCode", "EmpCodeName", id);
                    var model = await _repository.Enquiries.Where(p => p.CreatedDate.Date >= startDate.Date && p.CreatedDate.Date <= endDate.Date && p.Username == User.Identity.Name).OrderBy(p => p.EmpCode).ToListAsync();
                    return View(model);
                }
                else
                {
                    ViewBag.EmployeeId = new SelectList(_userManager.Users.Where(p => p.UserName == user.UserName), "EmployeeCode", "EmpCodeName");
                    return View(await _repository.Enquiries.Where(p => p.CreatedDate.Date >= defStartDate.Date && p.CreatedDate.Date <= endDate.Date && p.Username == user.UserName).OrderBy(p => p.EmpCode).ToListAsync());
                }
            }
        }

        public async Task<IActionResult> Period(string fromdate = null, string todate = null)
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "admin");

            if (isAdmin)
            {
                if (fromdate != null && todate != null)
                {
                    var fDate = Convert.ToDateTime(fromdate);
                    var tDate = Convert.ToDateTime(todate);

                    var model = await _repository.Enquiries.Where(p => p.CreatedDate.Date >= fDate.Date && p.CreatedDate.Date <= tDate.Date).OrderBy(p => p.EmpCode).ToListAsync();

                    return View(model);
                }
                else
                {
                    return View(await _repository.Enquiries.OrderBy(p => p.EmpCode).ToListAsync());
                }
            }
            else
            {
                if (fromdate != null && todate != null)
                {
                    var fDate = Convert.ToDateTime(fromdate);
                    var tDate = Convert.ToDateTime(todate);

                    var model = await _repository.Enquiries.Where(p => p.CreatedDate.Date >= fDate.Date && p.CreatedDate.Date <= tDate.Date && p.Username == User.Identity.Name).OrderBy(p => p.EmpCode).ToListAsync();
                    return View(model);
                }
                else
                {
                    return View(await _repository.Enquiries.Where(p => p.Username == user.UserName).OrderBy(p => p.EmpCode).ToListAsync());
                }
            }
        }

        public async Task<IActionResult> LocationWise(int? id = null)
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "admin");

            ViewBag.CityId = new SelectList(_repository.Cities, "Id", "CityName", id);

            if (isAdmin)
            {
                if (id != null && id > 0)
                {
                    var model = await _repository.Enquiries.Where(p => p.CityId == id).OrderBy(p => p.EmpCode).ToListAsync();

                    return View(model);
                }
                else
                {
                    return View(await _repository.Enquiries.OrderBy(p => p.EmpCode).ToListAsync());
                }
            }
            else
            {
                if (id != null && id > 0)
                {
                    var model = await _repository.Enquiries.Where(p => p.CityId == id && p.Username == User.Identity.Name).OrderBy(p => p.EmpCode).ToListAsync();
                    return View(model);
                }
                else
                {
                    return View(await _repository.Enquiries.Where(p => p.Username == user.UserName).OrderBy(p => p.EmpCode).ToListAsync());
                }
            }
        }

    }
}

