using BulletSharp;
using BulletSharp.Math;
using System;
using System.Drawing;
using TGC.Core.BoundingVolumes;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Text;
using TGC.Group.Model.Items;

namespace TGC.Group.Model.Elements.ElementFactories
{
    public class Shark : Entity
    {
        private static float DefaultVelocity = 5f;

        private Vector3 LookAt = new Vector3(1, 0, 0);
        private static TgcText2D DrawText = new TgcText2D();
        private bool dead = false;
        public Shark(TgcMesh model, RigidBody rigidBody) : base(model, rigidBody)
        {
        }



        public override void Update(Camera camera)
        {
            var difference = camera.Position.ToBulletVector3() - RigidBody.CenterOfMassPosition;

            var sharkBody = (CapsuleShapeX)RigidBody.CollisionShape;
            var cameraBody = (CapsuleShape)camera.RigidBody.CollisionShape;
            
            if (
            FastMath.Pow2(difference.X) <= FastMath.Pow2(sharkBody.Radius + sharkBody.HalfHeight - cameraBody.Radius) &&
            FastMath.Pow2(difference.Y) <= FastMath.Pow2(sharkBody.Radius - (cameraBody.Radius + cameraBody.HalfHeight)) &&
            FastMath.Pow2(difference.Z) <= FastMath.Pow2(sharkBody.Radius - cameraBody.Radius)
             )
            {
                dead = true;

            }

            difference.Normalize();
            RigidBody.Translate(difference * DefaultVelocity);


            base.Update(camera);

        }



        public override void Render()
        {
            base.Render();
            //DrawText.drawText(a.ToString(), 50, 50, Color.Black);
            if (dead)
            {
                var point = GetCenter();
                DrawText.drawText("TE MORISTE WEEE xddxdxd", point.X, point.Y, Color.Red);
            }

        }
        private static Point GetCenter()
        {
            return new Point(
                D3DDevice.Instance.Device.Viewport.Width / 2,
                D3DDevice.Instance.Device.Viewport.Height / 2
                );
        }

        public TGCMatrix CalculateRotation(Camera camera)
        {
            var vector = camera.Position.ToBulletVector3() - RigidBody.CenterOfMassPosition;
            vector.Normalize();

            return TGCMatrix.RotationX(vector.X) * TGCMatrix.RotationY(vector.Y) * TGCMatrix.RotationZ(vector.Z);
        }

        public override IRenderObject getCollisionVolume()
        {
            CapsuleShapeX capsule = (CapsuleShapeX)RigidBody.CollisionShape;

            var radius = new TGCVector3(capsule.Radius + capsule.HalfHeight, capsule.Radius, capsule.Radius);

            return new TgcBoundingElipsoid(new TGCVector3(RigidBody.CenterOfMassPosition), radius);
        }

        public override void Dispose()
        {
            this.Mesh.Dispose();
            this.RigidBody.Dispose();
        }
    }
}