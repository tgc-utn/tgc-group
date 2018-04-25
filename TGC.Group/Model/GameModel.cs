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

        private bool interaccionConCaja = false;
       

        private Directorio directorio;
        /*private TGCVector3 vectorNormalPlanoColisionado;
        private TGCVector3 vectorNormalPlanoColisionado;*/
        private TgcSkeletalMesh personaje;
        private TgcThirdPersonCamera camaraInterna;
        private Calculos calculo = new Calculos();

        private TGCVector3 velocidad = TGCVector3.Empty;
        private TGCVector3 aceleracion = TGCVector3.Empty;
        private float Ypiso = 25f;
        

        //Define direccion del mesh del personaje dependiendo el movimiento
        private Personaje dirPers = new Personaje();
        private Escenario escenario;
        

        
        private List<TgcBoundingAxisAlignBox> objetosColisionables = new List<TgcBoundingAxisAlignBox>();
        //private TgcBoundingAxisAlignBox personajeBox;
        private TgcBoundingSphere esferaPersonaje;
        private TGCVector3 scaleBoundingVector;
        private SphereCollisionManager ColisionadorPersonaje;
        
        private TgcArrow directionArrow;
        private TgcMesh box;
        

        private bool BoundingBox = false;

        private float jumping;
        private bool moving;

       
        public override void Init()
        {
            
            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;

            //Objeto que conoce todos los path de MediaDir
            directorio = new Directorio(MediaDir);

            //Cagar escenario especifico para el juego.
            escenario = new Escenario(directorio.EscenaSelva);
            box = escenario.Cajas()[0];

            //Cargar personaje con animaciones
            var skeletalLoader = new TgcSkeletalLoader();
            var pathAnimacionesPersonaje = new[] { directorio.RobotCaminando, directorio.RobotParado, };
            personaje = skeletalLoader.
                        loadMeshAndAnimationsFromFile(directorio.RobotSkeletalMesh,
                                                      directorio.RobotDirectorio,
                                                      pathAnimacionesPersonaje);

            //Configurar animacion inicial
            personaje.playAnimation("Parado", true);
            //Posicion inicial 2
            personaje.Position = new TGCVector3(0,Ypiso + 20, -6000);
            //No es recomendado utilizar autotransform en casos mas complicados, se pierde el control.
            personaje.AutoTransform = true;
            
            
            //Rotar al robot en el Init para que mire hacia el otro lado
            personaje.RotateY(calculo.AnguloARadianes(180f, 1f));
            //Le cambiamos la textura para diferenciarlo un poco
            personaje.changeDiffuseMaps(new[]
            {
                TgcTexture.createTexture(D3DDevice.Instance.Device, directorio.RobotTextura)
            });


            esferaPersonaje = new TgcBoundingSphere(personaje.BoundingBox.calculateBoxCenter() - new TGCVector3(5f,50f,0f), personaje.BoundingBox.calculateBoxRadius()*0.4f);
            scaleBoundingVector = new TGCVector3(1.5f, 1f, 1.2f);


            objetosColisionables.Clear();
            foreach(var objetoColisionable in escenario.MeshesColisionables())
            {
                objetosColisionables.Add(objetoColisionable.BoundingBox);
            }

            //Crear linea para mostrar la direccion del movimiento del personaje
            directionArrow = new TgcArrow();
            directionArrow.BodyColor = Color.Red;
            directionArrow.HeadColor = Color.Green;
            directionArrow.Thickness = 1;
            directionArrow.HeadSize = new TGCVector2(10, 20);

            ColisionadorPersonaje = new SphereCollisionManager();
            ColisionadorPersonaje.GravityEnabled = true;
         
            


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
            personaje.BoundingBox.scaleTranslate(personaje.Position, scaleBoundingVector);
            //personaje.BoundingBox.transform(TGCMatrix.Scaling(new TGCVector3(100f, 2f, 2f)));

            //obtener velocidades de Modifiers
            var velocidadCaminar = 300f;
            var coeficienteSalto = 30f;
            float saltoRealizado = 0;
            var moveForward = 0f;

            moving = false;
            var animacion = "";

            while (ElapsedTime > 1)
            {
                ElapsedTime = ElapsedTime / 10;
            };

            if (Input.keyPressed(Key.F))
            {
                BoundingBox = !BoundingBox;
            }

            RotarMesh();

            if (!interaccionConCaja) // Para que no se pueda saltar cuando agarras algun objeto
            {
                if (Input.keyUp(Key.Space) && jumping < coeficienteSalto)
                {
                    jumping = coeficienteSalto;
                }
                if (Input.keyUp(Key.Space) || jumping > 0)
                {
                    jumping -= coeficienteSalto * ElapsedTime;
                    saltoRealizado = jumping;
                }
            }

            

            //Vector de movimiento
            var movementVector = TGCVector3.Empty;

            float movX = 0;
            float movY = saltoRealizado;
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

            ColisionadorPersonaje.GravityEnabled = true;
            ColisionadorPersonaje.GravityForce = new TGCVector3(0, -10, 0);
            ColisionadorPersonaje.SlideFactor = 1.3f;

            moverMundo(movementVector);
            
            

            //Ejecuta la animacion del personaje
            personaje.playAnimation(animacion, true);
            //Hacer que la camara siga al personaje en su nueva posicion
            camaraInterna.Target = personaje.Position;
            
            //Actualizar valores de la linea de movimiento
            directionArrow.PStart = esferaPersonaje.Center;
            directionArrow.PEnd = esferaPersonaje.Center + TGCVector3.Multiply(movementVector, 50);
            directionArrow.updateValues();

            PostUpdate();
        }

        public void moverMundo(TGCVector3 movementVector)
        {
            
            //Si se aprieta R y hay colision pongo el flag en true, tambien sirve para el salto
            if (Input.keyDown(Key.R) && TgcCollisionUtils.testAABBAABB(personaje.BoundingBox, box.BoundingBox)) interaccionConCaja = true;
            //Si se suelta la R cambio el flag
            if (Input.keyUp(Key.R)) interaccionConCaja = false;
            
           //Mover personaje con detección de colisiones, sliding y gravedad
            var movimientoRealPersonaje = ColisionadorPersonaje.moveCharacter(esferaPersonaje, movementVector, objetosColisionables);
            
            if (interaccionConCaja && !colisionObjetos(movementVector))box.Move(movimientoRealPersonaje);
            if (colisionObjetos(movementVector)) box.Move(-movimientoRealPersonaje);
            personaje.Move(movimientoRealPersonaje);
        }

        //La colision no esta orientada. La caja queda pegada a la pared una vez colisionada
        public bool colisionObjetos(TGCVector3 movementVector)
        {
            return escenario.Paredes().Exists(pared =>colisionOrientadaObjeto(pared,movementVector));
        }

        public bool colisionOrientadaObjeto(TgcMesh pared,TGCVector3 movementVector)
        {
            bool collisionFound = false;
            //Solo si existe colision entre la caja y la pared.
            if (TgcCollisionUtils.testAABBAABB(box.BoundingBox, pared.BoundingBox))
            {
                collisionFound = true;

                //Hay que comprobrar si se empuja a la caja en direccion a la pared.
                TgcBoundingAxisAlignBox.Face collisionFace = null;
                var faces = box.BoundingBox.computeFaces();
                var i = 0;

                foreach (var face in faces)
                {
                    if (!TgcCollisionUtils.testPlaneAABB(face.Plane, pared.BoundingBox))
                    {
                        collisionFace = face;
                        i++; //Debuguear cuantas caras no colisionadas encuentra
                    }
                    }

                var vectorNormalPlanoColisionado = TgcCollisionUtils.getPlaneNormal(TGCPlane.Normalize(collisionFace.Plane));
                Console.WriteLine(vectorNormalPlanoColisionado);

                Console.WriteLine(TGCVector3.Normalize(movementVector));


                var vectorResultante = TGCVector3.Cross(vectorNormalPlanoColisionado, movementVector);

                //No me importa el eje Y.
                vectorResultante.Y = 0;
                //Compruebo si el vector movimiento y el vector normal al plano colisionado son paralelos
                Console.WriteLine(vectorResultante);
                if (vectorResultante != TGCVector3.Empty)
                {
                    //Si son paralelos, los movimientos son en la misma direccion, entonces no deberian moverse.
                    collisionFound = false;
                    Console.WriteLine("No Colision");
                }
                
               
            Console.WriteLine(i);
            }
            return collisionFound;
        }

        

        public bool colisionOrientada(TGCVector3 movimientoRealCaja, TgcMesh pared)
        {
            TgcCollisionUtils.intersectSegmentAABB(movimientoRealCaja, box.Position, pared.BoundingBox, out TGCVector3 resultante);
            Console.WriteLine("Mov. Caja" + movimientoRealCaja);
            Console.WriteLine("Resultante" + resultante);
            return true;
        }

        public void RotarMesh()
        {
            //Adelante
            if (Input.keyDown(Key.W)) RotateMesh(Key.W);
            //Atras
            if (Input.keyDown(Key.S)) RotateMesh(Key.S);
            //Derecha
            if (Input.keyDown(Key.D)) RotateMesh(Key.D);
            //Izquierda
            if (Input.keyDown(Key.A)) RotateMesh(Key.A);
            //UpLeft
            if (Input.keyDown(Key.W) && Input.keyDown(Key.A)) RotateMesh(Key.W, Key.A);
            //UpRight
            if (Input.keyDown(Key.W) && Input.keyDown(Key.D)) RotateMesh(Key.W, Key.D);
            //DownLeft
            if (Input.keyDown(Key.S) && Input.keyDown(Key.A)) RotateMesh(Key.S, Key.A);
            //DownRight
            if (Input.keyDown(Key.S) && Input.keyDown(Key.D)) RotateMesh(Key.S, Key.D);
        }

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

       


        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aquí todo el código referido al renderizado.
        ///     Borrar todo lo que no haga falta.
        /// </summary>
        public override void Render()
        {
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
            PreRender();

            DrawText.drawText("Posicion Actual: " + personaje.Position + "\n Posicion Caja: " + box.Position, 0, 30, Color.GhostWhite);

            escenario.RenderAll();
            personaje.animateAndRender(ElapsedTime);

            if (BoundingBox)
            {
            
                personaje.BoundingBox.Render();
                esferaPersonaje.Render();
                escenario.RenderizarBoundingBoxes();
            }


            directionArrow.Render();


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