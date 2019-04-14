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

        public Scenary(TGCVector3 initialPoint)
        {
            this.chunks = new List<Chunk>();
            this.chunks.Add(new Chunk(initialPoint));
        }
        
        public void Update()
        {
            this.chunks.ForEach(chunk => chunk.Update());
        }

        public void Render()
        {
            this.chunks.ForEach(chunk => chunk.Render());
        }

        public void Dispose()
        {
            this.chunks.ForEach(chunk => chunk.Dispose());
        }
    }
}
