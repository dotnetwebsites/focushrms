using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FocusHRMS.Areas.Identity.Data;
using FocusHRMS.Services;
using FocusHRMS.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace FocusHRMS.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "admin")]
    //[AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IMailService _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IMailService emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Please enter first name")]
            [DataType(DataType.Text)]
            [Display(Name = "First Name")]
            public string Firstname { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Middle Name")]
            public string Middlename { get; set; }

            [Required(ErrorMessage = "Please enter last name")]
            [DataType(DataType.Text)]
            [Display(Name = "Last Name")]
            public string Lastname { get; set; }

            [Required(ErrorMessage = "Required mobile no")]
            [MaxLength(10)]
            [MinLength(10, ErrorMessage = "Mobile no must be 10-digit without prefix")]
            [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile no must be numeric")]
            [Display(Name = "Mobile No")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Please enter email address")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            [DataType(DataType.EmailAddress)]
            [Display(Name = "Email Address")]
            public string Email { get; set; }

            // [Required(ErrorMessage = "Password must required")]
            // [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            // [DataType(DataType.Password)]
            // [Display(Name = "Password")]
            public string Password
            {
                get
                {
                    return "Abc123#$%";
                }
            }

            // [DataType(DataType.Password)]
            // [Display(Name = "Confirm password")]
            // [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            // public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Gender must required")]
            public bool Gender { get; set; }

            [Required(ErrorMessage = "Required Designation")]
            public string Designation { get; set; }

            [Required(ErrorMessage = "Please enter employee code")]
            public string EmployeeCode { get; set; }

            [Required(ErrorMessage = "Please select work location")]
            public string WorkLocation { get; set; }

            [Required(ErrorMessage = "Please enter Date of Joining")]
            [DataType(DataType.Date)]
            public DateTime DateOfJoining { get; set; }

            [Required(ErrorMessage = "Please select department.")]
            public string Department { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    FirstName = Input.Firstname,
                    MiddleName = Input.Middlename,
                    LastName = Input.Lastname,
                    Designation = Input.Designation,
                    EmployeeCode = Input.EmployeeCode,
                    UserName = Input.Email,
                    Email = Input.Email,
                    PhoneNumber = Input.PhoneNumber,
                    Gender = Input.Gender,
                    DateofJoining = Input.DateOfJoining,
                    Department = Input.Department,
                    WorkLocation = Input.WorkLocation,
                    IsActive = true
                };

                var codeExists = _userManager.Users.Any(p => p.EmployeeCode == Input.EmployeeCode);
                var emailExists = _userManager.Users.Any(p => p.Email == Input.Email);

                if (codeExists)
                {
                    ModelState.AddModelError(string.Empty, "Employee code already exists.");
                    return Page();
                }

                if (emailExists)
                {
                    ModelState.AddModelError(string.Empty, "Email already exists.");
                    return Page();
                }

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    //code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

                    //if we need to confirm email manually
                    await _userManager.ConfirmEmailAsync(user, code);

                    // string htmlBody = "<p>Dear " + user.FullName + $@"</p>
                    //                 <p>Your Password is : Abc123#$%</p><p>Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.</<p>";

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

                    _emailSender.SendEmail(Mail.DNR, Input.Email, user.FullName, "Your Focus HRMS account has been created.", htmlBody);

                    StatusMessage = "Employee successfully added.";

                    //Input = new InputModel();
                    return Page();

                    // if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    // {
                    //     return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    // }
                    // else
                    // {
                    //     await _signInManager.SignInAsync(user, isPersistent: false);
                    //     return LocalRedirect(returnUrl);
                    // }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
