using System.Collections.Generic;

namespace ObjectCubeServer.Models.PublicClasses
{
    /// <summary>
    /// Repressents a cell in the cube.
    /// Has x,y,z coordinates and the CubeObjects associated with the Cell 
    /// (based on which tags are on position x,y,z on X,Y,Z-axis.
    /// </summary>
    public class Cell
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public List<CubeObjectFileURI> CubeObjectFileURIs { get; set; }
    }
}
