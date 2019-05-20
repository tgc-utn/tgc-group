using BulletSharp;
using BulletSharp.Math;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    public abstract class Entity : Collisionable
    {
        public TgcMesh Mesh { get; }
        public RigidBody RigidBody { get; }

        public Vector3 Position { get; protected set; }

        public Entity(TgcMesh mesh, RigidBody rigid)
        {
            Mesh = mesh;
            RigidBody = rigid;
        }


        public virtual void Render()
        {
            this.Mesh.Render();

        }
        public virtual void Update(Camera camera)
        {
            this.Mesh.Position = new TGCVector3(this.RigidBody.CenterOfMassPosition);
            this.Mesh.Transform =
                TGCMatrix.Scaling(this.Mesh.Scale) *
                new TGCMatrix(this.RigidBody.CenterOfMassTransform);
        }
        public abstract void Dispose();
        public override abstract IRenderObject getCollisionVolume();
    }
}