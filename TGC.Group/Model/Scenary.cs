using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class Scenary
    {
        private List<Chunk> chunks;
        private List<Entity> entities;

        public Scenary(TGCVector3 initialPoint)
        {
            this.chunks = new List<Chunk>();
            this.chunks.Add(new Chunk(initialPoint));

            this.entities = new List<Entity>();

            foreach(var chunk in chunks)
            {
                entities.AddRange(chunk.Init());
            }
        }

        public List<Collisionable> GetCollisionables()
        {
            var res = new List<Collisionable>();

            res.AddRange(entities);

            foreach (var chunk in chunks)
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

        public void Dispose()
        {
            this.chunks.ForEach(chunk => chunk.Dispose());
            this.entities.ForEach(entity => entity.Dispose());
        }
    }
}
