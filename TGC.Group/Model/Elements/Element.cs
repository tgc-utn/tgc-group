using BulletSharp;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Items;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model.Elements
{
    public abstract class Element : Collisionable
    {
        public TgcMesh Mesh { get; }
        public RigidBody PhysicsBody { get; set; }
        public bool Selectable { get; set; }

        public abstract IItem item { get; }


        public Element(TgcMesh model, RigidBody rigidBody)
        {
            this.Mesh = model;
            this.PhysicsBody = rigidBody;
        }

        public bool isIntersectedBy(TgcRay ray)
        {
            var aabb = getCollisionVolume();
            var toTest = new Cube(aabb.PMin, aabb.PMax);
            return toTest.isIntersectedBy(ray);
        }

        public void Update()
        {
            this.Mesh.Position = new TGCVector3(this.PhysicsBody.CenterOfMassPosition.X, this.PhysicsBody.CenterOfMassPosition.Y, this.PhysicsBody.CenterOfMassPosition.Z);
            this.Mesh.Transform = 
                TGCMatrix.Scaling(this.Mesh.Scale) *
                new TGCMatrix(this.PhysicsBody.CenterOfMassTransform);
            this.Selectable = false;
        }

        public void Render()
        {
            this.Mesh.Render();
            
            if(this.Selectable)
                getCollisionVolume().Render();
        }

        public void Dispose()
        {
            this.Mesh.Dispose();
            this.PhysicsBody.Dispose();
        }

        public override TgcBoundingAxisAlignBox getCollisionVolume()
        {
            return this.Mesh.BoundingBox;
        }
    }
}