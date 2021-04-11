namespace SchuleManager.Models
{
    public class UserDetailsEditModel
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string RoleCode { get; set; }
        public string Role { get; set; }
        public string UserName { get; set; }
        public string SurName { get; set; }
        public string OtherNames { get; set; }
        public string EmailId { get; set; }
        public string PhoneNo1 { get; set; }
        public bool IsCashier { get; set; }
        public string CashierGl { get; set; }
        public bool IsDisabled { get; set; }
    }
}
