using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Utils
{
    public static class TGCUtils
    {

        public static TgcMesh createInstanceFromMesh(TgcMesh meshOrigin, string instanceName)
        {
            if(meshOrigin.ParentInstance == null)
            {
                return meshOrigin.createMeshInstance(instanceName);
            }
            else
            {
                return meshOrigin.ParentInstance.createMeshInstance(instanceName, meshOrigin.Position, meshOrigin.Rotation, meshOrigin.Scale);
            }
        }


        public static TgcMesh createInstanceFromMesh(ref TgcMesh meshOrigin, string instanceName, Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            TgcMesh instance;
            if (meshOrigin.ParentInstance == null)
            {
                instance = meshOrigin.createMeshInstance(instanceName, meshOrigin.Position + translation, meshOrigin.Rotation + rotation, meshOrigin.Scale + scale);
            }
            else
            {
                instance = meshOrigin.ParentInstance.createMeshInstance(instanceName, meshOrigin.Position + translation, meshOrigin.Rotation + rotation, meshOrigin.Scale + scale);                
            }
            instance.UpdateMeshTransform();
            instance.BoundingBox = meshOrigin.BoundingBox.clone();
            instance.BoundingBox = updateMeshBoundingBox(instance);
            
            return instance;
        }

        public static TgcBoundingAxisAlignBox updateMeshBoundingBox(TgcMesh mesh)
        {
            Matrix scaleMatrix     = new Matrix();
            Matrix rotationMatrixX = new Matrix();
            Matrix rotationMatrixY = new Matrix();
            Matrix rotationMatrixZ = new Matrix();
            Matrix translateMatrix = new Matrix();
            scaleMatrix.Scale(mesh.Scale);
            rotationMatrixX.RotateX(mesh.Rotation.X);
            rotationMatrixY.RotateY(mesh.Rotation.Y);
            rotationMatrixZ.RotateZ(mesh.Rotation.Z);
            scaleMatrix.Multiply(rotationMatrixX);
            scaleMatrix.Multiply(rotationMatrixY);
            scaleMatrix.Multiply(rotationMatrixZ);
            translateMatrix.Translate(mesh.Position);
            scaleMatrix.Multiply(translateMatrix);

            mesh.BoundingBox.transform(scaleMatrix);

            return mesh.BoundingBox;
        }


    }
}
