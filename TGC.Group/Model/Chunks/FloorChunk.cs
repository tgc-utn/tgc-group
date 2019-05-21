using System.Collections.Generic;
using TGC.Core.Mathematica;
using TGC.Group.Model.Elements;
using TGC.Group.Model.Elements.ElementFactories;
using TGC.Group.Model.Utils;
using Element = TGC.Group.Model.Elements.Element;

namespace TGC.Group.Model.Chunks
{
    public class FloorChunk : Chunk
    {
        private static readonly string FloorTexture = Game.Default.MediaDirectory + Game.Default.TexturaTierra;
        
        public FloorChunk(TGCVector3 origin) : base(origin, AquaticPhysics.Instance)
        {
            var max = origin + DefaultSize;

            var segments = Segment.GenerateSegments(origin, max, 10);

            var divisions = (int)(DefaultSize.X / 100);

            CreateElements(segments, divisions);
            CreateFloor(origin);
            AddElementsToPhysicsWorld();
        }

        private void CreateElements(List<Segment> segments, int divisions)
        {
            Elements.AddRange(CreateCorals(segments, divisions));
            segments.ForEach(segment => Elements.AddRange(CreateFishes(segment, divisions)));
        }

        private void CreateFloor(TGCVector3 origin)
        {

        }

        private static IEnumerable<Element> CreateFishes(Segment segment, int divisions)
        {
            return segment.GenerateElements(divisions / 2, SpawnRate.Of(1, 750), FishFactory.Instance);
        }

        private static IEnumerable<Element> CreateCorals(List<Segment> segments, int divisions)
        {
            var corals = segments[0].GenerateElements(divisions / 2, SpawnRate.Of(1, 25), CoralFactory.Instance);
            segments.Remove(segments[0]);
            return corals;
        }

        private new void AddElementsToPhysicsWorld()
        {
            base.AddElementsToPhysicsWorld();
        }

        public override void Render()
        {
            base.Render();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}