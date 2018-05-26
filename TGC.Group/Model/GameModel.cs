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
using TGC.Group.SphereCollisionUtils;
using TGC.Group.Model.AI;

using System.Runtime.InteropServices;
using TGC.Group.GUI;

using TGC.Group.Optimizacion;
using TGC.Core.Shaders;


namespace TGC.Group.Model
{
    public class GameModel : TgcExample
    {
        public GameModel(string amediaDir, string shadersDir) : base(amediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
            mediaDir = amediaDir;
        }

        private bool interaccionCaja = false;

        static string mediaDir;

        

        private Directorio directorio;
        private TgcSkeletalMesh personaje;
        private TgcThirdPersonCamera camaraInterna;
        private Calculos calculo = new Calculos();

        private TGCVector3 velocidad = TGCVector3.Empty;
        private TGCVector3 aceleracion = TGCVector3.Empty;
        private float Ypiso = 20f;

        private Microsoft.DirectX.Direct3D.Effect effectLuzComun;
        private Microsoft.DirectX.Direct3D.Effect effectLuzLava;
        private Microsoft.DirectX.Direct3D.Effect personajeLightShader;

        //Define direccion del mesh del personaje dependiendo el movimiento
        private DireccionPersonaje direccionPersonaje = new DireccionPersonaje();
        private Escenario escenario;
        private TgcMesh objetoMovibleG;


        private TgcBoundingSphere esferaPersonaje;
        private TGCVector3 scaleBoundingVector;
        private SphereCollisionManager ColisionadorEsferico;
        private TGCVector3 movimientoRealPersonaje;
        private TGCVector3 movimientoRelativoPersonaje = TGCVector3.Empty;
        private TGCVector3 movimientoRealCaja = TGCVector3.Empty;
        private TgcBoundingSphere esferaCaja;
        //TGCVector3 movimientoPorPlataforma = new TGCVector3(0, 0, 0);
        private bool colisionPlataforma = false;
        private List<Plataforma> plataformas;
        private List<PlataformaRotante> plataformasRotantes;


        private bool boundingBoxActivate = false;

        private float saltoActual = 0;
        private bool moving;

        private PisoInercia pisoResbaloso = null; //Es null cuando no esta pisando ningun piso resbaloso

        private bool paused = true;
        private bool perdiste = false;
        private bool menu = true;

        private float offsetHeight = 400;
        private float offsetForward = -800;
        private float tiempoAcumulado;




        private DXGui gui = new DXGui();

        // Defines
        public const int IDOK = 0;

        public const int IDCANCEL = 1;
        public const int ID_JUGAR = 100;
        public const int ID_CONFIGURAR = 103;
        public const int ID_APP_EXIT = 105;
        public const int ID_PROGRESS1 = 107;
        public const int ID_RESET_CAMARA = 108;

        private Color[] lst_colores = new Color[12];
        private int cant_colores = 12;

        public TGC.Group.GUI.gui_progress_bar progress_bar;
        public bool msg_box_app_exit = false;
        public bool profiling = false;

        //private Microsoft.DirectX.Direct3D.Effect effect;
        public struct POINTAPI
        {
            public Int32 x;
            public Int32 y;
        }

        public enum PeekMessageOption
        {
            PM_NOREMOVE = 0,
            PM_REMOVE
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PeekMessage(ref MSG lpMsg, Int32 hwnd, Int32 wMsgFilterMin, Int32 wMsgFilterMax, PeekMessageOption wRemoveMsg);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool TranslateMessage(ref MSG lpMsg);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern Int32 DispatchMessage(ref MSG lpMsg);

        public const Int32 WM_QUIT = 0x12;

        public struct MSG
        {
            public Int32 hwmd;
            public Int32 message;
            public Int32 wParam;
            public Int32 lParam;
            public Int32 time;
            public POINTAPI pt;
        }

        private Octree octree;


        public static SoundManager soundManager;

