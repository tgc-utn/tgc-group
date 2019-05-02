using BulletSharp;
using System;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    internal class RigidBodyFactory
    {
        public static RigidBody CreateCapsule(TgcMesh mesh)
        {
            var mass = 10f;
            var radius = mesh.BoundingBox.calculateAxisRadius();
            var capsule = new CapsuleShapeX(radius.Y, radius.X - radius.Y);
            var transform = TGCMatrix.Identity;
            transform.Origin = mesh.Position;
            var inertia = capsule.CalculateLocalInertia(mass);

            var motionState = new DefaultMotionState(transform.ToBsMatrix);
            var rigidBodyInfo = new RigidBodyConstructionInfo(mass, motionState, capsule, inertia);
            var rigidBody = new RigidBody(rigidBodyInfo);
            rigidBody.Gravity = new TGCVector3(0, -10, 0).ToBulletVector3();

            PhysicsWorld.DynamicsWorld.AddRigidBody(rigidBody);

            return rigidBody;
        }

        public static RigidBody CreateFloor(TgcPlane plane )
        {
            var floorShape = new StaticPlaneShape(TGCVector3.Up.ToBulletVector3(), 0);
            var floorMotionState = new DefaultMotionState();
            var floorInfo = new RigidBodyConstructionInfo(0, floorMotionState, floorShape);
            var floorRigidBody = new RigidBody(floorInfo);
            floorRigidBody.Friction = 0f;
            floorRigidBody.RollingFriction = 0f;
            floorRigidBody.Restitution = 0f;
            PhysicsWorld.DynamicsWorld.AddRigidBody(floorRigidBody);

            return floorRigidBody;
        }
    }
}