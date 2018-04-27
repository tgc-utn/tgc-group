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
        private bool interaccion = false;


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
        private TgcMesh objeto;


        private TgcBoundingSphere esferaPersonaje;
        private TGCVector3 scaleBoundingVector;
        private SphereCollisionManager ColisionadorEsferico;
        private TgcArrow directionArrow;
        private TGCVector3 movimientoRealPersonaje;
        private TGCVector3 movimientoRelativoPersonaje = TGCVector3.Empty;
        private TGCVector3 movimientoRealCaja = TGCVector3.Empty;
        private TgcBoundingSphere esferaCaja;
       


        private bool boundingBoxActivate = false;

        private float jumping;
        private bool moving;

       
        public override void Init()
        {
            
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
        /// 
        
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

            if (Input.keyPressed(Key.F))
            {
                boundingBoxActivate = !boundingBoxActivate;
            }

            RotarMesh();

            if (Input.keyDown(Key.R)) interaccion = true;
            if(Input.keyUp(Key.R)) interaccion = false;

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

            movimientoRelativoPersonaje = movementVector;

            ColisionadorEsferico.GravityEnabled = true;
            ColisionadorEsferico.GravityForce = new TGCVector3(0, -10, 0);
            ColisionadorEsferico.SlideFactor = 1.3f;

            
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
        TgcMesh objetoEscenario;
        private bool choqueEscenario = false;
        public void moverMundo(TGCVector3 movementVector)
        {

            var box = obtenerColisionCajaPersonaje();
            if (box != null && box != objeto) objeto = box;
            //Mover personaje con detección de colisiones, sliding y gravedad
            objetoEscenario = colisionEscenario();

            movimientoRealPersonaje = ColisionadorEsferico.moveCharacter(esferaPersonaje, movementVector, escenario.MeshesColisionablesBB());

            if (objeto != null)
            {
            
                esferaCaja = new TgcBoundingSphere(objeto.BoundingBox.calculateBoxCenter() + new TGCVector3(0f, 20f, 0f), objeto.BoundingBox.calculateBoxRadius() * 0.7f);
                
                movimientoRealCaja = ColisionadorEsferico.moveCharacter(esferaCaja, movementVector, escenario.MeshesColisionablesBBSin(objeto));


                if (objetoEscenario == null)
                {
                    if (interaccion && testColisionObjetoPersonaje(objeto)) objeto.Move(movimientoRealCaja);
                    choqueEscenario = true;
                }
               else if(choqueEscenario)
                {
                    objeto.Move(-movimientoRealCaja);
                    choqueEscenario = false;
                }


                if (!interaccion && movimientoRealCaja.Y < 0) objeto.Move(movimientoRealCaja);

            }
            // if (choqueEscenario) objeto.Move(-movimientoRealCaja);
            personaje.Move(movimientoRealPersonaje);
            
        }
        
        public TgcMesh colisionEscenario()
        {
            return escenario.MeshesColisionables().FindAll(mesh => mesh.Layer != "CAJAS" && mesh.Layer != "PISOS").Find(mesh => TgcCollisionUtils.testAABBAABB(personaje.BoundingBox, mesh.BoundingBox));
        } 
        public TgcMesh obtenerColisionCajaPersonaje()
        {
            return escenario.Cajas().Find(caja => TgcCollisionUtils.testAABBAABB(caja.BoundingBox, personaje.BoundingBox) && caja != objeto);
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

            DrawText.drawText("Posicion Actual: " + personaje.Position + "\n" 
                            + "Vector Movimiento Real Personaje" + movimientoRealPersonaje + "\n"
                            + "Vector Movimiento Relativo Personaje" + movimientoRelativoPersonaje + "\n"
                            + "Vector Movimiento Real Caja" + movimientoRealCaja + "\n"
                            + "Interaccion Con Caja: " + interaccionConCaja + "\n"
                            + "Objeto Escenario " + objetoEscenario, 0, 30, Color.GhostWhite);

            escenario.RenderAll();
            personaje.animateAndRender(ElapsedTime);

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