        public override void Init()
        {
            perdiste = false;
            paused = false;
            pisoResbaloso = null;
            direccionPersonaje = new DireccionPersonaje();
            velocidad =new TGCVector3(0,0,0);
            aceleracion = new TGCVector3(0,0,0);

            

            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;

            //Objeto que conoce todos los path de MediaDir
            directorio = new Directorio(MediaDir);

            //Cargo el SoundManager
            soundManager = new SoundManager(directorio,this.DirectSound.DsDevice);
            soundManager.playSonidoFondo();

            //Cagar escenario especifico para el juego.
            escenario = new Escenario(directorio.EscenaCrash);

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
            personaje.Position = new TGCVector3(400, Ypiso, -900);
           // personaje.Position = new TGCVector3(-4133.616f, 20f, 5000f);
            //No es recomendado utilizar autotransform en casos mas complicados, se pierde el control.
            personaje.AutoTransform = false;
            
            
            //Rotar al robot en el Init para que mire hacia el otro lado
            personaje.RotateY(FastMath.ToRad(180f));

            //Le cambiamos la textura para diferenciarlo un poco
            personaje.changeDiffuseMaps(new[]
            {
                TgcTexture.createTexture(D3DDevice.Instance.Device, directorio.RobotTextura)
            });

            //Para desplazar un poco el centro de la esfera.
            TGCVector3 vectorAjuste = new TGCVector3(0f, 50f, 0f);
            //Para reducir el radio de la esfera.
            float coeficienteReductivo = 0.4f;
            esferaPersonaje = new TgcBoundingSphere(personaje.BoundingBox.calculateBoxCenter()-vectorAjuste, 
                                                    personaje.BoundingBox.calculateBoxRadius()*coeficienteReductivo);
            scaleBoundingVector = new TGCVector3(1.5f, 1f, 1.2f);
            


            //Inicializamos el collisionManager.
            ColisionadorEsferico = new SphereCollisionManager();
            ColisionadorEsferico.GravityEnabled = true;

            //Obtenemos las plataformas segun su tipo de movimiento.
            plataformas = escenario.Plataformas();
            plataformasRotantes = escenario.PlataformasRotantes();

           //Posición de la camara.
            camaraInterna = new TgcThirdPersonCamera(personaje.Position, 500, -1000);
           
            //Configuro donde esta la posicion de la camara y hacia donde mira.
            Camara = camaraInterna;

            personaje.BoundingBox.scaleTranslate(personaje.Position, scaleBoundingVector);
            var meshesSinPlatXZ = escenario.scene.Meshes.FindAll(mesh => mesh.Name != "PlataformaX" && mesh.Name != "PlataformaZ");

            octree = new Octree();
            octree.create(meshesSinPlatXZ, escenario.BoundingBox());
            octree.createDebugOctreeMeshes();// --> Para renderizar las "cajas" que genera


            inicializarGUI();

           

            Frustum.Color = Color.Black;

            effectLuzComun = TgcShaders.Instance.TgcMeshPhongShader;
            effectLuzLava = effectLuzComun.Clone(effectLuzComun.Device);
            foreach (TgcMesh mesh in escenario.MeshesColisionables())
            {
                Microsoft.DirectX.Direct3D.Effect defaultEffect = mesh.Effect;

                TgcMesh luz = escenario.getClosestLight(mesh.BoundingBox.calculateBoxCenter(), 2500f);

                if(luz == null)
                {
                    mesh.Effect = defaultEffect;
                }
                else
                {
                    if(luz.Layer == "Luces")
                    {
                        mesh.Effect = effectLuzComun;
                        mesh.Technique = TgcShaders.Instance.getTgcMeshTechnique(mesh.RenderType);
                        mesh.Effect.SetValue("lightPosition", TGCVector3.Vector3ToFloat4Array(luz.Position));
                        mesh.Effect.SetValue("eyePosition", TGCVector3.Vector3ToFloat4Array(Camara.Position));
                        mesh.Effect.SetValue("ambientColor", ColorValue.FromColor(Color.FromArgb(50, 50, 50)));
                        mesh.Effect.SetValue("diffuseColor", ColorValue.FromColor(Color.White));
                        mesh.Effect.SetValue("specularColor", ColorValue.FromColor(Color.DimGray));
                        mesh.Effect.SetValue("specularExp", 500f);
                    }
                    else
                    {
                        mesh.Effect = effectLuzLava;
                        mesh.Technique = TgcShaders.Instance.getTgcMeshTechnique(mesh.RenderType);
                        mesh.Effect.SetValue("lightPosition", TGCVector3.Vector3ToFloat4Array(luz.Position));
                        mesh.Effect.SetValue("eyePosition", TGCVector3.Vector3ToFloat4Array(Camara.Position));
                        mesh.Effect.SetValue("ambientColor", ColorValue.FromColor(Color.Red));
                        mesh.Effect.SetValue("diffuseColor", ColorValue.FromColor(Color.Red));
                        mesh.Effect.SetValue("specularColor", ColorValue.FromColor(Color.Orange));
                        mesh.Effect.SetValue("specularExp", 1000f);
                    }
                }

                //mesh.Technique = "RenderScene2";
            }
            personajeLightShader = TgcShaders.Instance.TgcSkeletalMeshPointLightShader;
            personaje.Effect = personajeLightShader;
            personaje.Technique = TgcShaders.Instance.getTgcSkeletalMeshTechnique(personaje.RenderType);

        }


