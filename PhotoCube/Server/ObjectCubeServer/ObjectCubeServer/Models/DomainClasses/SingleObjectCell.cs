using Microsoft.EntityFrameworkCore;


namespace ObjectCubeServer.Models.DomainClasses
{
    /// <summary>
    /// Represents a cell in the cube.
    /// Has x,y,z coordinates and the CubeObjects associated with the Cell 
    /// (based on which tags are on position x,y,z on X,Y,Z-axis.)
    /// </summary>
    [Keyless]
    public class SingleObjectCell
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public int count { get; set; }
        public int id { get; set; }
        public string fileURI { get; set; }

        public SingleObjectCell(int x, int y, int z, int count, int id, string fileURI)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.count = count;
            this.id = id;
            this.fileURI = fileURI;
        }
    }
}
