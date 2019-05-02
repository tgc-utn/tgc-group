using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Elements.RigidBodyFactories
{
    class BoxFactory : IRigidBodyFactory
    {
        public RigidBody Create(TgcMesh mesh)
        {
            var radius = mesh.BoundingBox.calculateAxisRadius();
            var boxShape = new BoxShape(radius.ToBulletVector3());
            var transform = TGCMatrix.Identity;
            transform.Origin = mesh.Position;
            var motionState = new DefaultMotionState(transform.ToBsMatrix);
            var rigidBody = new RigidBody(new RigidBodyConstructionInfo(0, motionState, boxShape));
            PhysicsWorld.DynamicsWorld.AddRigidBody(rigidBody);
            return rigidBody;
        }
    }
}