        public override void Update()
        {
            PreUpdate();

            //TODO: Reificar estos valores.
            //Obtenemos los valores default
            var velocidadCaminar = 1000f;
            var coeficienteSalto = 30f;
            float saltoRealizado = 0;
            var moveForward = 0f;
            moving = false;
            var animacion = "";

            while (ElapsedTime > 1) ElapsedTime = ElapsedTime / 10;
            tiempoAcumulado += ElapsedTime;
            
            //Corroboramos si el jugador perdio la partida.
            if (perdiste && Input.keyPressed(Key.Y)) Init();
            
            //Pausa
            if (Input.keyPressed(Key.P)) paused = !paused;

            //Menu
            if (Input.keyPressed(Key.M))
            {
                menu = true;
                paused = true;
            }
             
            //Bounding Box activos.
            if (Input.keyPressed(Key.F))boundingBoxActivate = !boundingBoxActivate;
            
            //Si el personaje se mantiene en caida, se pierda la partida.
            if (personaje.Position.Y < -700)perdiste = true;
            

            //Si se sigue en juego, se continua con la logica del juego.
            if (!paused && !perdiste)
            {
                //MOVIMIENTOS BASICOS
                RotarPersonaje();

                if (Input.keyDown(Key.R)) interaccionCaja = true;
                else interaccionCaja = false;

                // Para que no se pueda saltar cuando agarras algun objeto
                //TODO: No debe saltar cuando ya esta saltando
                if (!interaccionCaja)
                {
                    if (Input.keyUp(Key.Space) && saltoActual < coeficienteSalto)
                    {
                        saltoActual = coeficienteSalto;
                        soundManager.playSonidoSaltar();
                    }
                    if (Input.keyUp(Key.Space) || saltoActual > 0 )
                    {
                        saltoActual -= coeficienteSalto * ElapsedTime;
                        saltoRealizado = saltoActual;
                    }
                   
                }

                //Vector de movimiento

                var movimientoOriginal = new TGCVector3(0,0,0);
                float movX = 0;
                float movY = saltoRealizado;
                float movZ = 0;

                if (moving)
                {
                    animacion = "Caminando";
                    moveForward = -velocidadCaminar;
                    movX = FastMath.Sin(personaje.Rotation.Y) * moveForward * ElapsedTime;
                    movZ = FastMath.Cos(personaje.Rotation.Y) * moveForward * ElapsedTime;
                    soundManager.playSonidoCaminar();
                }
                else
                {
                    animacion = "Parado";
                    soundManager.stopSonidoCaminar();
                }
                movementVector = new TGCVector3(movX, movY, movZ);

                //MOVIMIENTOS POR PISO
                var vectorSlide = new TGCVector3(0, 0, 0);

                movimientoOriginal = new TGCVector3(movX, movY, movZ);


                
                ColisionadorEsferico.GravityEnabled = true;
                ColisionadorEsferico.GravityForce = new TGCVector3(0, -10, 0);
                ColisionadorEsferico.SlideFactor = 1.3f;

                //MOVIMIENTOS POR PISO
                moverMundo(movimientoOriginal);

                //Ejecuta la animacion del personaje
                personaje.playAnimation(animacion, true);

                //Reajustamos la camara
                ajustarCamara();

                //Esto soluciona el Autrotransform = false
                personaje.UpdateMeshTransform();
                //Actualizo posición del Frustum
                Frustum.updateMesh(camaraInterna.Position + traslacionFrustum, camaraInterna.LookAt);
                PostUpdate();
            }
        }
        private TGCVector3 traslacionFrustum = new TGCVector3(0f, -0, -2800f);

