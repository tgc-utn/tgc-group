using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Camara;
using TGC.Core.Input;
using TGC.Group.Model.Interfaz;
using TGC.Group.Model.Scenes;

namespace TGC.Group.Model.Escenas {
    class GameOverEscena : Escena {
        ElementoTexto textoGameOver;
        Boton reiniciar;

        public void init(string mediaDir) {
            textoGameOver = new ElementoTexto("GAME OVER", 0.4f, 0f);
            reiniciar = new Boton("REINICIAR", 0.5f, 0.5f, () => EscenaManager.getInstance().goBack());

        }

        public void update(float deltaTime, TgcD3dInput input, TgcCamera camara) {
            reiniciar.Update(deltaTime, input);
        }

        public void render(float deltaTime) {
            textoGameOver.Render();
            reiniciar.Render();
        }

        public void dispose() {
            textoGameOver.Dispose();
            reiniciar.Dispose();
        }

    }
}
