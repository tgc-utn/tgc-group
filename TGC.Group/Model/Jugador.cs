using Microsoft.DirectX.DirectInput;
using TGC.Core.BulletPhysics;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Jugador : ObjetoJuego
    {
        //Objetos de juego
        private Microsoft.DirectX.DirectInput.Key inputAvanzar;

        public Jugador(TgcMesh mesh, TGCVector3 translation=new TGCVector3(), TGCVector3 rotation=new TGCVector3(),float angle=0) :base(mesh,translation,rotation,angle)
        {
            inputAvanzar = Key.UpArrow;

            this.translation.Y += 10;

            cuerpo = BulletRigidBodyFactory.Instance.CreateBox(Mesh.BoundingBox.calculateSize(), 1f, new TGCVector3(translation), 0f, 0f, 0f, 1f, true);
        }

        public void HandleInput(TgcD3dInput input)
        {
            if (input.keyDown(inputAvanzar) )
            {
                BulletSharp.Math.Vector3 velocity = new BulletSharp.Math.Vector3(0, 0, -5);
                velocity = rotation.Rotate(velocity);
                cuerpo.ApplyImpulse(velocity, new BulletSharp.Math.Vector3());
            }
            
            if(input.keyUp(inputAvanzar))
            {
                cuerpo.LinearVelocity = new BulletSharp.Math.Vector3(0, 0, 0);
            }
        }

    }
}
