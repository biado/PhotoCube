using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObjectCubeServer.Models.DomainClasses.TagTypes
{
    [Table("time_tags")]
    public class TimeTag : Tag
    {
        [Column("name")]
        public DateTime Name { get; set; }
    }
}
