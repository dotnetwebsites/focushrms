using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FocusHRMS.Utilities;
using Microsoft.AspNetCore.Http;

namespace FocusHRMS.Models
{
    public class AppoinmentLetter : SchemaLog
    {
        [Key]
        public int Id { get; set; }        
        public string Username { get; set; }
        public string AppoinmentId { get; set; }
        public string URL { get; set; }
    }

    public class AppoinmentViewModel        
    {   
        [Required(ErrorMessage = "Please choose appoinment letter file")]
        public string Username { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Please choose appoinment letter file")]
        [Display(Name = "Appoinment Letter")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile AppoinmentLetterFile { get; set; }
    }
}