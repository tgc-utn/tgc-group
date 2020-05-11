using TGC.Core.BulletPhysics;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Pelota : ObjetoJuego
    {
        public Pelota(TgcMesh mesh, TGCVector3 translation = new TGCVector3(), TGCVector3 rotation = new TGCVector3(), float angle = 0) : base(mesh, translation, rotation, angle)
        {
			cuerpo = BulletRigidBodyFactory.Instance.CreateBall(1f, 1f, new TGCVector3(translation));
        }
		
    }
}
