using System;
using System.Collections.Generic;
using TGC.Core.Mathematica;
using TGC.Group.Model.Elements;
using TGC.Group.Model.Elements.ElementFactories;
using TGC.Group.Model.Elements.RigidBodyFactories;
using TGC.Group.Model.Resources.Meshes;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model.Chunks
{
    public class AquaticChunk : Chunk
    {
        public AquaticChunk(TGCVector3 origin) : base(origin, AquaticPhysics.Instance)
        {
            var max = origin + DefaultSize;

            var segments = Segment.GenerateSegments(origin, max, (int)DefaultSize.Y / 1000);

            var divisions = (int)(DefaultSize.X / 100);

            GenerateElements(segments, divisions);
            AddElementsToPhysicsWorld();

        }

        private void GenerateElements(List<Segment> segments, int divisions)
        {
            segments.ForEach(segment => this.Elements.AddRange(GenerateElementsBySegment(segment, divisions)));
        }

        private static IEnumerable<Element> GenerateElementsBySegment(Segment segment, int divisions)
        {
            return segment.GenerateElements(divisions / 2, SpawnRate.Of(1, 750), FishFactory.Instance);
        }

    }
}