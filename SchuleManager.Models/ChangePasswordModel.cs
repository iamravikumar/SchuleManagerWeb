using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchuleManager.Models
{
    [NotMapped]
    public class ChangePasswordModel
    {
        [Required (ErrorMessage = "Enter Old Password")]
        [MinLength(7, ErrorMessage = "Minimum Password Length is 7 characters")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Enter New Password")]
        [MinLength(7, ErrorMessage = "Minimum Password Length is 7 characters")]
        public string NewPassword { get; set; }
    }
}
