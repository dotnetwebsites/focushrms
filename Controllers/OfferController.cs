using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FocusHRMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FocusHRMS.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using FocusHRMS.Data;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Linq;

namespace FocusHRMS.Controllers
{
    [Authorize(Roles = "superadmin")]
    public class OfferController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        public OfferController(ILogger<HomeController> logger,
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
            var offerletters = _db.OfferLetters.ToList();
            return View(offerletters);
        }

        [HttpGet]
        public async Task<ActionResult> UserDownload()
        {
            var user = await _userManager.GetUserAsync(User);
            if (!_db.OfferLetters.Any(p => p.EmployeeCode == user.EmployeeCode))
            {
                ModelState.AddModelError(string.Empty, "Offer Letter not available");
                var offerletters = _db.OfferLetters.ToList();

                return View("Index", offerletters);
            }

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "offerletter/");
            string fileName = _db.OfferLetters.First(p => p.EmployeeCode == user.EmployeeCode).URL;
            return File(new FileStream(filePath + fileName, FileMode.Open), "application/pdf", "OfferLetter.pdf");
        }

        [HttpGet]
        public ActionResult Upload()
        {

            ViewBag.EmployeeCode = new SelectList(_userManager.Users, "EmployeeCode", "EmployeeCode");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(OfferLetterViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool exists = _db.OfferLetters.Any(p => p.EmployeeCode == model.EmployeeCode);
                if (exists)
                {
                    ViewBag.EmployeeCode = new SelectList(_userManager.Users, "EmployeeCode", "EmployeeCode", model.EmployeeCode);
                    ModelState.AddModelError(string.Empty, "Offer letter already exists for this user.");
                    return View(model);
                }

                string uniqueFiles = UploadedFile(model);

                OfferLetter offer = new OfferLetter
                {
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    EmployeeCode = model.EmployeeCode,

                    URL = uniqueFiles
                };

                _db.OfferLetters.Add(offer);

                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.EmployeeCode = new SelectList(_userManager.Users, "EmployeeCode", "EmployeeCode", model.EmployeeCode);

            return View(model);
        }

        private string UploadedFile(OfferLetterViewModel model)
        {
            string uniqueFileName = null;

            if (model.OfferLetterFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "offerletter");

                if (!System.IO.Directory.Exists(uploadsFolder))
                    System.IO.Directory.CreateDirectory(uploadsFolder);

                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.OfferLetterFile.FileName;

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.OfferLetterFile.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        [HttpGet, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmedAssort(int id)
        {
            OfferLetter offerLetter = await _db.OfferLetters.FindAsync(id);

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "offerletter/", offerLetter.URL);

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            _db.OfferLetters.Remove(offerLetter);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


    }
}
