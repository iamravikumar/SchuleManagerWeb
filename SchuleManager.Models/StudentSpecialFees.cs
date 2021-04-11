using System;
using System.ComponentModel.DataAnnotations;

namespace SchuleManager.Models
{
    public class StudentSpecialFees
    {
        public int SFID { get; set; }
        public string StudentID { get; set; }
        public string StudentName { get; set; }

        public int AcademicID { get; set; }

        [Display(Name = "Product")]
        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        [Display(Name = "Fee")]
        public string FeeCode { get; set; }

        public string FeeDescription { get; set; }

        public string PercentageOrAmount { get; set; }

        public decimal Value { get; set; }

        public string Remarks { get; set; }

        public int AcademicYear { get; set; }

        public int TermID { get; set; }

        public SysStaticData.Terms Term { get; set; }

        public DateTime Year { get; set; }

        public string Action { get; set; }
    }
}
