using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace TGC.Group.Modelo.Cajas
{
    class CajaNitro : Caja
    {
        public TgcMesh cajaMesh;
        public Escenario escenario;

        public CajaNitro(TgcMesh cajaMesh, Escenario escenario) : base(cajaMesh, escenario)
        {
            this.cajaMesh = cajaMesh;
            this.escenario = escenario;
        }

        public override void afectar(Personaje personaje)
        {
            aumentarVelocidad(personaje);
            escenario.eliminarObjeto(cajaMesh);
        }
    }
}
