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

        public Rueda(TgcMesh mesh,TGCVector3 pos)
        {
            this.mesh = mesh;
            mesh.AutoTransform = false;
            var escala = TGCMatrix.Scaling(0.05f, 0.05f, 0.05f);
            var traslado = TGCMatrix.Translation(pos.X, pos.Y, pos.Z);
            transformacion = escala * traslado;
        }

        public void Transform()
        {
            mesh.Transform = this.transformacion;
        }

        public void Render()
        {
            mesh.Render();
        }

        public void Move(TGCVector3 desplazamiento)
        {
            transformacion = transformacion * TGCMatrix.Translation(desplazamiento.X, desplazamiento.Y, desplazamiento.Z);
        }

        public void Rotate(TGCMatrix matrizRotacion)
        {
            transformacion = matrizRotacion * transformacion;
        }
    }
}
