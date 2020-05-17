using TGC.Core.BulletPhysics;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Pelota : ObjetoJuego
    {
        private float scaleFactor;


        public Pelota(TgcMesh mesh, TGCVector3 translation = new TGCVector3(), TGCVector3 rotation = new TGCVector3(), float angle = 0) : base(mesh, translation, rotation, angle)
        {
            scaleFactor = 0.5f;

			cuerpo = BulletRigidBodyFactory.Instance.CreateBall(17f * scaleFactor, 1f, new TGCVector3(translation));
        }

        public override void Render()
        {
            Mesh.Transform = TGCMatrix.Scaling(scaleFactor, scaleFactor, scaleFactor) * new TGCMatrix(cuerpo.InterpolationWorldTransform);
            Mesh.Render();

            Mesh.BoundingBox.transform(Mesh.Transform);
            Mesh.BoundingBox.Render();

            RenderRigidBodyBoundingBox();
        }
    }
}
