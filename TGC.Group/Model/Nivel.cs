using System.Collections.Generic;
using System.Linq;
using TGC.Core.BoundingVolumes;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace TGC.Group.Model {
    class Nivel {
        List<TgcPlane> pisosNormales;
        List<TgcPlane> pisosResbaladizos;
        List<Caja> cajas;
        List<Plataforma> pEstaticas;
        List<PlataformaDesplazante> pDesplazan;
        List<PlataformaRotante> pRotantes;
        // List<Bloque> pAscensor;
        TgcPlane deathPlane;

        public Nivel(string mediaDir) {
            pisosNormales = new List<TgcPlane>();
            pisosResbaladizos = new List<TgcPlane>();
            cajas = new List<Caja>();
            pEstaticas = new List<Plataforma>();
            pDesplazan = new List<PlataformaDesplazante>();
            pRotantes = new List<PlataformaRotante>();

            deathPlane = new TgcPlane(/*0, -2000, 0*/);
            // si colisiona con el death plane lo mandamos al origen

            var pisoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "pisoJungla.jpg");
            var hieloTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "hielo.jpg");
            var cajaTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "caja.jpg");
            var paredJunglaTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "paredJungla.jpg");
            var desiertoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "arena.jpg");
            var piedraTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "piedra.png");
            var precipicioTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "precipicio.jpg");
            var maderaTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "tronco.jpg");

            var piso = new TgcPlane(new TGCVector3(-500, 0, -500), new TGCVector3(2500, 0, 2500), TgcPlane.Orientations.XZplane, pisoTexture);
            pisosNormales.Add(piso); //piso de la jungla

            piso = new TgcPlane(new TGCVector3(-500, -180, 2600), new TGCVector3(1000, 0, 3500), TgcPlane.Orientations.XZplane, desiertoTexture);
            pisosNormales.Add(piso); // piso del desierto

            piso = new TgcPlane(new TGCVector3(-500, 0, -3000), new TGCVector3(2500, 0, 2500), TgcPlane.Orientations.XZplane, hieloTexture);
            pisosResbaladizos.Add(piso); // piso de hielo

            /*
            cajas.Add(new Caja(mediaDir, new TGCVector3(-250, 40, -1000))); //coordenada Y = 40 para que tengan 40 hasta
            cajas.Add(new Caja(mediaDir, new TGCVector3(250, 40, 250)));   //el piso y 40 hasta el techo
            cajas.Add(new Caja(mediaDir, new TGCVector3(1250, 40, 1200)));
            cajas.Add(new Caja(mediaDir, new TGCVector3(700, 40, 0)));
            */

            // paredes de la jungla
            pEstaticas.Add(new Plataforma(new TGCVector3(-500, 150, 600), new TGCVector3(100, 300, 2800), paredJunglaTexture)); //laterales jungla derecha
            pEstaticas.Add(new Plataforma(new TGCVector3(500, 150, 600), new TGCVector3(100, 300, 2800), paredJunglaTexture));
            pEstaticas.Add(new Plataforma(new TGCVector3(1975, 150, 0), new TGCVector3(50, 300, 1600), paredJunglaTexture)); // borde izquierdo jungla derecha
            pEstaticas.Add(new Plataforma(new TGCVector3(1250, 30, 1990), new TGCVector3(1500, 60, 20), paredJunglaTexture)); // fondo jungla izquierda

            //paredes del desierto; el desierto está a un nivel inferior que la jungla y los glaciares
            pEstaticas.Add(new Plataforma(new TGCVector3(500, -150, 4350), new TGCVector3(100, 60, 3500), desiertoTexture));
            pEstaticas.Add(new Plataforma(new TGCVector3(-500, -150, 4350), new TGCVector3(100, 60, 3500), desiertoTexture));
            pEstaticas.Add(new Plataforma(new TGCVector3(0, -165, 6090), new TGCVector3(900, 70, 20), desiertoTexture));

            //escalinatas de piedra, separan jungla de desierto
            var tamanioEscalinata = new TGCVector3(900, 60, 200);
            pEstaticas.Add(new Plataforma(new TGCVector3(0, -150, 2500), tamanioEscalinata, piedraTexture));  // escalinata inferior
            pEstaticas.Add(new Plataforma(new TGCVector3(0, -90, 2300), tamanioEscalinata, piedraTexture));   // escalinata del medio
            pEstaticas.Add(new Plataforma(new TGCVector3(0, -30, 2100), tamanioEscalinata, piedraTexture));   // escalinata superior
            pEstaticas.Add(new Plataforma(new TGCVector3(500, -140, 2500), new TGCVector3(100, 80, 200), piedraTexture));  // contornos inferior
            pEstaticas.Add(new Plataforma(new TGCVector3(-500, -140, 2500), new TGCVector3(100, 80, 200), piedraTexture));
            pEstaticas.Add(new Plataforma(new TGCVector3(500, -115, 2300), new TGCVector3(100, 130, 200), piedraTexture));  // contornos del medio
            pEstaticas.Add(new Plataforma(new TGCVector3(-500, -115, 2300), new TGCVector3(100, 130, 200), piedraTexture));
            pEstaticas.Add(new Plataforma(new TGCVector3(500, -80, 2100), new TGCVector3(100, 200, 200), piedraTexture));  // contornos superior
            pEstaticas.Add(new Plataforma(new TGCVector3(-500, -80, 2100), new TGCVector3(100, 200, 200), piedraTexture));

            //paredes de los glaciares
            pEstaticas.Add(new Plataforma(new TGCVector3(-500, 200, -1900), new TGCVector3(100, 400, 2200), hieloTexture)); // derecha
            pEstaticas.Add(new Plataforma(new TGCVector3(750, 200, -2510), new TGCVector3(2500, 400, 20), hieloTexture));  // fondo
            pEstaticas.Add(new Plataforma(new TGCVector3(1975, 200, -1900), new TGCVector3(50, 400, 2200), hieloTexture)); // izquierda

            // precipicio del tronco
            piso = new TgcPlane(new TGCVector3(2000, -200, 800), new TGCVector3(1000, 0, 1200), TgcPlane.Orientations.XZplane, precipicioTexture);
            pisosNormales.Add(piso); //TODO: Configurar el deathplane, deberia ir ahi?
            pEstaticas.Add(new Plataforma(new TGCVector3(2500, -100, 800), new TGCVector3(1000, 200, 2), precipicioTexture)); // fondo
            pEstaticas.Add(new Plataforma(new TGCVector3(2000, -100, 1400), new TGCVector3(2, 200, 1200), precipicioTexture)); // derecha
            pEstaticas.Add(new Plataforma(new TGCVector3(3000, -100, 1400), new TGCVector3(2, 200, 1200), precipicioTexture)); // izquierda
            pEstaticas.Add(new Plataforma(new TGCVector3(2500, -100, 2000), new TGCVector3(1000, 200, 2), precipicioTexture)); // frontal

            pDesplazan.Add(new PlataformaDesplazante(new TGCVector3(300, 120, 300), new TGCVector3(200, 50, 200), cajaTexture, new TGCVector3(-300, 100, -300), new TGCVector3(0.5f, 0, 0.5f)));
            // tronco que se desplaza en el precipicio; TODO: Revisar movimiento en conjunto con tgcito
            pDesplazan.Add(new PlataformaDesplazante(new TGCVector3(2075, -60, 1400), new TGCVector3(150, 50, 80), maderaTexture, new TGCVector3(2925, -60, 1400), new TGCVector3(0.2f, 0, 0)));

            pRotantes.Add(new PlataformaRotante(new TGCVector3(0, 100, 300), new TGCVector3(200, 50, 200), cajaTexture, FastMath.QUARTER_PI/4));
        }

        public void update() {
            foreach (var p in pDesplazan) {
                p.update();
            }

            foreach (var p in pRotantes) {
                p.update();
            }
        }

        public void render() {
            foreach (var piso in pisosNormales) {
                piso.Render();
            }

            foreach (var hielo in pisosResbaladizos) {
                hielo.Render();
            }

            foreach (var caja in cajas) {
                caja.render();
            }

            foreach (var p in pEstaticas) {
                p.render();
            }

            foreach (var p in pDesplazan) {
                p.render();
            }

            foreach (var p in pRotantes) {
                p.render();
            }
        }

        public void dispose() {
            foreach (var piso in pisosNormales) {
                piso.Dispose();
            }

            foreach (var hielo in pisosResbaladizos) {
                hielo.Dispose();
            }

            foreach (var caja in cajas) {
                caja.dispose();
            }

            foreach (var p in pEstaticas) {
                p.dispose();
            }

            foreach (var p in pDesplazan) {
                p.dispose();
            }

            foreach (var p in pRotantes) {
                p.dispose();
            }
        }

        public List<TgcBoundingAxisAlignBox> getBoundingBoxes() {
            var list = new List<TgcBoundingAxisAlignBox>();
            list.AddRange(getPisos().ToArray());
            list.AddRange(cajas.Select(caja => caja.getSuperior()).ToArray());
            list.AddRange(cajas.Select(caja => caja.getCuerpo()).ToArray());
            return list;
        }

        public List<TgcBoundingAxisAlignBox> getPisos() {
            var list = new List<TgcBoundingAxisAlignBox>();
            list.AddRange(pisosNormales.Select(piso => piso.BoundingBox).ToArray());
            list.AddRange(pisosResbaladizos.Select(piso => piso.BoundingBox).ToArray());
            list.AddRange(pEstaticas.Select(caja => caja.getAABB()).ToArray());
            list.AddRange(pDesplazan.Select(caja => caja.getAABB()).ToArray());
            list.AddRange(pRotantes.Select(caja => caja.getAABB()).ToArray());

            return list;
        }

        public List<Caja> getCajas() {
            return cajas;
        }

        public bool esPisoResbaladizo(TgcBoundingAxisAlignBox piso) {
            return pisosResbaladizos.Select(p => p.BoundingBox).Contains(piso);
        }

        public bool esPisoDesplazante(TgcBoundingAxisAlignBox piso) {
            return pDesplazan.Select(p => p.getAABB()).Contains(piso);
        }

        public bool esPisoRotante(TgcBoundingAxisAlignBox piso) {
            return pRotantes.Select(p => p.getAABB()).Contains(piso);
        }

        public PlataformaDesplazante getPlataformaDesplazante(TgcBoundingAxisAlignBox piso) {
            return pDesplazan.Find(p => p.getAABB() == piso);
        }

        public PlataformaRotante getPlataformaRotante(TgcBoundingAxisAlignBox piso) {
            return pRotantes.Find(p => p.getAABB() == piso);
        }
    }
}
