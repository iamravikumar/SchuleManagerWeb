using System;

namespace SchuleManager.Models
{
    public class NewTrans
    {
        public decimal ColumnID { get; set; }
        public DateTime ValueDate { get; set; }
        public string AccountID { get; set; }
        public string AccountType { get; set; }
        public string ProductCode { get; set; }
        public string PartTranType { get; set; }
        public string ReceiptNo { get; set; }
        public decimal TranAmount { get; set; }
        public string TranCode { get; set; }
        public string TranParticulars { get; set; }
        public string TranRemarks { get; set; }
        public string FeeCode { get; set; }
        public decimal BillID { get; set; }
        public decimal TermID { get; set; }
        public string BankSlip { get; set; }
        public bool IsRev { get; set; }
        public DateTime RevTranDate { get; set; }
        public decimal RevTranID { get; set; }
        public decimal RevTranSerialNo { get; set; }
    }
}
