using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Examples.Camara;


namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo para implementar el TP.
    ///     Inicialmente puede ser renombrado o copiado para hacer más ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar el modelo que instancia GameForm <see cref="Form.GameForm.InitGraphics()" />
    ///     line 97.
    /// </summary>
    public class Vuelo : TgcExample
    {
        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        /// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
        /// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
        public Vuelo(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        //Caja que se muestra en el ejemplo.
        private TGCBox box { get; set; }
        private UnaPicaraCaja otraBox { get; set; }
        private TGCBox boxCamera { get; set; }
        private TgcScene scene;
        private TgcThirdPersonCamera camaraInterna;

        //Boleano para ver si dibujamos el boundingbox
        private bool BoundingBox { get; set; }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aquí todo el código de inicialización: cargar modelos, texturas, estructuras de optimización, todo
        ///     procesamiento que podemos pre calcular para nuestro juego.
        ///     Borrar el codigo ejemplo no utilizado.
        /// </summary>
        public override void Init()
        {
            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;

            scene = new TgcSceneLoader().loadSceneFromFile(MediaDir + "Ciudad\\Ciudad-TgcScene.xml");

            var pathTexturaCaja = MediaDir + "troll.jpg";
            var texture = TgcTexture.createTexture(pathTexturaCaja);
            var color = Color.Red;
            var size = new TGCVector3(4, 4, 10);

            box = TGCBox.fromSize(size, texture);
            boxCamera = TGCBox.fromSize(new TGCVector3(1, 1, 1), color);
            //Posición 
            box.Position = new TGCVector3(0, 200, 0);
            boxCamera.Position = new TGCVector3(0, 200, 0);


            //Vamos a utilizar la camara en 3ra persona para que siga al objeto principal a medida que se mueve
            camaraInterna = new TgcThirdPersonCamera(boxCamera.Position, 20, 80);
            Camara = camaraInterna;

            otraBox = new UnaPicaraCaja(MediaDir, ShadersDir);
            otraBox.Init();
        }


        /// </summary>
        public override void Update()
        {
            PreUpdate();
            otraBox.Update(ElapsedTime);
            //Obtenemos acceso al objeto que maneja input de mouse y teclado del framework
            var input = Input;

            //Declaramos un vector de movimiento inicializado en cero.
            //El movimiento sobre el suelo es sobre el plano XZ.
            //Sobre XZ nos movemos con las flechas del teclado o con las letas WASD.
            var movement = new TGCVector3(0,0,-1);
            var cameraMovement = new TGCVector3(0, 0, -1);

            //Movernos de izquierda a derecha, sobre el eje X.
            if (input.keyDown(Key.Left) || input.keyDown(Key.A))
            {
                movement.X = 1;
            }
            else if (input.keyDown(Key.Right) || input.keyDown(Key.D))
            {
                movement.X = -1;
            }

            //Movernos adelante y atras, sobre el eje Z.
            if (input.keyDown(Key.Up) || input.keyDown(Key.W))
            {
                movement.Y = 1;
            }
            else if (input.keyDown(Key.Down) || input.keyDown(Key.S))
            {
                movement.Y = -1;
            }

            //Guardar posicion original antes de cambiarla
            var originalPos = box.Position;

            //Multiplicar movimiento por velocidad y elapsedTime
            movement *= 50f * ElapsedTime;
            box.Position = box.Position + movement;
            box.Transform = TGCMatrix.Translation(box.Position);
            cameraMovement *= 50f * ElapsedTime;
            boxCamera.Position = boxCamera.Position + cameraMovement;

            camaraInterna.Target = boxCamera.Position;
            PostUpdate();
        }


        public override void Render()
        {
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
            PreRender();

            scene.RenderAll();
            boxCamera.Render();
            box.Render();
            otraBox.Render();



            PostRender();
        }

        public override void Dispose()
        {
            box.Dispose();
            otraBox.Dispose();
            boxCamera.Dispose();
            scene.DisposeAll();
        }
    }
}