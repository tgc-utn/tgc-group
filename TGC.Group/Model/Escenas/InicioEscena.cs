using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Drawing;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Textures;
using TGC.Group.Model.Interfaz;
using TGC.Group.Model.Scenes;

namespace TGC.Group.Model.Escenas {
    class InicioEscena : Escena {
        private Boton jugar;
        private Boton opciones;
        private Sprite s;
        private TgcTexture logo;
        private TgcTexture fondo;
        private Viewport viewport = D3DDevice.Instance.Device.Viewport;

        public void init(string mediaDir, string shaderDir) {
            jugar = new Boton("Jugar", 0f, 0.8f, () => EscenaManager.getInstance().addScene(new GameEscena()));
            opciones = new Boton("Opciones", 0f, 0.9f, () => EscenaManager.getInstance().addScene(new OpcionesEscena()));
            s = new Sprite(D3DDevice.Instance.Device);
            logo = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "logo.png");
            fondo = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "fondoInicio.jpg");
        }

        public void update(float deltaTime, TgcD3dInput input, TgcCamera camara) {
            jugar.Update(deltaTime, input);
            opciones.Update(deltaTime, input);
        }

        public void render(float deltaTime) {

            s.Begin(SpriteFlags.None);

            var scaling = new TGCVector3(
                (float) viewport.Width / fondo.Width,
                (float) viewport.Height / fondo.Height,
            0);

            s.Transform = TGCMatrix.Scaling(scaling);
            s.Draw(fondo.D3dTexture, Vector3.Empty, Vector3.Empty, 0xFFFFFF);

            s.Transform = TGCMatrix.Translation(new TGCVector3(
                viewport.Width / 2 - logo.Width / 2, 0, 0));
            s.Draw(logo.D3dTexture, Vector3.Empty, Vector3.Empty, 0xFFFFFF);

            s.End();

            jugar.Render();
            opciones.Render();
        }

        public void dispose() {
            jugar.Dispose();
            opciones.Dispose();
        }

    }
}
