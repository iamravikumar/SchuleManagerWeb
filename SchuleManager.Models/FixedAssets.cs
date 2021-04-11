using System;

namespace SchuleManager.Models
{
    public class FixedAssets
    {
        public string AssetID { get; set; }
        public string Description { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string AssetIDPrefix { get; set; }
        public string SupplierID { get; set; }
        public string SerialNo { get; set; }
        public string TagNo { get; set; }
        public string BrandName { get; set; }
        public string Location { get; set; }
        public string DepMthd { get; set; }
        public decimal DepRate { get; set; }
        public decimal CostPrice { get; set; }
        public int Terms { get; set; }
        public string PurchasedOn { get; set; }
        public string DepFrom { get; set; }
        public DateTime DisposalDate { get; set; }
        public decimal DisposalValue { get; set; }
        public decimal ProfitLoss { get; set; }
        public DateTime DepEnd { get; set; }
        public decimal NetBookValue { get; set; }
        public decimal ClearBalance { get; set; }
        public bool IsBooked { get; set; }
        public DateTime BookDate { get; set; }
        public bool IsVerified { get; set; }
        public bool FABooked { get; set; }
        public bool FAStartedDep { get; set; }
        public bool FADisposed { get; set; }
        public decimal AccumDep { get; set; }
        public decimal UnSupCR { get; set; }
        public decimal UnSupDR { get; set; }
        public int RemainingTerms { get; set; }
        public bool FullyDep { get; set; }
    }
}
