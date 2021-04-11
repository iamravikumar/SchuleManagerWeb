namespace SchuleManager.Models
{
    public class StudentClass
    {
        public int ClassId { get; set; }
        public string ClassCode { get; set; }
        public string ClassDescription { get; set; }
        public string FormCode { get; set; }
        public string StreamCode { get; set; }
        public bool? Deleted { get; set; }
    }
}
