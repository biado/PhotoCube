using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObjectCubeServer.Models.DomainClasses
{
    /// <summary>
    /// Repressents a Tagset in the M^3 model.
    /// Has a name.
    /// Has a collection of tags.
    /// Has a collection of Hierarchies.
    /// </summary>
    [Table("tagsets")]
    public class Tagset
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name{ get; set; }

        public List<Tag> Tags { get; set; }
        public List<Hierarchy> Hierarchies { get; set; }
    }
}
