using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObjectCubeServer.Models.PublicClasses
{
    /// <summary>
    /// Represents an Object in the M^3 datamodel. (Simplified public model)
    /// Has an Id (CubeObjectId) and a FileURI.
    /// </summary>
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
