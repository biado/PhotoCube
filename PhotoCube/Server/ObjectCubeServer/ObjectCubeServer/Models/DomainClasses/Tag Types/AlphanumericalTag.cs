using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObjectCubeServer.Models.DomainClasses.Tag_Types
{
    [Table("alphanumerical_tags")]
    public class AlphanumericalTag : Tag, IComparable
    {   
        [Column("name")]
        public string Name { get; set; }

        [Column("tagset_id")]
        public int TagsetIdReplicate { get; set; }

        override
        public string GetTagName()
        {
            return Name;
        }

        public int CompareTo(object obj)
        {
            return string.Compare(Name, ((AlphanumericalTag)obj).Name, StringComparison.Ordinal);
        }
    }
}
