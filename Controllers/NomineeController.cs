using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FocusHRMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FocusHRMS.Areas.Identity.Data;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using FocusHRMS.Data;
using Microsoft.EntityFrameworkCore;

namespace FocusHRMS.Controllers
{
    [Authorize]
    public class NomineeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public NomineeController(ILogger<HomeController> logger,
                            UserManager<ApplicationUser> userManager,
                            SignInManager<ApplicationUser> signInManager,
                            IWebHostEnvironment webHostEnvironment,
                            ApplicationDbContext db)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (_db.Nominees.Any(p => p.Username == User.Identity.Name))
            {
                var nominee = await _db.Nominees.FirstOrDefaultAsync(p => p.Username == User.Identity.Name);
                return View(nominee);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Nominee nominee)
        {
            if (ModelState.IsValid)
            {
                if (_db.Nominees.Any(p => p.Username == User.Identity.Name))
                {
                    var nomi = _db.Nominees.FirstOrDefault(p => p.Username == User.Identity.Name);

                    nomi.NomineeName = nominee.NomineeName;
                    nomi.DateOfBirth = nominee.DateOfBirth;
                    nomi.Relation = nominee.Relation;
                    nomi.Contact = nominee.Contact;
                    nomi.Address1 = nominee.Address1;
                    nomi.Address2 = nominee.Address2;
                    nomi.City = nominee.City;
                    nomi.State = nominee.State;
                    nomi.Pincode = nominee.Pincode;

                    nomi.UpdatedBy = User.Identity.Name;
                    nomi.UpdatedDate = DateTime.Now;

                    _db.Entry(nomi).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else
                {
                    nominee.CreatedBy = User.Identity.Name;
                    nominee.CreatedDate = DateTime.Now;
                    nominee.Username = User.Identity.Name;

                    _db.Nominees.Add(nominee);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

            }

            return View(nominee);
        }

        public async Task<IActionResult> Index()
        {
            var nominees = await _db.Nominees.ToListAsync();
            return View(nominees);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {

            }

            var user = await _userManager.GetUserAsync(User);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Nominee nominee)
        {

            if (ModelState.IsValid)
            {
                nominee.UpdatedBy = User.Identity.Name;
                nominee.UpdatedDate = DateTime.Now;

                _db.Entry(nominee).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(nominee);
        }

    }
}
