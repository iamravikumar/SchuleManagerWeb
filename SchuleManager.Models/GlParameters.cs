using System.ComponentModel.DataAnnotations;

namespace SchuleManager.Models
{
    public class GlParameters
    {
        public int SerialID { get; set; }

        [Display(Name = "GL Account ID")]
        public string AccountID { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }

        [Display(Name = "GL Parameter")]
        public string Description { get; set; }

    }
}