        public TGCVector3 MovimientoPorSliding(TGCVector3 movimientoOriginal)
        {
            var vectorSlide = new TGCVector3(0, 0, 0);
            foreach (TgcMesh mesh in escenario.ResbalososMesh())
            {
                if (pisoResbaloso == null)
                {
                    pisoResbaloso = new PisoInercia(mesh, 0.999f);
                }

                if (pisoResbaloso.aCollisionFound(personaje))
                {
                    var VectorSlideActual = pisoResbaloso.VectorEntrada;
                    var versorMovimientoOriginal = movimientoOriginal * (1 / TGCVector3.Length(movimientoOriginal));

                    if (VectorSlideActual == TGCVector3.Empty || ((versorMovimientoOriginal != pisoResbaloso.VersorEntrada) && TGCVector3.Length(movimientoOriginal) > 0))
                    {
                        pisoResbaloso.VectorEntrada = movimientoOriginal;
                    }
                    else
                    {
                        vectorSlide = VectorSlideActual;
                    }
                    break;
                }
                else
                {
                    pisoResbaloso = null;
                    //pisoResb.VectorEntrada = TGCVector3.Empty;
                }
            }
            return vectorSlide;
        }
        //Objeto Movible del escenario, utilizado para mantener la referencia a una caja cuando cae
        TgcMesh objetoEscenario;
        SphereOBBCollider colliderOBB = new SphereOBBCollider();
        public void moverMundo(TGCVector3 movimientoOriginal)
        {
            
            //Actualizo el vector de movimiento del personaje segun el piso resbaloso
            movimientoOriginal += MovimientoPorSliding(movimientoOriginal);

            //Busca la caja con la cual se esta colisionando
            var cajaColisionante = obtenerColisionCajaPersonaje();
            //Si es una caja nueva updatea la referencia global
            if (cajaColisionante != null && cajaColisionante != objetoEscenario) objetoEscenario = cajaColisionante;

            if (objetoEscenario != null) generarMovimiento(objetoEscenario, movimientoOriginal);

            movimientoDePlataformas();
            //Actualizo el vector de movimiento del personaje segun la plataforma colisionante
            movimientoOriginal += movimientoPorPlataformas();

            //Busca una plataforma rotante con la que se este colisionando
            //NOTA: para estas plataformas se colisiona Esfera -> OBB y no Esfera -> AABB como las demás colisiones
            var plataformaRotante = plataformasRotantes.Find(plat => colliderOBB.colisionaEsferaOBB(esferaPersonaje,plat.OBB));
            //Si colisiona con una maneja la colision para las rotantes sino usa el metodo general
            if (plataformaRotante != null) movimientoRealPersonaje = colliderOBB.manageColisionEsferaOBB(esferaPersonaje, movimientoOriginal,plataformaRotante.OBB);
            else movimientoRealPersonaje = ColisionadorEsferico.moveCharacter(esferaPersonaje, movimientoOriginal, escenario.MeshesColisionablesBB());
             
            personaje.Move(movimientoRealPersonaje);
        }
        public void movimientoDePlataformas()
        {
            foreach (Plataforma plataforma in plataformas) plataforma.Update(tiempoAcumulado);
        }
        public TGCVector3 movimientoPorPlataformas()
        {

            Plataforma plataformaColisionante = plataformas.Find(plataforma => plataforma.colisionaConPersonaje(esferaPersonaje));
            if (plataformaColisionante != null) colisionPlataforma = true;
            else colisionPlataforma = false;

            if (colisionPlataforma) return plataformaColisionante.VectorMovimiento();
            else return TGCVector3.Empty;
        }
        public void generarMovimiento(TgcMesh objetoMovible, TGCVector3 movementV)
        {
            if (objetoMovibleG == null || objetoMovibleG != objetoMovible) objetoMovibleG = objetoMovible;

            esferaCaja = new TgcBoundingSphere(objetoMovible.BoundingBox.calculateBoxCenter() + new TGCVector3(0f, 15f, 0f), objetoMovible.BoundingBox.calculateBoxRadius() * 0.7f);

            movimientoRealCaja = ColisionadorEsferico.moveCharacter(esferaCaja, movementV, escenario.MeshesColisionablesBBSin(objetoMovible));

            var testCol = testColisionObjetoPersonaje(objetoMovible);

            if (interaccionCaja && testCol)
            {
                if (!colisionEscenario()) objetoMovible.Move(movimientoRealCaja);
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
            return escenario.ObjetosColisionablesConCajas().Exists(objeto => objeto != box && TgcCollisionUtils.testAABBAABB(box.BoundingBox, objeto.BoundingBox));
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
            PreRender();


            if(menu)gui_render(ElapsedTime);
            else
            
            {
                 Frustum.render();
                if (!perdiste)
                {


                    DrawText.drawText("Posicion Actual: " + personaje.Position + "\n"
                               + "Vector Movimiento Real Personaje" + movimientoRealPersonaje + "\n"
                               /*+ "Vector Movimiento Relativo Personaje" + movimientoRelativoPersonaje + "\n"
                               + "Vector Movimiento Real Caja" + movimientoRealCaja + "\n"
                               + "Interaccion Con Caja: " + interaccionConCaja + "\n"*/
                               + "Colision Plataforma: " + colisionPlataforma + "\n"
                               /*+ "Movimiento por plataforma: " + movimientoPorPlataforma*/, 0, 30, Color.GhostWhite);

                    DrawText.drawText((paused ? "EN PAUSA" : "") + "\n", 500, 500, Color.Red);

                    escenario.RenderAll();

                    //Renderizo OBB de las plataformas rotantes
                    plataformasRotantes.ForEach(plat => plat.Render(tiempoAcumulado));


                    if (!paused)
                    {
                        octree.render(Frustum, boundingBoxActivate);
                        renderizarRestantes();
                        personaje.animateAndRender(ElapsedTime);
                    }
                    else
                    {
                        DrawText.drawText("Perdiste" + "\n" + "¿Reiniciar? (Y)", 500, 500, Color.Red);
                        personaje.Render();
                    }

                    if (boundingBoxActivate)
                    {

                        personaje.BoundingBox.Render();
                        esferaPersonaje.Render();
                        escenario.RenderizarBoundingBoxes();
                    }
                                    
                    personaje.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
                    personaje.Effect.SetValue("lightPosition", TGCVector3.Vector3ToFloat4Array(escenario.getClosestLight(personaje.Position,0f).Position));
                    personaje.Effect.SetValue("eyePosition", TGCVector3.Vector3ToFloat4Array(Camara.Position));

                    personaje.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.White));
                    personaje.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.FromArgb(50, 50, 50)));
                    personaje.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                    personaje.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.DimGray));
                    personaje.Effect.SetValue("materialSpecularExp", 500f);

