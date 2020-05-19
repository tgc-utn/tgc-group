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
        public LaserDeJugador(string direccionDeScene, TGCVector3 posicionInicial, TGCVector3 direccion) : base(direccionDeScene,posicionInicial,direccion)
        {
        }


    }
}
