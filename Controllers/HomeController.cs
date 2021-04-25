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
using Microsoft.AspNetCore.Identity.UI.Services;
using FocusHRMS.Services;
using Microsoft.AspNetCore.Http;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace FocusHRMS.Controllers
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
        //private readonly IEmailSender _emailSender;

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
            //_emailSender.SendEmail("kpljain21@gmail.com", "Kapil Jain", "subject", "content");

            ApplicationUserViewModel model = new ApplicationUserViewModel();

            var user = await _userManager.GetUserAsync(User);
            var account = _db.BankAccounts.FirstOrDefault(p => p.Username == user.UserName);

            if (user != null)
                model.User = user;

            if (account != null)
                model.Account = account;

            string path = Path.Combine(_webHostEnvironment.WebRootPath, "/profileimages/");

            if (user.ProfileImage != null)
            {
                user.ProfileImage = path + user.ProfileImage;
            }

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
                    if (model.PanCardCopy == null && !_db.MyDocuments.Any(p =>
                    p.EmployeeCode == user.EmployeeCode &&
                    p.DocumentName == Documents.PANCARD.ToString()))
                    {
                        ModelState.AddModelError(string.Empty, "Please select Pancard copy and try again.");
                        return View(user);
                    }

                    if (model.AadhaarCopy == null && !_db.MyDocuments.Any(p =>
                    p.EmployeeCode == user.EmployeeCode &&
                    p.DocumentName == Documents.AADHAARCARD.ToString()))
                    {
                        ModelState.AddModelError(string.Empty, "Please select Aadhaar copy and try again.");
                        return View(user);
                    }

                    if (model.HQCopy == null && !_db.MyDocuments.Any(p =>
                    p.EmployeeCode == user.EmployeeCode &&
                    p.DocumentName == Documents.HIGHESTQUALIFICATION.ToString()))
                    {
                        ModelState.AddModelError(string.Empty, "Please select Highest Qualification copy and try again.");
                        return View(user);
                    }

                    if (model.Avatar != null)
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "profileimages/", user.ProfileImage);

                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }

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
                        user.IsPanConfirmed = false;
                    }

                    if (user.AadhaarNo != model.AadhaarNo)
                    {
                        user.AadhaarNo = model.AadhaarNo;
                        user.IsAadhaarConfirmed = false;
                    }

                    if (user.HighestQualification != model.HighestQualification)
                    {
                        user.HighestQualification = model.HighestQualification;
                        user.IsDegreeConfirmed = false;
                    }

                    await _db.SaveChangesAsync();

                    if (model.PanCardCopy != null)
                        await SaveDocument(Documents.PANCARD, model.PanCardCopy, user.EmployeeCode);

                    if (model.AadhaarCopy != null)
                        await SaveDocument(Documents.AADHAARCARD, model.AadhaarCopy, user.EmployeeCode);

                    if (model.HQCopy != null)
                        await SaveDocument(Documents.HIGHESTQUALIFICATION, model.HQCopy, user.EmployeeCode);

                    CookieOptions option = new CookieOptions();
                    string path = Path.Combine(_webHostEnvironment.WebRootPath, "/profileimages/");
                    if (user.ProfileImage != null)
                        Response.Cookies.Append("userProf", path + user.ProfileImage, option);
                    else
                        Response.Cookies.Append("userProf", "", option);

                    if (user.Gender == false)
                    {
                        Response.Cookies.Append("gender", "0", option);
                    }
                    else if (user.Gender == true)
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
                    if (user.Email != model.Email)
                    {
                        user.Email = model.Email;
                        user.EmailConfirmed = true;
                        var setUserNameResult = await _userManager.SetUserNameAsync(user, user.Email);
                        if (!setUserNameResult.Succeeded)
                        {
                            return View(model);
                        }

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        await _userManager.ResetPasswordAsync(user, token, "Abc123#$%");

                        //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                        // var callbackUrl = Url.Action("ConfirmEmail", "Action",
                        // values: new { userId = userId, email = user.Email, code = code },
                        // protocol: Request.Scheme);                        
                        var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                        string htmlBody = @"<div style='font-family: Segoe UI;font-size: small;'>
                                            <strong>Dear " + user.FullName + @",</strong>
                                            <p>Greeting from Focus HRMS.</p>
                                            <p>Welcome to FOCUS Employee Self Service web portal that you can signin accesses with your </p>
                                            <p>Email ID: " + user.Email + @"</p>
                                            <p>Password: Abc123#$%</p>
                                            <p>Please click on URL: <a href='http://www.focushrms.com/'>http://www.focushrms.com/</a></p>
                                            <p>Once you log in, you can access employee information.</p>
                                            <p><strong>Regards,</strong></p>
                                            <p><strong>Focus HR Solutions</strong></p>
                                            </div>";

                        // string body = "<p>Dear " + user.FullName + $@"</p>
                        //             <p>Your Password is : Abc123#$%</p><p>Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.</<p>";

                        _emailSender.SendEmail(
                            Mail.DNR,
                            user.Email,
                            user.FullName,
                            "Email address has been updated", htmlBody);

                    }

                    user.EmployeeCode = model.EmployeeCode;

                    user.PhoneNumber = model.PhoneNumber;
                    user.Designation = model.Designation;
                    user.DateofJoining = model.DateofJoining;
                    user.Department = model.Department;
                    user.WorkLocation = model.WorkLocation;
                    user.IsActive = model.IsActive;

                    //string result = await ChangeEmail(user, model);
                    //await _signInManager.RefreshSignInAsync(user);
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


        [HttpGet]
        public ActionResult BankDetail()
        {
            var account = _db.BankAccounts.FirstOrDefault(p => p.Username == User.Identity.Name);

            return View(account);
        }

        [HttpPost]
        public async Task<ActionResult> BankDetail(BankAccount model)
        {
            var exists = _db.BankAccounts.Any(p => p.Username == User.Identity.Name);
            if (ModelState.IsValid)
            {

                if (!exists)
                {
                    model.CreatedBy = User.Identity.Name;
                    model.CreatedDate = DateTime.Now;
                    model.Username = User.Identity.Name;

                    _db.BankAccounts.Add(model);
                    await _db.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                else
                {
                    var b = _db.BankAccounts.FirstOrDefault(p => p.Username == User.Identity.Name);
                    b.AccountHolder = model.AccountHolder;
                    b.AccountNo = model.AccountNo;
                    b.BankName = model.BankName;
                    b.IFSC = model.IFSC;
                    b.AccountType = model.AccountType;
                    b.UpdatedBy = User.Identity.Name;
                    b.UpdatedDate = DateTime.Now;

                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");

                }
            }

            return View(model);
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

        public IActionResult Privacy()
        {
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
