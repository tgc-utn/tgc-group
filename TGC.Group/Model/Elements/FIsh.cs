using BulletSharp;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Items;

namespace TGC.Group.Model.Elements
{
    class Fish : Element
    {
        public Fish(TgcMesh model, RigidBody rigidBody) : base(model, rigidBody)
        {
        }

        /*
        public override IRenderObject getCollisionVolume()
        {
            CapsuleShapeX capsule = (CapsuleShapeX) PhysicsBody.CollisionShape;

            var radius = new TGCVector3(capsule.Radius + capsule.HalfHeight, capsule.Radius, capsule.Radius);

            return new TgcBoundingElipsoid(new TGCVector3(PhysicsBody.CenterOfMassPosition), radius);
        }
        */
        public override IItem item { get; } = new Items.Fish();

    }
}
