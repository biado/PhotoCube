using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObjectCubeServer.Models.DomainClasses.TagTypes
{
    [Table("numerical_tags")]
    public class NumericalTag : Tag
    {
        [Column("name")]
        public int Name { get; set; }

        [Column("tagset_id")]
        public int TagsetId { get; set; }
        public Tagset Tagset { get; set; }
    }
}
