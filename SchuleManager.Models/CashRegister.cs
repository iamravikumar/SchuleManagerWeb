using System;

namespace SchuleManager.Models
{
    public class CashRegister
    {
        public string TranDate { get; set; }
        public string Tran_SrNo { get; set; }
        public string PartTranType { get; set; }
        public string ProductCode { get; set; }
        public string AccountID { get; set; }
        public string AccountName { get; set; }
        public decimal Amount { get; set; }
        public string OperatorID { get; set; }
        public string SupervisorID { get; set; }
        public string CashierGL { get; set; }
        public string Narration { get; set; }
    }
}
