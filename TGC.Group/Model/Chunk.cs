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
        private List<Element> elements;
        private List<Entity> entities;

        private TGCVector3 size;

        static private TGCVector3 DefaultSize = new TGCVector3(1000, 1000, 1000);
        static private TGCVector3 DefaultFloorSize = new TGCVector3(1000, 0, 1000);


        public Chunk(TGCVector3 origin)
        {
            var pisoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, "c:\\workspace\\tgc\\Media\\Texturas\\tierra.jpg");
            this.floor = new TgcPlane(origin, DefaultSize, Orientations.XZplane, pisoTexture);
            this.elements = new List<Element>(); //rand
            this.entities = new List<Entity>(); //rand, depending on difficulty?
            this.size = DefaultSize;
        }

        public void Update()
        {
            this.entities.ForEach(element => element.Update());
        }

        public void Render()
        {
            this.floor.Render();
            this.elements.ForEach(element => element.Render());
            this.entities.ForEach(entity => entity.Render());
        }

        public void Dispose()
        {
            this.floor.Dispose();
            this.elements.ForEach(element => element.Dispose());
            this.entities.ForEach(entity => entity.Dispose());
        }
    }
}