using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SchuleManager.Models
{
    public class SystemUsers
    {
        [MinLength(5, ErrorMessage = "Minimum Username must be 5 in charaters")]
        [Required(ErrorMessage = "Username Required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "RoleId Required")]
        public string RoleCode { get; set; }

        public string Description { get; set; }

        [MinLength(8, ErrorMessage = "Minimum Password must be 8 in charaters")]
        [Required(ErrorMessage = "Password Required")]
        public string UserPassword { get; set; }

        [Compare("UserPassword", ErrorMessage = "Enter the same Valid Password")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Enter Surname")]
        public string SurName { get; set; }

        [Required(ErrorMessage = "Enter Other names")]
        public string OtherNames { get; set; }

        [Required(ErrorMessage = "Enter Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail address")]
        public string EmailId { get; set; }

        [RegularExpression(@"^(\d{12})$", ErrorMessage = "Unrecognized Phoneno Format")]
        public string PhoneNo1 { get; set; }

        [RegularExpression(@"^(\d{12})$", ErrorMessage = "Unrecognized Phoneno Format")]
        [DefaultValue(null)]
        public string PhoneNo2 { get; set; }

        [DefaultValue(false)]
        public bool IsCashier { get; set; }

        [DefaultValue(null)]
        public string CashierGl { get; set; }
        public string CashierGLName { get; set; }

        [DefaultValue(false)]
        public bool PasswordNeverExpires { get; set; }

        [DefaultValue(false)]
        public bool MustChangePassword { get; set; }

        [DefaultValue(false)]
        public bool IsDisabled { get; set; }
        [DefaultValue(false)]
        public bool IsVerified { get; set; }
        public string CreatedBy { get; set; }

        public DateTime ClosedOn { get; set; }

        public string ClosedBy { get; set; }

        public string CloseReason { get; set; }
    }
}
