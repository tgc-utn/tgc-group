using BulletSharp;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Mathematica;
using BulletSharp.Math;
using TGC.Group.Model.Utils;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Items;
using Microsoft.DirectX.Direct3D;

namespace TGC.Group.Model.Elements
{
    public abstract class Element: Collisionable
    {

        public TgcMesh Mesh { get; }
        public RigidBody PhysicsBody { get; set; }
        public bool Selectable { get; set; }

        public abstract IItem item { get; }

        public Vector3 Position { get; protected set; }
        private Effect effect;
        public Effect Effect
        {
            set
            {
                this.Mesh.Effect = effect = value;
                this.Mesh.Technique = "FedeTechnique";
            }
            get
            {
                return effect;
            }
        }

        public Element(TgcMesh model, RigidBody rigidBody)
        {
            this.Mesh = model;
            this.PhysicsBody = rigidBody;
        }
        public bool isIntersectedBy(TgcRay ray)
        {
            var aabb = (TgcBoundingAxisAlignBox) getCollisionVolume();
            var toTest = new Cube(aabb.PMin, aabb.PMax);
            return toTest.isIntersectedBy(ray);
        }

        public virtual void Update(Camera camera)
        {
            this.Position = this.Mesh.Position.ToBulletVector3();
            this.Mesh.Position = new TGCVector3(this.PhysicsBody.CenterOfMassPosition.X, this.PhysicsBody.CenterOfMassPosition.Y, this.PhysicsBody.CenterOfMassPosition.Z);
            this.Mesh.Transform = 
                TGCMatrix.Scaling(this.Mesh.Scale) *
                new TGCMatrix(this.PhysicsBody.CenterOfMassTransform);
            this.Selectable = false;
        }

        public virtual void Render()
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

        public override IRenderObject getCollisionVolume() 
        { 
            Vector3 aabbMin, aabbMax;
            PhysicsBody.GetAabb(out aabbMin, out aabbMax);
            return new TgcBoundingAxisAlignBox(new TGCVector3(aabbMin), new TGCVector3(aabbMax));
        }
    }
}