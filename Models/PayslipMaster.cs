using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FocusHRMS.Utilities;
using Microsoft.AspNetCore.Http;

namespace FocusHRMS.Models
{
    public class PayslipMaster : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Employee Code")]
        public string EmployeeCode { get; set; }

        public string Month { get; set; }
        public int Year { get; set; }

        [Display(Name = "Working Days")]
        public int WorkingDays { get; set; }
        //public double GrossPay { get; set; }

        [Display(Name = "Basic Salary")]
        public double Basic { get; set; }

        public double HRA { get; set; }

        [Display(Name = "Transport Allowance")]
        public double TA { get; set; }

        [Display(Name = "Leave Travel Allowance")]
        public double LTA { get; set; }

        public double CEA { get; set; }

        [Display(Name = "Special Allowance")]
        public double SPL { get; set; }
        public double Arrears { get; set; }

        [Display(Name = "Gross Earning (A)")]
        public double MonthlyGross { get; set; }

        [Display(Name = "Provident Fund")]
        public double EmpEPF { get; set; }

        [Display(Name = "ESI")]
        public double EmpESI { get; set; }

        [Display(Name = "Professional Tax")]
        public double TAX { get; set; }

        public double TDS { get; set; }

        [Display(Name = "Other Deductions")]
        public double OtherDeduction { get; set; }

        [Display(Name = "Total Deduction (B)")]
        public double TotalDeductions { get; set; }

        [Display(Name = "Net Salary (A-B)")]
        public double TakeHome { get; set; }
        public DateTime FinDate { get; set; }

        [NotMapped]
        public string MonthYear
        {
            get
            {
                return Month + " " + Year;
            }
        }
    }


    public class PaySlipMasterViewModel
    {
        [Required(ErrorMessage = "Please select month")]
        public string Month { get; set; }

        [Required(ErrorMessage = "Please select year")]
        [Range(2019, 2040, ErrorMessage = "Please select year")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Please choose excel file")]
        [Display(Name = "Excel File")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".xlsx", ".xls" })]
        public IFormFile ExcelFile { get; set; }

    }
}