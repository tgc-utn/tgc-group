using System;
using BulletSharp;
using BulletSharp.Math;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Entities
{
    public class MovementToEntity
    {
        private Vector3 LookAt;
        private float RotationVelocity;

        public MovementToEntity(Vector3 lookAt, float rotationVelocity)
        {
            LookAt = lookAt;
            RotationVelocity = rotationVelocity;
        }

        public float Rotation(Vector3 myPosition, Vector3 destination)
        {
            LookAt.Normalize();
            var angleToRotate = SenseOfRotation(myPosition, destination) * RotationVelocity;
            LookAt = Vector3.TransformNormal(LookAt, Matrix.RotationY(angleToRotate));
            return angleToRotate;

        }
        

        private int SenseOfRotation(Vector3 myPosition, Vector3 destination)
        {
            var positionTranslation = Matrix.Translation(myPosition);
            var normalizedDestination = Vector3.TransformNormal(destination, positionTranslation);
            var normalizedLookAt = Vector3.TransformNormal(LookAt, positionTranslation);
            return Math.Sign(Vector3.Cross(normalizedDestination , normalizedLookAt ).Y);
            
        }
    }
}