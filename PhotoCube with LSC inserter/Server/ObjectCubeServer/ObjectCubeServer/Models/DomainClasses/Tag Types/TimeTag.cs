using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObjectCubeServer.Models.DomainClasses.TagTypes
{
    [Table("time_tags")]
    public class TimeTag : Tag
    {
        [Column("name", TypeName = "Time")]
        public TimeSpan Name { get; set; }

        [Column("tagset_id")]
        public int TagsetId { get; set; }
        public Tagset Tagset { get; set; }
    }
}
