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

        private TgcMesh auto { get; set; }
        private CamaraEnTerceraPersona camaraInterna;
        private float velocidadVehiculo = 50f;
        private TGCBox cubo;
        private TgcPlane piso;

        public override void Init()
        {
            //Crear piso
            var pisoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Texturas\\pasto.jpg");
            piso = new TgcPlane(new TGCVector3(-500, 0, -500), new TGCVector3(1000, 0, 1000), TgcPlane.Orientations.XZplane, pisoTexture);

            var loader = new TgcSceneLoader();
            var scene = loader.loadSceneFromFile(MediaDir + "meshCreator\\meshes\\Vehiculos\\Camioneta\\Camioneta-TgcScene.xml");
            auto = scene.Meshes[0];
            auto.Scale = new TGCVector3(0.1f, 0.1f, 0.1f);

            //cubo
            cubo = TGCBox.fromSize(new TGCVector3(-50, 10, -20), new TGCVector3(15, 15, 15), Color.Black);

            camaraInterna = new CamaraEnTerceraPersona(auto.Position, 30, -75);
            Camara = camaraInterna;

        }

        public override void Update()
        {
            PreUpdate();

            float velocidadReal = this.velocidadVehiculo * ElapsedTime;

            //Calcular proxima posicion de personaje segun Input
            bool moving = false;
            TGCVector3 movement = TGCVector3.Empty;

            //Adelante
            if (Input.keyDown(Key.W))
            {
                movement.Z = velocidadReal;
                moving = true;
            }

            //Atras
            if (Input.keyDown(Key.S))
            {
                movement.Z = -velocidadReal;
                moving = true;
            }

            //Derecha
            if (Input.keyDown(Key.D))
            {
                movement.X = velocidadReal;
                moving = true;
            }

            //Izquierda
            if (Input.keyDown(Key.A))
            {
                movement.X = -velocidadReal;
                moving = true;
            }
            //Salto
            if (Input.keyDown(Key.Space))
            {
                movement.Y = velocidadReal;
                moving = true;
            }

            //Si hubo desplazamiento
            if (moving)
            {
                //Aplicar movimiento, internamente suma valores a la posicion actual del mesh.
                auto.Move(movement);
            }
            //Hacer que la camara siga al personaje en su nueva posicion
            camaraInterna.Target = auto.Position;

            PostUpdate();
        }

        public override void Render()
        {
            
            PreRender();

            piso.Render();

            auto.Transform =
                TGCMatrix.Scaling(auto.Scale)
                            * TGCMatrix.RotationYawPitchRoll(auto.Rotation.Y, auto.Rotation.X, auto.Rotation.Z)
                            * TGCMatrix.Translation(auto.Position);
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
            auto.Dispose();

            //Dispose del cubo
            cubo.Dispose();
        }
    }
}