namespace SchuleManager.Models
{
    public class FixedAssetsProducts
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string AssetIDPrefix { get; set; }
        public bool AllowCredits { get; set; }
        public bool CanGoInCredit { get; set; }
        public bool AllowDebits { get; set; }
        public bool CanGoInDebit { get; set; }
        public string ControlAccountGL { get; set; }
        public string ControlAccountGLName { get; set; }
        public string ControlAccountType { get; set; }
        public string AccumDepGL { get; set; }
        public string AccumDepGLName { get; set; }
        public string AccumDepGLType { get; set; }
        public string DepExpenseGL { get; set; }
        public string DepExpenseGLName { get; set; }
        public string DepExpenseGLType { get; set; }
        public string SaleoffLossGL { get; set; }
        public string SaleoffLossGLName { get; set; }
        public string SaleoffLossGLType { get; set; }
        public string SaleoffProfitGL { get; set; }
        public string SaleoffProfitGLName { get; set; }
        public string SaleoffProfitGLType { get; set; }
        public bool Deleted { get; set; }
    }
}
