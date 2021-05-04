using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ObjectCubeServer.Models.DomainClasses.TagTypes
{
    [Table("date_tags")]
    public class DateTag : Tag, IComparable
    {
        [Column("name", TypeName = "Date")]
        public DateTime Name { get; set; }

        [Column("tagset_id")]
        public int TagsetIdReplicate { get; set; }

        override
        public string GetTagName()
        {
            return Name.ToShortDateString();
        }

        public int CompareTo(object obj)
        {
            return Name.CompareTo(((DateTag)obj).Name);
        }
    }
}
