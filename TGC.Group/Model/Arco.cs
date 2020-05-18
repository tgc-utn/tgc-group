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
            TGCQuaternion rot = new TGCQuaternion();
            rot.RotateAxis(new TGCVector3(0, 1, 0), meshRotationAngle);
            //cuerpo.WorldTransform = TGCMatrix.RotationTGCQuaternion(new TGCQuaternion(0, 1, 0, meshRotationAngle)).ToBulletMatrix();
            cuerpo.WorldTransform = TGCMatrix.RotationTGCQuaternion(rot).ToBulletMatrix();
        }

    }
}
