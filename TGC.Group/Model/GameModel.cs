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

namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo para implementar el TP.
    ///     Inicialmente puede ser renombrado o copiado para hacer más ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar el modelo que instancia GameForm <see cref="Form.GameForm.InitGraphics()" />
    ///     line 97.
    /// </summary>
    public class GameModel : TgcExample
    {
        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        /// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
        /// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        private Vehiculo auto;
        private CamaraEnTerceraPersona camaraInterna;
        private TGCBox cubo;
        private TgcPlane piso;

        public override void Init()
        {
            //Crear piso
            var pisoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Texturas\\pasto.jpg");
            piso = new TgcPlane(new TGCVector3(-500, 0, -500), new TGCVector3(1000, 0, 1000), TgcPlane.Orientations.XZplane, pisoTexture);

            auto = new Vehiculo(MediaDir + "meshCreator\\meshes\\Vehiculos\\Camioneta\\Camioneta-TgcScene.xml");
            auto.escalar(new TGCVector3(0.1f, 0.1f, 0.1f));
            auto.rotarEnY(FastMath.PI);

            //cubo
            cubo = TGCBox.fromSize(new TGCVector3(-50, 10, -20), new TGCVector3(15, 15, 15), Color.Black);

            camaraInterna = new CamaraEnTerceraPersona(auto.posicion(), 30, -75);
            Camara = camaraInterna;

        }

        public override void Update()
        {
            PreUpdate();

            TGCVector3 movement = TGCVector3.Empty;

            //Adelante
            if (Input.keyDown(Key.W) && !(Input.keyDown(Key.D) || Input.keyDown(Key.A)))
            {
                auto.avanzar(ElapsedTime);

            }

            //Atras
            if (Input.keyDown(Key.S) && !(Input.keyDown(Key.D) || Input.keyDown(Key.A)))
            {
                auto.retroceder(ElapsedTime);
            }

            //Derecha
            if (Input.keyDown(Key.D))
            {
                if(Input.keyDown(Key.W)){
                    auto.avanzarHaciaLaDerecha(ElapsedTime, camaraInterna);
                }
                else if (Input.keyDown(Key.S) && ElapsedTime != 0f) {
                    auto.retrocederHaciaLaDerecha(ElapsedTime, camaraInterna);
                }
            }

            //Derecha
            if (Input.keyDown(Key.A))
            {
                if (Input.keyDown(Key.W))
                {
                    auto.avanzarHaciaLaIzquierda(ElapsedTime, camaraInterna);
                }
                else if (Input.keyDown(Key.S))
                {
                    auto.retrocederHaciaLaIzquierda(ElapsedTime, camaraInterna);
                }
            }

            //Salto
            if (Input.keyDown(Key.Space))
            {
                auto.saltar(ElapsedTime);
            }

            auto.actualizarSalto(ElapsedTime);


            //Hacer que la camara siga al personaje en su nueva posicion
            camaraInterna.Target = auto.posicion();
            
            

            PostUpdate();
        }

        public override void Render()
        {
            
            PreRender();

            piso.Render();

            auto.transformar();
            
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