using Microsoft.DirectX.DirectInput;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
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
using TGC.Examples.Collision.SphereCollision;
using TGC.Group.Obstaculos;


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
        private TGCVector3 aceleracion = new TGCVector3(0, -100, 0);
        private float Ypiso = 25f;
        private float anguloMovido;
        private bool onGround = true;

        //Define direccion del mesh del personaje dependiendo el movimiento
        private Personaje dirPers = new Personaje();
        private Escenario escenario;

        private readonly List<TgcMesh> objectsBehind = new List<TgcMesh>();
        private readonly List<TgcMesh> objectsInFront = new List<TgcMesh>();
        private List<TgcBoundingAxisAlignBox> objetosColisionables = new List<TgcBoundingAxisAlignBox>();
        private TgcBoundingSphere esferaPersonaje;
        private SphereCollisionManager collisionManager;

        private bool BoundingBox { get; set; }

        private float jumping;
        private bool jump = false;
        private bool moving;


        //Hago 3 cajas para probar el sliding
        private TGCBox piso1;
        private PisoInercia piso2;
        private TGCBox piso3;

        public void RotateMesh(Key input)
        {
            moving = true;
            personaje.RotateY(dirPers.RotationAngle(input));
        }
        public void RotateMesh(Key i1, Key i2)
        {
            moving = true;
            personaje.RotateY(dirPers.RotationAngle(i1,i2));
        }
        
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
            personaje.Position = new TGCVector3(0, Ypiso + 50, 0);
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


            piso1 = TGCBox.fromSize(new TGCVector3(500, 50, 500),Color.Red);
            piso2 = PisoInercia.fromSize(new TGCVector3(500, 50, 500));
            piso3 = TGCBox.fromSize(new TGCVector3(500, 50, 500),Color.Green);
            piso2.AceleracionFrenada = 1f;

            //Tuve que poner los autoTransform porque no funcionan los boundingbox de estas cajas por ahora
            piso1.AutoTransform = true;
            piso1.Position = new TGCVector3(0, -25, 0);
            //piso1.Transform = TGCMatrix.Translation(0, -25, 0);// 
            //piso1.BoundingBox.scaleTranslate(piso1.Position,new TGCVector3(1,1,1));
            //piso1.updateValues();

            piso2.AutoTransform = true;
            piso2.Position = new TGCVector3(500, -25, 0);

            //piso2.Transform = TGCMatrix.Translation(500, -25, 0);  //
            //piso2.BoundingBox.scaleTranslate(piso1.Position,new TGCVector3(1,1,1));
            //piso2.updateValues();

            piso3.AutoTransform = true;
            piso3.Position = new TGCVector3(1000, -25, 0);
            //piso3.Transform = TGCMatrix.Translation(1000, -25, 0); //.Position = new TGCVector3(1700, -25, -100);
            //piso3.BoundingBox.move(TGCVector3.Empty - piso3.Position);
            //piso3.updateValues();

            objetosColisionables.Clear();
            //foreach(var objetoColisionable in escenario.MeshesColisionables())
            //{
            //    objetosColisionables.Add(objetoColisionable.BoundingBox);
            //}

            objetosColisionables.Add(piso1.BoundingBox);
            objetosColisionables.Add(piso2.BoundingBox);
            objetosColisionables.Add(piso3.BoundingBox);


        
            collisionManager = new SphereCollisionManager();
            collisionManager.GravityEnabled = true;



            //Suelen utilizarse objetos que manejan el comportamiento de la camara.
            //Lo que en realidad necesitamos gráficamente es una matriz de View.
            //El framework maneja una cámara estática, pero debe ser inicializada.
            //Posición de la camara.

            camaraInterna = new TgcThirdPersonCamera(personaje.Position, 500, -900);
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

            //obtener velocidades de Modifiers
            var velocidadCaminar = 300f;
           // var velocidadSalto = 1000f;

           
            //Calcular proxima posicion de personaje segun Input
            var moveForward = 0f;
            moving = false;
            //float jump = 0;

            while (ElapsedTime > 1)
            {
                ElapsedTime = ElapsedTime / 10;
            };

            if (Input.keyPressed(Key.F))
            {
                BoundingBox = !BoundingBox;
            }


            //Adelante
            if (Input.keyDown(Key.W)) RotateMesh(Key.W);
            //Atras
            if (Input.keyDown(Key.S)) RotateMesh(Key.S);
            //Derecha
            if (Input.keyDown(Key.D)) RotateMesh(Key.D);
            //Izquierda
            if (Input.keyDown(Key.A)) RotateMesh(Key.A);
            //UpLeft
            if (Input.keyDown(Key.W) && Input.keyDown(Key.A)) RotateMesh(Key.W,Key.A);
            //UpRight
            if (Input.keyDown(Key.W) && Input.keyDown(Key.D)) RotateMesh(Key.W, Key.D);
            //DownLeft
            if (Input.keyDown(Key.S) && Input.keyDown(Key.A)) RotateMesh(Key.S, Key.A);
            //DownRight
            if (Input.keyDown(Key.S) && Input.keyDown(Key.D)) RotateMesh(Key.S, Key.D);
            //TODO: JUMP
            if (Input.keyDown(Key.Space) && onGround)
            {
                onGround = false;
                velocidad.Y = 200f;
                //aceleracion.Y = -100f;
            }

            velocidad = velocidad + ElapsedTime * aceleracion;


            var animacion = "";

            //Vector de movimiento
            var movementVector = TGCVector3.Empty;
            

            float movX = 0;
            float movY =  velocidad.Y * ElapsedTime;
            float movZ = 0;
            if (moving)
            {
                animacion = "Caminando";
                moveForward = -velocidadCaminar;
                movX = FastMath.Sin(personaje.Rotation.Y) * moveForward * ElapsedTime;
                movZ = FastMath.Cos(personaje.Rotation.Y) * moveForward * ElapsedTime;
            }
            else animacion = "Parado";

            movementVector = new TGCVector3(movX, movY, movZ);

            var SlideVector = TGCVector3.Empty;

            if (piso2.aCollisionFound(personaje))
            {
                var VectorSlideActual = piso2.VectorEntrada;
                if (VectorSlideActual == TGCVector3.Empty)
                {
                    piso2.VectorEntrada = movementVector;
                }
                else
                {
                    SlideVector = VectorSlideActual;
                }
            }
            else
            {
                piso2.VectorEntrada = TGCVector3.Empty;
            }

            //Mover personaje con detección de colisiones, sliding y gravedad
            var realMovement = collisionManager.moveCharacter(esferaPersonaje, movementVector + SlideVector, objetosColisionables);

            if(collisionManager.onGround)
            {
                velocidad = TGCVector3.Empty;
                onGround = true;
            }

            personaje.Move(realMovement); // PONGO MOVEMENT VECTOR PARA QUE PRUEBES EL MOVIMIENTO, SI PONES REAL MOVEMENT QUEDA TRABADO PORQUE LA ESFERA COLISIONA CON TODO

            //if(personaje.Position.Y < 0)
            //{
            //    aceleracion = TGCVector3.Empty;
            //    personaje.Position = new TGCVector3(personaje.Position.X,0,personaje.Position.Z);
            //    velocidad = TGCVector3.Empty;
            //    onGround = true;
            //}

            //Ejecuta la animacion del personaje
            personaje.playAnimation(animacion, true);

            //Hacer que la camara siga al personaje en su nueva posicion
            camaraInterna.Target = personaje.Position;

            
            //Ver cual de las mallas se interponen en la visión de la cámara en 3ra persona.
            objectsBehind.Clear();
            objectsInFront.Clear();
            foreach (var mesh in escenario.MeshesColisionables())
            {
                TGCVector3 q;
                if (TgcCollisionUtils.intersectSegmentAABB(Camara.Position, camaraInterna.Target,
                    mesh.BoundingBox, out q))
                {
                    objectsBehind.Add(mesh);
                }
                else
                {
                    objectsInFront.Add(mesh);
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


            DrawText.drawText("Posicion Actual: " + personaje.Position, 0, 30, Color.GhostWhite);
            DrawText.drawText("Vector Velocidad Actual: " + velocidad, 0, 100, Color.GhostWhite);
            DrawText.drawText("Vector Aceleración Actual: " + aceleracion, 0, 150, Color.GhostWhite);
            DrawText.drawText("Colison con inercia: " + (piso2.aCollisionFound(personaje) ?  "Si":"No"), 0, 190, Color.GhostWhite);



            //escenario.RenderAll();

            //foreach (var mesh in objectsInFront)
            //{
            //    mesh.Render();
            //    if (BoundingBox)
            //    {
            //        mesh.BoundingBox.Render();
            //    }
            //}
            //foreach (var mesh in objectsBehind)
            //{
            //    mesh.BoundingBox.Render();
            //}

            piso1.Render();
            piso2.Render();
            piso3.Render();

            if (BoundingBox)
            {
                esferaPersonaje.Render();
                piso1.BoundingBox.Render();
                piso2.BoundingBox.Render();
                piso3.BoundingBox.Render();
            }


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
            escenario.DisposeAll();
            
        }
    }
}