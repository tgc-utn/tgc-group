using System.Collections.Generic;
using TGC.Core.BoundingVolumes;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace TGC.Group.Model {
    class Nivel {
        TgcPlane piso;

        public Nivel(string mediaDir) {
            var pisoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "cajaMadera4.jpg");
            piso = new TgcPlane(new TGCVector3(-500, -60, -500), new TGCVector3(1000, 0, 1000), TgcPlane.Orientations.XZplane, pisoTexture);
        }

        public void render() {
            piso.Render();
        }

        public void dispose() {
            piso.Dispose();
        }

        public List<TgcBoundingAxisAlignBox> getBoundingBoxes() {
            var list = new List<TgcBoundingAxisAlignBox>();
            list.Add(piso.BoundingBox);
            // agregar otros boxes

            return list;
        }

    }
}
