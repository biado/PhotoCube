using System.ComponentModel.DataAnnotations.Schema;

namespace ObjectCubeServer.Models.DomainClasses
{
    /// <summary>
    /// Repressents a many-to-many relationship between CubeObjects and Tags.
    /// </summary>
    [Table("objecttagrelations")]
    public class ObjectTagRelation
    {
        [Column("object_id")]
        public int ObjectId { get; set; }
        public CubeObject CubeObject { get; set; }

        [Column("tag_id")]
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}