using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    
    class ModeloCompuesto
    {
        private readonly List<TgcMesh> meshes;

        public ModeloCompuesto(string direccionDelModelo, TGCVector3 posicionInicial)
        {
            meshes = new TgcSceneLoader().loadSceneFromFile(direccionDelModelo).Meshes;
            this.CambiarPosicion(posicionInicial);

        }

        private void TransformarModelo(Action<TgcMesh> funcionTransformacion)
        {
            meshes.ForEach(delegate (TgcMesh unMesh) { funcionTransformacion(unMesh); });
        }

        public void CambiarPosicion(TGCVector3 nuevaPosicion)
        {
            TransformarModelo(delegate (TgcMesh unMesh) { unMesh.Position = nuevaPosicion; });
            TransformarModelo(delegate (TgcMesh unMesh) { unMesh.Transform = TGCMatrix.Translation(unMesh.Position); });
        }

        public void CambiarRotacion(TGCVector3 nuevaRotacion)
        {
            /*
            TGCQuaternion rotationX = TGCQuaternion.RotationAxis(new TGCVector3(1.0f, 0.0f, 0.0f), nuevaRotacion.X);
            TGCQuaternion rotationY = TGCQuaternion.RotationAxis(new TGCVector3(0.0f, 1.0f, 0.0f), nuevaRotacion.Y);
            TGCQuaternion rotationZ = TGCQuaternion.RotationAxis(new TGCVector3(0.0f, 0.0f, 1.0f), nuevaRotacion.Z);

            TGCQuaternion rotation = rotationX * rotationY * rotationZ;

            TGCMatrix baseScaleRotation = TGCMatrix.Scaling(new TGCVector3(0.05f, 0.05f, 0.05f)) * TGCMatrix.RotationY(FastMath.PI_HALF);

            TransformarModelo(delegate (TgcMesh unMesh) { unMesh.Rotation = nuevaRotacion; });
            TransformarModelo(delegate (TgcMesh unMesh) { unMesh.Transform = TGCMatrix.RotationTGCQuaternion(rotation); });
            */
        }

        public void Render()
        {
            TransformarModelo(delegate (TgcMesh unMesh) { unMesh.Render(); });
        }






    }
}
