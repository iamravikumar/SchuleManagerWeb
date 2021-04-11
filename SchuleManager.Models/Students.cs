using System;

namespace SchuleManager.Models
{
    public class Students
    {
        public string StudentID { get; set; }
        public string IndexNo { get; set; }
        public string UNEBNo { get; set; }
        public string OtherID1 { get; set; }
        public string OtherID2 { get; set; }
        public string SurName { get; set; }
        public string OtherNames { get; set; }
        public DateTime Dateofbirth { get; set; }
        public string GenderID { get; set; }
        public string NationalityCode { get; set; }
        public DateTime RegDate { get; set; }
        public string RAddress { get; set; }
        public string ReligionCode { get; set; }
        public string HouseCode { get; set; }
        public string FormerSchool { get; set; }
        public string ClassCode { get; set; }
        public string SchSectionID { get; set; }
        public string StatusID { get; set; }
        public string Status { get; set; }
        public decimal FeesBalance { get; set; }
        public string FormSection { get; set; }
        public bool Registered { get; set; }

    }
}
