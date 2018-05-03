using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Vehiculos
{
    class Ruedas
    {
        private TgcMesh delanteraIzquierda;
        private TgcMesh delanteraDerecha;
        private List<TgcMesh> ruedasTraseras = new List<TgcMesh>();

        public Ruedas(TgcMesh delanteraIzquierda, TgcMesh delanteraDerecha, List<TgcMesh> ruedasTraseras)
        {
            this.delanteraIzquierda = delanteraIzquierda;
            this.delanteraDerecha = delanteraDerecha;
            this.ruedasTraseras = ruedasTraseras;
            this.Escalar();
        }

        private void Escalar()
        {
            TGCVector3 escalado = new TGCVector3(0.05f, 0.05f, 0.05f);
            this.delanteraIzquierda.Scale = escalado;
            this.delanteraDerecha.Scale = escalado;
            foreach (TgcMesh rueda in this.ruedasTraseras)
            {
                rueda.Scale = escalado;
            }
        }

        public void Move(TGCVector3 desplazamiento)
        {
            this.delanteraIzquierda.Move(desplazamiento);
            this.delanteraDerecha.Move(desplazamiento);
            foreach (TgcMesh rueda in this.ruedasTraseras)
            {
                rueda.Move(desplazamiento);
            }
        }

        public void Render()
        {
            this.delanteraIzquierda.Render();
            this.delanteraDerecha.Render();
            foreach (TgcMesh rueda in this.ruedasTraseras)
            {
                rueda.Render();
            }
        }

        public void rotate(TGCVector3 eje, float rotacion)
        {
            this.delanteraIzquierda.Transform.RotateAxis(eje, rotacion);
            this.delanteraDerecha.Transform.RotateAxis(eje, rotacion);
            foreach (TgcMesh rueda in this.ruedasTraseras)
            {
                rueda.Transform.RotateAxis(eje, rotacion);
            }
        }

    }

}
