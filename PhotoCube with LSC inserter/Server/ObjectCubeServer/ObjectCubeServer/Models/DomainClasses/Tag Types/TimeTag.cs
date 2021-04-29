using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ObjectCubeServer.Models.DomainClasses.TagTypes
{
    [Table("time_tags")]
    public class TimeTag : Tag, IComparable
    {
        [Column("name", TypeName = "Time")]
        public TimeSpan Name { get; set; }

        [Column("tagset_id")]
        public int TagsetIdReplicate { get; set; }

        override
        public string GetTagName()
        {
            return Name.Hours + ":" + Name.Minutes;
        }

        public int CompareTo(object obj)
        {
            return Name.CompareTo(((TimeTag)obj).Name);
        }
    }
}
