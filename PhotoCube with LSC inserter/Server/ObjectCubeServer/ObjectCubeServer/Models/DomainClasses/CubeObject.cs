using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObjectCubeServer.Models.DomainClasses
{
    /// <summary>
    /// Repressents an Object in the M^3 datamodel.
    /// Has a fileName and a Thumbnail (visual presentation of the object).
    /// Is tagged with tags in ObjectTagRelation (many to many relation table to Tag).
    /// In the fututre i can repressent different types of files.
    /// </summary>
    [Table("cubeobjects")]
    public class CubeObject
    {
        [Key][Column("id")]
        public int Id { get; set; }

        [Column("file_uri")]
        public string FileURI { get; set; }

        [Column("file_type")]
        public FileType FileType { get; set; }

        [Column("thumbnail_uri")]
        public string ThumbnailURI { get; set; }

        public List<ObjectTagRelation> ObjectTagRelations { get; set; }
    }
}
