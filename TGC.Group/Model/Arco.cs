using Microsoft.DirectX;
using System.Drawing;
using System.Runtime.CompilerServices;
using TGC.Core.BoundingVolumes;
using TGC.Core.BulletPhysics;
using TGC.Core.Collision;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{

    class Arco : ObjetoJuego
    {
        private float meshRotationAngle;
        public TgcBoundingAxisAlignBox AABBGol { get; private set; }

        public Arco(TgcMesh mesh, float meshRotationAngle) : base(mesh)
        {
            this.meshRotationAngle = meshRotationAngle;

            cuerpo = BulletRigidBodyFactory.Instance.CreateRigidBodyFromTgcMesh(mesh);
            TGCQuaternion rot = new TGCQuaternion();
            rot.RotateAxis(new TGCVector3(0, 1, 0), meshRotationAngle);
            cuerpo.WorldTransform = TGCMatrix.RotationTGCQuaternion(rot).ToBulletMatrix();

            UpdateAABB();
            AABBGol = new TgcBoundingAxisAlignBox(AABB.PMin + new TGCVector3(10,10,10), AABB.PMax - new TGCVector3(10, 10, 10));
            AABBGol.transform(Mesh.Transform);
        }
        public override bool CheckCollideWith(ObjetoJuego objeto) => TgcCollisionUtils.testAABBAABB(this.AABBGol, objeto.AABB);

        public override void Render()
        {
            base.Render();

            AABBGol.setRenderColor(Color.Red);
            AABBGol.Render();
        }

    }
}
