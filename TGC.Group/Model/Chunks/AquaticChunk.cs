using TGC.Core.Mathematica;
using TGC.Group.Model.Elements;
using TGC.Group.Model.Elements.RigidBodyFactories;
using TGC.Group.Model.Resources.Meshes;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model.Chunks
{
    public class AquaticChunk : Chunk
    {
        public AquaticChunk(TGCVector3 origin) : base(origin)
        {
            var max = origin + DefaultSize;

            var segments = Segment.GenerateSegments(origin, max, (int)DefaultSize.Y/1000);

            var divisions = (int) (DefaultSize.X / 100);
            
            segments.ForEach(segment => 
                    this.Elements.AddRange(
                        segment.GenerateElements(
                            divisions/2, 
                            SpawnRate.Of(1,750), 
                            new ElementFactory(FishMeshes.All(), new CapsuleFactory()
                            )
                        )
                   )
           );
        }
    }
}