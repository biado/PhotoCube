using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObjectCubeServer.Models.DomainClasses
{
    [Table("alphanumerical_tags")]
    public class AlphanumericalTag : Tag
    {   
        [Column("name")]
        public string Name { get; set; }
    }
}
