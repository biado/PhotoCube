using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace ObjectCubeServer.Models.DomainClasses.Tag_Types
{
    [Table("timestamp_tags")]
    public class TimestampTag : Tag, IComparable
    {
        [Column("name", TypeName = "Timestamp")]
        public DateTime Name { get; set; }

        [Column("tagset_id")]
        public int TagsetIdReplicate { get; set; }

        override 
        public string GetTagName()
        {
            return Name.ToString("G");
        }

        public int CompareTo(object obj)
        {
            return Name.CompareTo(((TimestampTag)obj).Name);
        }
    }
}