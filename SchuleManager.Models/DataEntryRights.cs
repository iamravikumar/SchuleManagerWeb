namespace SchuleManager.Models
{
    public class DataEntryRights
    {
        public string RoleCode { get; set; }
        public int MenuId { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public decimal PostingLimit { get; set; }
        public bool CanSupervise { get; set; }
        public decimal SupervisionLimit { get; set; }
    }
}
