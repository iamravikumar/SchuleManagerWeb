using System;

namespace SchuleManager.Models
{
    public class Account
    {
        public string AccountID { get; set; }
        public string AccountType { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ClientID { get; set; }
        public string ClientType { get; set; }
        public string AccountName { get; set; }
        public DateTime OpenDate { get; set; }
        public string OpenedBy { get; set; }
        public bool IsVerified { get; set; }
        public DateTime VerifiedOn { get; set; }
        public string VerifiedBy { get; set; }
        public decimal CreditTurnOver { get; set; }
        public decimal DebitTurnOver { get; set; }
        public DateTime LastCrTranDate { get; set; }
        public decimal LastCrTranAmt { get; set; }
        public DateTime LastDrTranDate { get; set; }
        public decimal LastDrTranAmt { get; set; }
        public decimal PrevBalance { get; set; }
        public decimal ClearBalance { get; set; }
        public decimal ShadowBalance { get; set; }
        public decimal UnSupervisedCredit { get; set; }
        public decimal UnSupervisedDebit { get; set; }
        public decimal LienAmount { get; set; }
        public decimal FrozenAmount { get; set; }
        public DateTime ClosedOn { get; set; }
        public string ClosedBy { get; set; }
        public string CloseReason { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }

    }
}
