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
        public int TagsetIdReplicate { get; set; }

        override
        public string GetTagName()
        {
            return Name.ToString();
        }
    }
}
