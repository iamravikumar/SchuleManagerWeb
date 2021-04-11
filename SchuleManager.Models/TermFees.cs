using System.ComponentModel.DataAnnotations;

namespace SchuleManager.Models
{
    public class TermFees
    {
        public int AcademicID { get; set; }

        [Display(Name = "Product")]
        public string ProductCode { get; set; }

        [Display(Name = "Fee")]
        public string FeeCode { get; set; }

        [Display(Name = "School Section")]
        [Required(ErrorMessage = "School Section is Required")]
        public string SchSectionID { get; set; }

        [Display(Name = "Term")]
        [Required(ErrorMessage = "Term is Required")]
        public int TermID { get; set; }

        [Display(Name = "Form")]
        public string FormCode { get; set; }
        public string FormDescription { get; set; }
        public decimal Amount { get; set; }
        public string Deleted { get; set; }
        public int Year { get; set; }
        public bool AllForms { get; set; }
    }
}
