using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Rampas
{
    abstract class Rampa
    {
        public TgcMesh rampaMesh;
        public Escenario escenario;

        public Rampa(TgcMesh rampaMesh,Escenario escenario)
        {
            this.rampaMesh = rampaMesh;
            this.escenario = escenario;
        }

        public TGCVector3[] getVertexPositions() => rampaMesh.getVertexPositions();

        public virtual float obtenerAlturaInstantanea(TGCVector3 posicionPersonaje)
        {
            return 0f;
        }
    }
}
