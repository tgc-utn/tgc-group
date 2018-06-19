﻿using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace TGC.Group.Model.Niveles {

    class NivelDemo : Nivel {

        private TgcTexture piso, caja;

        public NivelDemo(string mediaDir) : base(mediaDir) { }

        public override void init(string mediaDir) {

            // Texturas empleadas
            piso = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "pisoJungla.jpg");
            texturasUsadas.Add(piso);
            caja = TgcTexture.createTexture(D3DDevice.Instance.Device, mediaDir + "caja.jpg");
            texturasUsadas.Add(caja);

            agregarPisoNormal(new TGCVector3(-5000, 0, -5000), new TGCVector3(10000, 10000, 10000), piso);

            // Cajas empujables
            cajas.Add(new Caja(mediaDir, new TGCVector3(0, 100, 0), new TGCVector3(100, 100, 100)));
            cajas.Add(new Caja(mediaDir, new TGCVector3(200, 100, 0), new TGCVector3(100, 100, 100)));
        }

        /*public override void dispose() {
            piso.dispose();
            caja.dispose();
            getRenderizables().ForEach(r => r.Dispose());
        }*/
    }

}
