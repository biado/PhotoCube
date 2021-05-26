using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObjectCubeServer.Models.PublicClasses
{
    public class PublicTag
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public PublicTag(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
