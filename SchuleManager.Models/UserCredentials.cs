using System;

namespace SchuleManager.Models
{
    public class UserCredentials
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string SurName { get; set; }
        public string RoleCode { get; set; }
        public string UserPassword { get; set; }
        public bool PasswordNeverExpires { get; set; }
        public bool HasExpired { get; set; }
        public bool MustChangePassword { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsVerified { get; set; }
        public bool IsClosed { get; set; }
        public string CloseReason { get; set; }
        public DateTime LoginDate { get; set; }
    }
}
