using System.Collections.Generic;
using System.Linq;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Chunks
{
    public class NoneChunk : Chunk

    {
        public NoneChunk() : base(TGCVector3.Empty)
        {
        }
        
        public override IEnumerable<Entity> Init()
        {
            return Enumerable.Empty<Entity>();
        }

        public override void Update()
        {
        }

        public override void Render()
        {
        }

        public override void RenderBoundingBox()
        {
        }

        public override void Dispose()
        {
        }
    }
}