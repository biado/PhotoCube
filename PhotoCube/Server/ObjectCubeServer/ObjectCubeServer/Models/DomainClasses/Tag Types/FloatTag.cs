using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObjectCubeServer.Models.DomainClasses.Tag_Types
{
    [Table("float_tags")]
    public class FloatTag : Tag, IComparable
    {
        [Column("name")]
        public float Name { get; set; }

        [Column("tagset_id")]
        public int TagsetIdReplicate { get; set; }

        override
        public string GetTagName()
        {
            return Convert.ToString(Name);
        }

        public int CompareTo(object obj)
        {
            return Name.CompareTo(((FloatTag)obj).Name);
        }
    }
}
