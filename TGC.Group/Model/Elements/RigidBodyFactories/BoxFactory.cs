using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using BulletSharp.Math;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Elements.RigidBodyFactories
{
    class BoxFactory : IRigidBodyFactory
    {
        public RigidBody Create(TgcMesh mesh)
        {
            var radius = mesh.BoundingBox.calculateAxisRadius().ToBulletVector3();
            return CreateRigidBody(mesh.Position, radius, 0);
        }

        public RigidBody CreatePlane(TgcPlane plane)
        {
            var halfSize = new TGCVector3(plane.Size.X / 2 , 2000, plane.Size.Z / 2 ).ToBulletVector3();
            return CreateRigidBody(new TGCVector3(plane.Position.X, plane.Position.Y - 2000, plane.Position.Z) , halfSize, 0);

        }

        private RigidBody CreateRigidBody(TGCVector3 position, Vector3 halfSize, float mass)
        {
            var boxShape = new BoxShape(halfSize);
            var transform = TGCMatrix.Translation(position);
            var motionState = new DefaultMotionState(transform.ToBsMatrix);
            var rigidBody = new RigidBody(new RigidBodyConstructionInfo(mass, motionState, boxShape));
            return rigidBody;
        }

    }
}
