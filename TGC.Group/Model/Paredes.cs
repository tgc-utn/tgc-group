using BulletSharp;
using TGC.Core.BulletPhysics;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Paredes: IObjetoJuego
    {
        
        public TgcMesh    Mesh { get; }
        public RigidBody  Cuerpo { get; }

        public Paredes( TgcMesh mesh)
        {
            Mesh = mesh;
            Cuerpo = BulletRigidBodyFactory.Instance.CreateRigidBodyFromTgcMesh(mesh);
        }

        public void Update(float ElapsedTime)
        {
        }

        public void Render()
        {
            Mesh.Render();
        }

        public void Dispose()
        {
        }
    }
}
