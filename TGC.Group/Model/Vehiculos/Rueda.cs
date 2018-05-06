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

        public Rueda(TgcMesh mesh,TGCVector3 pos)
        {
            this.mesh = mesh;
            mesh.AutoTransform = false;
            var escala = TGCMatrix.Scaling(0.05f, 0.05f, 0.05f);
            var traslado = TGCMatrix.Translation(pos.X, pos.Y, pos.Z);
            trasladoInicial = pos;
            transformacion = escala * traslado;
        }

        public void Transform(TGCVector3 posicion, TGCVector3 adelante, TGCVector3 costado)
        {
            var posicionFinal = posicion + adelante * trasladoInicial.X + costado * trasladoInicial.Z;
            mesh.Transform = TGCMatrix.Scaling(0.05f, 0.05f, 0.05f) * TGCMatrix.Translation(posicionFinal);
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
            //transformacion = TGCMatrix.RotationY(rotacion) * transformacion;
        }
    }
}
