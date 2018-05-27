using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.Textures;
using TGC.Group.Model.Niveles;

namespace TGC.Group.Model {
    class NivelDemo : Nivel {
        TgcTexture piso, hielo, caja, paredJungla, desierto, piedra, precipicio, madera;

        public NivelDemo(string mediaDir) : base(mediaDir) {
        }

        public override void init(string mediaDir) {
            piso = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "pisoJungla.jpg");
            hielo = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "hielo.jpg");
            caja = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "caja.jpg");
            paredJungla = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "paredJungla.jpg");
            desierto = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "arena.jpg");
            piedra = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "piedra.png");
            precipicio = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "precipicio.jpg");
            madera = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "tronco.jpg");

            var plano = new TgcPlane(new TGCVector3(-500, 0, -500), new TGCVector3(2500, 0, 2500), TgcPlane.Orientations.XZplane, piso);
            pisosNormales.Add(plano); //piso de la jungla

            // pisos del desierto
            plano = new TgcPlane(new TGCVector3(-500, -180, 2600), new TGCVector3(1000, 0, 700), TgcPlane.Orientations.XZplane, desierto);
            pisosNormales.Add(plano);
            plano = new TgcPlane(new TGCVector3(-500, -180, 3300), new TGCVector3(350, 0, 800), TgcPlane.Orientations.XZplane, desierto);
            pisosNormales.Add(plano);
            plano = new TgcPlane(new TGCVector3(350, -180, 3300), new TGCVector3(150, 0, 2800), TgcPlane.Orientations.XZplane, desierto);
            pisosNormales.Add(plano);
            plano = new TgcPlane(new TGCVector3(-150, -180, 3500), new TGCVector3(500, 0, 2600), TgcPlane.Orientations.XZplane, desierto);
            pisosNormales.Add(plano);
            plano = new TgcPlane(new TGCVector3(-500, -180, 4100), new TGCVector3(100, 0, 2000), TgcPlane.Orientations.XZplane, desierto);
            pisosNormales.Add(plano);
            plano = new TgcPlane(new TGCVector3(-400, -180, 4900), new TGCVector3(250, 0, 1200), TgcPlane.Orientations.XZplane, desierto);
            pisosNormales.Add(plano);

            plano = new TgcPlane(new TGCVector3(-500, 0, -3000), new TGCVector3(2500, 0, 2500), TgcPlane.Orientations.XZplane, hielo);
            pisosResbaladizos.Add(plano); // piso de hielo

            // paredes de la jungla
            pEstaticas.Add(new Plataforma(new TGCVector3(-500, 150, 600), new TGCVector3(100, 300, 2800), paredJungla)); //laterales jungla derecha
            pEstaticas.Add(new Plataforma(new TGCVector3(500, 150, 600), new TGCVector3(100, 300, 2800), paredJungla));
            pEstaticas.Add(new Plataforma(new TGCVector3(1975, 150, 0), new TGCVector3(50, 300, 1600), paredJungla)); // borde izquierdo jungla derecha
            pEstaticas.Add(new Plataforma(new TGCVector3(1250, 30, 1990), new TGCVector3(1500, 60, 20), paredJungla)); // fondo jungla izquierda

            // paredes del desierto; el desierto está a un nivel inferior que la jungla y los glaciares
            pEstaticas.Add(new Plataforma(new TGCVector3(500, -150, 4350), new TGCVector3(100, 60, 3500), desierto));
            pEstaticas.Add(new Plataforma(new TGCVector3(-500, -150, 4350), new TGCVector3(100, 60, 3500), desierto));
            pEstaticas.Add(new Plataforma(new TGCVector3(0, -165, 6090), new TGCVector3(900, 70, 20), desierto));

            // precipicios del desierto
            plano = new TgcPlane(new TGCVector3(-150, -380, 3300), new TGCVector3(500, 0, 200), TgcPlane.Orientations.XZplane, precipicio);
            pMuerte.Add(plano); // precipicio ancho
            plano = new TgcPlane(new TGCVector3(-400, -380, 4100), new TGCVector3(250, 0, 800), TgcPlane.Orientations.XZplane, precipicio);
            pMuerte.Add(plano); // precipicio largo
            pEstaticas.Add(new Plataforma(new TGCVector3(100, -280, 3300), new TGCVector3(500, 200, 2), precipicio)); // paredes precipicio ancho
            pEstaticas.Add(new Plataforma(new TGCVector3(350, -280, 3400), new TGCVector3(2, 200, 200), precipicio));
            pEstaticas.Add(new Plataforma(new TGCVector3(-150, -280, 3400), new TGCVector3(2, 200, 200), precipicio));
            pEstaticas.Add(new Plataforma(new TGCVector3(100, -280, 3500), new TGCVector3(500, 200, 2), precipicio));
            pEstaticas.Add(new Plataforma(new TGCVector3(-275, -280, 4100), new TGCVector3(250, 200, 2), precipicio)); // paredes precipicio largo
            pEstaticas.Add(new Plataforma(new TGCVector3(-150, -280, 4500), new TGCVector3(2, 200, 800), precipicio));
            pEstaticas.Add(new Plataforma(new TGCVector3(-400, -280, 4500), new TGCVector3(2, 200, 800), precipicio));
            pEstaticas.Add(new Plataforma(new TGCVector3(-275, -280, 4900), new TGCVector3(250, 200, 2), precipicio));

            // escalinatas de piedra, separan jungla de desierto
            var tamanioEscalinata = new TGCVector3(900, 60, 200);
            pEstaticas.Add(new Plataforma(new TGCVector3(0, -150, 2500), tamanioEscalinata, piedra));  // escalinata inferior
            pEstaticas.Add(new Plataforma(new TGCVector3(0, -90, 2300), tamanioEscalinata, piedra));   // escalinata del medio
            pEstaticas.Add(new Plataforma(new TGCVector3(0, -30, 2100), tamanioEscalinata, piedra));   // escalinata superior
            pEstaticas.Add(new Plataforma(new TGCVector3(500, -140, 2500), new TGCVector3(100, 80, 200), piedra));  // contornos inferior
            pEstaticas.Add(new Plataforma(new TGCVector3(-500, -140, 2500), new TGCVector3(100, 80, 200), piedra));
            pEstaticas.Add(new Plataforma(new TGCVector3(500, -115, 2300), new TGCVector3(100, 130, 200), piedra));  // contornos del medio
            pEstaticas.Add(new Plataforma(new TGCVector3(-500, -115, 2300), new TGCVector3(100, 130, 200), piedra));
            pEstaticas.Add(new Plataforma(new TGCVector3(500, -80, 2100), new TGCVector3(100, 200, 200), piedra));  // contornos superior
            pEstaticas.Add(new Plataforma(new TGCVector3(-500, -80, 2100), new TGCVector3(100, 200, 200), piedra));

            //paredes de los glaciares
            pEstaticas.Add(new Plataforma(new TGCVector3(-500, 200, -1900), new TGCVector3(100, 400, 2200), hielo)); // derecha
            pEstaticas.Add(new Plataforma(new TGCVector3(750, 200, -2510), new TGCVector3(2500, 400, 20), hielo));  // fondo
            pEstaticas.Add(new Plataforma(new TGCVector3(1975, 200, -1900), new TGCVector3(50, 400, 2200), hielo)); // izquierda

            // precipicio del tronco
            plano = new TgcPlane(new TGCVector3(2000, -200, 800), new TGCVector3(1000, 0, 1200), TgcPlane.Orientations.XZplane, precipicio);
            pMuerte.Add(plano); //TODO: Configurar el deathplane, deberia ir ahi?
            pEstaticas.Add(new Plataforma(new TGCVector3(2500, -100, 800), new TGCVector3(1000, 200, 2), precipicio)); // fondo
            pEstaticas.Add(new Plataforma(new TGCVector3(2000, -100, 1400), new TGCVector3(2, 200, 1200), precipicio)); // derecha
            pEstaticas.Add(new Plataforma(new TGCVector3(3000, -100, 1400), new TGCVector3(2, 200, 1200), precipicio)); // izquierda
            pEstaticas.Add(new Plataforma(new TGCVector3(2500, -100, 2000), new TGCVector3(1000, 200, 2), precipicio)); // frontal

            pDesplazan.Add(new PlataformaDesplazante(new TGCVector3(0, -50, 5000), new TGCVector3(200, 50, 200), caja, new TGCVector3(-200, -50, 5000), new TGCVector3(2f, 0, 0)));
            pDesplazan.Add(new PlataformaDesplazante(new TGCVector3(2075, -60, 1400), new TGCVector3(150, 50, 80), madera, new TGCVector3(2925, -60, 1400), new TGCVector3(2f, 0, 0)));

            pRotantes.Add(new PlataformaRotante(new TGCVector3(0, 50, 300), new TGCVector3(200, 50, 200), caja, FastMath.PI * 100));
            pAscensor.Add(new PlataformaAscensor(new TGCVector3(0, -200, 5200), new TGCVector3(200, 50, 200), caja, 200, 1));

            cajas.Add(new Caja(mediaDir, new TGCVector3(0, 0, 5800)));
        }

        public override void dispose() {
            piso.dispose();
            hielo.dispose();
            caja.dispose();
            paredJungla.dispose();
            desierto.dispose();
            piedra.dispose();
            precipicio.dispose();
            madera.dispose();

            getRenderizables().ForEach(r => r.Dispose());
        }
    }
}
