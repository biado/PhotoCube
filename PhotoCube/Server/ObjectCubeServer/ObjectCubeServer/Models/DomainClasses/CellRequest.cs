#nullable enable
using System.Collections.Generic;

namespace ObjectCubeServer.Models.DomainClasses
{
    public class CellRequest
    {
        public ParsedAxis? xAxis { get; set; }
        public ParsedAxis? yAxis { get; set; }
        public ParsedAxis? zAxis { get; set; }
        public IList<ParsedFilter>? filters { get; set; }
        public bool? all { get; set; }
    }
}