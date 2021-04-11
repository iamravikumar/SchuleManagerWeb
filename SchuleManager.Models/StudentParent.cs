namespace SchuleManager.Models
{
    public class StudentParent
    {
        public int SPID { get; set; }
        public string StudentID { get; set; }
        public string ParentID { get; set; }
        public string TitleCode { get; set; }
        public string ParentName { get; set; }
        public string RshipCode { get; set; }
        public string Relationship { get; set; }
        public string MoreInfo { get; set; }
        public bool LiveswithStudent { get; set; }
        public string _LiveswithStudent { get; set; }
        public bool PaysFees { get; set; }
        public string _PaysFees { get; set; }
    }
}
