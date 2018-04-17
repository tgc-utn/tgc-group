using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using System.Linq;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.SkeletalAnimation;
using TGC.Core.Textures;


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

        private Cam camaraInterna;

        float convertirARad(float angulo)
        {
            return angulo * 3.1415965f / 180f;
        }

        // Mesh del jugador.
        private TgcSkeletalMesh personaje;

        // Escena de la ciudad.
        private TgcScene scene;

        float piValue = 3.1415965f;
        float quarterPi = 3.1415965f / 4f;
        float halfPi = 3.1415965f / 2f;
        string movementOrientation = "Forward";

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

            var skeletalLoader = new TgcSkeletalLoader();
            personaje = skeletalLoader.loadMeshAndAnimationsFromFile(


                MediaDir + "Robot\\Robot-TgcSkeletalMesh.xml",
                MediaDir + "Robot\\",
                new[]
                {
                     MediaDir + "Robot\\Caminando-TgcSkeletalAnim.xml",
                     MediaDir + "Robot\\Parado-TgcSkeletalAnim.xml"
                });

            personaje.buildSkletonMesh();
            personaje.playAnimation("Parado", true);
            personaje.Scale = new TGCVector3(0.5f, 0.5f, 0.5f);
            personaje.Position = new TGCVector3(0, 10, 0);

            scene = new TgcSceneLoader().loadSceneFromFile(MediaDir + "Jungla\\Jungla -TgcScene.xml");

            camaraInterna = new Cam(personaje.Position, 220, 270);
            var lookAt = TGCVector3.Empty;
            Camara = camaraInterna;
        }


        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>

        public override void Update()
        {
            PreUpdate();

            var moving = false;

            TGCVector3 movement = new TGCVector3(0, 0, 0);

            /* if (Input.keyDown(Key.UpArrow) || Input.keyDown(Key.W))
             {
                 if (movementOrientation != "Forward")
                 {
                     if (movementOrientation == "Backwards")
                     {
                         personaje.RotateY(piValue);
                         movementOrientation = "Forward";
                         camaraInterna.rotateY(180f * ElapsedTime);

                     }

                     else if (movementOrientation == "Left")
                     {
                         personaje.RotateY(halfPi);
                         movementOrientation = "Forward";
                         camaraInterna.rotateY(90f * ElapsedTime);
                     }

                     else if (movementOrientation == "Right")
                     {
                         personaje.RotateY(-halfPi);
                         movementOrientation = "Forward";
                         camaraInterna.rotateY(-90f * ElapsedTime);
                     }
                 }

                 movement.Z -= 1;
                 moving = true;
             }

             else if (Input.keyDown(Key.DownArrow) || Input.keyDown(Key.S))
             {
                 if (movementOrientation != "Backwards")
                 {
                     if (movementOrientation == "Forward")
                     {
                         personaje.RotateY(piValue);
                         movementOrientation = "Backwards";
                         camaraInterna.rotateY(180f * ElapsedTime);
                     }

                     else if (movementOrientation == "Left")
                     {
                         personaje.RotateY(-halfPi);
                         movementOrientation = "Backwards";
                         camaraInterna.rotateY(-90f * ElapsedTime);
                     }

                     else if (movementOrientation == "Right")
                     {
                         personaje.RotateY(halfPi);
                         movementOrientation = "Backwards";
                         camaraInterna.rotateY(90f * ElapsedTime);
                     }
                 }

                 movement.Z += 1;
                 moving = true;
             }

             else if (Input.keyDown(Key.RightArrow) || Input.keyDown(Key.D))
             {
                 if (movementOrientation != "Right")
                 {
                     if (movementOrientation == "Backwards")
                     {
                         personaje.RotateY(-halfPi);
                         movementOrientation = "Right";
                         camaraInterna.rotateY(-90f * ElapsedTime);
                     }

                     else if (movementOrientation == "Left")
                     {
                         personaje.RotateY(-piValue);
                         movementOrientation = "Right";
                         camaraInterna.rotateY(180f * ElapsedTime);
                     }

                     else if (movementOrientation == "Forward")
                     {
                         personaje.RotateY(halfPi);
                         movementOrientation = "Right";
                         camaraInterna.rotateY(90f * ElapsedTime);
                     }
                 }

                 movement.X -= 1;
                 moving = true;
             }

             else if (Input.keyDown(Key.LeftArrow) || Input.keyDown(Key.A))
             {
                 if (movementOrientation != "Left")
                 {
                     if (movementOrientation == "Backwards")
                     {
                         personaje.RotateY(halfPi);
                         movementOrientation = "Left";
                         camaraInterna.rotateY(90f * ElapsedTime);
                     }

                     else if (movementOrientation == "Forward")
                     {
                         personaje.RotateY(-halfPi);
                         movementOrientation = "Left";
                         camaraInterna.rotateY(-90f * ElapsedTime);
                     }

                     else if (movementOrientation == "Right")
                     {
                         personaje.RotateY(piValue);
                         movementOrientation = "Left";
                         camaraInterna.rotateY(180f * ElapsedTime);
                     }
                 }

                 movement.X += 1;
                 moving = true;
             }

             if (moving)
             {

                 personaje.playAnimation("Caminando", true);
             }

             else
             {

                 personaje.playAnimation("Parado", true);
             }

             movement = movement * 220f * ElapsedTime;
             personaje.Move(movement);
     */
            var velocidadCaminar = 12000f*ElapsedTime;
            var velocidadRotacion = 180f;

            var moveForward = 0f;
            float rotate = 0;
            var rotating = false;

            if (Input.keyDown(Key.W))
            {
                moveForward = -velocidadCaminar;
                moving = true;
            }

            //Atras
            if (Input.keyDown(Key.S))
            {
                moveForward = velocidadCaminar;
                moving = true;
            }

            //Derecha
            if (Input.keyDown(Key.D))
            {
                rotate = velocidadRotacion;
                rotating = true;
            }

            //Izquierda
            if (Input.keyDown(Key.A))
            {
                rotate = -velocidadRotacion;
                rotating = true;
            }

            //Si hubo rotacion
            if (rotating)
            {
                //Rotar personaje y la camara, hay que multiplicarlo por el tiempo transcurrido para no atarse a la velocidad el hardware
                var rotAngle = convertirARad(rotate * ElapsedTime);
                personaje.RotateY(rotAngle);
                camaraInterna.rotateY(rotAngle);
            }

            //Si hubo desplazamiento
            if (moving)
            {
                //Activar animacion de caminando
                personaje.playAnimation("Caminando", true);
            }

            else
            {
                personaje.playAnimation("Parado", true);
            }
                //Aplicar movimiento hacia adelante o atras segun la orientacion actual del Mesh
                var lastPos = personaje.Position;

                personaje.MoveOrientedY(moveForward * ElapsedTime);

                camaraInterna.Target = personaje.Position;

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

            scene.RenderAll();

            //Cuando tenemos modelos mesh podemos utilizar un método que hace la matriz de transformación estándar.
            //Es útil cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones jerárquicas o complicadas.
            personaje.UpdateMeshTransform();
            //Render del mesh
            personaje.animateAndRender(ElapsedTime);

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
            personaje.Dispose();
            scene.DisposeAll();
        }
    }
}