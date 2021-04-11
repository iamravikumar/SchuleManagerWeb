namespace SchuleManager.Models
{
    public class ProductItems
    {
        public int ItemID { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string PAndLAccountGL { get; set; }
        public string PAndLAccountGLName { get; set; }
        public string PAndLAccountType { get; set; }
        public string ContraAccountGL { get; set; }
        public string ContraAccountGLName { get; set; }
        public string ContraAccountType { get; set; }
        public string ApplyTo { get; set; }
        public char Deleted { get; set; }
        public SysStaticData.FeesApplyTo AppliesTo { get; set; }

    }
}
