using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObjectCubeServer.Models.DomainClasses
{
   [Table("tag_types")]
    public class TagType
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("description")]
        public string Description { get; set; }
    }
}
