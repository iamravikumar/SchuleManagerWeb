using System;

namespace SchuleManager.Models
{
    public class AcademicPeriods
    {
        public int TermID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime HFromDate { get; set; }
        public DateTime HToDate { get; set; }
    }
}
