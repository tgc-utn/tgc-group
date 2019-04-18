using System.Collections.Generic;

using TGC.Core.Geometry;
using static TGC.Core.Geometry.TgcPlane;
using TGC.Core.Mathematica;
using TGC.Core.Textures;
using TGC.Core.Direct3D;

namespace TGC.Group.Model
{
    class Chunk
    {
        private TgcPlane floor;
        public List<Element> Elements { get; }

        private TGCVector3 size;

        static private TGCVector3 DefaultSize = new TGCVector3(1000, 1000, 1000);
        static private TGCVector3 DefaultFloorSize = new TGCVector3(1000, 0, 1000);


        public Chunk(TGCVector3 origin)
        {
            var pisoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, "c:\\workspace\\tgc\\Media\\Texturas\\tierra.jpg");
            this.floor = new TgcPlane(origin, DefaultSize, Orientations.XZplane, pisoTexture);
            this.Elements = new List<Element>(); //rand
            this.size = DefaultSize;
        }

        public List<Entity> Init()
        {
            return new List<Entity>();
        }

        public void Update()
        {
            this.Elements.ForEach(element => element.Update());
        }

        public void Render()
        {
            this.floor.Render();
            this.Elements.ForEach(element => element.Render());
        }

        public void Dispose()
        {
            this.floor.Dispose();
            this.Elements.ForEach(element => element.Dispose());
        }
    }
}