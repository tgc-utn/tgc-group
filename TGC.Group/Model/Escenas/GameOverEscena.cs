using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Group.Model.Interfaz;
using TGC.Group.Model.Scenes;

namespace TGC.Group.Model.Escenas {

    class GameOverEscena : Escena {

        ElementoTexto textoGameOver;
        Boton reiniciar;
        Sprite s;
        Texture crashDead;

        public void init(string mediaDir) {

            textoGameOver = new ElementoTexto("GAME OVER", 0.4f, 0f);
            reiniciar = new Boton("REINICIAR", 0.5f, 0.5f, () => EscenaManager.getInstance().goBack());
            s = new Sprite(D3DDevice.Instance.Device);
            crashDead = TextureLoader.FromFile(D3DDevice.Instance.Device, mediaDir + "gameOverCrash.jpg");

        }

        public void update(float deltaTime, TgcD3dInput input, TgcCamera camara) {

            reiniciar.Update(deltaTime, input);

        }

        public void render(float deltaTime) {

            textoGameOver.Render();
            reiniciar.Render();
            s.Begin(SpriteFlags.None);
            s.Draw(crashDead, new Vector3(0, 0, 0), new Vector3(200, 200, 0), 16777215);
            s.End();

        }

        public void dispose() {

            textoGameOver.Dispose();
            reiniciar.Dispose();

        }

    }
}
