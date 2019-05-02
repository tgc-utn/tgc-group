using TGC.Core.BoundingVolumes;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using BulletSharp;

namespace TGC.Group.Model
{
    public class Element : Collisionable
    {
        public TgcMesh Mesh { get; }
        public RigidBody PhysicsBody { get; set; }


        public Element(TgcMesh model, RigidBody rigidBody)
        {
            this.Mesh = model;
            this.PhysicsBody = rigidBody;
        }

        public void Update()
        {
            Mesh.Position = new TGCVector3(PhysicsBody.CenterOfMassPosition.X, PhysicsBody.CenterOfMassPosition.Y, PhysicsBody.CenterOfMassPosition.Z);
            Mesh.Transform = 
                TGCMatrix.Scaling(Mesh.Scale) *
                TGCMatrix.Translation(new TGCVector3(PhysicsBody.CenterOfMassPosition));

            return;
        }

        public void Render()
        {
            Mesh.Render();
            return;
        }

        public void Dispose()
        {
            Mesh.Dispose();
            PhysicsWorld.DynamicsWorld.RemoveRigidBody(PhysicsBody);
            return;
        }

        public override TgcBoundingAxisAlignBox getCollisionVolume()
        {
            return Mesh.BoundingBox;
        }
    }
}