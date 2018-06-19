using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Textures;
using TGC.Group.Model.Interfaz;
using TGC.Group.Model.Scenes;

namespace TGC.Group.Model.Escenas {

    class GameOverEscena : Escena {

        private ElementoTexto textoGameOver;
        private Boton reiniciar;
        private Sprite s;
        //private Texture crashDead;
        private TgcTexture fondo;
        private Viewport viewport = D3DDevice.Instance.Device.Viewport;

        public void init(string mediaDir, string shaderDir) {

            textoGameOver = new ElementoTexto("GAME OVER", 0.4f, 0f);
            reiniciar = new Boton("REINICIAR", 0.5f, 0.5f, () => EscenaManager.getInstance().goBack());
            s = new Sprite(D3DDevice.Instance.Device);
            //crashDead = TextureLoader.FromFile(D3DDevice.Instance.Device, mediaDir + "gameOverCrash.jpg");
            fondo = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "gameOverFondo.jpg");

        }

        public void update(float deltaTime, TgcD3dInput input, TgcCamera camara) {

            reiniciar.Update(deltaTime, input);

        }

        public void render(float deltaTime) {

            s.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortDepthFrontToBack);

            var scaling = new TGCVector3(
                (float) viewport.Width / fondo.Width,
                (float) viewport.Height / fondo.Height,
            0);

            s.Transform = TGCMatrix.Scaling(scaling);
            s.Draw(fondo.D3dTexture, Rectangle.Empty, Vector3.Empty, Vector3.Empty, Color.White);

            s.End();

            textoGameOver.Render();

            reiniciar.Render();

            //s.Begin(SpriteFlags.None);
            //s.Draw(crashDead, new Vector3(0, 0, 0), new Vector3(200, 200, 0), 16777215);
            //s.End();

        }

        public void dispose() {

            textoGameOver.Dispose();
            reiniciar.Dispose();

        }

    }
}
