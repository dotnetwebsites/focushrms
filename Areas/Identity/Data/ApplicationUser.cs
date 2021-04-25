using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FocusHRMS.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace FocusHRMS.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Employee Code")]
        public string EmployeeCode { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [PersonalData]
        [Display(Name = "Father Name")]
        [Column(TypeName = "nvarchar(100)")]
        public string FatherName { get; set; }

        [PersonalData]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [PersonalData]
        public bool? Gender { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(500)")]
        [Display(Name = "Profile Image")]
        public string ProfileImage { get; set; }

        [Column(TypeName = "nvarchar(1000)")]
        public string Address { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string Designation { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Joining")]
        public DateTime DateofJoining { get; set; }

        [Display(Name = "Work Location")]
        public string WorkLocation { get; set; }

        public string PAN { get; set; }

        [Display(Name = "Aadhar No")]
        [Column(TypeName = "nvarchar(20)")]
        [MaxLength(12, ErrorMessage = "Max 16 digit")]
        [MinLength(12, ErrorMessage = "Please enter 12 digit")]
        public string AadhaarNo { get; set; }

        [Display(Name = "Highest Qualification")]
        [Column(TypeName = "nvarchar(200)")]
        public string HighestQualification { get; set; }

        [Display(Name = "Department")]
        [Required(ErrorMessage = "Please select department.")]
        public string Department { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                if (MiddleName != null)
                    return FirstName + " " + MiddleName + " " + LastName;
                else
                    return FirstName + " " + LastName;
            }
        }

        [NotMapped]
        [Display(Name = "Profile Image")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
        public IFormFile Avatar { get; set; }

        [NotMapped]
        [Display(Name = "Upload PAN")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".pdf", ".jpg", ".jpeg", ".png" })]
        [MaxFileSize(1000000, ErrorMessage = "Pan card file size must be 1 MB maximum")]
        public IFormFile PanCardCopy { get; set; }

        [NotMapped]
        [Display(Name = "Upload Aadhaar")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".pdf", ".jpg", ".jpeg", ".png" })]
        [MaxFileSize(1000000, ErrorMessage = "Aadhaar file size must be 1 MB maximum")]
        public IFormFile AadhaarCopy { get; set; }

        [NotMapped]
        [Display(Name = "Upload Highest Qualification Doc")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".pdf", ".jpg", ".jpeg", ".png" })]
        [MaxFileSize(1000000, ErrorMessage = "Highest Qualification file size must be 1 MB maximum")]
        public IFormFile HQCopy { get; set; }

        public bool IsActive { get; set; }
        public bool IsPanConfirmed { get; set; }
        public bool IsAadhaarConfirmed { get; set; }
        public bool IsDegreeConfirmed { get; set; }
    }

}