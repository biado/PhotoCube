using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace ObjectCubeServer.Models.PublicClasses
{
    /// <summary>
    /// Represents an Object in the M^3 datamodel. (Simplified public model)
    /// Has an Id (CubeObjectId) and a FileURI.
    /// </summary>
    [Keyless]
    public class PublicCubeObject
    {
        public int Id { get; set; }
        public string FileURI { get; set; }

        public PublicCubeObject(int id, string fileURI)
        {
            this.Id = id;
            this.FileURI = fileURI;
        }
    }
}
