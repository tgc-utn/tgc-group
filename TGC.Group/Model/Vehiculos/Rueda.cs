using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Geometry;

namespace TGC.Group.Model.Vehiculos
{
    class Rueda
    {
        //public TGCMatrix transformacion;
        public TgcMesh mesh;
        public TGCVector3 trasladoInicial;
        public TGCMatrix escala = TGCMatrix.Scaling(0.045f, 0.045f, 0.045f);
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

        public void RotateY(float rotacion)
        {
            this.rotacion = TGCMatrix.RotationY(rotacion) * this.rotacion;
        }

        public void RotateAxis(TGCVector3 eje, float rotacion)
        {
            this.rotacion = TGCMatrix.RotationX(rotacion*0.0001f) * this.rotacion;
        }

        public void RotacionSobreSiMisma(float velocidad)
        {
            this.rotacion = TGCMatrix.RotationX(velocidad * 0.0001f) * this.rotacion;
        }
    }
}
