using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Vehiculos
{
    class Rueda
    {
        public TGCMatrix transformacion;
        public TgcMesh mesh;
        public TGCVector3 trasladoInicial;
        public TGCMatrix escala = TGCMatrix.Scaling(0.05f, 0.05f, 0.05f);
        public TGCMatrix traslado;
        public TGCMatrix rotacion = TGCMatrix.RotationY(0);

        public Rueda(TgcMesh mesh,TGCVector3 traslado)
        {
            this.mesh = mesh;
            mesh.AutoTransform = false;
            trasladoInicial = traslado;
        }

        public void Transform(TGCVector3 posicion, TGCVector3 adelante, TGCVector3 costado)
        {
            var posicionFinal = posicion + adelante * trasladoInicial.X + costado * trasladoInicial.Z;
            this.traslado = TGCMatrix.Translation(posicionFinal);
            mesh.Transform = escala * rotacion * traslado;
        }

        public void Render()
        {
            mesh.Render();
        }

        public void Move(TGCVector3 desplazamiento)
        {
            //transformacion = transformacion * TGCMatrix.Translation(desplazamiento.X, desplazamiento.Y, desplazamiento.Z);
        }

        public void Rotate(TGCVector3 axis, float rotacion)
        {
            this.rotacion = TGCMatrix.RotationY(rotacion) * this.rotacion;
        }
    }
}
