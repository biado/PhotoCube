using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObjectCubeServer.Models.DomainClasses.Tag_Types
{
    [Table("numerical_tags")]
    public class NumericalTag : Tag, IComparable
    {
        [Column("name")]
        public int Name { get; set; }

        [Column("tagset_id")]
        public int TagsetIdReplicate { get; set; }

        override
        public string GetTagName()
        {
            return Convert.ToString(Name);
        }

        public int CompareTo(object obj)
        {
            return Name.CompareTo(((NumericalTag)obj).Name);
        }
    }
}
