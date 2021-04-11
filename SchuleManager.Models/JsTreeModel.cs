using System.Collections.Generic;

namespace SchuleManager.Models
{
    public class JsTreeModel
    {
        public string data;
        public JsTreeAttribute attr;
        public string parent;
        public List<JsTreeModel> children;
    }

    public class JsTreeAttribute
    {
        public string id;
        public bool selected;
    }
}