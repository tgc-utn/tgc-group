using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.BoundingVolumes;
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
        
        private Directorio directorio;

        private TgcSkeletalMesh personaje;
        private TgcThirdPersonCamera camaraInterna;

        private Calculos calculo = new Calculos();

        private TGCVector3 velocidad = TGCVector3.Empty;
        private TGCVector3 aceleracion = new TGCVector3(0, -100f, 0);
        private float Ypiso = 25f;
        private float anguloMovido;

        bool OnGround = true;
        bool onObject = false;

        //Define direccion del mesh del personaje dependiendo el movimiento
        private Personaje dirPers = new Personaje();
        private Escenario escenario;

        private TGCBox Box { get; set; }
        private TgcMesh piso { get; set; }

        private readonly List<TgcMesh> objectsBehind = new List<TgcMesh>();
        private readonly List<TgcMesh> objectsInFront = new List<TgcMesh>();
        private List<TgcBoundingAxisAlignBox> objetosColisionables = new List<TgcBoundingAxisAlignBox>();
        private TgcBoundingSphere esferaPersonaje;
        

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

            //Objeto que conoce todos los path de MediaDir
            directorio = new Directorio(MediaDir);

            //Cagar escenario especifico para el juego.
            escenario = new Escenario(directorio.EscenaSelva);
            
            //Cargar personaje con animaciones
            var skeletalLoader = new TgcSkeletalLoader();
            var pathAnimacionesPersonaje = new[] { directorio.RobotCaminando, directorio.RobotParado, };
            personaje = skeletalLoader.
                        loadMeshAndAnimationsFromFile(directorio.RobotSkeletalMesh,
                                                      directorio.RobotDirectorio,
                                                      pathAnimacionesPersonaje);

            //Configurar animacion inicial
            personaje.playAnimation("Parado", true);
            //Posicion inicial
            personaje.Position = new TGCVector3(0, Ypiso + 10, 0);
            //No es recomendado utilizar autotransform en casos mas complicados, se pierde el control.
            personaje.AutoTransform = true;
            //Rotar al robot en el Init para que mire hacia el otro lado
            personaje.RotateY(calculo.AnguloARadianes(180f, 1f));
            //Le cambiamos la textura para diferenciarlo un poco
            personaje.changeDiffuseMaps(new[]
            {
                TgcTexture.createTexture(D3DDevice.Instance.Device, directorio.RobotTextura)
            });
            esferaPersonaje = new TgcBoundingSphere(personaje.BoundingBox.calculateBoxCenter(), personaje.BoundingBox.calculateBoxRadius());

            objetosColisionables.Clear();
            foreach(var objetoColisionable in escenario.MeshesColisionables())
            {
                objetosColisionables.Add(objetoColisionable.BoundingBox);
            }

         
            


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
            
            //(Si el personaje aparece en cualquier lado descomentar esto)
            while (ElapsedTime > 1)
            {
                ElapsedTime = ElapsedTime / 10;
            };

            if (Input.keyPressed(Key.F))
            {
                BoundingBox = !BoundingBox;
            }


            /*****************************MOVIMIENTOS********************************/

            var moveForward = 0f;
            var moveID = 0f;
            var moving = false;
            var movingID = false;

            //Saltar
            if (Input.keyPressed(Key.Space))
            {
                if (OnGround || onObject)
                {
                    velocidad.Y = velocidadSalto;
                    OnGround = false;
                    onObject = false;
                }
            }

            //Adelante
            if (Input.keyDown(Key.W))
            {
                moveForward = -velocidadCaminar;
                moving = true;
                anguloMovido = dirPers.RotationAngle(Key.W);
                personaje.RotateY(anguloMovido);
            }

            //Atras
            if (Input.keyDown(Key.S))
            {
                moveForward = -velocidadCaminar;
                moving = true;
                anguloMovido = dirPers.RotationAngle(Key.S);
                personaje.RotateY(anguloMovido);
            }

            //Derecha
            if (Input.keyDown(Key.D))
            {
                moveID = velocidadCaminarID;
                movingID = true;
                anguloMovido = dirPers.RotationAngle(Key.D);
                personaje.RotateY(anguloMovido);
            }

            //Izquierda
            if (Input.keyDown(Key.A))
            {
                moveID = -velocidadCaminarID;
                movingID = true;
                anguloMovido = dirPers.RotationAngle(Key.A);
                personaje.RotateY(anguloMovido);
            }

            var animation = "";

            //Diagonales
            //UpLeft
            if(Input.keyDown(Key.W) && Input.keyDown(Key.A))
            {
                moveID = -velocidadCaminarID / 2;
                movingID = true;
                moveForward = -velocidadCaminar / 2;
                moving = true;

                anguloMovido = dirPers.RotationAngle(Key.W, Key.A);
                personaje.RotateY(anguloMovido);

            }
            //UpRight
            if (Input.keyDown(Key.W) && Input.keyDown(Key.D))
            {
                moveID = velocidadCaminarID / 2;
                movingID = true;
                moveForward = -velocidadCaminar / 2;
                moving = true;

                anguloMovido = dirPers.RotationAngle(Key.W, Key.D);
                personaje.RotateY(anguloMovido);
            }
            //DownLeft
            if (Input.keyDown(Key.S) && Input.keyDown(Key.A))
            {
                moveID = -velocidadCaminarID / 2;
                movingID = true;
                moveForward = -velocidadCaminar / 2;
                moving = true;

                anguloMovido = dirPers.RotationAngle(Key.S, Key.A);
                personaje.RotateY(anguloMovido);
            }
            //DownRight
            if (Input.keyDown(Key.S) && Input.keyDown(Key.D))
            {
                moveID = velocidadCaminarID / 2;
                movingID = true;
                moveForward = -velocidadCaminar / 2;
                moving = true;

                anguloMovido = dirPers.RotationAngle(Key.S, Key.D);
                personaje.RotateY(anguloMovido);
            }

            //Ejecución de movimiento
            if (moving)
            {
                //Activar animacion de caminando
                animation = "Caminando";
            }
            else animation = "Parado";

            personaje.playAnimation(animation, true);
            /********************************EJECUCION MOVIMIENTOS************************************************************************/

            var movementVector = TGCVector3.Empty;
            if(moving)
            {
                movementVector = new TGCVector3(FastMath.Sin(personaje.Rotation.Y) * moveForward, jump,
                   FastMath.Cos(personaje.Rotation.Y) * moveForward);
            }


            //var realMovement;
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
            /*DrawText.drawText("OnGround: " + OnGround, 0, 45, Color.GhostWhite);
            DrawText.drawText("OnObject: " + onObject, 0, 60, Color.GhostWhite); para validar saltos correctos*/
            /* DrawText.drawText("Angulo actual: " + dirPers.angleDir.angulo, 0, 45, Color.GhostWhite);
             DrawText.drawText("Angulo actual rad: " + dirPers.angleDir.anguloRad, 0, 60, Color.GhostWhite);
             DrawText.drawText("Angulo movido rad: " + anguloMovido, 0, 75, Color.GhostWhite); para validar correcto movimiento*/

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