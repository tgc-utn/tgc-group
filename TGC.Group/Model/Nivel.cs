using System.Collections.Generic;
using System.Linq;
using TGC.Core.BoundingVolumes;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace TGC.Group.Model {
    class Nivel {
        TgcPlane piso;
        List<TgcPlane> pisosNormales;
        List<TgcPlane> pisosResbaladizos;

        public Nivel(string mediaDir) {
            pisosNormales = new List<TgcPlane>();
            pisosResbaladizos = new List<TgcPlane>();

            var pisoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "piso.jpg");
            var hieloTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "hielo.jpg");

            piso = new TgcPlane(new TGCVector3(-500, 0, -500), new TGCVector3(1000, 0, 1000), TgcPlane.Orientations.XZplane, pisoTexture);
            pisosNormales.Add(piso);

            piso = new TgcPlane(new TGCVector3(-500, 0, -1500), new TGCVector3(1000, 0, 1000), TgcPlane.Orientations.XZplane, hieloTexture);
            pisosResbaladizos.Add(piso);
        }

        public void render() {
            foreach (var piso in pisosNormales) {
                piso.Render();
            }

            foreach (var hielo in pisosResbaladizos) {
                hielo.Render();
            }
        }

        public void dispose() {
            foreach (var piso in pisosNormales) {
                piso.Dispose();
            }

            foreach (var hielo in pisosResbaladizos) {
                hielo.Dispose();
            }
        }

        public List<TgcBoundingAxisAlignBox> getBoundingBoxes() {
            var list = new List<TgcBoundingAxisAlignBox>();
            list.AddRange(pisosNormales.Select(piso => piso.BoundingBox).ToArray());
            list.AddRange(pisosResbaladizos.Select(piso => piso.BoundingBox).ToArray());
            // agregar otros boxes

            return list;
        }

        public bool esPisoResbaladizo(TgcBoundingAxisAlignBox piso) {
            return pisosResbaladizos.Select(p => p.BoundingBox).Contains(piso);
        }

    }
}
