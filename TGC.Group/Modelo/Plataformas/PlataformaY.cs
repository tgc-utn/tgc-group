using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Collision;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Modelo.Plataformas
{
    class PlataformaY : Plataforma
    {
        private int toleranciaLimiteSuperior = 10;
        private TgcMesh plataformaMesh;
        private Escenario escenario;
        private TGCVector3 posicionInicial;

        public PlataformaY(TgcMesh plataformaMesh, Escenario escenario) : base(plataformaMesh, escenario)
        {
            this.plataformaMesh = plataformaMesh;
            this.posicionInicial = plataformaMesh.Position;
            this.escenario = escenario;
            vectorMovimiento = new TGCVector3(0, -5, 0);
        }

        public override TGCVector3 VectorMovimiento()
        {
            if (vectorMovimiento.Y < 0) return vectorMovimiento;
            else return vectorMovimiento + vectorMovimiento;
        }


        public override void Update(float tiempo)
        {

            TGCVector3 posicionSiguiente = plataformaMesh.Position + vectorMovimiento;

            //Si la plataforma colisiona con el piso, cambiamos el sentido de movimiento.
            if (escenario.colisionaConPiso(plataformaMesh) || escenario.colisionaConLava(plataformaMesh)) cambiarDirMovimiento();

            //Si la plataforma supera en una cierta cantidad de veces a su altura inicial, cambiamos el sentido de movimiento.
            if (posicionSiguiente.Y >= toleranciaLimiteSuperior * posicionInicial.Y) cambiarDirMovimiento();

            //Desplazamos la plataforma en el sentido correcto.
            plataformaMesh.Move(vectorMovimiento);
        }

		public void cambiarDirMovimiento()
        {
            vectorMovimiento = TGCVector3.Multiply(vectorMovimiento, -1);
        }

    }
}
