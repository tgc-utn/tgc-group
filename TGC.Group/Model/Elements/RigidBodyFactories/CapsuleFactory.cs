using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Elements.RigidBodyFactories
{
    class CapsuleFactory : IRigidBodyFactory
    {
        public RigidBody Create(TgcMesh mesh)
        {
            var mass = 10f;
            var radius = mesh.BoundingBox.calculateAxisRadius();

            CapsuleShape capsule;
            if (radius.X >= radius.Y)
            {
                capsule = new CapsuleShapeX(radius.Y, radius.X - radius.Y);
            }
            else
            {
                capsule = new CapsuleShape(radius.X, radius.Y - radius.X);
            }
            var inertia = capsule.CalculateLocalInertia(mass);
            var transform = TGCMatrix.Translation(mesh.Position);

            var motionState = new DefaultMotionState(transform.ToBsMatrix);
            var rigidBodyInfo = new RigidBodyConstructionInfo(mass, motionState, capsule, inertia);
            var rigidBody = new RigidBody(rigidBodyInfo);

            return rigidBody;
        }
    }
}
