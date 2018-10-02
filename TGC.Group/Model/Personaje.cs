using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SkeletalAnimation;
using TGC.Core.Mathematica;
using Microsoft.DirectX.DirectInput;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model
{
    public class Personaje
    {
        public TgcSkeletalMesh Mesh { get; set; }
        public bool moving;
        public bool colisionaEnY;
        private TGCMatrix ultimaPosicion;
        public TGCVector3 movimiento;
        private const float VelocidadDesplazamiento = 50f;
        public TGCVector3 Position
        {
            get => Mesh.Transform.Origin;
        }
        public TgcBoundingAxisAlignBox BoundingBox
        {
            get => Mesh.BoundingBox;
        }

        private GameModel Context;
        //
        public float VelocidadY = 0f;
        float VelocidadSalto = 90f;
        float Gravedad = -60f;
        float VelocidadTerminal = -50f;
        float DesplazamientoMaximoY = 5f;
        //

        public void Init(GameModel context)
        {
            Context = context;
            string mediaDir = context.MediaDir;

            var skeletalLoader = new TgcSkeletalLoader();
            Mesh = skeletalLoader.loadMeshAndAnimationsFromFile(
                mediaDir + "primer-nivel\\pozo-plataformas\\tgc-scene\\Robot\\Robot-TgcSkeletalMesh.xml",
                mediaDir + "primer-nivel\\pozo-plataformas\\tgc-scene\\Robot\\",
                new[]
                {
                    mediaDir + "primer-nivel\\pozo-plataformas\\tgc-scene\\Robot\\Caminando-TgcSkeletalAnim.xml",
                    mediaDir + "primer-nivel\\pozo-plataformas\\tgc-scene\\Robot\\Parado-TgcSkeletalAnim.xml"
                });


            Mesh.AutoTransform = false;
            Mesh.Transform = TGCMatrix.Identity;

            //Configurar animacion inicial
            Mesh.playAnimation("Parado", true);
            //Escalarlo porque es muy grande
            Mesh.Position = new TGCVector3(25, 0, 50);
            Mesh.Scale = new TGCVector3(0.15f, 0.15f, 0.15f);
            ultimaPosicion = TGCMatrix.Translation(Mesh.Position);
        }

        public void Update()
        {
            var elapsedTime = Context.ElapsedTime;
            var input = Context.Input;

            var velocidadCaminar = VelocidadDesplazamiento * elapsedTime;

            //Calcular proxima posicion de personaje segun Input
            moving = false;
            colisionaEnY = false;
            movimiento = TGCVector3.Empty;
            //transformacionPersonaje = TGCMatrix.Identity;

            if (input.keyDown(Key.W))
            {
                movimiento += new TGCVector3(0, 0, -velocidadCaminar);
                moving = true;
            }

            if (input.keyDown(Key.S))
            {
                movimiento += new TGCVector3(0, 0, velocidadCaminar);
                moving = true;
            }

            if (input.keyDown(Key.D))
            {
                movimiento += new TGCVector3(-velocidadCaminar, 0, 0);
                moving = true;
            }

            if (input.keyDown(Key.A))
            {
                movimiento += new TGCVector3(velocidadCaminar, 0, 0);
                moving = true;
            }
            //
            if (input.keyPressed(Key.Space))
            {
                VelocidadY = VelocidadSalto;
            }

            if (!colisionaEnY)
            {
                VelocidadY = FastMath.Clamp(VelocidadY + Gravedad * elapsedTime, VelocidadTerminal, -VelocidadTerminal);

                movimiento += new TGCVector3(0, FastMath.Clamp(VelocidadY * elapsedTime, -DesplazamientoMaximoY, DesplazamientoMaximoY), 0);
                moving = true;
            }
        }

        public void Movete(TGCVector3 movimiento)
        {
            if (moving)
            {
                Mesh.playAnimation("Caminando", true);

                //personaje.Position += movimiento;
                ultimaPosicion *= TGCMatrix.Translation(movimiento);
            }
            else
            {
                Mesh.playAnimation("Parado", true);
            }
        }

        public void Render()
        {
            Mesh.Transform =
                TGCMatrix.Scaling(Mesh.Scale)
                * TGCMatrix.RotationYawPitchRoll(Mesh.Rotation.Y, Mesh.Rotation.X, Mesh.Rotation.Z)
                * ultimaPosicion;

            Mesh.BoundingBox.transform(Mesh.Transform);

            Mesh.animateAndRender(Context.ElapsedTime);

            if (Context.BoundingBox)
            {
                Mesh.BoundingBox.Render();
            }
        }

        public void Dispose()
        {
            Mesh.Dispose();
        }

        internal void ColisionoEnY()
        {
            this.colisionaEnY = true;
            if (movimiento.X == 0 && movimiento.Z == 0)
                this.moving = false;
        }
    }
}