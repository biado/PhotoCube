using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObjectCubeServer.Models.DomainClasses
{
    public class ParsedFilter
    {
        public string type { get; set; }
        public int Id { get; set; }
        public string name { get; set; }

        // Time tag
        public TimeSpan startTime { get; set; }
        public TimeSpan endTime { get; set; }
    }
}
