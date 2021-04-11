using System;
using System.ComponentModel.DataAnnotations;

namespace SchuleManager.Models
{
    public class GlAccounts
    {
        [MinLength(10, ErrorMessage = "Account Number must be 10 digits")]
        [RegularExpression(@"\d{10}", ErrorMessage = "Please enter a valid account number")]
        public string AccountID { get; set; }

        [Required(ErrorMessage = "GL A/C Name is required")]
        [Display(Name = "GL A/C Name")]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "GL Type is required")]
        [Display(Name = "GL Type")]
        public string AccountType { get; set; }
        public string AccountTypeCode { get; set; }
        public string AccountSubTypeCode { get; set; }

        [Required(ErrorMessage = "GL SubType is required")]
        [Display(Name = "GL SubType")]
        public string AccountSubType { get; set; }
        public decimal TotalDebits { get; set; }
        public decimal TotalCredits { get; set; }
        public decimal PrevBalance { get; set; }
        public decimal Balance { get; set; }
        public decimal ShadowBalance { get; set; }
        public decimal ClearBalance { get; set; }
        public decimal UnSupervisedCredit { get; set; }
        public decimal UnSupervisedDebit { get; set; }
        public decimal CreditTurnOver { get; set; }
        public decimal DebitTurnOver { get; set; }
        public DateTime LastCrTranDate { get; set; }
        public decimal LastCrTranAmt { get; set; }
        public DateTime LastDrTranDate { get; set; }
        public decimal LastDrTranAmt { get; set; }
        public bool IsVerified { get; set; }
        public DateTime VerifiedOn { get; set; }
        public decimal PrevEOYPLBal { get; set; }
        public string CreatedBy { get; set; }
    }
}
