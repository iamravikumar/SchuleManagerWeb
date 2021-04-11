using System;

namespace SchuleManager.Models
{
    public class Audit
    {
        public int AuditId { get; set; }
        public string UserName { get; set; }
        public string SessionId { get; set; }
        public string IpAddress { get; set; }
        public string PageAccessed { get; set; }
        public DateTime? AccessedTime { get; set; }
        public DateTime? LoggedOutAt { get; set; }
        public string LoginStatus { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string UrlReferrer { get; set; }

    }
}
