using System;

namespace SchuleManager.Models
{
    public class StudentBills
    {
        public decimal BillID { get; set; }
        public string AccountID { get; set; }
        public string BillDate { get; set; }
        public string FeeCode { get; set; }
        public decimal TranID { get; set; }
        public decimal TranSerialNo { get; set; }
        public decimal OSAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public string Description { get; set; }
        public decimal TermID { get; set; }
        public bool IsRev { get; set; }
        public DateTime? RevTranDate { get; set; }
        public decimal? RevTranID { get; set; }
        public decimal? RevTranSerialNo { get; set; }
    }
}
