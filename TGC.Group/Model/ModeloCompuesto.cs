using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.Direct3D;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{

    class ModeloCompuesto
    {
        private readonly List<TgcMesh> meshes;
        private TGCMatrix baseScaleRotation;
        private TGCMatrix rotacionMesh;
        public ModeloCompuesto(string direccionDelModelo, TGCVector3 posicionInicial)
        {
            meshes = new TgcSceneLoader().loadSceneFromFile(direccionDelModelo).Meshes;
            this.CambiarPosicion(posicionInicial);

        }

        private void TransformarModelo(Action<TgcMesh> funcionTransformacion)
        {
            meshes.ForEach(delegate (TgcMesh unMesh) { funcionTransformacion(unMesh); });
        }
        private void TransformarBoundingBox(Action<TgcMesh> funcionTransformacion)
        {
            meshes.ForEach(delegate (TgcMesh unMesh) {
                unMesh.BoundingBox.transform(unMesh.Transform);
                funcionTransformacion(unMesh);
            });
        }

        public void CambiarPosicion(TGCVector3 nuevaPosicion)
        {
            TransformarModelo(delegate (TgcMesh unMesh) { unMesh.Position = nuevaPosicion; });
            TransformarModelo(delegate (TgcMesh unMesh) { unMesh.Transform = TGCMatrix.Translation(unMesh.Position); });
        }

        public void CambiarRotacion(TGCVector3 nuevaRotacion)
        {

            TGCQuaternion rotationX = TGCQuaternion.RotationAxis(new TGCVector3(1.0f, 0.0f, 0.0f), nuevaRotacion.X);
            TGCQuaternion rotationY = TGCQuaternion.RotationAxis(new TGCVector3(0.0f, 1.0f, 0.0f), nuevaRotacion.Y);
            TGCQuaternion rotationZ = TGCQuaternion.RotationAxis(new TGCVector3(0.0f, 0.0f, 1.0f), nuevaRotacion.Z);

            TGCQuaternion rotation = rotationX * rotationY * rotationZ;

            baseScaleRotation = TGCMatrix.Scaling(new TGCVector3(0.15f, 0.15f, 0.15f)) * TGCMatrix.RotationY(FastMath.PI_HALF);

            TGCMatrix.RotationTGCQuaternion(rotation);
            //TransformarModelo(delegate (TgcMesh unMesh) { unMesh.Rotation = nuevaRotacion; });
            //TransformarModelo(delegate (TgcMesh unMesh) { unMesh.Transform = TGCMatrix.RotationTGCQuaternion(rotation); });
            rotacionMesh = TGCMatrix.RotationTGCQuaternion(rotation);
        }

        public void AplicarTransformaciones()
        {
            TransformarModelo(delegate (TgcMesh unMesh) { unMesh.Transform = baseScaleRotation * rotacionMesh * TGCMatrix.Translation(unMesh.Position); });
        }


        public void Render()
        {
            TransformarModelo(delegate (TgcMesh unMesh) { unMesh.Render(); });
            TransformarBoundingBox(delegate (TgcMesh unMesh) { unMesh.BoundingBox.Render(); });
        }

        public void Dispose()
        {
            TransformarModelo(delegate (TgcMesh unMesh) { unMesh.Dispose(); });
        }

        public List<TgcBoundingAxisAlignBox> BoundingBoxesDelModelo()
        {
            List<TgcBoundingAxisAlignBox> boundingBoxes = new List<TgcBoundingAxisAlignBox>();
            meshes.ForEach(delegate (TgcMesh unMesh) { boundingBoxes.Add(unMesh.BoundingBox); });
            return boundingBoxes;
        }

        public Boolean ColisionaConColisionable(Colisionable unColisionable) 
        {
            return BoundingBoxesDelModelo().Any(bound => TgcCollisionUtils.testAABBAABB(bound, unColisionable.GetBoundingBox()));
        }

    }
}
