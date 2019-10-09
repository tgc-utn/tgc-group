using Microsoft.DirectX.Direct3D;

namespace TGC.Group.Entities
{
    public class HeightmapModel
    {
        public VertexBuffer Terrain { get; set; }
        public int TotalVertex { get; set; }
        public int[,] HeightmapData { get; set; }
    }
}
