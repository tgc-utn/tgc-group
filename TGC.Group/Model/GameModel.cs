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
using TGC.Group.Model.AI;


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
        private bool interaccion = false;


        private Directorio directorio;
        private TgcSkeletalMesh personaje;
        private TgcThirdPersonCamera camaraInterna;
        private Calculos calculo = new Calculos();

        private TGCVector3 velocidad = TGCVector3.Empty;
        private TGCVector3 aceleracion = TGCVector3.Empty;
        private float Ypiso = 20f;
        

        //Define direccion del mesh del personaje dependiendo el movimiento
        private DireccionPersonaje direccionPersonaje = new DireccionPersonaje();
        private Escenario escenario;
        private TgcMesh objetoMovibleG;


        private TgcBoundingSphere esferaPersonaje;
        private TGCVector3 scaleBoundingVector;
        private SphereCollisionManager ColisionadorEsferico;
        private TgcArrow directionArrow;
        private TGCVector3 movimientoRealPersonaje;
        private TGCVector3 movimientoRelativoPersonaje = TGCVector3.Empty;
        private TGCVector3 movimientoRealCaja = TGCVector3.Empty;
        private TgcBoundingSphere esferaCaja;
        TGCVector3 movimientoPorPlataforma = new TGCVector3(0, 0, 0);
        private bool colisionPlataforma = false;
        private List<Plataforma> plataformas;


        private bool boundingBoxActivate = false;

        private float jumping;
        private bool moving;

        private PisoInercia pisoResb = null; //Es null cuando no esta pisando ningun piso resbaloso

        private bool paused = false;
        private bool perdiste = false;

        private float offsetHeight = 400;
        private float offsetForward = -800;

        public override void Init()
        {
            perdiste = false;
            paused = false;
            pisoResb = null;
            direccionPersonaje = new DireccionPersonaje();
            velocidad = TGCVector3.Empty;
            aceleracion = TGCVector3.Empty;

            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;

            //Objeto que conoce todos los path de MediaDir
            directorio = new Directorio("..\\..\\Media\\");

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
            //Posicion inicial 2
            personaje.Position = new TGCVector3(-3630, Ypiso, -7600);
            //No es recomendado utilizar autotransform en casos mas complicados, se pierde el control.
            personaje.AutoTransform = true;
            
            
            //Rotar al robot en el Init para que mire hacia el otro lado
            personaje.RotateY(calculo.AnguloARadianes(180f, 1f));
            //Le cambiamos la textura para diferenciarlo un poco
            personaje.changeDiffuseMaps(new[]
            {
                TgcTexture.createTexture(D3DDevice.Instance.Device, directorio.RobotTextura)
            });


            esferaPersonaje = new TgcBoundingSphere(personaje.BoundingBox.calculateBoxCenter() - new TGCVector3(10f,50f,0f), personaje.BoundingBox.calculateBoxRadius()*0.4f);
            scaleBoundingVector = new TGCVector3(1.5f, 1f, 1.2f);
            


            //Crear linea para mostrar la direccion del movimiento del personaje
            directionArrow = new TgcArrow();
            directionArrow.BodyColor = Color.Red;
            directionArrow.HeadColor = Color.Green;
            directionArrow.Thickness = 1;
            directionArrow.HeadSize = new TGCVector2(10, 20);

            ColisionadorEsferico = new SphereCollisionManager();
            ColisionadorEsferico.GravityEnabled = true;

            plataformas = escenario.Plataformas();

            

            //Suelen utilizarse objetos que manejan el comportamiento de la camara.
            //Lo que en realidad necesitamos gráficamente es una matriz de View.
            //El framework maneja una cámara estática, pero debe ser inicializada.
            //Posición de la camara.

            camaraInterna = new TgcThirdPersonCamera(personaje.Position, 500, -900);
           
            //Configuro donde esta la posicion de la camara y hacia donde mira.
            Camara = camaraInterna;
           
            //Internamente el framework construye la matriz de view con estos dos vectores.
            //Luego en nuestro juego tendremos que crear una cámara que cambie la matriz de view con variables como movimientos o animaciones de escenas.
        }


        public override void Update()
        {
            PreUpdate();
            personaje.BoundingBox.scaleTranslate(personaje.Position, scaleBoundingVector);
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

            if (perdiste && Input.keyPressed(Key.Y))
            {
                Init();
            }

            if (Input.keyPressed(Key.P))
            {
                paused = !paused;
            }

            if (Input.keyPressed(Key.F))
            {
                boundingBoxActivate = !boundingBoxActivate;
            }

            if (personaje.Position.Y < -700)
            {
                perdiste = true;
            }

            if (!paused && !perdiste)
            {
                RotarPersonaje();

                if (Input.keyDown(Key.R)) interaccion = true;
                else interaccion = false;

                if (!interaccion) // Para que no se pueda saltar cuando agarras algun objeto
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


                var SlideVector = TGCVector3.Empty;

                foreach (TgcMesh mesh in escenario.ResbalososMesh())
                {
                    if (pisoResb == null)
                    {
                        pisoResb = new PisoInercia(mesh, 0.999f);
                    }

                    if (pisoResb.aCollisionFound(personaje))
                    {
                        var VectorSlideActual = pisoResb.VectorEntrada;
                        if (VectorSlideActual == TGCVector3.Empty)
                        {
                            pisoResb.VectorEntrada = movementVector;
                        }
                        else
                        {
                            SlideVector = VectorSlideActual;
                        }
                        break;
                    }
                    else
                    {
                        pisoResb = null;
                        //pisoResb.VectorEntrada = TGCVector3.Empty;
                    }
                }
                movimientoRelativoPersonaje = movementVector;

                ColisionadorEsferico.GravityEnabled = true;
                ColisionadorEsferico.GravityForce = new TGCVector3(0, -10, 0);
                ColisionadorEsferico.SlideFactor = 1.3f;


                moverMundo(movementVector + SlideVector);

               

               
                //Actualizar valores de la linea de movimiento
                directionArrow.PStart = esferaPersonaje.Center;
                directionArrow.PEnd = esferaPersonaje.Center + TGCVector3.Multiply(movementVector, 50);
                directionArrow.updateValues();


                //Ejecuta la animacion del personaje
                personaje.playAnimation(animacion, true);

                ajustarCamara();

                PostUpdate();


            }
        }
        TgcMesh objetoEscenario;
        private bool colEsc = false;
        public void moverMundo(TGCVector3 movementVector)
        {

             var box = obtenerColisionCajaPersonaje();
             if (box != null && box != objetoEscenario) objetoEscenario = box;
            //Mover personaje con detección de colisiones, sliding y gravedad
            colEsc = colisionEscenario();
            if (objetoEscenario != null) generarMovimiento(objetoEscenario, movementVector);

            movimientoRealPersonaje = ColisionadorEsferico.moveCharacter(esferaPersonaje, movementVector, escenario.MeshesColisionablesBB());

            foreach (Plataforma plataforma in plataformas) plataforma.Update();

            Plataforma plataformaColisionante = plataformas.Find(plataforma => plataforma.colisionaConPersonaje(esferaPersonaje));
            if (plataformaColisionante != null) colisionPlataforma = true;
            else colisionPlataforma = false;

            if (colisionPlataforma) movimientoPorPlataforma = plataformaColisionante.VectorMovimiento();
            else movimientoPorPlataforma = new TGCVector3(0, 0, 0);

            personaje.Move(movimientoRealPersonaje + movimientoPorPlataforma);

            TGCVector3 movimientoCentroEsfera = movimientoPorPlataforma;
            esferaPersonaje.moveCenter(movimientoCentroEsfera);
        }

      

        public void generarMovimiento(TgcMesh objetoMovible, TGCVector3 movementV)
        {
            if (objetoMovibleG == null || objetoMovibleG != objetoMovible) objetoMovibleG = objetoMovible;

            esferaCaja = new TgcBoundingSphere(objetoMovible.BoundingBox.calculateBoxCenter() + new TGCVector3(0f, 15f, 0f), objetoMovible.BoundingBox.calculateBoxRadius() * 0.7f);
            escenario.MeshesColisionables();
            movimientoRealCaja = ColisionadorEsferico.moveCharacter(esferaCaja, movementV, escenario.MeshesColisionablesBBSin(objetoMovible));

            var testCol = testColisionObjetoPersonaje(objetoMovible);

            if (interaccion && testCol)
            {
                if (!colEsc) objetoMovible.Move(movimientoRealCaja);
                else if (colisionConPilar() || testColisionObjetoPersonaje(objetoMovible)) movimientoRealCaja = TGCVector3.Empty;
                else objetoMovible.Move(-movimientoRealCaja);
                
            }
            else if (movimientoRealCaja.Y < 0) objetoMovible.Move(movimientoRealCaja);

        }

        public bool colisionEscenario()
        {
            return escenario.MeshesColisionables().FindAll(mesh => mesh.Layer != "CAJAS" && mesh.Layer != "PISOS").Find(mesh => TgcCollisionUtils.testAABBAABB(personaje.BoundingBox, mesh.BoundingBox)) != null;
        } 
        public TgcMesh obtenerColisionCajaPersonaje()
        {
            return escenario.CajasMesh().Find(caja => TgcCollisionUtils.testAABBAABB(caja.BoundingBox, personaje.BoundingBox) && caja != objetoMovibleG);
        }
        public bool colisionConPilar()
        {
            return escenario.PilaresMesh().Exists(mesh => TgcCollisionUtils.testAABBAABB(personaje.BoundingBox, mesh.BoundingBox));
        }
        
        public bool testColisionObjetoPersonaje(TgcMesh objetoColisionable)
        {
            return TgcCollisionUtils.testAABBAABB(personaje.BoundingBox, objetoColisionable.BoundingBox);
        }
      
        public bool testColisionCajasObjetos(TgcMesh box)
        {
           // return escenario.Paredes().Exists(pared =>colisionConCajaOrientada(box,pared,movementVector));
            return escenario.ObjetosColisionables().Exists(objeto => objeto != box && TgcCollisionUtils.testAABBAABB(box.BoundingBox, objeto.BoundingBox));
        }

        public void RotarPersonaje()
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
            personaje.RotateY(direccionPersonaje.RotationAngle(input));
        }
        public void RotateMesh(Key i1, Key i2)
        {
            moving = true;
            personaje.RotateY(direccionPersonaje.RotationAngle(i1,i2));
        }

        public void ajustarCamara()
        {
            //Actualizar valores de camara segun modifiers
            
            camaraInterna.TargetDisplacement = new TGCVector3(0, 50, 0);
            camaraInterna.OffsetHeight = offsetHeight;
            camaraInterna.OffsetForward = offsetForward;

            //Pedirle a la camara cual va a ser su proxima posicion
            TGCVector3 position;
            TGCVector3 target;
            camaraInterna.CalculatePositionTarget(out position, out target);

            //Detectar colisiones entre el segmento de recta camara-personaje y todos los objetos del escenario
            TGCVector3 q;
            var minDistSq = FastMath.Pow2(camaraInterna.OffsetForward);
            foreach (var obstaculo in escenario.ObstaculosColisionablesConCamara())
            {
                //Hay colision del segmento camara-personaje y el objeto
                if (TgcCollisionUtils.intersectSegmentAABB(target, position, obstaculo.BoundingBox, out q))
                {
                    //Si hay colision, guardar la que tenga menor distancia
                    var distSq = TGCVector3.Subtract(q, target).LengthSq();
                    //Hay dos casos singulares, puede que tengamos mas de una colision hay que quedarse con el menor offset.
                    //Si no dividimos la distancia por 2 se acerca mucho al target.

                    minDistSq = FastMath.Min(distSq / 2, minDistSq);
                }
            }
            //Acercar la camara hasta la minima distancia de colision encontrada (pero ponemos un umbral maximo de cercania)

            var newOffsetForward = -FastMath.Sqrt(minDistSq);

            if (FastMath.Abs(newOffsetForward) < 10)
            {
                newOffsetForward = 10;
            }

            camaraInterna.OffsetForward = newOffsetForward;

            //Asignar la ViewMatrix haciendo un LookAt desde la posicion final anterior al centro de la camara

            camaraInterna.CalculatePositionTarget(out position, out target);
            camaraInterna.SetCamera(position, target);

            //Hacer que la camara siga al personaje en su nueva posicion
            camaraInterna.Target = personaje.Position;
        }


        public override void Render()
        {
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
            PreRender();

            if (!perdiste)
            {
                

                DrawText.drawText("Posicion Actual: " + personaje.Position + "\n"
                           + "Vector Movimiento Real Personaje" + movimientoRealPersonaje + "\n"
                           + "Vector Movimiento Relativo Personaje" + movimientoRelativoPersonaje + "\n"
                           + "Vector Movimiento Real Caja" + movimientoRealCaja + "\n"
                           + "Interaccion Con Caja: " + interaccionConCaja + "\n"
                           + "Colision Plataforma: " + colisionPlataforma + "\n"
                           + "Movimiento por plataforma: " + movimientoPorPlataforma, 0, 30, Color.GhostWhite);

                DrawText.drawText((paused ? "EN PAUSA" : "") + "\n", 500, 500, Color.Red);

                escenario.RenderAll();
                if (!paused)
                {
                    personaje.animateAndRender(ElapsedTime);
                }
                else
                {
                    personaje.Render();
                }

                if (boundingBoxActivate)
                {

                    personaje.BoundingBox.Render();
                    esferaPersonaje.Render();
                    escenario.RenderizarBoundingBoxes();
                    directionArrow.Render();
                    if (esferaCaja != null)
                    {
                        esferaCaja.Render();

                    }

                }


            }
            else
            {
                DrawText.drawText("Perdiste" + "\n" + "¿Reiniciar? (Y)", 500, 500, Color.Red);
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
           
            
            personaje.Dispose();
            escenario.DisposeAll();
            
        }
    }
}