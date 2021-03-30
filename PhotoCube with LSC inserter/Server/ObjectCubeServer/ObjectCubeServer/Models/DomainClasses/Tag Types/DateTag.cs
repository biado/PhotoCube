using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObjectCubeServer.Models.DomainClasses.TagTypes
{
    [Table("date_tags")]
    public class DateTag : Tag
    {
        [Column("name")]
        public DateTime Name { get; set; }
    }
}
