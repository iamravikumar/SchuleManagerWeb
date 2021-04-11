using System.ComponentModel.DataAnnotations;

namespace SchuleManager.Models
{
    public class Education
    {
        public int EducationId { get; set; }
        public string EducationCode { get; set; }
        [Display(Name = "Education")]
        public string EducationLevel { get; set; }
        public bool Deleted { get; set; }
    }
}
