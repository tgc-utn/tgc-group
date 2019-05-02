using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.Textures;
using TGC.Group.Model.Elements;
using TGC.Group.Model.Elements.RigidBodyFactories;
using TGC.Group.Model.Resources.Meshes;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model.Chunks
{
    public class FloorChunk : Chunk
    {
        private static readonly TgcTexture FloorTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, 
            Game.Default.MediaDirectory + Game.Default.TexturaTierra);

        private readonly TgcPlane floor;

        public FloorChunk(TGCVector3 origin) : base(origin)
        {            
            var max = origin + DefaultSize;

            var segments = Segment.GenerateSegments(origin, max, 10);

            var divisions = (int) (DefaultSize.X / 100);

            
            this.Elements.AddRange(segments[0].GenerateElements(divisions/2, SpawnRate.Of(1,25), 
                new ElementFactory(CoralMeshes.All(), new BoxFactory())));

            segments.Remove(segments[0]);
            segments
                .ForEach(segment => 
                    this.Elements.AddRange(segment.GenerateElements(divisions/2, SpawnRate.Of(1,750), 
                    new ElementFactory(FishMeshes.All(), new CapsuleFactory()))));
            this.floor = new TgcPlane(origin, DefaultSize, TgcPlane.Orientations.XZplane, FloorTexture);
            //RigidBodyFactory.CreateFloor(this.floor);
        }
        
        public override void Render()
        {
            base.Render();
            this.floor.updateValues();
            this.floor.Render();
        }

        public override void Dispose()
        {
            this.floor.Dispose();
            base.Dispose();
        }
    }
}