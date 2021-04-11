using System.ComponentModel.DataAnnotations;

namespace SchuleManager.Models
{
    public class SystemSettings
    {
        public int SysID { get; set; }

        [Display(Name = "School Name")]
        public string CompanyName { get; set; }

        public int CompanyID { get; set; }

        [Display(Name = "Physical Address")]
        public string PAddress { get; set; }

        [Display(Name = "P.O Box Address")]
        public string BoxAddress { get; set; }

        [Display(Name = "Email Address 1")]
        [RegularExpression(
            @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
            ErrorMessage = "Please enter a valid e-mail address")]
        public string Email1 { get; set; }

        [Display(Name = "Email Address 2")]
        [RegularExpression(
            @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
            ErrorMessage = "Please enter a valid e-mail address")]
        public string Email2 { get; set; }

        public string Phone1 { get; set; }
        public string Phone2 { get; set; }

        [Display(Name = "School Motto")]
        public string TagLine { get; set; }

        public string Website { get; set; }
        public byte[] Logo { get; set; }

        [Display(Name = "Enforce Password History")]
        public int PswdHistory { get; set; }

        [Display(Name = "Maximum Password Age")]
        public int PswdAge { get; set; }

        [Display(Name = "Minimum Password Length")]
        public int PswdLength { get; set; }

        [Display(Name = "Enable Password Complexity Requirements")]
        public bool PswdComplexity { get; set; }
        [Display(Name = "Lock System After")]
        public int LockSysPeriod { get; set; }

        public bool abClearBalance { get; set; }
        public bool abLienAmount { get; set; }
        public bool abFrozenAmount { get; set; }
        public bool abMinimumBalance { get; set; }
        public bool abUnsupervisedCredit { get; set; }
        public bool abUnsupervisedDebit { get; set; }
        public bool tbClearBalance { get; set; }
        public bool tbLienAmount { get; set; }
        public bool tbFrozenAmount { get; set; }
        public bool tbMinimumBalance { get; set; }
        public bool tbUnsupervisedCredit { get; set; }
        public bool tbUnsupervisedDebit { get; set; }
    }
}
