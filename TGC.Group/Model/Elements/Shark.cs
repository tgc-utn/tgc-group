using System;
using System.Drawing;
using BulletSharp;
using BulletSharp.Math;
using TGC.Core.BoundingVolumes;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Text;
using TGC.Group.Model.Entities;

namespace TGC.Group.Model.Elements
{
    public class Shark : Entity
    {
        private static readonly TgcText2D DrawText = new TgcText2D();
        private MovementToEntity MovementToCamera;
        private bool dead;

        public Shark(TgcMesh model, RigidBody rigidBody) : base(model, rigidBody)
        {
            MovementToCamera = new MovementToEntity(new Vector3(1f, 0f, 0f), FastMath.PI / 100f);
        }


        public override void Update(Camera camera)
        {

            var difference = camera.Position.ToBulletVector3() - RigidBody.CenterOfMassPosition;

            var sharkBody = (CapsuleShapeX)RigidBody.CollisionShape;
            var cameraBody = (CapsuleShape)camera.RigidBody.CollisionShape;

            VerifyCollision(difference, sharkBody, cameraBody);

            difference.Normalize();
            RigidBody.Translate(difference * 5f);

           
            Mesh.RotateY(
                MovementToCamera.Rotation(
                    Mesh.Position.ToBulletVector3(),
                    camera.Position.ToBulletVector3()
                    )
            );
            
                                         
            //base.Update(camera);
        }
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          
        private void VerifyCollision(Vector3 difference, CapsuleShapeX sharkBody, CapsuleShape cameraBody)
        {
            dead =
                FastMath.Pow2(difference.X) <=
                FastMath.Pow2(sharkBody.Radius + sharkBody.HalfHeight - cameraBody.Radius) &&
                FastMath.Pow2(difference.Y) <=
                FastMath.Pow2(sharkBody.Radius - (cameraBody.Radius + cameraBody.HalfHeight)) &&
                FastMath.Pow2(difference.Z) <=
                FastMath.Pow2(sharkBody.Radius - cameraBody.Radius);
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
        
        public override IRenderObject getCollisionVolume()
        {
            CapsuleShapeX capsule = (CapsuleShapeX)RigidBody.CollisionShape;

            var radius = new TGCVector3(capsule.Radius + capsule.HalfHeight, capsule.Radius, capsule.Radius);

            return new TgcBoundingElipsoid(new TGCVector3(RigidBody.CenterOfMassPosition), radius);
        }

        public override void Dispose()
        {
            Mesh.Dispose();
            RigidBody.Dispose();
        }
    }
}