using TGC.Core.Mathematica;

namespace TGC.Group.Model {
    class Opciones {
        private static Opciones instance;
        private int volumenMaestro;

        private Opciones() {
            volumenMaestro = 10;
        }

        public static Opciones getInstance() {
            if (instance == null) instance = new Opciones();
            return instance;
        }

        public int getVolumenMaestro() => volumenMaestro;

        public void cambiarVolumenMaestro(int cantidad) {
            volumenMaestro = FastMath.Clamp(volumenMaestro + cantidad, 0, 10);
            Musica.getInstance().setVolume(volumenMaestro);
        }
    }
}
