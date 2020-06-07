using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class LaserDeJugador : Laser
    {
        public LaserDeJugador(string direccionDeScene, TGCVector3 posicionInicial, TGCVector3 direccion) : base(direccionDeScene,posicionInicial,direccion)
        {
            this.velocidad = 10f;
        }
        public override void Update(float elapsedTime)
        {
            if (SuperoTiempoDeVida(1) || ColisionaConMapa() || ImpactoAUnDestruible)
            {
                GameManager.Instance.QuitarRenderizable(this);
            }

            base.Update(elapsedTime);

        }

        public override TGCQuaternion QuaternionDireccion(TGCVector3 direccionDisparoNormalizado)
        {
            return TGCQuaternion.RotationAxis(new TGCVector3(0, 0, 0), 0);
        }

    }
}
