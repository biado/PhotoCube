using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ObjectCubeServer.Models.PublicClasses;

namespace ObjectCubeServer.Models.DomainClasses
{
    /// <summary>
    /// Represents an Object in the M^3 datamodel.
    /// Has a FileURI and a ThumbnailURI (to be used to fetch the visual representation of the object from an external server).
    /// Is tagged with tags in ObjectTagRelation (many to many relation table to Tag).
    /// In the future it can represent different types of files.
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

        // To change the domain model cubeObject to public model cubeObject
        public PublicCubeObject GetPublicCubeObject()
        {
            return new PublicCubeObject(this.Id, this.FileURI);
        }
    }
}
