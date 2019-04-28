using System.Collections.Generic;
using TGC.Core.Geometry;
using static TGC.Core.Geometry.TgcPlane;
using TGC.Core.Mathematica;
using TGC.Core.Textures;
using TGC.Core.Direct3D;
using TGC.Group.Model.Resources.Meshes;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model
{
    internal class Chunk
    {
        private readonly TgcPlane floor;
        
        private static readonly TgcTexture FloorTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, 
            Game.Default.MediaDirectory + Game.Default.TexturaTierra);
        public List<Element> Elements { get; }

        public static TGCVector3 DefaultSize { get; } = new TGCVector3(2000, 2000, 2000);

        public Chunk(TGCVector3 origin)
        {
            var max = origin + DefaultSize;

            var segments = Segment.GenerateSegments(origin, max, 10);

            var divisions = (int) (DefaultSize.X / 100);

            this.Elements = new List<Element>();
            
            this.Elements.AddRange(segments[0].GenerateElements(divisions/2, SpawnRate.Of(1,1), CoralMeshes.All()));

            segments.FindAll(segment => segment != segments[0])
                .ForEach(segment => 
                    this.Elements.AddRange(segment.GenerateElements(divisions/2, SpawnRate.Of(1,100), FishMeshes.All()))
                    );
            
            this.floor = new TgcPlane(origin, DefaultSize, Orientations.XZplane, FloorTexture);
        }

        public IEnumerable<Entity> Init()
        {
            return new List<Entity>();
        }

        public void Update()
        {
            this.Elements.ForEach(element => element.Update());
        }

        public void Render()
        {
            this.floor.updateValues();
            this.floor.Render();
            this.Elements.ForEach(element => element.Render());
        }

        public void RenderBoundingBox()
        {
            this.Elements.ForEach(element => element.getCollisionVolume().Render());
        }

        public void Dispose()
        {
            this.floor.Dispose();
            this.Elements.ForEach(element => element.Dispose());
        }
    }
}