                    personaje.Effect.SetValue("lightIntensity", 20);
                    personaje.Effect.SetValue("lightAttenuation", 25);
                    
              }
            }

            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        public void inicializarGUI()
        {
            // levanto el GUI
            gui.Create(MediaDir);

            //soundManager.playSonidoFondo();

            // menu principal
            gui.InitDialog(true);
            int W = D3DDevice.Instance.Width;
            int H = D3DDevice.Instance.Height;
            int x0 = 70;
            int y0 = 10;
            int dy = 120;
            int dy2 = dy;
            int dx = 400;
            int item_epsilon = 50;
            gui.InsertImage("menu.png",1850,450, directorio.Menu);
            
            gui.InsertMenuItem(ID_JUGAR, "Jugar", "open.png", x0, y0, MediaDir, dx, dy);
            gui.InsertMenuItem(ID_CONFIGURAR, "Configurar", "navegar.png", x0+dx+item_epsilon, y0 , MediaDir, dx, dy);
            gui.InsertMenuItem(ID_APP_EXIT, "Salir", "salir.png", x0, y0 += dy2, MediaDir, dx, dy);

            // lista de colores
            lst_colores[0] = Color.FromArgb(100, 220, 255);
            lst_colores[1] = Color.FromArgb(100, 255, 220);
            lst_colores[2] = Color.FromArgb(220, 100, 255);
            lst_colores[3] = Color.FromArgb(220, 255, 100);
            lst_colores[4] = Color.FromArgb(255, 100, 220);
            lst_colores[5] = Color.FromArgb(255, 220, 100);
            lst_colores[6] = Color.FromArgb(128, 128, 128);
            lst_colores[7] = Color.FromArgb(64, 255, 64);
            lst_colores[8] = Color.FromArgb(64, 64, 255);
            lst_colores[9] = Color.FromArgb(255, 0, 255);
            lst_colores[10] = Color.FromArgb(255, 255, 0);
            lst_colores[11] = Color.FromArgb(0, 255, 255);
        }

        public void gui_render(float elapsedTime)
        {
            // ------------------------------------------------
            GuiMessage msg = gui.Update(elapsedTime, Input);


            // proceso el msg
            switch (msg.message)
            {
                case MessageType.WM_COMMAND:
                    switch (msg.id)
                    {
                        case IDOK:

                        case IDCANCEL:
                            // Resultados OK, y CANCEL del ultimo messagebox
                            gui.EndDialog();
                            profiling = false;
                            if (msg_box_app_exit)
                            {
                                // Es la resupuesta a un messagebox de salir del sistema
                                if (msg.id == IDOK)
                                {
                                    // Salgo del sistema
                                    System.Windows.Forms.Application.Exit();
                                }
                            }
                            msg_box_app_exit = false;
                            break;

                        case ID_JUGAR:
                            menu=false;
                            paused = false;
                            break;

                        /*case ID_CONFIGURAR:
                            Configurar();
                            break;*/

                        case ID_APP_EXIT:
                            gui.MessageBox("Desea Salir del Juego?",directorio.Menu, "Crash Bandicoot");
                            msg_box_app_exit = true;
                            break;

                        default:
                            if (msg.id >= 0 && msg.id < cant_colores)
                            {
                                // Cambio el color
                                int color = msg.id;

                               // effect.SetValue("color_global", new TGCVector4((float)lst_colores[color].R / 255.0f, (float)lst_colores[color].G / 255.0f, (float)lst_colores[color].B / 255.0f, 1));
                            }
                            break;
                    }
                    break;

                default:
                    break;
            }
            gui.Render();
        }




        /// <summary>
        ///     Se llama cuando termina la ejecución del ejemplo.
        ///     Hacer Dispose() de todos los objetos creados.
        ///     Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        /// </summary>

        private void renderizarRestantes() => plataformas.ForEach(plat => { if (plat.plataformaMesh.Name == "PlataformaX" || plat.plataformaMesh.Name == "PlataformaZ") plat.plataformaMesh.Render(); });


        public override void Dispose()
        {
            personaje.Dispose();
            escenario.DisposeAll();  
        }
    }


}