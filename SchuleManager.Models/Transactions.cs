using System;
using System.ComponentModel.DataAnnotations;

namespace SchuleManager.Models
{
    public class Transactions
    {
        public DateTime TranDate { get; set; }
        public decimal TranID { get; set; }
        public decimal TranSerialNo { get; set; }
        public DateTime ValueDate { get; set; }
        public string TranType { get; set; }
        public string TranCat { get; set; }
        public string TranMode { get; set; }
        public string AccountID { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public string ProductCode { get; set; }
        public string PartTranType { get; set; }
        public string ReceiptNo { get; set; }
        public string BankSlip { get; set; }
        [Range(0, 999999999999)]
        public decimal TranAmount { get; set; }
        public string TranCode { get; set; }
        public string TranCodeDescription { get; set; }
        public string TranParticulars { get; set; }
        public string TranRemarks { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? VerifiedOn { get; set; }
        public string VerifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string GLSubHead { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string DelReason { get; set; }
        public bool DelVerified { get; set; }
        public DateTime? DelVerifiedOn { get; set; }
        public string DelVerifiedBy { get; set; }
        public bool IsSysTran { get; set; }
        public bool IsEODGLTran { get; set; }
        public string FeeCode { get; set; }
        public decimal BillID { get; set; }
        public decimal TermID { get; set; }
        public bool IsRev { get; set; }
        public DateTime? RevTranDate { get; set; }
        public decimal? RevTranID { get; set; }
        public decimal? RevTranSerialNo { get; set; }
    }
}
