using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObjectCubeServer.Models.DomainClasses.Tags
{
    [Table("date_tags")]
    public class DateTag : Tag
    {
        [Column("name", TypeName = "Date")]
        public DateTime Name { get; set; }
    }
}
