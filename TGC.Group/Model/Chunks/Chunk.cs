using System;
using System.Collections.Generic;
using TGC.Core.Mathematica;
using TGC.Group.Model.Elements;

namespace TGC.Group.Model.Chunks
{
    public class Chunk
    {
        public List<Element> Elements { get; }

        protected TGCVector3 Origin { get; }
        protected AquaticPhysics Physics { get; }

        public static readonly Chunk None = new NoneChunk();
        
        public static TGCVector3 DefaultSize { get; } = new TGCVector3(1000, 1000, 1000);

        protected Chunk(TGCVector3 origin, AquaticPhysics physicsWorld)
        {
            this.Origin = origin;
            this.Physics = physicsWorld;
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

        protected void AddElementsToPhysicsWorld()
        {
            this.Elements.ForEach(element => Physics.Add(element.PhysicsBody));
        }
        public virtual IEnumerable<Entity> Init()
        {
            return new List<Entity>();
        }

        public virtual void Update(Camera camera)
        {
            this.Elements.ForEach(element => element.Update(camera));
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
            this.Elements.ForEach(element => {
                Physics.Remove(element.PhysicsBody);
                element.Dispose();
            });
        }

        public void Remove(Element selectableElement)
        {
            if (!this.Elements.Contains(selectableElement)) return;

            Physics.Remove(selectableElement.PhysicsBody);
            this.Elements.Remove(selectableElement);
            selectableElement.Dispose();
        }
    }
}