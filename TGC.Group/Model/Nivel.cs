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

            var pisoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "piso.jpg");
            var hieloTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "hielo.jpg");
            var cajaTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "caja.jpg");

            var piso = new TgcPlane(new TGCVector3(-500, 0, -500), new TGCVector3(2500, 0, 2500), TgcPlane.Orientations.XZplane, pisoTexture);
            pisosNormales.Add(piso);

            piso = new TgcPlane(new TGCVector3(-500, 0, -2500), new TGCVector3(2500, 0, 2000), TgcPlane.Orientations.XZplane, hieloTexture);
            pisosResbaladizos.Add(piso);

            /*
            cajas.Add(new Caja(mediaDir, new TGCVector3(-250, 40, -1000))); //coordenada Y = 40 para que tengan 40 hasta
            cajas.Add(new Caja(mediaDir, new TGCVector3(250, 40, 250)));   //el piso y 40 hasta el techo
            cajas.Add(new Caja(mediaDir, new TGCVector3(1250, 40, 1200)));
            cajas.Add(new Caja(mediaDir, new TGCVector3(700, 40, 0)));
            */

            pEstaticas.Add(new Plataforma(new TGCVector3(-500, 0, 500), new TGCVector3(100, 500, 3000), pisoTexture));
            pEstaticas.Add(new Plataforma(new TGCVector3(500, 0, 500), new TGCVector3(100, 500, 3000), pisoTexture));

            pDesplazan.Add(new PlataformaDesplazante(new TGCVector3(300, 100, 300), new TGCVector3(200, 50, 200), cajaTexture, new TGCVector3(-300, 100, -300), new TGCVector3(1, 0, 1)));

            pRotantes.Add(new PlataformaRotante(new TGCVector3(0, 100, 300), new TGCVector3(200, 50, 200), cajaTexture, FastMath.PI_HALF));
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
