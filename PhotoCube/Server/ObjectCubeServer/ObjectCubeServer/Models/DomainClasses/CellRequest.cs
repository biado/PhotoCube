#nullable enable
using System.Collections.Generic;

namespace ObjectCubeServer.Models.DomainClasses
{
    public class CellRequest
    {
        public ParsedAxis? xAxis { get; set; }
        public ParsedAxis? yAxis { get; set; }
        public ParsedAxis? zAxis { get; set; }
        public List<ParsedFilter>? filters { get; }
        public bool? all { get; }
    }
}