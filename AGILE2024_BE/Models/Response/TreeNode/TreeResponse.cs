namespace AGILE2024_BE.Models.Response.TreeNode
{
    public class TreeResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public List<OrgTreeNodeResponse> OrgTree { get; set; }
    }
}
