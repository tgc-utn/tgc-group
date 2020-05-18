using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class LaserDeJugador : Laser
    {
        public LaserDeJugador(string mediaDir, TGCVector3 posicionInicial, TGCVector3 direccion, Nave naveDelJugador) : base(mediaDir, posicionInicial, direccion, naveDelJugador)
        {
            this.direccionDeScene = mediaDir + "Xwing\\laserBueno-TgcScene.xml";
        }
    }
}
