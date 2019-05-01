using System;
using System.Collections.Generic;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Chunks
{
    public class Chunk
    {
        public List<Element> Elements { get; }

        private TGCVector3 Origin { get; }

        public static readonly Chunk None = new NoneChunk();
        
        public static TGCVector3 DefaultSize { get; } = new TGCVector3(1000, 1000, 1000);

        protected Chunk(TGCVector3 origin)
        {
            this.Origin = origin;
            this.Elements = new List<Element>();
        }
        
        public static Chunk ByYAxis(TGCVector3 origin)
        {
            if (origin.Y < 0)
                return None;
            
            if (Math.Abs(origin.Y) < DefaultSize.Y)
                return new FloorChunk(origin);
            
            return new AquaticChunk(origin);
        }
        
        public virtual IEnumerable<Entity> Init()
        {
            return new List<Entity>();
        }

        public virtual void Update()
        {
            this.Elements.ForEach(element => element.Update());
        }

        public virtual void Render()
        {
            this.Elements.ForEach(element => element.Render());
        }

        public virtual void RenderBoundingBox()
        {
            this.Elements.ForEach(element => element.getCollisionVolume().Render());
        }

        public virtual void Dispose()
        {
            this.Elements.ForEach(element => element.Dispose());
        }
    }
}