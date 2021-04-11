using System.ComponentModel.DataAnnotations;
using System.Web;

namespace SchuleManager.Models
{
    public class ClientPhotos
    {
        [Required(ErrorMessage = "Select a Client ID")]
        public string EntityID { get; set; }

        public string PhotoData { get; set; }
        public string PhotoExtension { get; set; }

        [Required]
        [Display(Name = "Upload File")]
        public HttpPostedFileBase FileAttach { get; set; }

        public PhotoObject Photo { get; set; }

        public bool Empty => string.IsNullOrWhiteSpace(EntityID);
    }
}
