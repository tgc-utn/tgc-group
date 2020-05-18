using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class LaserEnemigo : Laser
    {
        public LaserEnemigo(string mediaDir, TGCVector3 posicionInicial, TGCVector3 direccion, Nave naveDelJugador)
            : base(mediaDir,posicionInicial,direccion, naveDelJugador)
        {
        }

        internal override void ColisionarConNave()
        {
            if (EstaColisionandoConNave())
            {
                naveDelJugador.ChocarConLaser();
            }
            
        }


        public override void Update(float elapsedTime)
        {
            ColisionarConNave();
            base.Update(elapsedTime);
        }

    }
}
