using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObjectCubeServer.Models.DomainClasses.Tags
{
    [Table("numerical_tags")]
    public class NumericalTag : Tag
    {
        [Column("name")]
        public int Name { get; set; }
    }
}
