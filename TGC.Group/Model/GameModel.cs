using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;

namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo para implementar el TP.
    ///     Inicialmente puede ser renombrado o copiado para hacer más ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar el modelo que instancia GameForm <see cref="Form.GameForm.InitGraphics()" />
    ///     line 97.
    /// </summary>
    public class GameModel : TGCExample
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

        //Mesh de TgcLogo.
        private TgcMesh Cancha { get; set; }

        private TgcMesh Arco { get; set; }
        private TGCSphere Pelota { get; set; }

        private List<Jugador> jugadores = new List<Jugador>();

        private TGCVector3 LookAt;
        private TGCVector3 CameraPosition;

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

            //Textura de la carperta Media. Game.Default es un archivo de configuracion (Game.settings) util para poner cosas.
            //Pueden abrir el Game.settings que se ubica dentro de nuestro proyecto para configurar.
            var pathTexturaCaja = MediaDir + Game.Default.TexturaCaja;

            //Cargamos una textura, tener en cuenta que cargar una textura significa crear una copia en memoria.
            //Es importante cargar texturas en Init, si se hace en el render loop podemos tener grandes problemas si instanciamos muchas.
            var texture = TgcTexture.createTexture(pathTexturaCaja);

            //Creamos una caja 3D ubicada de dimensiones (5, 10, 5) y la textura como color.
            var size = new TGCVector3(5, 10, 5);

            TgcScene escena = new TgcSceneLoader().loadSceneFromFile(MediaDir + "Cancha2-TgcScene.xml");
            Cancha = escena.Meshes[0];
            Arco = escena.Meshes[1];
            TgcMesh Auto = escena.Meshes[2];
            TgcMesh Tractor = escena.Meshes[5];
            TgcMesh Patrullero = escena.Meshes[3];
            TgcMesh Tanque = escena.Meshes[4];

            jugadores.Add(new Jugador(Auto, new TGCVector3(-20, 0, 100), new TGCVector3(0, 0, 0)));
            jugadores.Add(new Jugador(Tractor, new TGCVector3(0, 0, -30), new TGCVector3(0, FastMath.PI, 0)));
            jugadores.Add(new Jugador(Patrullero, new TGCVector3(0, 0, 30), new TGCVector3(0, 0, 0)));
            jugadores.Add(new Jugador(Tanque, new TGCVector3(20, 0, -100), new TGCVector3(0, FastMath.PI, 0)));

            Pelota = new TGCSphere(10, Color.White, new TGCVector3(0, 5f, 0));
            Pelota.Radius = 10;

            //Suelen utilizarse objetos que manejan el comportamiento de la camara.
            //Lo que en realidad necesitamos gráficamente es una matriz de View.
            //El framework maneja una cámara estática, pero debe ser inicializada.
            //Posición de la camara.
            var cameraPosition = new TGCVector3(0, 100, 225);
            //Quiero que la camara mire hacia el origen (0,0,0).
            var lookAt = TGCVector3.Empty;

            LookAt = new TGCVector3(TGCVector3.Empty);
            CameraPosition = cameraPosition;
            //Configuro donde esta la posicion de la camara y hacia donde mira.
            base.Camera.SetCamera(cameraPosition, lookAt);
            //Internamente el framework construye la matriz de view con estos dos vectores.
            //Luego en nuestro juego tendremos que crear una cámara que cambie la matriz de view con variables como movimientos o animaciones de escenas.
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        public override void Update()
        {
            PreUpdate();

            //Capturar Input teclado
            if (Input.keyPressed(Key.F))
            {
                BoundingBox = !BoundingBox;
            }

            //Capturar Input Mouse
            /*if (Input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Como ejemplo podemos hacer un movimiento simple de la cámara.
                //En este caso le sumamos un valor en Y
                Camera.SetCamera(Camera.Position + new TGCVector3(0, 10f, 0), Camera.LookAt);
                //Ver ejemplos de cámara para otras operaciones posibles.

                //Si superamos cierto Y volvemos a la posición original.
                if (Camera.Position.Y > 300f)
                {
                    Camera.SetCamera(new TGCVector3(Camera.Position.X, 0f, Camera.Position.Z), Camera.LookAt);
                }
            }*/


            LookAt.X += Input.XposRelative * 0.5f;
            LookAt.Y += Input.YposRelative * 0.5f;
            Camera.SetCamera(CameraPosition, LookAt);

            if (Input.keyDown(Key.W))
            {
                CameraPosition.Z +=  0.5f * ElapsedTime;
            }

            PostUpdate();
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aquí todo el código referido al renderizado.
        ///     Borrar todo lo que no haga falta.
        /// </summary>
        public override void Render()
        {
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
            PreRender();

            //Dibuja un texto por pantalla
            DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.OrangeRed);
            DrawText.drawText("Con clic izquierdo subimos la camara [Actual]: " + TGCVector3.PrintTGCVector3(Camera.Position), 0, 30, Color.OrangeRed);

            ////Siempre antes de renderizar el modelo necesitamos actualizar la matriz de transformacion.
            ////Debemos recordar el orden en cual debemos multiplicar las matrices, en caso de tener modelos jerárquicos, tenemos control total.
            //Box.Transform = TGCMatrix.Scaling(Box.Scale) * TGCMatrix.RotationYawPitchRoll(Box.Rotation.Y, Box.Rotation.X, Box.Rotation.Z) * TGCMatrix.Translation(Box.Position);
            ////A modo ejemplo realizamos toda las multiplicaciones, pero aquí solo nos hacia falta la traslación.
            ////Finalmente invocamos al render de la caja
            //Box.Render();

            Arco.Rotation = new TGCVector3(0, 0, 0);
            Arco.UpdateMeshTransform();
            Arco.Render();

            Arco.Rotation = new TGCVector3(0, FastMath.PI, 0);
            Arco.UpdateMeshTransform();
            Arco.Render();

            Pelota.Transform = TGCMatrix.Scaling(new TGCVector3(Pelota.Radius, Pelota.Radius, Pelota.Radius)) * TGCMatrix.RotationYawPitchRoll(Pelota.Rotation.Y, Pelota.Rotation.X, Pelota.Rotation.Z) * TGCMatrix.Translation(Pelota.Position);
            Pelota.updateValues();
            Pelota.Render();

            //Cuando tenemos modelos mesh podemos utilizar un método que hace la matriz de transformación estándar.
            //Es útil cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones jerárquicas o complicadas.
            Cancha.UpdateMeshTransform();

            //Render del mesh
            Cancha.Render();

            foreach (var jugador in jugadores)
            {
                jugador.Render();
            }

            //Render de BoundingBox, muy útil para debug de colisiones.
            if (BoundingBox)
            {
                Cancha.BoundingBox.Render();
            }

            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        /// <summary>
        ///     Se llama cuando termina la ejecución del ejemplo.
        ///     Hacer Dispose() de todos los objetos creados.
        ///     Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        /// </summary>
        public override void Dispose()
        {
            //Dispose del mesh.
            Cancha.Dispose();
            //Dispose del Arco
            Arco.Dispose();
            Pelota.Dispose();
        }
    }
}