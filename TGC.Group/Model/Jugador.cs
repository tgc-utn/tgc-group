using BulletSharp.Math;
using Microsoft.DirectX.DirectInput;
using System;
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

            cuerpo = BulletRigidBodyFactory.Instance.CreateBox(Mesh.BoundingBox.calculateSize(), 1f, new TGCVector3(translation), rotation.X, rotation.Y, rotation.Z, 1f, true);
            cuerpo.SetSleepingThresholds(0, 0);
        }

        public void HandleInput(TgcD3dInput input)
        {
            if (input.keyDown(inputAvanzar))
            {
                cuerpo.ApplyForce(Vector3.Transform(new Vector3(0, 0, -50), rotation), new Vector3(0,0,0));
            }
            if (input.keyDown(Key.DownArrow))
            {
                cuerpo.ApplyForce(Vector3.Transform(new Vector3(0, 0, 50), rotation), new Vector3(0, 0, 0));
            }
            //cuerpo.LinearVelocity = cuerpo.LinearVelocity + new Vector3(0,1,0);
            if (input.keyDown(Key.RightArrow))
            {
                Vector3 velocity = Vector3.Transform(new Vector3(-1, 0, 0), rotation);
                cuerpo.ApplyForce(velocity * 50, Vector3.Transform(new Vector3(0, 0, -5), rotation));
                cuerpo.ApplyForce(velocity * -50, Vector3.Transform(new Vector3(0, 0, 5), rotation));
                //cuerpo.LinearVelocity = Vector3.Transform(cuerpo.LinearVelocity, new Quaternion().);
            }
            if (input.keyDown(Key.LeftArrow))
            {
                Vector3 velocity = Vector3.Transform(new Vector3(1, 0, 0), rotation);
                cuerpo.ApplyForce(velocity * 50, Vector3.Transform(new Vector3(0, 0, -5), rotation));
                cuerpo.ApplyForce(velocity * -50, Vector3.Transform(new Vector3(0, 0, 5), rotation));
            }
        }

    }
}
