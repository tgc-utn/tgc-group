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
        private Key inputAvanzar;

        private Boolean EnElAire => translation.Y > 3; // TODO: Capaz se puede mejorar

        private int turbo = 100;
        public int Turbo
        {
            get => turbo;
            private set => turbo = Math.Min(value, 100);
        }


        public Jugador(TgcMesh mesh, TGCVector3 translation=new TGCVector3(), TGCVector3 rotation=new TGCVector3(),float angle=0) :base(mesh,translation,rotation,angle)
        {
            inputAvanzar = Key.UpArrow;

            this.translation.Y += 10;

            cuerpo = BulletRigidBodyFactory.Instance.CreateBox(Mesh.BoundingBox.calculateSize(), 1f, new TGCVector3(translation), rotation.X, rotation.Y, rotation.Z, 1f, true);
            cuerpo.SetSleepingThresholds(0, 0);
        }

        private void HandleInputSuelo(TgcD3dInput input)
        {
            if (input.keyDown(inputAvanzar))
            {
                cuerpo.ApplyCentralForce(Vector3.Transform(new Vector3(0, 0, -50), rotation));
            }
            if (input.keyDown(Key.DownArrow))
            {
                cuerpo.ApplyCentralForce(Vector3.Transform(new Vector3(0, 0, 50), rotation));
            }
            if (cuerpo.LinearVelocity.Length > 1)
            {
                Vector3 rotacion = Vector3.Transform(new Vector3(1, 0, 0), rotation) * 500 / cuerpo.LinearVelocity.Length;
                if (input.keyDown(Key.RightArrow))
                {
                    cuerpo.ApplyForce(rotacion, Vector3.Transform(new Vector3(0, 0, 5), rotation));
                    cuerpo.ApplyForce(-rotacion, Vector3.Transform(new Vector3(0, 0, -5), rotation));
                }
                if (input.keyDown(Key.LeftArrow))
                {
                    cuerpo.ApplyForce(rotacion, Vector3.Transform(new Vector3(0, 0, -5), rotation));
                    cuerpo.ApplyForce(-rotacion, Vector3.Transform(new Vector3(0, 0, 5), rotation));
                }
                if (input.keyDown(Key.A))
                {
                    cuerpo.AngularVelocity = Vector3.Zero;
                }
            }
            if (input.keyDown(Key.Space))
            {
                cuerpo.ApplyCentralForce(new Vector3(0, 1000, 0));
            }
        }

        private void HandleInputAire(TgcD3dInput input)
        {
            
            Vector3 fuerzaPitch = Vector3.Transform(new Vector3(0, 1, 0), rotation) * 50;
            Vector3 fuerzaYaw = Vector3.Transform(new Vector3(1, 0, 0), rotation) * 50;
            if (input.keyDown(inputAvanzar))
            {
                cuerpo.ApplyForce(fuerzaPitch, Vector3.Transform(new Vector3(0, 0, 5), rotation));
                cuerpo.ApplyForce(-fuerzaPitch, Vector3.Transform(new Vector3(0, 0, -5), rotation));
            }
            if (input.keyDown(Key.DownArrow))
            {
                cuerpo.ApplyForce(fuerzaPitch, Vector3.Transform(new Vector3(0, 0, -5), rotation));
                cuerpo.ApplyForce(-fuerzaPitch, Vector3.Transform(new Vector3(0, 0, 5), rotation));
            }
            if (input.keyDown(Key.RightArrow))
            {
                cuerpo.ApplyForce(fuerzaYaw, Vector3.Transform(new Vector3(0, 0, 5), rotation));
                cuerpo.ApplyForce(-fuerzaYaw, Vector3.Transform(new Vector3(0, 0, -5), rotation));
            }
            if (input.keyDown(Key.LeftArrow))
            {
                cuerpo.ApplyForce(fuerzaYaw, Vector3.Transform(new Vector3(0, 0, -5), rotation));
                cuerpo.ApplyForce(-fuerzaYaw, Vector3.Transform(new Vector3(0, 0, 5), rotation));
            }
        }

        private void HandleInputTurbo(TgcD3dInput input)
        {
            if (input.keyDown(Key.LeftControl) && Turbo > 0)
            {
                cuerpo.ApplyCentralForce(Vector3.Transform(new Vector3(0, 0, -50), rotation));
                Turbo--;
            }
        }

        public void HandleInput(TgcD3dInput input)
        {
            if (EnElAire)
                HandleInputAire(input);
            else
                HandleInputSuelo(input);
            HandleInputTurbo(input);
        }

    }
}
