using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;


namespace TGC.Group.Model.Rampas
{
    class RampaX : Rampa
    {
        public TgcMesh rampaMesh;
        public Escenario escenario;

        public RampaX(TgcMesh rampaMesh, Escenario escenario) : base(rampaMesh, escenario)
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
            
            float longitudRampa = FastMath.Abs(verticeMasAlto.X - verticeMasBajo.X);
            

            float pendienteRampa = (verticeMasAlto.Y - verticeMasBajo.Y) / FastMath.Abs(verticeMasAlto.X - verticeMasBajo.Y);
            float diferenciaPersonajeRampa = FastMath.Abs(verticeMasAlto.X - posicionPersonaje.X);


            float YPorDesnivel = pendienteRampa * FastMath.Abs(longitudRampa - diferenciaPersonajeRampa);
            

            return YPorDesnivel;

           
        }
    }
}
