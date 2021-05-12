using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ObjectCubeServer.Models.DomainClasses
{
    /// <summary>
    /// A Node in the Hierarchy.
    /// Has a one to one relation with a tag.
    /// Has child nodes.
    /// If Children is empty, then this node is a leaf.
    /// </summary>
    [Table("nodes")]
    public class Node
    {
        [Column("id")]
        public int Id { get; set; }

        [NotMapped]
        public int TagId { get; set; }
        public Tag Tag { get; set; }

        [Column("hierarchy_id")]
        public int HierarchyId { get; set; }
        public Hierarchy Hierarchy { get; set; }

        [ForeignKey("parentnode_id")]
        public List<Node> Children { get; set; }

        [NotMapped]
        public int? ParentNodeId { get; set; }
    }
}
