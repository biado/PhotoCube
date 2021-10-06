using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObjectCubeServer.Models.DomainClasses
{
    /// <summary>
    /// Repressents a Hiearchy in the M^3 model.
    /// Has a name.
    /// Belongs to a Tagset.
    /// Has a collection of nodes.
    /// Has a root node.
    /// </summary>
    [Table("hierarchies")]
    public class Hierarchy
    {
        [Key][Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }
        
        public Tagset Tagset { get; set; }
        [Column("tagset_id")]
        public int TagsetId { get; set; }
        
        public List<Node> Nodes { get; set; }

        [Column("rootnode_id")]
        public int RootNodeId { get; set; }
    }
}
