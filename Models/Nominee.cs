using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusHRMS.Models
{
    public class Nominee : SchemaLog
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }

        [Required(ErrorMessage = "Required Nominee Full name")]
        [Display(Name = "Nominee Full Name")]
        //[Column(TypeName = "nvarchar(100)")]
        public string NomineeName { get; set; }

        [Required(ErrorMessage = "Required Nominee Date of Birth")]
        [Display(Name = "Nominee Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Required Relationship with Employee")]
        [Display(Name = "Relationship with Emoployee")]
        public string Relation { get; set; }

        [Required(ErrorMessage = "Required Nominee Contact No")]
        [Display(Name = "Nominee Contact No")]
        public string Contact { get; set; }

        [Required(ErrorMessage = "Required Address Line 1")]
        [Display(Name = "Address Line 1")]
        public string Address1 { get; set; }

        [Required(ErrorMessage = "Required Address Line 2")]
        [Display(Name = "Address Line 2")]
        public string Address2 { get; set; }

        [Required(ErrorMessage = "Required City")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Required State/Province/Region")]
        [Display(Name = "State/Province/Region")]
        public string State { get; set; }

        [Required(ErrorMessage = "Required Postal/Zip Code")]
        [Display(Name = "Postal/Zip Code")]
        public string Pincode { get; set; }
    }
}