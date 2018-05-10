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

        private TgcStaticSound aceleracion, salto;

        private Camioneta auto;
        private CamaraEnTerceraPersona camaraInterna;
        private TGCVector3 camaraDesplazamiento = new TGCVector3(0,5,40);
        private TgcText2D textoVelocidadVehiculo, textoOffsetH, textoOffsetF, textoPosicionVehiculo, textoVectorAdelante, textoVectorCostado;
        //escena
        private Objeto scene;
        //habitacion
        private Objeto cajaZapatillas, sillon, escoba, cama, mesaDeLuz, ropero, armario, escritorio, sillaEscritorio;
        //cocina
        private Objeto mesadaCocina1, cajaCocina, dispenser, mesaCocina, heladera, tacho, sillaCocina1, sillaCocina2, sillaCocina3, muebleCocina, libroCocina1, libroCocina2, libroCocina3;
        //banio
        private Objeto bathtub, inodoro, cepillo, esponja, jabon, banqueta, espejo;
        private List<Objeto> objetosEscenario = new List<Objeto>();

        public override void Init()
        {

            TgcSceneLoader loader = new TgcSceneLoader();
            this.scene = this.dameMesh("Texturas\\Habitacion\\escenaFinal-TgcScene.xml", new TGCVector3(1,1,1), new TGCVector3(0,0,0), new TGCVector3(0,0,0));
            //cocina
            this.cama = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Cama\\Cama-TgcScene.xml", new TGCVector3(1, 1, 1), new TGCVector3(0, FastMath.PI, 0), new TGCVector3(-36f, 0, -124f));
            this.mesaDeLuz = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\MesaDeLuz\\MesaDeLuz-TgcScene.xml", new TGCVector3(1, 1, 1), new TGCVector3(0, FastMath.PI, 0), new TGCVector3(22f, 0, -158f));
            this.ropero = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Placard\\Placard-TgcScene.xml", new TGCVector3(1, 1, 1), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(-205f, 0, -122f));
            this.armario = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Armario\\Armario-TgcScene.xml", new TGCVector3(1, 1, 1), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(-30f, 0, 110f));
            this.escritorio = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Escritorio\\Escritorio-TgcScene.xml", new TGCVector3(1, 1, 1), new TGCVector3(0, 0, 0), new TGCVector3(183f, 0f, -107f));
            this.sillaEscritorio = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\SillaEscritorio\\SillaEscritorio-TgcScene.xml", new TGCVector3(0.5f, 0.3f, 0.5f), new TGCVector3(0, 0, 0), new TGCVector3(151f, -1f, -101f));
            this.dispenser = this.dameMesh("MeshCreator\\Meshes\\Cocina\\DispenserAgua\\DispenserAgua-TgcScene.xml", new TGCVector3(1f, 1f, 1f), new TGCVector3(0, 0, 0), new TGCVector3(-32f, -1f, 360f));
            this.mesaCocina = this.dameMesh("MeshCreator\\Meshes\\Cocina\\Mesa\\Mesa-TgcScene.xml", new TGCVector3(1f, 1f, 1f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(94f, 0f, 335f));
            this.heladera = this.dameMesh("MeshCreator\\Meshes\\Cocina\\Heladera\\Heladera-TgcScene.xml", new TGCVector3(1f, 1f, 1f), new TGCVector3(0, 0, 0), new TGCVector3(190f, -1f, 180f));
            this.tacho = this.dameMesh("MeshCreator\\Meshes\\Cocina\\Tacho\\Tacho-TgcScene.xml", new TGCVector3(0.6f, 0.6f, 0.6f), new TGCVector3(0, 0, 0), new TGCVector3(-30f, -1f, 165f));
            this.sillaCocina1 = this.dameMesh("MeshCreator\\Meshes\\Cocina\\Silla\\silla-TgcScene.xml", new TGCVector3(0.4f, 0.4f, 0.4f), new TGCVector3(0, -FastMath.QUARTER_PI, 0), new TGCVector3(116f, 0f, 295f));
            this.sillaCocina2 = this.dameMesh("MeshCreator\\Meshes\\Cocina\\Silla\\silla-TgcScene.xml", new TGCVector3(0.4f, 0.4f, 0.4f), new TGCVector3(0, -FastMath.QUARTER_PI, 0), new TGCVector3(90f, 0f, 265f));
            this.sillaCocina3 = this.dameMesh("MeshCreator\\Meshes\\Cocina\\Silla\\silla-TgcScene.xml", new TGCVector3(0.4f, 0.4f, 0.4f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(140f, 0f, 335f));
            this.muebleCocina = this.dameMesh("MeshCreator\\Meshes\\Cocina\\Mueble\\Mueble-TgcScene.xml", new TGCVector3(1f, 1f, 0.5f),new TGCVector3(0,0,0), new TGCVector3(10f, -1f, 357f));
            this.libroCocina1 = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Libros\\Comun\\Comun-TgcScene.xml", new TGCVector3(0.15f, 0.15f, 0.15f), new TGCVector3(0, 0, 0), new TGCVector3(110f, 23.85f, 290f));
            this.libroCocina2 = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Libros\\Comun\\Comun-TgcScene.xml", new TGCVector3(0.15f, 0.15f, 0.15f), new TGCVector3(0, -FastMath.QUARTER_PI, 0), new TGCVector3(110f, 25.1f, 290f));
            this.libroCocina3 = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Libros\\Arquitectura\\Arquitectura-TgcScene.xml", new TGCVector3(0.15f, 0.15f, 0.15f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(110f, 26.35f, 290f));
            this.cajaCocina = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Caja\\Caja-TgcScene.xml", new TGCVector3(0.25f, 0.26f, 0.25f), new TGCVector3(0,-FastMath.QUARTER_PI,0), new TGCVector3(65f, 0f ,235f));
            this.mesadaCocina1 = this.dameMesh("MeshCreator\\Meshes\\Cocina\\Mueble\\Mueble-TgcScene.xml", new TGCVector3(1f, 1f, 0.5f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(202f, 0f, 342f));

            //banio
            this.jabon = this.dameMesh("MeshCreator\\Meshes\\Bathroom\\Jabon\\Jabon-TgcScene.xml", new TGCVector3(0.7f, 0.7f, 0.7f), new TGCVector3(0, 0, 0), new TGCVector3(-75f, 0f, 265f));
            this.bathtub = this.dameMesh("MeshCreator\\Meshes\\Bathroom\\Bathtub\\Bathtub-TgcScene.xml", new TGCVector3(1.5f, 1.8f, 1.5f), new TGCVector3(0, 0, 0), new TGCVector3(-164f, 0f, 270f));
            this.inodoro = this.dameMesh("MeshCreator\\Meshes\\Bathroom\\InodoroCuadrado\\InodoroCuadrado-TgcScene.xml", new TGCVector3(2f, 3f, 3f), new TGCVector3(0, 0, 0), new TGCVector3(-85, 0f, 175));
            this.cepillo = this.dameMesh("MeshCreator\\Meshes\\Bathroom\\Cepillo\\Cepillo-TgcScene.xml", new TGCVector3(1.4f, 1.4f, 1.4f), new TGCVector3(0, FastMath.QUARTER_PI, 0), new TGCVector3(-125, 0f, 170f));
            this.esponja = this.dameMesh("MeshCreator\\Meshes\\Bathroom\\Esponja\\Esponja-TgcScene.xml", new TGCVector3(1f, 1f, 1f), new TGCVector3(0, FastMath.QUARTER_PI, 0), new TGCVector3(-165, 0f, 190));
            this.banqueta = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Banqueta\\Banqueta-TgcScene.xml", new TGCVector3(0.4f, 0.35f, 0.4f), new TGCVector3(0, 0, 0), new TGCVector3(-170, 0f, 215));
            this.espejo = this.dameMesh("MeshCreator\\Meshes\\Bathroom\\Espejo\\Espejo-TgcScene.xml", new TGCVector3(1f, 1f, 1f), new TGCVector3(0, 0, 0), new TGCVector3(-90f, 0f, 298f));

            this.cajaZapatillas = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\CajaZapatillas\\CajaZapatillas-TgcScene.xml", new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(100f, 0f, -147f));
            this.sillon = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Sillon\\Sillon-TgcScene.xml", new TGCVector3(1f, 1f, 1f), new TGCVector3(1.35f, FastMath.PI, 0), new TGCVector3(0f, 22f, 20f));
            this.escoba = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Escoba\\Escoba-TgcScene.xml", new TGCVector3(1.3f, 1.3f, 1.3f), new TGCVector3(FastMath.PI_HALF, 0, 0), new TGCVector3(-105f, 1f, -60f));
            //this.toalla = this.dameMesh("MeshCreator\\Meshes\\Bathroom\\Toalla\\Toalla-TgcScene.xml", new TGCVector3(1f, 1f, 1f), new TGCVector3(0, FastMath.QUARTER_PI, 0), new TGCVector3(-90, 0f, 245));

            this.auto = new Camioneta(MediaDir, new TGCVector3(-100f, 0f, 0f));
            this.auto.mesh.AutoTransform = false;

            this.camaraInterna = new CamaraEnTerceraPersona(auto.GetPosicionCero() + camaraDesplazamiento, 0.8f, -33);
            this.Camara = camaraInterna;

            this.loadSound(MediaDir + "Sound\\eee.wav", ref aceleracion);
            this.loadSound(MediaDir + "Sound\\Salto3.wav", ref salto);

        }

        private void loadSound(string path, ref TgcStaticSound audio)
        {
            //Borrar sonido anterior
            if (audio != null)
            {
                audio.dispose();
                audio = null;
            }

            //Cargar sonido
            audio = new TgcStaticSound();
            audio.loadSound(path, DirectSound.DsDevice);
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

            dialogo = "VectorCostado = ({0} | {1} | {2})";
            dialogo = string.Format(dialogo, auto.GetVectorCostado().X, auto.GetVectorCostado().Y, auto.GetVectorCostado().Z);
            textoVectorCostado = Textos.newText(dialogo, 120, 55);

            dialogo = "OffsetHeight = {0}";
            dialogo = string.Format(dialogo, this.camaraInterna.OffsetHeight);
            textoOffsetH = Textos.newText(dialogo, 120, 70);

            dialogo = "OffsetForward = {0}";
            dialogo = string.Format(dialogo, this.camaraInterna.OffsetForward);
            textoOffsetF = Textos.newText(dialogo, 120, 85);

            this.auto.SetElapsedTime(ElapsedTime);

            //si el usuario teclea la W y ademas no tecla la D o la A
            if (Input.keyDown(Key.W))
            {
                this.aceleracion.play();
                this.auto.GetEstado().Advance();

            }

            //lo mismo que para avanzar pero para retroceder
            if (Input.keyDown(Key.S))
            {
                this.auto.GetEstado().Back();
            }

            //si el usuario teclea D
            if (Input.keyDown(Key.D))
            {
                this.auto.GetEstado().Right(camaraInterna);
                
            }else if (Input.keyDown(Key.A))
            {
                this.auto.GetEstado().Left(camaraInterna);
            }

            //Si apreta espacio, salta
            if (Input.keyDown(Key.Space))
            {
                this.salto.play();
                this.auto.GetEstado().Jump();
            }

            if (!Input.keyDown(Key.W) && !Input.keyDown(Key.S))
            {
                this.auto.GetEstado().SpeedUpdate();
            }

            //esto es algo turbio que tengo que hacer, por que sino es imposible modelar el salto
            this.auto.GetEstado().JumpUpdate();


            //Hacer que la camara siga al auto.mesh en su nueva posicion
            this.camaraInterna.Target = (TGCVector3.transform(auto.GetPosicionCero(), auto.GetTransformacion())) + auto.GetVectorAdelante() * 30 ;

            //bool collide = false;
            TgcMesh collider = null;
            this.auto.mesh.BoundingBox.setRenderColor(Color.Yellow);
            foreach (Objeto objeto in objetosEscenario)
            {
                if ((collider = objeto.TestColision(auto.mesh)) != null)
                {
                    this.auto.mesh.BoundingBox.setRenderColor(Color.Red);
                    collider.BoundingBox.setRenderColor(Color.Red);
                    this.auto.SetVelocidadActual(-this.auto.GetVelocidadActual() * 4);
                    this.auto.SetEstado(new Backward(this.auto));
                }

                else
                {
                    objeto.SetColorBoundingBox(Color.Yellow);
                }

            }
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


            if (this.auto.GetVelocidadActual() == 0)
                this.aceleracion.stop();

            this.PostUpdate();
        }

        public override void Render()
        {

            this.PreRender();

            this.textoVelocidadVehiculo.render();

            this.textoPosicionVehiculo.render();
            this.textoVectorAdelante.render();
            this.textoVectorCostado.render();
            this.textoOffsetF.render();
            this.textoOffsetH.render();
            
            foreach (Objeto objeto in objetosEscenario)
            {   
                objeto.RenderBoundingBox();
                objeto.Render();
            }
            
            this.auto.Transform();
            this.auto.Render();
            this.auto.mesh.BoundingBox.Render();

            this.PostRender();
        }

        public override void Dispose()
        {
            //Dispose del auto.
            this.auto.Dispose();
            foreach (Objeto objeto in objetosEscenario)
            {
                objeto.Dispose();
            }
        }

        private Objeto dameMesh(string ruta, TGCVector3 escala, TGCVector3 rotacion, TGCVector3 traslado)
        {
            TgcScene tgcScene = new TgcSceneLoader().loadSceneFromFile(MediaDir + ruta);

            TGCMatrix matrixEscalado = TGCMatrix.Scaling(escala);
            TGCMatrix matrixRotacionX = TGCMatrix.RotationX(rotacion.X);
            TGCMatrix matrixRotacionY = TGCMatrix.RotationY(rotacion.Y);
            TGCMatrix matrixRotacionZ = TGCMatrix.RotationZ(rotacion.Z);
            TGCMatrix matrixRotacion = matrixRotacionX * matrixRotacionY * matrixRotacionZ;
            TGCMatrix matrixTraslacion = TGCMatrix.Translation(traslado);
            TGCMatrix transformacion = matrixEscalado * matrixRotacion * matrixTraslacion;

            Objeto nuevoObjeto = new Objeto(tgcScene.Meshes, transformacion);
            objetosEscenario.Add(nuevoObjeto);

            return nuevoObjeto;

        }
    }
}