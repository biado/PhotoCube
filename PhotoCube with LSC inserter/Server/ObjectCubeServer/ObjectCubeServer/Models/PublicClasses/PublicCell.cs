using System.Collections.Generic;

namespace ObjectCubeServer.Models.PublicClasses
{
    /// <summary>
    /// Represents a cell in the cube.
    /// Has x,y,z coordinates and the publicCubeObjects associated with the Cell 
    /// (based on which tags are on position x,y,z on X,Y,Z-axis.
    /// </summary>
    public class PublicCell
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public List<PublicCubeObject> CubeObjects { get; set; }

        public PublicCell(int x, int y, int z, List<PublicCubeObject> cubeObjects)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.CubeObjects = cubeObjects;
        }
    }
}
