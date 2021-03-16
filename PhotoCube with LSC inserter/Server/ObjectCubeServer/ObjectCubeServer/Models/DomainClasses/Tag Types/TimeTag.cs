using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObjectCubeServer.Models.DomainClasses
{
    [Table("time_tags")]
    public class TimeTag : Tag
    {
        [Column("name", TypeName = "Time")]
        public DateTime Name { get; set; }
    }
}
