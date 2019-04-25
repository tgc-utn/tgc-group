using System.Collections.Generic;

using TGC.Core.Geometry;
using static TGC.Core.Geometry.TgcPlane;
using TGC.Core.Mathematica;
using TGC.Core.Textures;
using TGC.Core.Direct3D;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Chunk
    {
        private TgcPlane floor;
        private TGCVector3 size;
        public List<Element> Elements { get; set; }

        static private TGCVector3 DefaultSize = new TGCVector3(1000, 1000, 1000);
        static private TGCVector3 DefaultFloorSize = new TGCVector3(1000, 0, 1000);


        public Chunk(TGCVector3 origin)
        {
            var pisoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, Game.Default.MediaDirectory + Game.Default.TexturaTierra);
            this.floor = new TgcPlane(origin, DefaultSize, Orientations.XZplane, pisoTexture);
            this.size = DefaultSize;
            this.Elements = new List<Element>();
            var max = origin + size;

            TGCVector3 floorOrigin, mediumOrigin, topOrigin, maxPoint, mediumMaxPoint, topMaxPoint;
            int divisions = 10;

            floorOrigin = origin;
            mediumOrigin = new TGCVector3(origin.X, max.Y / 3, origin.Z);
            topOrigin = new TGCVector3(origin.X, 2 * max.Y / 3, origin.Z);

            maxPoint = new TGCVector3(max.X, max.Y/3, max.Z);
            mediumMaxPoint = new TGCVector3(max.X, 2* max.Y/ 3, max.Z);
            topMaxPoint = new TGCVector3(max.X, max.Y, max.Z);

            //TODO borrar esto
            //var pathTexturaCaja = Game.Default.MediaDirectory + Game.Default.TexturaCaja;
            //var texture = TgcTexture.createTexture(pathTexturaCaja);
            //var Box = new Element(new TGCVector3(-25, 0, 0), TGCBox.fromSize(size, texture).ToMesh("caja"));
            List<Element> tmp = new List<Element>();
            TgcMesh Mesh = new TgcSceneLoader().loadSceneFromFile(Game.Default.MediaDirectory + "LogoTGC-TgcScene.xml").Meshes[0];

            tmp.Add(new Element(new TGCVector3(-25, 0, 0),Mesh));
            //-----------------

            this.Elements.AddRange(Segment.GenerateOf(floorOrigin, maxPoint, divisions, tmp));
            this.Elements.AddRange(Segment.GenerateOf(mediumOrigin, mediumMaxPoint, divisions, tmp));
            this.Elements.AddRange(Segment.GenerateOf(topOrigin, topMaxPoint, divisions, tmp));
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

        public void RenderBoundingBox()
        {
            this.Elements.ForEach(element => element.getCollisionVolume().Render());
        }

        public void Dispose()
        {
            this.floor.Dispose();
            this.Elements.ForEach(element => element.Dispose());
        }
    }
}