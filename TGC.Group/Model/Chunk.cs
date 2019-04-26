using System.Collections.Generic;

using TGC.Core.Geometry;
using static TGC.Core.Geometry.TgcPlane;
using TGC.Core.Mathematica;
using TGC.Core.Textures;
using TGC.Core.Direct3D;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    internal class Chunk
    {
        private readonly TgcPlane floor;
        private readonly TGCVector3 size;
        public List<Element> Elements { get; }

        public static TGCVector3 DefaultSize { get; } = new TGCVector3(1000, 1000, 1000);

        public Chunk(TGCVector3 origin)
        {
            var floorTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, 
                Game.Default.MediaDirectory + Game.Default.TexturaTierra);
            this.floor = new TgcPlane(origin, DefaultSize, Orientations.XZplane, floorTexture);
            this.size = DefaultSize;
            this.Elements = new List<Element>();
            var max = origin + this.size;

            const int divisions = 8;

            var floorOrigin = origin;
            var mediumOrigin = new TGCVector3(origin.X, max.Y / 3, origin.Z);
            var topOrigin = new TGCVector3(origin.X, 2 * max.Y / 3, origin.Z);

            var maxPoint = new TGCVector3(max.X, max.Y/3, max.Z);
            var mediumMaxPoint = new TGCVector3(max.X, 2* max.Y/ 3, max.Z);
            var topMaxPoint = new TGCVector3(max.X, max.Y, max.Z);

            //TODO borrar esto
            //var pathTexturaCaja = Game.Default.MediaDirectory + Game.Default.TexturaCaja;
            //var texture = TgcTexture.createTexture(pathTexturaCaja);
            //var Box = new Element(new TGCVector3(-25, 0, 0), TGCBox.fromSize(size, texture).ToMesh("caja"));
            var tmp = new List<Element>();
            TgcMesh mesh = new TgcSceneLoader().loadSceneFromFile(Game.Default.MediaDirectory + "LogoTGC-TgcScene.xml").Meshes[0];

            tmp.Add(new Element(new TGCVector3(-25, 0, 0),mesh));
            //-----------------

            this.Elements.AddRange(Segment.GenerateOf(floorOrigin, maxPoint, divisions, tmp));
            this.Elements.AddRange(Segment.GenerateOf(mediumOrigin, mediumMaxPoint, divisions, tmp));
            this.Elements.AddRange(Segment.GenerateOf(topOrigin, topMaxPoint, divisions, tmp));
        }

        public IEnumerable<Entity> Init()
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