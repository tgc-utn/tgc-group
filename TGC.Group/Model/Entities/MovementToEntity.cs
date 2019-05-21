using System;
using BulletSharp;
using BulletSharp.Math;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Entities
{
    public class MovementToEntity
    {
        private Vector3 LookAt;
        private float RotationVelocity;
        private float TranslationVelocity;
        
        public MovementToEntity(Vector3 lookAt, float rotationVelocity, float translationVelocity)
        {
            LookAt = lookAt;
            RotationVelocity = rotationVelocity;
            TranslationVelocity = translationVelocity;
        }

        
        
        public void Move(TgcMesh mesh, RigidBody rigidBody, TGCVector3 destination, Vector3 difference)
        {
            var angles = AnglesToRotate(mesh, destination);
            UpdateLookAtVector(Matrix.RotationY(angles.Y));
            mesh.RotateY(angles.Y);
            rigidBody.Translate(- LookAt * TranslationVelocity);

        }

        private Vector3 AnglesToRotate(TgcMesh mesh, TGCVector3 destination)
        {
            return SenseOfRotation(mesh.Position.ToBulletVector3(), destination.ToBulletVector3()) * RotationVelocity;
        }

        public Matrix CalculateRotation(Vector3 myPosition, Vector3 destination)
        {
            LookAt.Normalize();
            var axisOfRotation = CalculateRotationY(myPosition, destination);
            UpdateLookAtVector(axisOfRotation);
            return axisOfRotation;

        }

        private void UpdateLookAtVector(Matrix axisOfRotation)
        {
            LookAt = Vector3.TransformNormal(LookAt, axisOfRotation);
        }

        private Matrix CalculateRotationY(Vector3 myPosition, Vector3 destination)
        {
            return Matrix.RotationY(SenseOfRotation(myPosition, destination).Y * RotationVelocity);
        }


        private Vector3 SenseOfRotation(Vector3 myPosition, Vector3 destination)
        { 
            var normal = Vector3.Cross( destination - myPosition, LookAt);
            return new Vector3(
                Math.Sign(normal.X),
                Math.Sign(normal.Y),
                Math.Sign(normal.Z)
                );
        }

    }
}