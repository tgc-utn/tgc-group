using BulletSharp.Math;
using Microsoft.DirectX.DirectInput;
using System;
using System.Windows.Forms;
using TGC.Core.BulletPhysics;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Form;

namespace TGC.Group.Model
{
    class Jugador : ObjetoJuego
    {
        //Objetos de juego
        private Key inputAvanzar;
        private TGCVector3 posicionInicial;
        private TGCVector3 rotacionInicial;

        private Boolean EnElAire => translation.Y > 3; // TODO: Capaz se puede mejorar

        private int turbo = 100;
        public int Turbo
        {
            get => turbo;
            private set => turbo = Math.Min(value, 100);
        }

        public Jugador(TgcMesh mesh, TGCVector3 translation=new TGCVector3(), TGCVector3 rotation=new TGCVector3(), float angle=0) :base(mesh,translation,rotation,angle)
        {
            inputAvanzar = Key.UpArrow;

            this.translation.Y += 10;

            cuerpo = BulletRigidBodyFactory.Instance.CreateBox(Mesh.BoundingBox.calculateSize()  * 0.5f, 2f, new TGCVector3(translation), rotation.X, rotation.Y, rotation.Z, 1f, true);
            cuerpo.SetSleepingThresholds(0, 0);

            posicionInicial = new TGCVector3(translation);
            rotacionInicial = new TGCVector3(rotation);
        }

        public void ReiniciarJugador()
        {
            cuerpo.WorldTransform = TGCMatrix.RotationYawPitchRoll(rotacionInicial.X, rotacionInicial.Y, rotacionInicial.Z).ToBulletMatrix() * TGCMatrix.Translation(posicionInicial).ToBulletMatrix();
            cuerpo.LinearVelocity = Vector3.Zero;
            cuerpo.AngularVelocity = Vector3.Zero;
        }

        private void HandleInputSuelo(TgcD3dInput input)
        {
            cuerpo.AngularVelocity = Vector3.Zero;
            Vector3 frente = Vector3.Transform(new Vector3(0, 0, -1), rotation); // Vector que apunta siempre al frente del auto
            Vector3 velocidadLineal = cuerpo.LinearVelocity;
            cuerpo.LinearVelocity = (velocidadLineal.Dot(frente) / frente.LengthSquared) * frente;
            if (input.keyDown(inputAvanzar))
            {
                cuerpo.ApplyCentralForce(frente * 50);
            }
            if (input.keyDown(Key.DownArrow))
            {
                cuerpo.ApplyCentralForce(frente * -50);
            }
            if (velocidadLineal.Length > 1)
            {
                Vector3 rotacion = Vector3.Transform(new Vector3(1, 0, 0), rotation) * Math.Min(5000f / (velocidadLineal.Length * .2f), 1000f);
                if (input.keyDown(Key.RightArrow))
                {
                    cuerpo.ApplyForce(rotacion, frente * -5);
                    cuerpo.ApplyForce(-rotacion, frente * 5);
                }
                if (input.keyDown(Key.LeftArrow))
                {
                    cuerpo.ApplyForce(rotacion, frente * 5);
                    cuerpo.ApplyForce(-rotacion, frente * -5);
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

        public void RecogerTurbo(Turbo turbo)
        {
            if (Turbo < 100)
                Turbo += turbo.Usar();
        }

    }
}
