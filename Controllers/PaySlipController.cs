using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FocusHRMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FocusHRMS.Areas.Identity.Data;
using System;
using FocusHRMS.Data;
using System.Linq;
using System.Data;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Collections.Generic;
using ExcelDataReader;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FocusHRMS.Controllers
{
    [Authorize]
    public class PaySlipController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration Configuration;
        private static int count = 0;
        private static bool Inserted = false;
        public PaySlipController(ILogger<HomeController> logger,
                                ApplicationDbContext dbContext,
                                UserManager<ApplicationUser> userManager,
                                ApplicationDbContext _db,
                                IWebHostEnvironment _environment,
                                IConfiguration _configuration)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            db = _db;
            _webHostEnvironment = _environment;
            Configuration = _configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                if (role == "admin")
                {
                    var payslips = db.PayslipMasters.ToList();
                    return View(payslips);
                }
            }

            var userpayslips = db.PayslipMasters.Where(p => p.EmployeeCode == user.EmployeeCode).ToList();
            return View(userpayslips);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string id)
        {
            string[] finYear = id.Split('-');

            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            //"APR2020-MAR2021"

            // DateTime fromDate = new DateTime(Convert.ToInt32(finYear[0].ToString()), 4, 1);
            // DateTime toDate = new DateTime(Convert.ToInt32(finYear[1].ToString()), 3, 31);


            foreach (var role in roles)
            {
                if (role == "admin")
                {
                    var payslips = await db.PayslipMasters.ToListAsync();
                    return View("Index", payslips);
                }
            }

            var userpayslips = await db.PayslipMasters.Where(p => p.EmployeeCode == user.EmployeeCode).ToListAsync();
            return View("Index", userpayslips);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Create()
        {

            ViewBag.EmployeeCode = new SelectList(_userManager.Users, "EmployeeCode", "EmployeeCode");

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(PayslipMaster payslipMaster)
        {
            if (ModelState.IsValid)
            {
                if (payslipMaster.Month == "" || payslipMaster.Month == null)
                {
                    ModelState.AddModelError(string.Empty, "Please select month and try again.");
                    ViewBag.EmployeeCode = new SelectList(_userManager.Users, "EmployeeCode", "EmployeeCode", payslipMaster.EmployeeCode);
                    return View(payslipMaster);
                }

                if (payslipMaster.Year < 2019)
                {
                    ModelState.AddModelError(string.Empty, "Please select year and try again.");
                    ViewBag.EmployeeCode = new SelectList(_userManager.Users, "EmployeeCode", "EmployeeCode", payslipMaster.EmployeeCode);
                    return View(payslipMaster);
                }

                var exists = db.PayslipMasters.Any(p =>
                     p.Month == payslipMaster.Month &&
                     p.Year == payslipMaster.Year &&
                     p.EmployeeCode == payslipMaster.EmployeeCode);

                MonthsEnum month = GetFinDate(payslipMaster.Month);

                payslipMaster.CreatedBy = User.Identity.Name;
                payslipMaster.CreatedDate = DateTime.Now;
                payslipMaster.FinDate = new DateTime(payslipMaster.Year, (int)month, (int)month == 3 ? 31 : 1);

                if (exists)
                {
                    ModelState.AddModelError(string.Empty, "Payslip already exist for this month.");
                    ViewBag.EmployeeCode = new SelectList(_userManager.Users, "EmployeeCode", "EmployeeCode", payslipMaster.EmployeeCode);
                    return View(payslipMaster);
                }

                _dbContext.PayslipMasters.Add(payslipMaster);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.EmployeeCode = new SelectList(_userManager.Users, "EmployeeCode", "EmployeeCode", payslipMaster.EmployeeCode);

            return View(payslipMaster);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            PayslipMaster payslipMaster = await db.PayslipMasters.FindAsync(id);
            if (payslipMaster == null)
            {
                return NotFound();
            }

            ViewBag.EmployeeCode = new SelectList(_userManager.Users, "EmployeeCode", "EmployeeCode", payslipMaster.EmployeeCode);

            return View(payslipMaster);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(PayslipMaster payslipMaster)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (payslipMaster.Month == "" || payslipMaster.Month == null)
                    {
                        ModelState.AddModelError(string.Empty, "Please select month and try again.");
                        ViewBag.EmployeeCode = new SelectList(_userManager.Users.OrderBy(p => p.EmployeeCode), "EmployeeCode", "EmployeeCode", payslipMaster.EmployeeCode);
                        return View(payslipMaster);
                    }

                    if (payslipMaster.Year < 2019)
                    {
                        ModelState.AddModelError(string.Empty, "Please select year and try again.");
                        ViewBag.EmployeeCode = new SelectList(_userManager.Users.OrderBy(p => p.EmployeeCode), "EmployeeCode", "EmployeeCode", payslipMaster.EmployeeCode);
                        return View(payslipMaster);
                    }

                    // var exists = db.PayslipMasters.Any(p =>
                    //      p.Month == payslipMaster.Month &&
                    //      p.Year == payslipMaster.Year &&
                    //      p.EmployeeCode == payslipMaster.EmployeeCode);

                    MonthsEnum month = GetFinDate(payslipMaster.Month);

                    payslipMaster.UpdatedBy = User.Identity.Name;
                    payslipMaster.UpdatedDate = DateTime.Now;
                    payslipMaster.FinDate = new DateTime(payslipMaster.Year, (int)month, (int)month == 3 ? 31 : 1);

                    // if (exists)
                    // {
                    //     ModelState.AddModelError(string.Empty, "Payslip already exist for this month.");
                    //     ViewBag.EmployeeCode = new SelectList(_userManager.Users, "EmployeeCode", "EmployeeCode", payslipMaster.EmployeeCode);
                    //     return View(payslipMaster);
                    // }

                    //_dbContext.PayslipMasters.Add(payslipMaster);
                    db.Entry(payslipMaster).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

                ViewBag.EmployeeCode = new SelectList(_userManager.Users.OrderBy(p => p.EmployeeCode), "EmployeeCode", "EmployeeCode", payslipMaster.EmployeeCode);

                return View(payslipMaster);
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message.Contains("IX_PayslipMasters_Month_Year_EmployeeCode"))
                    ViewBag.Message = " Payslip already exist for this month.";

                ViewBag.EmployeeCode = new SelectList(_userManager.Users.OrderBy(p => p.EmployeeCode), "EmployeeCode", "EmployeeCode", payslipMaster.EmployeeCode);
                return View(payslipMaster);
            }

        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationUserViewModel model = new ApplicationUserViewModel();

            var luser = await _userManager.GetUserAsync(User);
            bool isAdmin = (await _userManager.IsInRoleAsync(luser, "admin"));

            var payslip = isAdmin ?
            await db.PayslipMasters.FindAsync(id) :
            await db.PayslipMasters.FirstOrDefaultAsync(p =>
            p.Id == id &&
            p.EmployeeCode == luser.EmployeeCode);


            if (payslip == null)
            {
                return NoContent();
            }

            var user = _userManager.Users.FirstOrDefault(p => p.EmployeeCode == payslip.EmployeeCode);

            if (user != null)
            {
                var account = db.BankAccounts.FirstOrDefault(p => p.Username == user.UserName);
                model.User = user;
                model.Account = account;
            }

            if (payslip != null)
                model.Payslip = payslip;

            return View(model);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult BulkUpload()
        {
            if (Inserted)
            {
                ViewBag.RecordSaved = count + " Records successfully inserted";
                count = 0;
                Inserted = false;
            }

            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> BulkUpload(PaySlipMasterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Month == "" || model.Month == null)
                {
                    ModelState.AddModelError(string.Empty, "Please select month and try again.");
                    return View(model);
                }

                if (model.Year < 2019)
                {
                    ModelState.AddModelError(string.Empty, "Please select year and try again.");
                    return View(model);
                }

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
                //, out count
                var payslips = this.GetRec(model);
                foreach (var payslip in payslips)
                {
                    var exists = db.PayslipMasters.Any(p =>
                     p.Month == payslip.Month &&
                     p.Year == payslip.Year &&
                     p.EmployeeCode == payslip.EmployeeCode);

                    if (!exists)
                    {
                        db.PayslipMasters.Add(payslip);
                        await db.SaveChangesAsync();
                        cnt++;
                    }
                }

                Inserted = true;
                count = cnt;
                return RedirectToAction(nameof(BulkUpload));
            }

            return View(model);
        }

        //, out int count
        public List<PayslipMaster> GetRec(PaySlipMasterViewModel model)
        {
            List<PayslipMaster> payslipMasters = new List<PayslipMaster>();
            var fileName = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\files"}" + "\\" + model.ExcelFile.FileName;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            //int cnt = 0;
            using (var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        if (!reader.GetValue(0).ToString().Contains("SNO"))
                        {
                            bool exists = db.PayslipMasters.Any(p =>
                            p.Month == model.Month &&
                            p.Year == model.Year &&
                            p.EmployeeCode == reader.GetValue(1).ToString());

                            if (exists)
                                continue;

                            MonthsEnum month = GetFinDate(model.Month);

                            var payslip = new PayslipMaster()
                            {
                                EmployeeCode = reader.GetValue(1).ToString(),
                                WorkingDays = Convert.ToInt32(reader.GetValue(2).ToString()),
                                //GrossPay = Convert.ToDouble(reader.GetValue(3).ToString()),
                                Basic = Convert.ToDouble(reader.GetValue(3).ToString()),
                                HRA = Convert.ToDouble(reader.GetValue(4).ToString()),
                                TA = Convert.ToDouble(reader.GetValue(5).ToString()),
                                LTA = Convert.ToDouble(reader.GetValue(6).ToString()),
                                CEA = Convert.ToDouble(reader.GetValue(7).ToString()),
                                SPL = Convert.ToDouble(reader.GetValue(8).ToString()),
                                Arrears = Convert.ToDouble(reader.GetValue(9).ToString()),
                                MonthlyGross = Convert.ToDouble(reader.GetValue(10).ToString()),
                                EmpEPF = Convert.ToDouble(reader.GetValue(11).ToString()),
                                EmpESI = Convert.ToDouble(reader.GetValue(12).ToString()),
                                TAX = Convert.ToDouble(reader.GetValue(13).ToString()),
                                TDS = Convert.ToDouble(reader.GetValue(14).ToString()),
                                OtherDeduction = Convert.ToDouble(reader.GetValue(15).ToString()),
                                TotalDeductions = Convert.ToDouble(reader.GetValue(16).ToString()),
                                TakeHome = Convert.ToDouble(reader.GetValue(17).ToString()),

                                Month = model.Month,
                                Year = model.Year,
                                CreatedBy = User.Identity.Name,
                                CreatedDate = DateTime.Now,
                                FinDate = new DateTime(model.Year, (int)month, (int)month == 3 ? 31 : 1)
                            };

                            payslipMasters.Add(payslip);
                            //cnt++;
                        }

                    }
                }
            }
            //Inserted = true;
            //count = cnt;
            return payslipMasters;
        }

        private MonthsEnum GetFinDate(string mon)
        {
            MonthsEnum month = new MonthsEnum();

            switch (mon)
            {
                case "January":
                    month = MonthsEnum.January;
                    break;
                case "February":
                    month = MonthsEnum.February;
                    break;
                case "March":
                    month = MonthsEnum.March;
                    break;
                case "April":
                    month = MonthsEnum.April;
                    break;
                case "May":
                    month = MonthsEnum.May;
                    break;
                case "June":
                    month = MonthsEnum.June;
                    break;
                case "July":
                    month = MonthsEnum.July;
                    break;
                case "August":
                    month = MonthsEnum.August;
                    break;
                case "September":
                    month = MonthsEnum.September;
                    break;
                case "October":
                    month = MonthsEnum.October;
                    break;
                case "November":
                    month = MonthsEnum.November;
                    break;
                case "December":
                    month = MonthsEnum.December;
                    break;
                default:
                    month = MonthsEnum.NotSet;
                    break;
            }

            return month;
        }


        [HttpGet]
        public async Task<IActionResult> PrintPage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationUserViewModel model = new ApplicationUserViewModel();

            var payslip = await db.PayslipMasters.FindAsync(id);

            if (payslip == null)
            {
                return NoContent();
            }

            var user = _userManager.Users.FirstOrDefault(p => p.EmployeeCode == payslip.EmployeeCode);

            if (user != null)
            {
                var account = db.BankAccounts.FirstOrDefault(p => p.Username == user.UserName);
                model.User = user;
                model.Account = account;
            }

            if (payslip != null)
                model.Payslip = payslip;

            return View(model);
        }

    }
}
