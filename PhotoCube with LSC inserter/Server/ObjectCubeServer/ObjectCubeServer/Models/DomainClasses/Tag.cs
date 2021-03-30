using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ObjectCubeServer.Models.DomainClasses.TagTypes;

namespace ObjectCubeServer.Models.DomainClasses
{
    /// <summary>
    /// Represents a Tag in the M^3 model.
    /// Belongs to a Tagset.
    /// Has a name.
    /// Has a many-to-many relationshop with CubeObjects.
    /// </summary>
    [Table("tags")]
    public abstract class Tag
    {
        [Key][Column("id")]
        public int Id { get; set; }

        [Column("tagtype_id")]
        public int TagTypeId { get; set; }
        public TagType TagType { get; set; }

        [Column("tagset_id")]
        public int TagsetId { get; set; }
        public Tagset Tagset { get; set; }

        [NotMapped]
        public AlphanumericalTag AlphanumericalTag { get; set; }
        [NotMapped]
        public NumericalTag NumericalTag { get; set; }
        [NotMapped]
        public DateTag DateTag { get; set; }
        [NotMapped]
        public TimeTag TimeTag { get; set; }

        public List<ObjectTagRelation> ObjectTagRelations { get; set; }
    }
}