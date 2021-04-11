namespace SchuleManager.Models
{
    public class RoleRights
    {
        public int SerialNo { get; set; }
        public string RoleCode { get; set; }
        public string Description { get; set; }
        public int ModuleId { get; set; }
        public int SubModuleId { get; set; }
        public int MenuId { get; set; }
        public string MenuDescription { get; set; }
        public bool HasAccess { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public decimal PostingLimit { get; set; }
        public bool CanSupervise { get; set; }
        public decimal SupervisionLimit { get; set; }
    }
}
