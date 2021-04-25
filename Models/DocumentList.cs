using System.ComponentModel.DataAnnotations;

namespace FocusHRMS.Models
{
    public class DocumentList
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter document name")]
        [Display(Name = "Document Name")]
        public string DocumentName { get; set; }
    }

}