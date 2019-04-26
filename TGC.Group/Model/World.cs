using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    internal class World
    {
        private List<Chunk> chunks;
        private List<Entity> entities;

        public World(TGCVector3 initialPoint)
        {
            //TODO generation conditions and algorithm
            this.chunks = new List<Chunk>
            {
                new Chunk(initialPoint), 
                new Chunk(new TGCVector3(Chunk.DefaultSize.X, 0, 0)),
                new Chunk(new TGCVector3(Chunk.DefaultSize.X*2, 0, 0)),
                new Chunk(new TGCVector3(0, 0, Chunk.DefaultSize.Z)),
                new Chunk(new TGCVector3(0, 0, Chunk.DefaultSize.Z*2)),
                new Chunk(new TGCVector3(Chunk.DefaultSize.X, 0, Chunk.DefaultSize.Z)),
                new Chunk(new TGCVector3(Chunk.DefaultSize.X*2, 0, Chunk.DefaultSize.Z)),
                new Chunk(new TGCVector3(Chunk.DefaultSize.X, 0, Chunk.DefaultSize.Z*2)),
                new Chunk(new TGCVector3(Chunk.DefaultSize.X*2, 0, Chunk.DefaultSize.Z*2))
            };

            this.entities = new List<Entity>();

            foreach(var chunk in this.chunks)
            {
                this.entities.AddRange(chunk.Init());
            }
        }

        public List<Collisionable> GetCollisionables()
        {
            var res = new List<Collisionable>();

            res.AddRange(this.entities);

            foreach (var chunk in this.chunks)
            {
                res.AddRange(chunk.Elements);
            }

            return res;
        }

        public void Update()
        {
            this.chunks.ForEach(chunk => chunk.Update());
            this.entities.ForEach(entity => entity.Update());
        }

        public void Render()
        {
            this.chunks.ForEach(chunk => chunk.Render());
            this.entities.ForEach(entity => entity.Render());

        }

        public void RenderBoundingBox()
        {
            this.chunks.ForEach(chunk => chunk.RenderBoundingBox());
        }

        public void Dispose()
        {
            this.chunks.ForEach(chunk => chunk.Dispose());
            this.entities.ForEach(entity => entity.Dispose());
        }
    }
}
