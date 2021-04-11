using System.ComponentModel.DataAnnotations;

namespace SchuleManager.Models
{
    public class Products
    {
        public int ProductID { get; set; }
        [Display(Name = "Product Code")]
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        [Display(Name = "Schule Client")]
        public string ClientType { get; set; }
        public string ClientTypeCode { get; set; }
        [Display(Name = "Product")]
        public string ProductTypeCode { get; set; }
        public string ProductType { get; set; }
        public string AccountPrefix { get; set; }
        public decimal MinBalance { get; set; }
        [Display(Name = "Allow Credits")]
        public bool AllowCredits { get; set; }
        [Display(Name = "Can Go In Credit")]
        public bool CanGoInCredit { get; set; }
        [Display(Name = "Allow Debits")]
        public bool AllowDebits { get; set; }
        [Display(Name = "Can Go In Debit")]
        public bool CanGoInDebit { get; set; }
        [Display(Name = "GL Control A/C")]
        public string ControlAccountGL { get; set; }
        public string ControlAccountGLName { get; set; }
        public string ControlAccountType { get; set; }
        [Display(Name = "GL Contra A/C")]
        public string ContraAccountGL { get; set; }
        public string ContraAccountGLName { get; set; }
        public string ContraAccountType { get; set; }
        [Display(Name = "P & L A/C")]
        public string PAndLAccountGL { get; set; }
        public string PAndLAccountGLName { get; set; }
        public string PAndLAccountType { get; set; }
        public string Deleted { get; set; }
    }
}
