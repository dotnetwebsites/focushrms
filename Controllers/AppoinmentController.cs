using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FocusHRMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FocusHRMS.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System;
using FocusHRMS.Data;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Linq;

namespace FocusHRMS.Controllers
{
    [Authorize(Roles = "superadmin")]    
    public class AppoinmentController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        public AppoinmentController(ILogger<HomeController> logger,
                                ApplicationDbContext db,
                                IWebHostEnvironment webHostEnvironment,
                                UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var appoinmentLetters = _db.AppoinmentLetters.ToList();
            return View(appoinmentLetters);
        }

        [HttpGet]
        public ActionResult UserDownload()
        {
            if (!_db.AppoinmentLetters.Any(p => p.Username == User.Identity.Name))
            {
                ModelState.AddModelError(string.Empty, "Appoinment Letter not available");
                var appoinmentLetters = _db.AppoinmentLetters.ToList();

                return View("Index", appoinmentLetters);
            }

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "appoinmentletter/");
            string fileName = _db.AppoinmentLetters.First(p => p.Username == User.Identity.Name).URL;
            return File(new FileStream(filePath + fileName, FileMode.Open), "application/pdf", "AppoinmentLetter.pdf");
        }

        [HttpGet]
        public ActionResult Upload()
        {

            ViewBag.Username = new SelectList(_userManager.Users, "UserName", "UserName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(AppoinmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool exists = _db.AppoinmentLetters.Any(p => p.Username == model.Username);
                if (exists)
                {
                    ViewBag.Username = new SelectList(_userManager.Users, "UserName", "UserName", model.Username);
                    ModelState.AddModelError(string.Empty, "Appoinment letter already exists for this user.");
                    return View(model);
                }

                string uniqueFiles = UploadedFile(model);

                AppoinmentLetter appoinment = new AppoinmentLetter
                {
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    Username = model.Username,

                    URL = uniqueFiles
                };

                _db.AppoinmentLetters.Add(appoinment);

                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Username = new SelectList(_userManager.Users, "UserName", "UserName", model.Username);

            return View(model);
        }

        private string UploadedFile(AppoinmentViewModel model)
        {
            string uniqueFileName = null;

            if (model.AppoinmentLetterFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "appoinmentletter");

                if (!System.IO.Directory.Exists(uploadsFolder))
                    System.IO.Directory.CreateDirectory(uploadsFolder);

                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.AppoinmentLetterFile.FileName;

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.AppoinmentLetterFile.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        [HttpGet, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmedAssort(int id)
        {
            AppoinmentLetter appoinmentLetter = await _db.AppoinmentLetters.FindAsync(id);

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "appoinmentletter/", appoinmentLetter.URL);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _db.AppoinmentLetters.Remove(appoinmentLetter);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
