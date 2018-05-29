using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Group.Model.Interfaz;
using TGC.Group.Model.Scenes;

namespace TGC.Group.Model.Escenas {
    class InicioEscena : Escena {
        ElementoTexto t;
        Sprite s;
        Texture logo;

        public void init(string mediaDir) {
            t = new Boton("Jugar", 0f, 0.9f, () => EscenaManager.getInstance().addScene(new GameEscena()));
            s = new Sprite(D3DDevice.Instance.Device);
            logo = TextureLoader.FromFile(D3DDevice.Instance.Device, mediaDir + "logo.png");
        }

        public void update(float deltaTime, TgcD3dInput input, TgcCamera camara) {
            t.Update(deltaTime, input);
        }

        public void render(float deltaTime) {
            t.Render();
            s.Begin(SpriteFlags.None);
            s.Draw(logo, Vector3.Empty, Vector3.Empty, 16777215);
            s.End();
        }

        public void dispose() {
            t.Dispose();
        }

    }
}
