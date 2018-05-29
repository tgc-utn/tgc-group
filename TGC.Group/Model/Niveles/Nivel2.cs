using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;

// SEGUNDO NIVEL, GLACIARES

namespace TGC.Group.Model.Niveles {
    class Nivel2 : Nivel
    {

        TgcTexture nieve, hielo, caja;
        TgcScene[] escenasLiquenes;
        TgcScene[] escenasRocas;
        private TgcMesh liquen;
        private TgcMesh roca;

        public Nivel2(string mediaDir) : base(mediaDir) {
            siguienteNivel = null;
        }

        public override void init(string mediaDir) {

            // Texturas empleadas
            nieve = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "nieve.jpg");
            hielo = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "hielo.jpg");
            caja = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "caja.jpg");

            // Bloques de piso (no precipicios); nieve no patina, hielo si
            agregarPisoNormal(new TGCVector3(-700, 0, 4500), new TGCVector3(1400, 0, 5500), nieve);
            agregarPisoNormal(new TGCVector3(-700, 0, 1000), new TGCVector3(1400, 0, 1000), nieve);
            agregarPisoResbaladizo(new TGCVector3(-700, 0, 2000), new TGCVector3(1400, 0, 1500), hielo);
            agregarPisoResbaladizo(new TGCVector3(-700, 0, 0), new TGCVector3(1400, 0, 500), hielo);

            // Limites del pasillo-escenario
            // DEFINIR: Van en precipicio tambien o no?
            agregarPared(new TGCVector3(710, 40, 5000), new TGCVector3(20, 80, 10000), hielo);  //limite izquierdo
            agregarPared(new TGCVector3(-710, 40, 5000), new TGCVector3(20, 80, 10000), hielo); //limite derecho
            agregarPared(new TGCVector3(0, 40, 9990), new TGCVector3(1400, 80, 20), hielo);     // frente
            agregarPared(new TGCVector3(0, 40, 10), new TGCVector3(1400, 80, 20), hielo);       // fondo

            // Cajas empujables
            cajas.Add(new Caja(mediaDir, new TGCVector3(0, 40, 3000)));

            // Plataformas para precipicios
            pDesplazan.Add(new PlataformaDesplazante(new TGCVector3(500, -25, 4400), new TGCVector3(200, 50, 200), caja, new TGCVector3(500, -25, 3600), new TGCVector3(0, 0, VELPEPE))); // Precipicio 1, desplaza en Z
            pDesplazan.Add(new PlataformaDesplazante(new TGCVector3(0, -25, 4400), new TGCVector3(200, 50, 200), caja, new TGCVector3(0, -25, 3600), new TGCVector3(0, 0, VELPEPE))); // Precipicio 1, desplaza en Z
            pDesplazan.Add(new PlataformaDesplazante(new TGCVector3(-500, -25, 4400), new TGCVector3(200, 50, 200), caja, new TGCVector3(-500, -25, 3600), new TGCVector3(0, 0, VELPEPE))); // Precipicio 1, desplaza en Z
            pDesplazan.Add(new PlataformaDesplazante(new TGCVector3(0, -25, 960), new TGCVector3(100, 50, 80), caja, new TGCVector3(0, -25, 540), new TGCVector3(0, 0, VELPEPE))); // Precipicio 2, desplaza en Z

            // Arco de hielo, decoracion
            pEstaticas.Add(new Plataforma(new TGCVector3(-200, 75, 2500), new TGCVector3(50, 150, 20), hielo));
            pEstaticas.Add(new Plataforma(new TGCVector3(200, 75, 2500), new TGCVector3(50, 150, 20), hielo));
            pEstaticas.Add(new Plataforma(new TGCVector3(0, 170, 2500), new TGCVector3(500, 40, 20), hielo));

            // Scenes para objetos decorativos
            escenasLiquenes = new TgcScene[8];
            for (int i = 0; i <= 7; i++)
            {
                escenasLiquenes[i] = loaderDeco.loadSceneFromFile(mediaDir + "\\Decorativos\\Arbusto2\\Arbusto2-TgcScene.xml");
            }
            escenasRocas = new TgcScene[12];
            for (int i = 0; i <= 11; i++)
            {
                escenasRocas[i] = loaderDeco.loadSceneFromFile(mediaDir + "\\Decorativos\\Roca\\Roca-TgcScene.xml");
            }

            // Objetos decorativos
            // Liquenes
            cargarDecorativo(liquen, escenasLiquenes[0], new TGCVector3(-600, 0, 4700), new TGCVector3(2, 2, 2), 0); // Primera nieve
            cargarDecorativo(liquen, escenasLiquenes[1], new TGCVector3(600, 0, 4700), new TGCVector3(2, 2, 2), 0);
            cargarDecorativo(liquen, escenasLiquenes[2], new TGCVector3(600, 0, 2500), new TGCVector3(1, 1, 1), 0); // Primer hielo
            cargarDecorativo(liquen, escenasLiquenes[3], new TGCVector3(-600, 0, 2500), new TGCVector3(1, 1, 1), 0);
            cargarDecorativo(liquen, escenasLiquenes[4], new TGCVector3(600, 0, 2900), new TGCVector3(1, 1, 1), 0);
            cargarDecorativo(liquen, escenasLiquenes[5], new TGCVector3(-600, 0, 2900), new TGCVector3(1, 1, 1), 0);
            cargarDecorativo(liquen, escenasLiquenes[6], new TGCVector3(-500, 0, 400), new TGCVector3(1.5f, 1.5f, 1.5f), 0); // Segundo hielo
            cargarDecorativo(liquen, escenasLiquenes[7], new TGCVector3(500, 0, 400), new TGCVector3(1.5f, 1.5f, 1.5f), 0);
            // Rocas
            cargarDecorativo(roca, escenasRocas[0], new TGCVector3(300, 0, 2500), new TGCVector3(1, 1, 1), 0); // Costados del arco
            cargarDecorativo(roca, escenasRocas[1], new TGCVector3(-300, 0, 2500), new TGCVector3(1, 1, 1), 0);
            cargarDecorativo(roca, escenasRocas[2], new TGCVector3(-600, 0, 2100), new TGCVector3(2, 3, 2), 0); // Esquinas primer hielo
            cargarDecorativo(roca, escenasRocas[3], new TGCVector3(600, 0, 2100), new TGCVector3(2, 3, 2), 0);
            cargarDecorativo(roca, escenasRocas[4], new TGCVector3(600, 0, 3400), new TGCVector3(2, 3, 2), 0);
            cargarDecorativo(roca, escenasRocas[5], new TGCVector3(600, 0, 3400), new TGCVector3(2, 3, 2), 0);
            cargarDecorativo(roca, escenasRocas[6], new TGCVector3(500, 0, 1100), new TGCVector3(2, 2, 2), 0); // Segunda nieve 
            cargarDecorativo(roca, escenasRocas[7], new TGCVector3(0, 0, 1100), new TGCVector3(2, 2, 2), 0);
            cargarDecorativo(roca, escenasRocas[8], new TGCVector3(-500, 0, 1100), new TGCVector3(2, 2, 2), 0);
            cargarDecorativo(roca, escenasRocas[9], new TGCVector3(0, 0, 8000), new TGCVector3(5, 7, 5), 0); // Piedras grandes primera nieve
            cargarDecorativo(roca, escenasRocas[10], new TGCVector3(300, 0, 6500), new TGCVector3(8, 2, 5), 0);
            cargarDecorativo(roca, escenasRocas[11], new TGCVector3(-300, 0, 5500), new TGCVector3(8, 2, 5), 0);

        }

        public override void dispose()
        {

            hielo.dispose();
            caja.dispose();
            nieve.dispose();
            getRenderizables().ForEach(r => r.Dispose());

        }

    }
}
