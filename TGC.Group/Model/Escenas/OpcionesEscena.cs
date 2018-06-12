using System;
using TGC.Core.Camara;
using TGC.Core.Input;
using TGC.Group.Model.Interfaz;
using TGC.Group.Model.Scenes;

namespace TGC.Group.Model.Escenas {
    class OpcionesEscena : Escena {
        Boton volver;
        Boton volumenArriba;
        Boton volumenAbajo;
        ElementoTexto volumen;
        ElementoTexto volumenLabel;

        public void dispose() {
            volver.Dispose();
            volumenArriba.Dispose();
            volumenAbajo.Dispose();
            volumen.Dispose();
            volumenLabel.Dispose();
        }

        public void init(string mediaDir, string shaderDir) {
            volver = new Boton("Volver", 0, 0, () => EscenaManager.getInstance().goBack());

            volumenLabel = new ElementoTexto("Volumen:", 0.5f, 0.9f);
            volumenAbajo = new Boton("-", 0.7f, 0.9f, () => Opciones.getInstance().cambiarVolumenMaestro(-1));
            volumen = new ElementoTexto("", 0.8f, 0.9f);
            volumenArriba = new Boton("+", 0.9f, 0.9f, () => Opciones.getInstance().cambiarVolumenMaestro(1));
        }

        public void render(float deltaTime) {
            volumenLabel.Render();
            volver.Render();
            volumenArriba.Render();
            volumenAbajo.Render();
            volumen.Render();
        }

        public void update(float deltaTime, TgcD3dInput input, TgcCamera camara) {
            volumen.setText(Opciones.getInstance().getVolumenMaestro().ToString());

            volver.Update(deltaTime, input);
            volumenArriba.Update(deltaTime, input);
            volumenAbajo.Update(deltaTime, input);
        }
    }
}
