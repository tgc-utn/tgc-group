using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;

namespace TGC.Group.Modelo.Rampas
{
    class RampaZ : Rampa
    {
        public TgcMesh rampaMesh;
        public Escenario escenario;

        public RampaZ(TgcMesh rampaMesh, Escenario escenario) : base(rampaMesh, escenario)
        {
            this.rampaMesh = rampaMesh;
            this.escenario = escenario;

        }

        public override float obtenerAlturaInstantanea(TGCVector3 posicionPersonaje)
        {

            List<TGCVector3> listaVertices = new List<TGCVector3>();
            var vertices = getVertexPositions().GetEnumerator();

            while (vertices.MoveNext()) listaVertices.Add((TGCVector3)(vertices.Current));
            listaVertices.Sort(new ComparadorYTgcVector3());

            TGCVector3 verticeMasAlto = listaVertices[0];
            listaVertices.Reverse();
            TGCVector3 verticeMasBajo = listaVertices[0];
            
            
            float longitudRampa = FastMath.Abs(verticeMasAlto.Z - verticeMasBajo.Z);
            

            float pendienteRampa = (verticeMasAlto.Y - verticeMasBajo.Y) / FastMath.Abs(verticeMasAlto.Z - verticeMasBajo.Z);
            float diferenciaPersonajeRampa = FastMath.Abs(verticeMasAlto.Z - posicionPersonaje.Z);


            float YPorDesnivel = pendienteRampa * FastMath.Abs(longitudRampa - diferenciaPersonajeRampa);
            

            return YPorDesnivel;
        }
    }
}
