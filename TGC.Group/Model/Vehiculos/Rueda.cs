﻿using System;
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
        public TGCMatrix rotationX = TGCMatrix.Identity;
        public TGCMatrix rotationY = TGCMatrix.Identity;

        public Rueda(TgcMesh mesh,TGCVector3 traslado)
        {
            this.mesh = mesh;
            mesh.AutoTransform = false;
            trasladoInicial = TGCMatrix.Translation(traslado);
        }

        public void Transform(TGCMatrix matrizAuto)
        {
            this.mesh.Transform = rotationX * rotationY * trasladoInicial * matrizAuto;
        }

        public void Render()
        {
            mesh.Render();
        }

        public void RotateY(float rotacion)
        {
            this.rotationY = TGCMatrix.RotationY(rotacion) * this.rotationY;
        }


        /// <summary>
        /// Rota la rueda sobre si misma
        /// </summary>
        public void RotateX(float velocidad)
        {
            this.rotationX = TGCMatrix.RotationX(-velocidad * 0.01f) * this.rotationX;
        }
    }
}