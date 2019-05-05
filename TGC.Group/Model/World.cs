using System.Collections.Generic;
using System.Linq;
using TGC.Core.Mathematica;
using TGC.Group.Model.Chunks;

namespace TGC.Group.Model
{
    internal class World
    {
        private const int RenderRadius = 5;
        private const int UpdateRadius = RenderRadius + 1;
        
        private readonly Dictionary<TGCVector3, Chunk> chunks;
        private readonly List<Entity> entities;

        public World(TGCVector3 initialPoint)
        {
            this.chunks = new Dictionary<TGCVector3, Chunk>();

            this.entities = new List<Entity>();

            AddChunk(initialPoint);
        }
        
        private Chunk AddChunk(TGCVector3 origin)
        {
            var chunk = Chunk.ByYAxis(origin);
            
            this.chunks.Add(origin, chunk);

            this.entities.AddRange(chunk.Init());

            return chunk;
        }

        public List<Collisionable> GetCollisionables()
        {
            var res = new List<Collisionable>();

            res.AddRange(this.entities);

            foreach (var chunk in this.chunks.Values)
            {
                res.AddRange(chunk.Elements);
            }

            return res;
        }

        private List<Chunk> GetChunksByRadius(TGCVector3 origin, int radius)
        {
            var toUpdate = new List<Chunk>();
            var intOrigin = new TGCVector3(
                (int)(origin.X/Chunk.DefaultSize.X), 
                (int)(origin.Y/Chunk.DefaultSize.Y), 
                (int)(origin.Z/Chunk.DefaultSize.Z));

            for (var i = -radius; i <= radius; i++)
            {
                for (var j = -radius; j <= radius; j++)
                {
                    for (var k = -radius; k <= radius; k++)
                    {
                        var position = new TGCVector3(
                            Chunk.DefaultSize.X * (intOrigin.X + i),
                            Chunk.DefaultSize.Y * (intOrigin.Y + j),
                            Chunk.DefaultSize.Z * (intOrigin.Z + k));
                        
                        toUpdate.Add(this.chunks.ContainsKey(position) ? this.chunks[position] : AddChunk(position));
                    }
                }
            }
            
            return toUpdate;
        }
        
        private List<Chunk> ToUpdate(TGCVector3 cameraPosition)
        {
            return GetChunksByRadius(cameraPosition, UpdateRadius);
        }

        private List<Chunk> ToRender(TGCVector3 cameraPosition)
        {
            return GetChunksByRadius(cameraPosition, RenderRadius);
        }

        public void Update(TGCVector3 cameraPosition)
        {
            ToUpdate(cameraPosition).ForEach(chunk => chunk.Update());
            this.entities.ForEach(entity => entity.Update());
        }

        public void Render(TGCVector3 cameraPosition)
        {
            ToRender(cameraPosition).ForEach(chunk => chunk.Render());
            this.entities.ForEach(entity => entity.Render());

        }

        public void RenderBoundingBox(TGCVector3 cameraPosition)
        {
            ToRender(cameraPosition).ForEach(chunk => chunk.RenderBoundingBox());
        }

        public void Dispose()
        {
            this.chunks.Values.ToList().ForEach(chunk => chunk.Dispose());
            this.entities.ForEach(entity => entity.Dispose());
        }
    }
}
