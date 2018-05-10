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
        public TGCMatrix trasladoInicial;
        public TGCMatrix rotacion = TGCMatrix.Identity;

        public Rueda(TgcMesh mesh,TGCVector3 traslado)
        {
            this.mesh = mesh;
            mesh.AutoTransform = false;
            trasladoInicial = TGCMatrix.Translation(traslado);
        }

        public void Transform(TGCMatrix matrizAuto)
        {
            this.mesh.Transform = rotacion * trasladoInicial * matrizAuto;
        }

        public void Render()
        {
            mesh.Render();
        }

        public void RotateY(float rotacion)
        {
            //esto es para las de adelante cuando apretas A,D
        }

        //para rotar sobre si misma
        public void RotateX(float velocidad)
        {
            this.rotacion = TGCMatrix.RotationX(velocidad * 0.0001f) * this.rotacion;
        }
    }
}