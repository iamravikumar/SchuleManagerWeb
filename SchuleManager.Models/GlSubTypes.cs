using System.ComponentModel.DataAnnotations;

namespace SchuleManager.Models
{
    public class GlSubTypes
    {
        public string GLSubTypeCode { get; set; }
        [Display(Name = "Sub Type Description")]
        public string GLSubType { get; set; }
        [Display(Name = "GL Type")]
        public string GLTypeCode { get; set; }
        public string Deleted { get; set; }
    }
}
