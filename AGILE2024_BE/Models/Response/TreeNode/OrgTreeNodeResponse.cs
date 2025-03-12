namespace AGILE2024_BE.Models.Response.TreeNode
{
    public class OrgTreeNodeResponse
    {
        public Guid EmplyeeCardId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string Location { get; set; }
        public string? Image { get; set; }
        public List<OrgTreeNodeResponse> children { get; set; } = new List<OrgTreeNodeResponse>();
        public bool isSuperior { get; set; } = false;
        public bool expanded { get; set; } = true;
    }
}
