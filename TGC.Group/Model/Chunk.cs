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
        private TGCVector3 size;
        private List<Element> elements;

        static private TGCVector3 DefaultSize = new TGCVector3(1000, 1000, 1000);
        static private TGCVector3 DefaultFloorSize = new TGCVector3(1000, 0, 1000);


        public Chunk(TGCVector3 origin)
        {
            var pisoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, Game.Default.MediaDirectory + Game.Default.TexturaTierra);
            this.floor = new TgcPlane(origin, DefaultSize, Orientations.XZplane, pisoTexture);
            this.size = DefaultSize;
            this.elements = new List<Element>();

            TGCVector3 floorOrigin, mediumOrigin, topOrigin, maxPoint, mediumMaxPoint, topMaxPoint;
            int boxQuantity;

            floorOrigin = origin;
            mediumOrigin = new TGCVector3(origin.X, this.size.Y / 3, origin.Z);
            topOrigin = new TGCVector3(origin.X, 2 * this.size.Y / 3, origin.Z);

            maxPoint = new TGCVector3(this.size.X, this.size.Y/3, origin.Z);
            mediumMaxPoint = new TGCVector3(this.size.X, 2*this.size.Y / 3, origin.Z);
            topMaxPoint = new TGCVector3(this.size.X, this.size.Y, origin.Z);

            this.elements.Add(Segment.GenerateOf(origin, maxPoint, 10, new List<Element>()));
            this.elements.Add(Segment.GenerateOf(mediumOrigin, mediumMaxPoint, 10, new List<Element>()));
            this.elements.Add(Segment.GenerateOf(topOrigin, topMaxPoint, 10, new List<Element>()));
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
            this.floor.updateValues();
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