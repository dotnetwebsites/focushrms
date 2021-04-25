using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FocusHRMS.Utilities;
using Microsoft.AspNetCore.Http;

namespace FocusHRMS.Models
{
    public class OfferLetter : SchemaLog
    {
        [Key]
        public int Id { get; set; }        
        public string EmployeeCode { get; set; }
        public string OfferId { get; set; }
        public string URL { get; set; }
    }

    public class OfferLetterViewModel        
    {   
        [Required(ErrorMessage = "Please select employee code")]
        public string EmployeeCode { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Please choose offer letter file")]
        [Display(Name = "Offer Letter")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile OfferLetterFile { get; set; }
    }
}