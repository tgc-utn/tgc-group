using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Elements.RigidBodyFactories
{
    class FloorFactory : IRigidBodyFactory
    {

        public RigidBody Create(TgcMesh mesh)
        {
            var floorShape = new BoxShape(new TGCVector3(5000f, 0f, 5000f).ToBulletVector3());
            var transform = TGCMatrix.Identity;
            transform.Translate(.0f, .0f, .0f);
            var floorMotionState = new DefaultMotionState(transform.ToBsMatrix);
            var floorinfo = new RigidBodyConstructionInfo(0, floorMotionState, floorShape);
            var floorRigidBody = new RigidBody(floorinfo);
            PhysicsWorld.DynamicsWorld.AddRigidBody(floorRigidBody);
            return floorRigidBody;

        }
        public RigidBody CreatePlaneBody(TgcPlane plane)
        {
            var floorShape = new StaticPlaneShape(TGCVector3.Up.ToBulletVector3(), 0);
            var floorMotionState = new DefaultMotionState();
            var floorInfo = new RigidBodyConstructionInfo(0, floorMotionState, floorShape);
            var floorRigidBody = new RigidBody(floorInfo);
            floorRigidBody.Friction = 5f;
            floorRigidBody.RollingFriction = 5f;
            floorRigidBody.Restitution = 5f;
            PhysicsWorld.DynamicsWorld.AddRigidBody(floorRigidBody);

            return floorRigidBody;
        }
    }
}
