using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObjectCubeServer.Models.DomainClasses.TagTypes
{
    [Table("date_tags")]
    public class DateTag : Tag
    {
        [Column("name", TypeName = "Date")]
        public DateTime Name { get; set; }

        [Column("tagset_id")]
        public int TagsetId { get; set; }
        public Tagset Tagset { get; set; }
    }
}
