using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObjectCubeServer.Models.PublicClasses
{
    public class CubeObjectFileURI
    {
        public int Id { get; set; }
        public string FileURI { get; set; }

        public CubeObjectFileURI(int id, string fileUri)
        {
            this.Id = id;
            this.FileURI = fileUri;
        }
    }
}
