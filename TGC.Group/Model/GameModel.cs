using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Group.Model;
using TGC.Group.Model.Vehiculos;
using TGC.Group.Model.Vehiculos.Estados;
using TGC.Core.Text;
using System.Collections.Generic;
using TGC.Core.Collision;
using TGC.Core.Sound;

namespace TGC.Group.Model
{
    public class GameModel : TgcExample
    {

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        private Camioneta auto;
        private CamaraEnTerceraPersona camaraInterna;
        private TGCVector3 camaraDesplazamiento = new TGCVector3(0,5,40);
        private TgcText2D textoVelocidadVehiculo, textoOffsetH, textoOffsetF, textoPosicionVehiculo, textoVectorAdelante;


        public override void Init()
        {
            ConceptosGlobales.getInstance().SetMediaDir(this.MediaDir);
            ConceptosGlobales.getInstance().SetDispositivoDeAudio(this.DirectSound.DsDevice);
            Escena.getInstance().init(this.MediaDir);
            this.auto = new Camioneta(MediaDir, new TGCVector3(-0f, 0f, 0f));
            this.auto.mesh.AutoTransform = false;

            this.camaraInterna = new CamaraEnTerceraPersona(auto.GetPosicionCero() + camaraDesplazamiento, 0.8f, -33);
            this.Camara = camaraInterna;
        }

        public override void Update()
        {
            this.PreUpdate();
           
            if (Input.keyDown(Key.NumPad4))
            {
                this.camaraInterna.rotateY(-0.005f);
            }
            if (Input.keyDown(Key.NumPad6))
            {
                this.camaraInterna.rotateY(0.005f);
            }

            if (Input.keyDown(Key.RightArrow))
            {
                this.camaraInterna.OffsetHeight += 0.05f;
            }
            if (Input.keyDown(Key.LeftArrow))
            {
                this.camaraInterna.OffsetHeight -= 0.05f;
            }

            if (Input.keyDown(Key.UpArrow))
            {
                this.camaraInterna.OffsetForward += 0.05f;
            }
            if (Input.keyDown(Key.DownArrow))
            {
                this.camaraInterna.OffsetForward -= 0.05f;
            }

            string dialogo;

            dialogo = "Velocidad = {0}km";
            dialogo = string.Format(dialogo, auto.GetVelocidadActual());
            textoVelocidadVehiculo = Textos.newText(dialogo, 120, 10);

            dialogo = "Posicion = ({0} | {1} | {2})";
            dialogo = string.Format(dialogo, auto.GetPosicion().X, auto.GetPosicion().Y, auto.GetPosicion().Z);
            textoPosicionVehiculo = Textos.newText(dialogo, 120, 25);

            dialogo = "VectorAdelante = ({0} | {1} | {2})";
            dialogo = string.Format(dialogo, auto.GetVectorAdelante().X, auto.GetVectorAdelante().Y, auto.GetVectorAdelante().Z);
            textoVectorAdelante = Textos.newText(dialogo, 120, 40);

            dialogo = "OffsetHeight = {0}";
            dialogo = string.Format(dialogo, this.camaraInterna.OffsetHeight);
            textoOffsetH = Textos.newText(dialogo, 120, 70);

            dialogo = "OffsetForward = {0}";
            dialogo = string.Format(dialogo, this.camaraInterna.OffsetForward);
            textoOffsetF = Textos.newText(dialogo, 120, 85);

            this.auto.SetElapsedTime(ElapsedTime);

            if (Input.keyDown(Key.W))
            {
                this.auto.GetEstado().Advance();

            }

            if (Input.keyDown(Key.S))
            {
                this.auto.GetEstado().Back();
            }

            if (Input.keyDown(Key.D))
            {
                this.auto.GetEstado().Right(camaraInterna);
                
            }else if (Input.keyDown(Key.A))
            {
                this.auto.GetEstado().Left(camaraInterna);
            }

            if (Input.keyDown(Key.Space))
            {
                this.auto.GetEstado().Jump();
            }

            if (!Input.keyDown(Key.W) && !Input.keyDown(Key.S))
            {
                this.auto.GetEstado().SpeedUpdate();
            }

            this.auto.GetEstado().JumpUpdate();

            this.camaraInterna.Target = (TGCVector3.transform(auto.GetPosicionCero(), auto.GetTransformacion())) + auto.GetVectorAdelante() * 30 ;

            //Comentado para que los sonidos funcionen correctamente
            //this.auto = Escena.getInstance().calculateCollisions(this.auto);
            
            
            
            
            /*
            if (collide)
            {
                    var movementRay = lastPosition - TGCMatrix.Translation(auto.getPosicion());
                    var rs = TGCVector3.Empty;
                    if (((auto.mesh.BoundingBox.PMax.X > collider.BoundingBox.PMax.X && movementRay.X > 0) ||
                        (auto.mesh.BoundingBox.PMin.X < collider.BoundingBox.PMin.X && movementRay.X < 0)) &&
                        ((auto.mesh.BoundingBox.PMax.Z > collider.BoundingBox.PMax.Z && movementRay.Z > 0) ||
                        (auto.mesh.BoundingBox.PMin.Z < collider.BoundingBox.PMin.Z && movementRay.Z < 0)))
                    {
                        if (auto.mesh.Position.X > collider.BoundingBox.PMin.X && auto.mesh.Position.X < collider.BoundingBox.PMax.X)
                        {
                            rs = new TGCVector3(movementRay.X, movementRay.Y, 0);
                        }
                        if (auto.mesh.Position.Z > collider.BoundingBox.PMin.Z && auto.mesh.Position.Z < collider.BoundingBox.PMax.Z)
                        {
                            rs = new TGCVector3(0, movementRay.Y, movementRay.Z);
                        }

                    }
                    else
                    {
                        if ((auto.mesh.BoundingBox.PMax.X > collider.BoundingBox.PMax.X && movementRay.X > 0) ||
                            (auto.mesh.BoundingBox.PMin.X < collider.BoundingBox.PMin.X && movementRay.X < 0))
                        {
                            rs = new TGCVector3(0, movementRay.Y, movementRay.Z);
                        }
                        if ((auto.mesh.BoundingBox.PMax.Z > collider.BoundingBox.PMax.Z && movementRay.Z > 0) ||
                            (auto.mesh.BoundingBox.PMin.Z < collider.BoundingBox.PMin.Z && movementRay.Z < 0))
                        {
                            rs = new TGCVector3(movementRay.X, movementRay.Y, 0);
                        }
                    }
                    auto.mesh.Position = lastPos - rs;
            }*/

            this.PostUpdate();
        }

        public override void Render()
        {

            this.PreRender();

            Escena.getInstance().render();

            this.textoVelocidadVehiculo.render();

            this.textoPosicionVehiculo.render();
            this.textoVectorAdelante.render();
            this.textoOffsetF.render();
            this.textoOffsetH.render();
                       
            this.auto.Transform();
            this.auto.Render();

            this.PostRender();
        }

        public override void Dispose()
        {
            Escena.getInstance().dispose();
            this.auto.Dispose();
           
        }

    }
}