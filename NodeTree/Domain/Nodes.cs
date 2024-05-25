using System.ComponentModel.DataAnnotations;

namespace NodeTree.Domain
{
    public class Nodes
    {
        [Key]
        public int NodeId { get; set; }

        [StringLength(50)]
        public string NodeName { get; set; }
        public int? ParentNodeId { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
    }
}
