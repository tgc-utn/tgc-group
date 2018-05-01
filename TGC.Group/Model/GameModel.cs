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
using TGC.Core.Text;

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

        private VehiculoLiviano auto;
        private CamaraEnTerceraPersona camaraInterna;
        private TGCVector3 camaraDesplazamiento = new TGCVector3(0,5,40);
        private TGCBox cubo;
        private TgcScene scene;
        private TgcText2D textoVelocidadVehiculo, textoAlturaVehiculo;

        public override void Init()
        {
            
            //en caso de querer cargar una escena
            TgcSceneLoader loader = new TgcSceneLoader();
            scene = loader.loadSceneFromFile(MediaDir + "Texturas\\Habitacion\\escenaFinal-TgcScene.xml");
            //creo el vehiculo liviano
            //si quiero crear un vehiculo pesado (camion) hago esto
            // VehiculoPesado camion = new VehiculoPesado(rutaAMesh);
            // se hace esta distinción de vehiculo liviano y pesado por que cada uno tiene diferentes velocidades,
            // peso, salto, etc.
            auto = new VehiculoLiviano(MediaDir + "meshCreator\\meshes\\Vehiculos\\Camioneta\\Camioneta-TgcScene.xml");

            //creo un cubo para tomarlo de referencia (para ver como se mueve el auto)
            cubo = TGCBox.fromSize(new TGCVector3(-50, 10, -20), new TGCVector3(15, 15, 15), Color.Black);

            //creo la camara en tercera persona (la clase CamaraEnTerceraPersona hereda de la clase real del framework
            //que te permite configurar la posicion, el lookat, etc. Lo que hacemos al heredar, es reescribir algunos
            //metodos y setear valores default para que la camara quede mirando al auto en 3era persona

            camaraInterna = new CamaraEnTerceraPersona(auto.posicion() + camaraDesplazamiento, 7.5f, -55);
            Camara = camaraInterna;

        }

        public override void Update()
        {
            PreUpdate();
           
            if (Input.keyDown(Key.NumPad4))
            {
                camaraInterna.rotateY(-0.05f);
            }
            if (Input.keyDown(Key.NumPad6))
            {
                camaraInterna.rotateY(0.05f);
            }
            if (Input.keyDown(Key.RightArrow))
            {
                camaraInterna.OffsetHeight += 0.3f;
            }
            if (Input.keyDown(Key.LeftArrow))
            {
                camaraInterna.OffsetHeight -= 0.3f;
            }

            if (Input.keyDown(Key.UpArrow))
            {
                camaraInterna.OffsetForward += 0.3f;
            }
            if (Input.keyDown(Key.DownArrow))
            {
                camaraInterna.OffsetForward -= 0.3f;
            }

            textoVelocidadVehiculo = new TgcText2D();
            string dialogo = "Velocidad = {0}km";
            textoVelocidadVehiculo.Text = string.Format(dialogo, auto.getVelocidadActual());
            //text3.Align = TgcText2D.TextAlign.RIGHT;
            textoVelocidadVehiculo.Position = new Point(55, 15);
            textoVelocidadVehiculo.Size = new Size(0, 0);
            textoVelocidadVehiculo.Color = Color.Gold;

            textoAlturaVehiculo = new TgcText2D();
            string dialogo2 = "Velocidad salto = {0}";
            textoAlturaVehiculo.Text = string.Format(dialogo2, auto.getVelocidadActualDeSalto());
            //text3.Align = TgcText2D.TextAlign.RIGHT;
            textoAlturaVehiculo.Position = new Point(55, 25);
            textoAlturaVehiculo.Size = new Size(0, 0);
            textoAlturaVehiculo.Color = Color.Gold;


            auto.setElapsedTime(ElapsedTime);

            //si el usuario teclea la W y ademas no tecla la D o la A
            if (Input.keyDown(Key.W))
            {
                //hago avanzar al auto hacia adelante. Le paso el Elapsed Time que se utiliza para
                //multiplicarlo a la velocidad del auto y no depender del hardware del computador
                auto.getEstado().advance();

            }

            //lo mismo que para avanzar pero para retroceder
            if (Input.keyDown(Key.S))
            {
                auto.getEstado().back();
            }

            //si el usuario teclea D
            if (Input.keyDown(Key.D))
            {
                auto.getEstado().right(camaraInterna);
                
            }else if (Input.keyDown(Key.A))
            {
                auto.getEstado().left(camaraInterna);
            }

            //Si apreta espacio, salta
            if (Input.keyDown(Key.Space))
            {
                auto.getEstado().jump();
            }

            if (!Input.keyDown(Key.W) && !Input.keyDown(Key.S))
            {
                auto.getEstado().speedUpdate();
            }

            //esto es algo turbio que tengo que hacer, por que sino es imposible modelar el salto
            auto.getEstado().jumpUpdate();


            //Hacer que la camara siga al personaje en su nueva posicion
            camaraInterna.Target = auto.posicion() + auto.getVectorAdelante() * 30 ;  

            PostUpdate();
        }

        public override void Render()
        {
            
            PreRender();

            textoVelocidadVehiculo.render();
            textoAlturaVehiculo.render();

            scene.RenderAll();

            auto.Render();

            cubo.Transform =
                TGCMatrix.Scaling(cubo.Scale)
                            * TGCMatrix.RotationYawPitchRoll(cubo.Rotation.Y, cubo.Rotation.X, cubo.Rotation.Z)
                            * TGCMatrix.Translation(cubo.Position);
            cubo.Render();

            PostRender();
        }

        public override void Dispose()
        {
            //Dispose del auto.
            auto.dispose();

            //Dispose del cubo
            cubo.Dispose();
        }
    }
}