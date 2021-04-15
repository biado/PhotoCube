using System.Collections.Generic;

namespace ObjectCubeServer.Models.PublicClasses
{
    /// <summary>
    /// Represents a cell in the cube. (Simplified public model)
    /// Has x,y,z coordinates and the FileURIs of the CubeObjects associated with the Cell 
    /// (based on which tags are on position x,y,z on X,Y,Z-axis.)
    /// </summary>
    public class SimpleFileURICell
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public List<string> CubeObjectFileURIs { get; set; }

        public SimpleFileURICell(int x, int y, int z, List<string> CubeObjectFileURIs)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.CubeObjectFileURIs = CubeObjectFileURIs;
        }
    }
}
