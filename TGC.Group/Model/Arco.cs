using TGC.Core.BulletPhysics;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{

    class Arco : ObjetoJuego
    {
        private float meshRotationAngle;

        public Arco(TgcMesh mesh, float meshRotationAngle) : base(mesh)
        {
            this.meshRotationAngle = meshRotationAngle;

            cuerpo = BulletRigidBodyFactory.Instance.CreateRigidBodyFromTgcMesh(mesh);
            cuerpo.WorldTransform = TGCMatrix.RotationTGCQuaternion(new TGCQuaternion(0, 1, 0, meshRotationAngle)).ToBulletMatrix();
        }

    }
}
