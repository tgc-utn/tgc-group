using Microsoft.DirectX.DirectInput;
using System;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.Terrain;

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

        private bool saltando;
        private int direccionSalto = 1;
        private float posInicialBandicoot;
        private float alturaMaximaSalto = 20f;
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        private const float MOVEMENT_SPEED = 100f;
        //private TgcThirdPersonCamera camaraInterna;
       
        //Caja que se muestra en el ejemplo.
        private TGCBox Suelo { get; set; }

        //Mesh de Bandicoot.
        private TgcMesh Bandicoot { get; set; }

        //Boleano para ver si dibujamos el boundingbox
        private bool BoundingBox { get; set; }

        private TgcSimpleTerrain terrain;

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

            var path = $"{MediaDir}\\Heightmap\\hawai.jpg";
            var center = new TGCVector3(0f, 0f, 0f);
            terrain = new TgcSimpleTerrain();
            terrain.loadHeightmap(path, 20f, 1.3f, center);

            //Textura de la carperta Media. Game.Default es un archivo de configuracion (Game.settings) util para poner cosas.
            //Pueden abrir el Game.settings que se ubica dentro de nuestro proyecto para configurar.
            var TexturaSuelo = MediaDir + "/floor2_3.jpg";

            //Cargamos una textura, tener en cuenta que cargar una textura significa crear una copia en memoria.
            //Es importante cargar texturas en Init, si se hace en el render loop podemos tener grandes problemas si instanciamos muchas.
            var texture = TgcTexture.createTexture(TexturaSuelo);

            //Creamos una caja 3D ubicada de dimensiones (5, 10, 5) y la textura como color.
            var size = new TGCVector3(80, 1, 300);
            //Construimos una caja según los parámetros, por defecto la misma se crea con centro en el origen y se recomienda así para facilitar las transformaciones.
            Suelo = TGCBox.fromSize(size, texture);
            //Posición donde quiero que este la caja, es común que se utilicen estructuras internas para las transformaciones.
            //Entonces actualizamos la posición lógica, luego podemos utilizar esto en render para posicionar donde corresponda con transformaciones.
            Suelo.Position = new TGCVector3(0, -.5f, -140);

            //Cargo el unico mesh que tiene la escena.
            Bandicoot = new TgcSceneLoader().loadSceneFromFile(MediaDir + "/crash/CRASH (2)-TgcScene.xml").Meshes[0];
            //Defino una escala en el modelo logico del mesh que es muy grande.
            Bandicoot.Scale = new TGCVector3(0.05f, 0.05f, 0.05f);
            Bandicoot.RotateY(3.12f);

            //Suelen utilizarse objetos que manejan el comportamiento de la camara.
            //Lo que en realidad necesitamos gráficamente es una matriz de View.
            //El framework maneja una cámara estática, pero debe ser inicializada.
            //Posición de la camara.
            var cameraPosition = new TGCVector3(-5, 20, 50);
            //Quiero que la camara mire hacia el origen (0,0,0).
            var lookAt = Bandicoot.Position;
            //Configuro donde esta la posicion de la camara y hacia donde mira.

            Camara.SetCamera(cameraPosition, lookAt);
            //Internamente el framework construye la matriz de view con estos dos vectores.
            //Luego en nuestro juego tendremos que crear una cámara que cambie la matriz de view con variables como movimientos o animaciones de escenas.
            Bandicoot.BoundingBox.setExtremes(new TGCVector3(0, 0, 0), new TGCVector3(-185f, 225f, -100f));
            posInicialBandicoot = Bandicoot.Position.Y;
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        public override void Update()
        {
            PreUpdate();

            //Capturar Input teclado utilizado para movimiento 

            var anguloCamara = TGCVector3.Empty;
            var movimiento = TGCVector3.Empty;

            if (Input.keyPressed(Key.F))
            {
                BoundingBox = !BoundingBox;
            }

            // movimiento lateral
            if (Input.keyDown(Key.Left) || Input.keyDown(Key.A))
            {
                movimiento.X = 1;
            }
            else if (Input.keyDown(Key.Right) || Input.keyDown(Key.D))
            {
                movimiento.X = -1;
            }

            //Movernos adelante y atras, sobre el eje Z.
            if (Input.keyDown(Key.Up) || Input.keyDown(Key.W))
            {
                movimiento.Z = -1;
            }
            else if (Input.keyDown(Key.Down) || Input.keyDown(Key.S))
            {
                movimiento.Z = 1;
            }

            //salto

            if(Input.keyPressed(Key.Space) && !saltando)
            {
                saltando = true;
                direccionSalto = 1;
            }

            //Posicion original del mesh principal (o sea del bandicoot)
            var originalPos = Bandicoot.Position;
            anguloCamara = Bandicoot.Position;

            //Multiplicar movimiento por velocidad y elapsedTime
            movimiento *= MOVEMENT_SPEED * ElapsedTime;

            Bandicoot.Move(movimiento);
            if (saltando){
                Bandicoot.Move(0,direccionSalto* MOVEMENT_SPEED * ElapsedTime, 0);

                //Si la posicion en Y es mayor a la maxima altura. 
                if (Bandicoot.Position.Y > alturaMaximaSalto) {
                    direccionSalto = -1;
                }

                if (Bandicoot.Position.Y <= posInicialBandicoot) {
                    saltando = false;
                }
            }



            //Desplazar camara para seguir al personaje

            Camara.SetCamera(Camara.Position + new TGCVector3(movimiento), anguloCamara);

            //Capturar Input Mouse
            if (Input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Como ejemplo podemos hacer un movimiento simple de la cámara.
                //En este caso le sumamos un valor en Y
                Camara.SetCamera(Camara.Position + new TGCVector3(0, 10f, 0), Camara.LookAt);
                //Ver ejemplos de cámara para otras operaciones posibles.

                //Si superamos cierto Y volvemos a la posición original.
                if (Camara.Position.Y > 300f)
                {
                    Camara.SetCamera(new TGCVector3(Camara.Position.X, 0f, Camara.Position.Z), Camara.LookAt);
                }
            }

            if (Input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_RIGHT))
            {
                //Pruebo si baja camara
                Camara.SetCamera(Camara.Position + new TGCVector3(0, -10f, 0), Camara.LookAt);

                //igual que si sube a cierta altura reinicio camara
                if (Camara.Position.Y < -200f)
                {
                    Camara.SetCamera(new TGCVector3(Camara.Position.X, 0f, Camara.Position.Z), Camara.LookAt);
                }
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
            DrawText.drawText("Con clic izquierdo subimos la camara [Actual]: " + TGCVector3.PrintVector3(Camara.Position), 0, 30, Color.OrangeRed);

            terrain.Render();

            //Siempre antes de renderizar el modelo necesitamos actualizar la matriz de transformacion.
            //Debemos recordar el orden en cual debemos multiplicar las matrices, en caso de tener modelos jerárquicos, tenemos control total.
            Suelo.Transform = TGCMatrix.Scaling(Suelo.Scale) * TGCMatrix.RotationYawPitchRoll(Suelo.Rotation.Y, Suelo.Rotation.X, Suelo.Rotation.Z) * TGCMatrix.Translation(Suelo.Position);
            //A modo ejemplo realizamos toda las multiplicaciones, pero aquí solo nos hacia falta la traslación.
            //Finalmente invocamos al render de la caja
            Suelo.Render();

            //Cuando tenemos modelos mesh podemos utilizar un método que hace la matriz de transformación estándar.
            //Es útil cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones jerárquicas o complicadas.
            Bandicoot.UpdateMeshTransform();
            //Render del mesh
            Bandicoot.Render();

            //Render de BoundingBox, muy útil para debug de colisiones.
            if (BoundingBox)
            {
                Suelo.BoundingBox.Render();
                Bandicoot.BoundingBox.Render();
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
            //Dispose de la caja.
            Suelo.Dispose();
            //Dispose del mesh.
            Bandicoot.Dispose();

            terrain.Dispose();
        }
    }
}