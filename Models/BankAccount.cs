using System.ComponentModel.DataAnnotations;

namespace FocusHRMS.Models
{
    public class BankAccount : SchemaLog
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }

        [Required(ErrorMessage="Please enter Account Holder Name")]
        public string AccountHolder { get; set; }
        
        [Required(ErrorMessage="Please enter Account No")]
        public string AccountNo { get; set; }

        [Required(ErrorMessage="Please enter Bank name")]        
        public string BankName { get; set; }

        [Required(ErrorMessage="Please enter IFSC Code")]        
        public string IFSC { get; set; }
        
        [Required(ErrorMessage="Please enter account type")]        
        public string AccountType { get; set; }
    }
}