namespace SchuleManager.Models
{
    public class StudentsToBill
    {
        public string AccountID { get; set; }
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public string SchSectionID { get; set; }
        public string FormSectionID { get; set; }
        public string FormCode { get; set; }
        public string SpecialFees { get; set; }
        public decimal ExpectedBill { get; set; }
        public decimal NetBillAmt { get; set; }
        public decimal AmountToBill { get; set; }
        public string Flag { get; set; }
    }
}
