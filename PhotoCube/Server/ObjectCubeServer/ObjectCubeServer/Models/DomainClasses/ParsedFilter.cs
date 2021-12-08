#nullable enable
using System.Collections.Generic;

namespace ObjectCubeServer.Models.DomainClasses
{
    public class ParsedFilter
    {
        public string Type { get; set; }
            //possible types:
            //"tag" : Tag filter, including year, month, day filters from Client's Left Dock 
            //"tagset" : Tagset filter. Tagged with at least 1 tag in a tagset 
            //"hierarchy" : Hierarchy filter. Tagged with at least 1 tag in a hierarchy
            //"time": Time range filter. Tagged with tags between startTime tag and endTime tag
            //"date": Applied same as tag filter. Request with tags from Year, Month (number), Day within month may have this type.
            //"day of week": Tag filter with OR search
            //"timestamp": timestamp range filter. Request with tags from Year, Month (number), Day, Hours, Minutes, Seconds within month may have this type.
        public List<int> Ids { get; set; }
        public List<List<string>>? Ranges { get; set; }
    }
}
