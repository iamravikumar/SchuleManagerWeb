using System;
using System.ComponentModel.DataAnnotations;

namespace SchuleManager.Models
{
    public class Parents
    {
        [Display(Name = "Parent ID")]
        public string ParentID { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Title is Required")]
        public string TitleCode { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Surname is Required")]
        public string SurName { get; set; }

        [Required(ErrorMessage = "Other names are Required")]
        [Display(Name = "Other Names")]
        public string OtherNames { get; set; }

        [Required(ErrorMessage = "Date of Birth is Required")]
        [Display(Name = "Date Of Birth")]
        public DateTime Dateofbirth { get; set; }

        [Display(Name = "Gender")]
        [Required(ErrorMessage = "Gender is Required")]
        public string Gender { get; set; }

        [Display(Name = "Nationality")]
        [Required(ErrorMessage = "Nationality is Required")]
        public string NationalityCode { get; set; }

        [Display(Name = "Nationality")]
        public string Nationality { get; set; }

        public string Occupation { get; set; }

        [Display(Name = "Education")]
        [Required(ErrorMessage = "Education Level is Required")]
        public string EducationCode { get; set; }

        [Display(Name = "Education")]
        public string EducationLevel { get; set; }

        [Display(Name = "Residential Address")]
        [Required(ErrorMessage = "Residential Address is Required")]
        public string RAddress { get; set; }

        [Display(Name = "Phone1")]
        [Required(ErrorMessage = "Residential Phone Number is Required")]
        [RegularExpression(@"^(\d{12})$", ErrorMessage = "Unrecognized Phoneno Format")]
        public string RPhone1 { get; set; }

        [Display(Name = "Phone2")]
        [RegularExpression(@"^(\d{12})$", ErrorMessage = "Unrecognized Phoneno Format")]
        public string RPhone2 { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail address")]
        public string REmailID1 { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail address")]
        public string REmailID2 { get; set; }

        [Display(Name = "Business Address")]
        public string BAddress { get; set; }

        [RegularExpression(@"^(\d{12})$", ErrorMessage = "Unrecognized Phoneno Format")]
        [Display(Name = "Phone1")]
        public string BPhone1 { get; set; }

        [RegularExpression(@"^(\d{12})$", ErrorMessage = "Unrecognized Phoneno Format")]
        [Display(Name = "Phone2")]
        public string BPhone2 { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail address")]
        [Display(Name = "Email ID")]
        public string BEmailID1 { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail address")]
        [Display(Name = "Email ID 2")]
        public string BEmailID2 { get; set; }

    }
}
