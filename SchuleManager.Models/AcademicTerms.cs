using System;
using System.ComponentModel.DataAnnotations;

namespace SchuleManager.Models
{
    public class AcademicTerms
    {
        public int AcademicID { get; set; }
        public int AcademicYear { get; set; }
        public int TermID { get; set; }

        public string AcademicTerm { get; set; }

        [Required(ErrorMessage = "From Date is Required")]
        public DateTime FromDate { get; set; }

        public string SFromDate { get; set; }

        [Required(ErrorMessage = "To Date is Required")]
        public DateTime ToDate { get; set; }

        public string SToDate { get; set; }
        public string Flag { get; set; }

        [Required(ErrorMessage = "Holiday From Date Required")]
        public DateTime HFromDate { get; set; }
        public string SHFromDate { get; set; }

        [Required(ErrorMessage = "Holiday To Date Required")]
        public DateTime HToDate { get; set; }
        public string SHToDate { get; set; }
    }
}
