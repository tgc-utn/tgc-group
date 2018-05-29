using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Group.Model.Niveles;

namespace TGC.Group.Model.Niveles
{
    class Nivel3 : Nivel
    {
        TgcTexture arena, piedra, caja;
        TgcScene[] escenasCalaveras;
        TgcScene[] escenasEsqueletos;
        TgcScene[] escenasFaraones;
        TgcScene[] escenasPalmeras;
        TgcScene[] escenasPilares;
        private TgcMesh calavera;
        private TgcMesh esqueleto;
        private TgcMesh faraon;
        private TgcMesh palmera;
        private TgcMesh pilar;

        public Nivel3(string mediaDir) : base(mediaDir)
        {
            siguienteNivel = null;
        }

        public override void init(string mediaDir)
        {

            // Texturas empleadas
            arena = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "arena.jpg");
            piedra = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "piedra.png");
            caja = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "caja.jpg");

            /// Bloques de piso (no precipicios)
            // Pisos inferiores, en Y = 0
            agregarPisoNormal(new TGCVector3(-700, 0, 9000), new TGCVector3(1400, 0, 1000), arena); 
            agregarPisoNormal(new TGCVector3(200, 0, 8000), new TGCVector3(200, 0, 1000), arena);
            agregarPisoNormal(new TGCVector3(-300, 0, 8000), new TGCVector3(200, 0, 1000), arena);
            agregarPisoNormal(new TGCVector3(-700, 0, 4500), new TGCVector3(1400, 0, 3500), arena);
            // Pisos superiores, en Y = 300
            agregarPisoNormal(new TGCVector3(-700, 300, 2000), new TGCVector3(1400, 0, 2500), arena);
            agregarPisoNormal(new TGCVector3(-700, 300, 0), new TGCVector3(1400, 0, 1000), arena);

            /// Limites del pasillo-escenario
            /// DEFINIR: Van en precipicio tambien o no?
            // Limites zona inferior
            agregarPared(new TGCVector3(710, 40, 7500), new TGCVector3(20, 80, 5000), arena);  // limite izquierdo
            agregarPared(new TGCVector3(-710, 40, 7500), new TGCVector3(20, 80, 5000), arena); // limite derecho
            agregarPared(new TGCVector3(0, 40, 9990), new TGCVector3(1400, 80, 20), arena);      // fondo
            // Limites zona ascensor
            agregarPared(new TGCVector3(710, 170, 4750), new TGCVector3(20, 340, 500), arena);  // limite izquierdo
            agregarPared(new TGCVector3(-710, 170, 4750), new TGCVector3(20, 340, 500), arena); // limite derecho
            agregarPared(new TGCVector3(0, 170, 4500), new TGCVector3(1400, 340, 20), arena);   // frente
            // Limites zona superior
            agregarPared(new TGCVector3(710, 340, 2250), new TGCVector3(20, 80, 4500), arena);  // limite izquierdo
            agregarPared(new TGCVector3(-710, 340, 2250), new TGCVector3(20, 80, 4500), arena); // limite derecho
            agregarPared(new TGCVector3(0, 340, 10), new TGCVector3(1400, 80, 20), arena);      // frente

            // Cajas empujables, en nivel inferior
            cajas.Add(new Caja(mediaDir, new TGCVector3(-300, 40, 7500)));
            cajas.Add(new Caja(mediaDir, new TGCVector3(-300, 40, 6500)));
            cajas.Add(new Caja(mediaDir, new TGCVector3(200, 40, 7500)));
            cajas.Add(new Caja(mediaDir, new TGCVector3(200, 40, 6500)));

            // Plataforma para cruzar precipicio superior
            pDesplazan.Add(new PlataformaDesplazante(new TGCVector3(0, 275, 1900), new TGCVector3(200, 50, 200), caja, new TGCVector3(0, 275, 1100), new TGCVector3(0, 0, VELPEPE)));

            // Ascensor para pasar de inferiores a superiores
            pAscensor.Add(new PlataformaAscensor(new TGCVector3(0, 25, 4750), new TGCVector3(300, 50, 500), piedra, 275f, 100));

            // Plataformas rotantes, en nivel superior
            pRotantes.Add(new PlataformaRotante(new TGCVector3(-200, 50, 3000), new TGCVector3(300, 50, 200), caja, FastMath.PI * 100));
            pRotantes.Add(new PlataformaRotante(new TGCVector3(200, 50, 3000), new TGCVector3(300, 50, 200), caja, FastMath.PI * (-100)));

            // Scenes para objetos decorativos
            escenasCalaveras = new TgcScene[8];
            for (int i = 0; i <= 7; i++)
            {
                escenasCalaveras[i] = loaderDeco.loadSceneFromFile(mediaDir + "\\Decorativos\\Calabera\\Calabera-TgcScene.xml");
            }
            escenasEsqueletos = new TgcScene[3];
            for (int i = 0; i <= 2; i++)
            {
                escenasEsqueletos[i] = loaderDeco.loadSceneFromFile(mediaDir + "\\Decorativos\\EsqueletoHumano\\Esqueleto-TgcScene.xml");
            }
            escenasPalmeras = new TgcScene[8];
            for (int i = 0; i <= 7; i++)
            {
                escenasPalmeras[i] = loaderDeco.loadSceneFromFile(mediaDir + "\\Decorativos\\Palmera\\Palmera-TgcScene.xml");
            }
            escenasPilares = new TgcScene[6];
            for (int i = 0; i <= 5; i++)
            {
                escenasPilares[i] = loaderDeco.loadSceneFromFile(mediaDir + "\\Decorativos\\PilarEgipcio\\PilarEgipcio-TgcScene.xml");
            }
            escenasFaraones = new TgcScene[2];
            for (int i = 0; i <= 1; i++)
            {
                escenasFaraones[i] = loaderDeco.loadSceneFromFile(mediaDir + "\\Decorativos\\EstatuaFaraon\\EstatuaFaraon-TgcScene.xml");
            }

            // Objetos decorativos
            // Calaveras
            cargarDecorativo(calavera, escenasCalaveras[0], new TGCVector3(375, 0, 8300), new TGCVector3(1, 1, 1), FastMath.PI_HALF);
            cargarDecorativo(calavera, escenasCalaveras[1], new TGCVector3(375, 0, 8600), new TGCVector3(1, 1, 1), FastMath.PI_HALF);
            cargarDecorativo(calavera, escenasCalaveras[2], new TGCVector3(225, 0, 8300), new TGCVector3(1, 1, 1), FastMath.PI_HALF * 3);
            cargarDecorativo(calavera, escenasCalaveras[3], new TGCVector3(225, 0, 8600), new TGCVector3(1, 1, 1), FastMath.PI_HALF * 3);
            cargarDecorativo(calavera, escenasCalaveras[4], new TGCVector3(-275, 0, 8300), new TGCVector3(1, 1, 1), FastMath.PI_HALF * 3);
            cargarDecorativo(calavera, escenasCalaveras[5], new TGCVector3(-275, 0, 8600), new TGCVector3(1, 1, 1), FastMath.PI_HALF * 3);
            cargarDecorativo(calavera, escenasCalaveras[6], new TGCVector3(-125, 0, 8300), new TGCVector3(1, 1, 1), FastMath.PI_HALF);
            cargarDecorativo(calavera, escenasCalaveras[7], new TGCVector3(-125, 0, 8600), new TGCVector3(1, 1, 1), FastMath.PI_HALF);
            // Esqueletos
            cargarDecorativo(esqueleto, escenasEsqueletos[0], new TGCVector3(-550, 0, 9100), new TGCVector3(1, 1, 1), 0);
            cargarDecorativo(esqueleto, escenasEsqueletos[1], new TGCVector3(-50, 0, 9100), new TGCVector3(1, 1, 1), 0);
            cargarDecorativo(esqueleto, escenasEsqueletos[2], new TGCVector3(500, 0, 9100), new TGCVector3(1, 1, 1), 0);
            // Estatuas de faraones
            cargarDecorativo(faraon, escenasFaraones[0], new TGCVector3(300, 470, 300), new TGCVector3(0.3f, 0.3f, 0.3f), FastMath.PI);
            cargarDecorativo(faraon, escenasFaraones[1], new TGCVector3(-300, 470, 300), new TGCVector3(0.3f, 0.3f, 0.3f), FastMath.PI);
            // Pilares egipcios
            cargarDecorativo(pilar, escenasPilares[0], new TGCVector3(-500, 0, 5200), new TGCVector3(2, 2, 2), 0);
            cargarDecorativo(pilar, escenasPilares[1], new TGCVector3(-400, 0, 5200), new TGCVector3(2, 2, 2), 0);
            cargarDecorativo(pilar, escenasPilares[2], new TGCVector3(-300, 0, 5200), new TGCVector3(2, 2, 2), 0);
            cargarDecorativo(pilar, escenasPilares[3], new TGCVector3(500, 0, 5200), new TGCVector3(2, 2, 2), 0);
            cargarDecorativo(pilar, escenasPilares[4], new TGCVector3(400, 0, 5200), new TGCVector3(2, 2, 2), 0);
            cargarDecorativo(pilar, escenasPilares[5], new TGCVector3(300, 0, 5200), new TGCVector3(2, 2, 2), 0);
            // Palmeras
            cargarDecorativo(palmera, escenasPalmeras[0], new TGCVector3(-600, 0, 6000), new TGCVector3(1, 1, 1), 0); // del piso inferior
            cargarDecorativo(palmera, escenasPalmeras[1], new TGCVector3(-600, 0, 7000), new TGCVector3(1, 1, 1), 0);
            cargarDecorativo(palmera, escenasPalmeras[2], new TGCVector3(600, 0, 6000), new TGCVector3(1, 1, 1), 0);
            cargarDecorativo(palmera, escenasPalmeras[3], new TGCVector3(600, 0, 7000), new TGCVector3(1, 1, 1), 0);
            cargarDecorativo(palmera, escenasPalmeras[4], new TGCVector3(-600, 300, 2800), new TGCVector3(1, 1, 1), 0); // del piso superior
            cargarDecorativo(palmera, escenasPalmeras[5], new TGCVector3(-600, 300, 3600), new TGCVector3(1, 1, 1), 0);
            cargarDecorativo(palmera, escenasPalmeras[6], new TGCVector3(600, 300, 2800), new TGCVector3(1, 1, 1), 0);
            cargarDecorativo(palmera, escenasPalmeras[7], new TGCVector3(600, 300, 3600), new TGCVector3(1, 1, 1), 0);

            //lfBox = TGCBox.fromSize(new TGCVector3(0, 0, 100), new TGCVector3(100, 100, 100));

        }

        public override void dispose()
        {

            arena.dispose();
            caja.dispose();
            piedra.dispose();
            getRenderizables().ForEach(r => r.Dispose());

        }
    }
}
