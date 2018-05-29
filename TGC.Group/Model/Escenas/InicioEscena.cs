using System;
using TGC.Core.Camara;
using TGC.Core.Input;
using TGC.Group.Model.Interfaz;
using TGC.Group.Model.Scenes;

namespace TGC.Group.Model.Escenas {
    class InicioEscena : Escena {
        ElementoTexto t;

        public void init(string mediaDir) {
            t = new Boton("Prueba GUI", 0f, 0f, () => EscenaManager.getInstance().addScene(new GameEscena()));
        }

        public void update(float deltaTime, TgcD3dInput input, TgcCamera camara) {
            t.Update(deltaTime, input);
        }

        public void render(float deltaTime) {
            t.Render();
        }

        public void dispose() {
            t.Dispose();
        }

    }
}
