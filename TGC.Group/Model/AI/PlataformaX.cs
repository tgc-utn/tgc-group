using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Collision;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model.AI
{
    class PlataformaX : Plataforma
    {
       
        private TgcMesh plataformaMesh;
        private Escenario escenario;
        private TGCVector3 posicionInicial;
         //Solo moveriamos en Y a las plataformas.

        public PlataformaX(TgcMesh plataformaMesh, Escenario escenario) : base(plataformaMesh, escenario)
        {
            this.plataformaMesh = plataformaMesh;
            this.posicionInicial = plataformaMesh.Position;
            this.escenario = escenario;
            vectorMovimiento = new TGCVector3(-10, 0, 0);
        }

        public override bool colisionaConPersonaje(TgcBoundingSphere esferaPersonaje)
        {
            bool colision = false;
            TgcBoundingSphere esferaAux = esferaPersonaje;
            TGCVector3 centroOriginal = esferaAux.Center;

            TGCVector3 centroTestColision = centroOriginal;
            centroTestColision.Y -= 50f;
            esferaAux.setCenter(centroTestColision);

            colision= base.colisionaConPersonaje(esferaAux);

            esferaAux.setCenter(centroOriginal);
            return colision;
        }

        public override void Update()
        {
            TGCVector3 posicionSiguiente = plataformaMesh.Position + vectorMovimiento;

            //Si la plataforma colisiona con una pared, cambiamos el sentido de movimiento.
            if (escenario.colisionaConPared(plataformaMesh)) vectorMovimiento.Multiply(-1);
            
            //Desplazamos la plataforma en el sentido correcto.
            plataformaMesh.Move(vectorMovimiento);
        }
    }
}
