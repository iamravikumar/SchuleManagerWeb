using System.ComponentModel.DataAnnotations;

namespace SchuleManager.Models
{
    public class SysStaticData
    {
        public class Terms
        {
            public int TermID { get; set; }
            public string Term { get; set; }
        }

        public class Genders
        {
            public int GenderID { get; set; }
            public string Gender { get; set; }
        }

        public class SchoolSections
        {
            public string SchSectionID { get; set; }
            public string SchoolSection { get; set; }
        }

        public class StudentStatus
        {
            public int StatusID { get; set; }
            public string Status { get; set; }
        }

        public enum FeesApplyTo
        {
            [Display(Name = "Continuing Students")]
            Cs = 67,
            [Display(Name = "Both")]
            B = 66,
            [Display(Name = "New Students")]
            N = 78
        }

    }
}
