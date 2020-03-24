using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Examples.Camara;


namespace TGC.Group.Model
{
    public class UnaPicaraCaja
    {

        public UnaPicaraCaja(string mediaDir, string shadersDir)
        {
            MediaDir = mediaDir;
        }

        private string MediaDir;
        private TGCBox Box { get; set; }

        public void Init()
        {
            var pathTexturaCaja = MediaDir + "luciano.jpg";
            var texture = TgcTexture.createTexture(pathTexturaCaja);
            var size = new TGCVector3(-16, 16, 16);
            Box = TGCBox.fromSize(size, texture);
            Box.Position = new TGCVector3(0, 200, 10);
        }

        public void Update(float elapsedTime)
        {

            var movement = new TGCVector3(0, 0, -1);
            movement *= 50f * elapsedTime;
            Box.Position = Box.Position + movement;
            Box.Transform = TGCMatrix.Translation(Box.Position);
        }
        public void Render()
        {
            Box.Render();
        }
        public void Dispose()
        {
            Box.Dispose();
        }
    }
}