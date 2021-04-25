using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FocusHRMS.Areas.Identity.Data;
using FocusHRMS.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace FocusHRMS.Models
{
    public class ApplicationUserViewModel
    {
        public ApplicationUser User { get; set; }
        public BankAccount Account { get; set; }
        public PayslipMaster Payslip { get; set; }
    }
}