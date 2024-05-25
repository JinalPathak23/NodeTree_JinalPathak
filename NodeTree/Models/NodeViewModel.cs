using System.ComponentModel.DataAnnotations;

namespace NodeTree.Models
{
    public class NodeViewModel
    {
        public int NodeId { get; set; }
      
        [Required(ErrorMessage = "Node Name is required.")]
        [StringLength(50, ErrorMessage = "Node Name must be at most 50 characters long.")]
        public string NodeName { get; set; }

        public int? ParentNodeId { get; set; }

        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        public List<NodeViewModel>? Children { get; set; }
        public string? ParentNodeName { get; internal set; }
    }
}
