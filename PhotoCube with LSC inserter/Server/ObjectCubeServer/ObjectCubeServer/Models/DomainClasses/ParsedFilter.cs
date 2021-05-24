using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObjectCubeServer.Models.DomainClasses
{
    public class ParsedFilter
    {
        public string type { get; set; }
            // types:
            //"tag" : Tag filter, including year, month, day filters from Client's Left Dock 
            //"tagset" : Tagset filter. Tagged with at least 1 tag in a tagset 
            //"hierarchy" : Hierarchy filter. Tagged with at least 1 tag in a hierarchy
            //"time": Time range filter. Tagged with tags between startTime tag and endTime tag
            //"day of week": Tag filter with OR search
        public int Id { get; set; }
        public string name { get; set; }

        // For time tag
        public string startTime { get; set; }
        public string endTime { get; set; }
    }
}
