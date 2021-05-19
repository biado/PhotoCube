using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObjectCubeServer.Models.PublicClasses
{
    public class PublicPage<T> : PageBase where T : class
    {
        public IList<T> Results { get; set; }

        public Page()
        {
            Results = new List<T>();
        }
    }
}
