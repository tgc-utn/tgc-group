using System;
using System.Drawing;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Group.Model.Interfaz;

namespace TGC.Group.Model.Escenas {
    class InicioEscena : Escena {
        ElementoTexto t;

        public void init(string mediaDir) {
            t = new ElementoTexto("Prueba GUI", 0.5f, 0.1f);
        }

        public void render(float deltaTime) {
            t.Render();
        }

        public void update(float deltaTime, TgcD3dInput input, TgcCamera camara) {
            t.Update(deltaTime);
        }

        public void dispose() {
            t.Dispose();
        }

    }
}
