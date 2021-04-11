namespace SchuleManager.Models
{
    public class JsTree
    {
        public string id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
        public bool children { get; set; } // if node has sub-nodes set true or not set false
        public int treeLevel { get; set; }
        public bool selected { get; set; }
    }
}
