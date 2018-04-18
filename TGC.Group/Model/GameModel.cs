using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.SkeletalAnimation;
using TGC.Core.Collision;
using System;
using System.Collections.Generic;

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
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }
        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        /// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
        /// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
        private Directorio directorio;
        private TgcSkeletalMesh personaje;
        private TgcThirdPersonCamera camaraInterna;
        private Calculos calculo = new Calculos();
        private TGCVector3 velocidad = TGCVector3.Empty;
        private TGCVector3 aceleracion = new TGCVector3(0, -100f, 0);
        private float Ypiso = 25f;

       
        //Define direccion del mesh del personaje dependiendo el movimiento
        private Personaje dirPers = new Personaje();

        private float BoxMoveDirection = 1f;
        private float BoxMoveSpeed = 25f;

        private Escenario escenario;

        private TGCBox Box { get; set; }
        private TgcMesh piso { get; set; }


        

        
        private bool BoundingBox { get; set; }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aquí todo el código de inicialización: cargar modelos, texturas, estructuras de optimización, todo
        ///     procesamiento que podemos pre calcular para nuestro juego.
        ///     Borrar el codigo ejemplo no utilizado.
        /// </summary>
        public override void Init()
        {
            dirPers.CreateDictionary();
            
            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;

            //Objeto que conoce todos los path de MediaDir
            directorio = new Directorio(MediaDir);

            escenario = new Escenario(directorio.EscenaSelva);
            piso = escenario.Piso();

           

            //Textura de la carperta Media. Game.Default es un archivo de configuracion (Game.settings) util para poner cosas.
            //Pueden abrir el Game.settings que se ubica dentro de nuestro proyecto para configurar.
            var pathTexturaCaja = MediaDir + Game.Default.TexturaCaja;
            var texture = TgcTexture.createTexture(pathTexturaCaja);
            var size = new TGCVector3(100, 50, 100);
            Box = TGCBox.fromSize(size, texture);
            Box.Position = new TGCVector3(-200, Ypiso, 0);

          

            piso.BoundingBox.move(TGCVector3.Empty - piso.Position);


            var skeletalLoader = new TgcSkeletalLoader();
            personaje = skeletalLoader.loadMeshAndAnimationsFromFile(
                                                                    directorio.RobotSkeletalMesh,
                                                                    directorio.RobotDirectorio,
                                                                    new[]
                                                                    {
                                                                        directorio.RobotCaminando,
                                                                        directorio.RobotParado
                                                                    });

            //Configurar animacion inicial
            personaje.playAnimation("Parado", true);
            //Posicion inicial
            personaje.Position = new TGCVector3(0, Ypiso+10, 0);
            //No es recomendado utilizar autotransform en casos mas complicados, se pierde el control.
            personaje.AutoTransform = true;
            //Rotar al robot en el Init para que mire hacia el otro lado
            personaje.RotateY(3.1415f);
            //Le cambiamos la textura para diferenciarlo un poco
            personaje.changeDiffuseMaps(new[]
            {
                TgcTexture.createTexture(D3DDevice.Instance.Device, directorio.RobotTextura)
            });


            //Suelen utilizarse objetos que manejan el comportamiento de la camara.
            //Lo que en realidad necesitamos gráficamente es una matriz de View.
            //El framework maneja una cámara estática, pero debe ser inicializada.
            //Posición de la camara.

            camaraInterna = new TgcThirdPersonCamera(personaje.Position, 800, -2000);
            //Quiero que la camara mire hacia el origen (0,0,0).
            var lookAt = TGCVector3.Empty;
            //Configuro donde esta la posicion de la camara y hacia donde mira.
            Camara = camaraInterna;
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

            var velocidadCaminar = 300f;
            var velocidadCaminarID = 300f;
            var velocidadSalto = 200f;
            var coeficienteDeSalto = 0.2f;
            bool OnGround = false;
            bool onObject = false;


            //(Si el personaje aparece en cualquier lado descomentar esto)
            while (ElapsedTime > 1)
            {
                ElapsedTime = ElapsedTime / 10;
            };

            if (Input.keyPressed(Key.F))
            {
                BoundingBox = !BoundingBox;
            }

            var moveForward = 0f;
            var moveID = 0f;
            var moving = false;
            var movingID = false;

            //Adelante
            if (Input.keyDown(Key.W))
            {
                moveForward = -velocidadCaminar;
                moving = true;
                personaje.RotateY(dirPers.RotationAngle(Key.W));
                dirPers.turnForward();
            }

            //Atras
            if (Input.keyDown(Key.S))
            {
                moveForward = -velocidadCaminar;
                moving = true;
                personaje.RotateY(dirPers.RotationAngle(Key.S));
                dirPers.turnBackwards();
            }

            //Derecha
            if (Input.keyDown(Key.D))
            {
                moveID = velocidadCaminarID;
                movingID = true;
                personaje.RotateY(dirPers.RotationAngle(Key.D));
                dirPers.turnRight();
            }

            //Izquierda
            if (Input.keyDown(Key.A))
            {
                moveID = -velocidadCaminarID;
                movingID = true;
                personaje.RotateY(dirPers.RotationAngle(Key.A));
                dirPers.turnLeft();
            }

            var animation = "";

            //Movimiento adelante-atrás
            if (moving)
            {
                //Activar animacion de caminando
                animation = "Caminando";

                var originalPos = personaje.Position;


                personaje.MoveOrientedY(moveForward * ElapsedTime);
                var newPos = personaje.Position;
                //(Temporal) No se movia cuando estaba arria de la caja porque lo contaba como colision
                personaje.Position += new TGCVector3(0, 1, 0);

                if (aCollisionFound(personaje, Box))
                {
                    personaje.Position = originalPos;
                    animation = "Parado";
                }
                else personaje.Position = newPos;
            }
            else animation = "Parado";

            //Movimiento Izq-Der
            if (movingID)
            {
                //Activar animacion de caminando
                animation = "Caminando";

                var originalPos = personaje.Position;

                personaje.Move(new TGCVector3(moveID, 0f, 0f) * ElapsedTime);
                var newPos = personaje.Position;
                //(Temporal) No se movia cuando estaba arria de la caja porque lo contaba como colision
                personaje.Position += new TGCVector3(0, 1, 0);

                if (aCollisionFound(personaje, Box))
                {
                    personaje.Position = originalPos;
                    if (animation != "Caminando")
                    {
                        animation = "Parado";
                    }
                }
                else personaje.Position = newPos;
            }
            else {
                if (animation != "Caminando") animation = "Parado";
            }



            personaje.playAnimation(animation, true);

            var perPos = personaje.Position;
            var posOriginal = perPos;

            //if (posOriginal.Y > 0) //DESCOMENTAR ESTE IF SI QUERES SACAR LA COLISION CON EL PISO SIN CAER AL INFINITO
            //{
                velocidad = velocidad + ElapsedTime * aceleracion;
                perPos = perPos + velocidad * ElapsedTime;
                perPos.Y -= coeficienteDeSalto;
                personaje.Move(perPos - personaje.Position);

                if (aCollisionFound(personaje, Box))
                {
                    onObject = true;
                    personaje.Position = posOriginal;
                }

                if (aCollisionFound(personaje, piso))
                {
                    OnGround = true;
                    personaje.Position = posOriginal;
                }

                

                
           // }

            var boxPosition = Box.Position;

            if (boxPosition.Y > 150f || Box.Position.Y < -20f)
            {
                BoxMoveDirection *= -1;
            }
            boxPosition.Y += BoxMoveSpeed * BoxMoveDirection * ElapsedTime;
            Box.Position = boxPosition;

            if (onObject)
            {
                personaje.Move(0, BoxMoveSpeed * BoxMoveDirection * ElapsedTime, 0);
            }


            //Saltar
            if (Input.keyPressed(Key.Space))
            {
                if (OnGround || onObject)
                {
                    velocidad.Y = velocidadSalto;
                    OnGround = false;
                }
            }

            //Camara sigue al personaje
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

            

            //Dibuja un texto por pantalla
            DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.GhostWhite);
            DrawText.drawText("Con clic izquierdo subimos la camara [Actual]: " + TGCVector3.PrintVector3(Camara.Position), 0, 30, Color.GhostWhite);
           
            //Siempre antes de renderizar el modelo necesitamos actualizar la matriz de transformacion.
            //Debemos recordar el orden en cual debemos multiplicar las matrices, en caso de tener modelos jerárquicos, tenemos control total.
            Box.Transform = TGCMatrix.Scaling(Box.Scale) * TGCMatrix.RotationYawPitchRoll(Box.Rotation.Y, Box.Rotation.X, Box.Rotation.Z) * TGCMatrix.Translation(Box.Position);
            //A modo ejemplo realizamos toda las multiplicaciones, pero aquí solo nos hacia falta la traslación.
            //Finalmente invocamos al render de la caja
            Box.Render();
            //BoxPiso.Render();
            escenario.RenderAll();
            //Cuando tenemos modelos mesh podemos utilizar un método que hace la matriz de transformación estándar.
            //Es útil cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones jerárquicas o complicadas.
            // Mesh.UpdateMeshTransform();
            //Render del mesh
            //Mesh.Render();

            //Render de BoundingBox, muy útil para debug de colisiones.
            personaje.animateAndRender(ElapsedTime);
            if (BoundingBox)
            {
                Box.BoundingBox.Render();
                piso.BoundingBox.Render();
                
                personaje.BoundingBox.Render();

                escenario.RenderizarBoundingBoxes();
            }

            

            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        public bool aCollisionFound(TgcSkeletalMesh personaje, TGCBox aBox)
        {
            var collisionFound = false;

            //Ejecutar algoritmo de detección de colisiones
            var collisionResult = TgcCollisionUtils.classifyBoxBox(personaje.BoundingBox, aBox.BoundingBox);

            //Hubo colisión con un objeto. 
            if (collisionResult != TgcCollisionUtils.BoxBoxResult.Afuera)
            {
                collisionFound = true;
            }

            //Si hubo alguna colisión, entonces restaurar la posición original del mesh
            return collisionFound;
        }


        public bool aCollisionFound(TgcSkeletalMesh personaje, TgcMesh mesh)
        {
            var collisionFound = false;

            //Ejecutar algoritmo de detección de colisiones
            var collisionResult = TgcCollisionUtils.classifyBoxBox(personaje.BoundingBox, mesh.BoundingBox);

            //Hubo colisión con un objeto. 
            if (collisionResult != TgcCollisionUtils.BoxBoxResult.Afuera)
            {
                collisionFound = true;
            }

            //Si hubo alguna colisión, entonces restaurar la posición original del mesh
            return collisionFound;
        }

        /// <summary>
        ///     Se llama cuando termina la ejecución del ejemplo.
        ///     Hacer Dispose() de todos los objetos creados.
        ///     Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        /// </summary>
        public override void Dispose()
        {
           
            Box.Dispose();
            personaje.Dispose();
            escenario.DisposeAll();
            
        }
    }
}