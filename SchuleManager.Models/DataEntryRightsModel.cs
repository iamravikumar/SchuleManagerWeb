using System.ComponentModel.DataAnnotations;

namespace SchuleManager.Models
{
    public class DataEntryRightsModel
    {
        public string RoleCode { get; set; }
        public int MenuId { get; set; }
        public string DataDescription { get; set; }
        public bool AskAdd { get; set; }
        public bool AskEdit { get; set; }
        public bool AskDelete { get; set; }
        public bool AskPostingLimit { get; set; }
        public string SupervisionDescription { get; set; }
        public bool AskSupervision { get; set; }
        public bool AskSupervisionLimit { get; set; }
        [Display(Name = "Can Add")]
        public bool CanAdd { get; set; }
        [Display(Name = "Can Edit")]
        public bool CanEdit { get; set; }
        [Display(Name = "Can Delete")]
        public bool CanDelete { get; set; }
        [Display(Name = "Posting Limit")]
        public decimal PostingLimit { get; set; }
        [Display(Name = "Can Supervise")]
        public bool CanSupervise { get; set; }
        [Display(Name = "Supervision Limit")]
        public decimal SupervisionLimit { get; set; }
    }
}
