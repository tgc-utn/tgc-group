using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

using TGC.Core.BoundingVolumes;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.SkeletalAnimation;
using TGC.Core.Collision;
using TGC.Core.Shaders;
using TGC.Core.Text;
using TGC.Core.Particle;

using TGC.Group.SphereCollisionUtils;
using TGC.Group.Modelo.Rampas;
using TGC.Group.Modelo;
using TGC.Group.Modelo.Plataformas;
using TGC.Group.Modelo.Cajas;
using TGC.Group.GUI;
using TGC.Group.Optimizacion;
using TGC.Group.Sprites;


namespace TGC.Group.Modelo
{
    public class Juego : TgcExample
    {
        public Juego(string amediaDir, string shadersDir) : base(amediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
            mediaDir = amediaDir;
        }

        static string mediaDir;
        private Directorio directorio;
        private Informador informador;
        private Escenario escenario;
        private EstadoJuego estadoJuego;

        public static Octree octree;
        public static SoundManager soundManager;

        //Define direccion del mesh del personaje dependiendo el movimiento
        private Caja objetoMovibleGlobal = null;
        private bool solicitudInteraccionConCaja = false;
        private bool colisionPlataforma = false;

        private bool interaccionCaja;

        private float tiempoAcumulado;

        private SphereCollisionManager ColisionadorEsferico;
        private TgcBoundingSphere esferaCaja;
        //Objeto Movible del escenario, utilizado para mantener la referencia a una caja cuando cae
        Caja objetoEscenario;
        SphereOBBCollider colliderOBB = new SphereOBBCollider();

        private List<Plataforma> plataformas;
        private List<PlataformaRotante> plataformasRotantes;
        private bool boundingBoxActivate = false;



        public static List<TgcMesh> meshesConLuz;
        private Microsoft.DirectX.Direct3D.Effect effectLuzComun;
        private Microsoft.DirectX.Direct3D.Effect effectLuzLava;
        private Microsoft.DirectX.Direct3D.Effect personajeLightShader;

        private Microsoft.DirectX.Direct3D.Effect olasLava;


        #region Personaje
        private Personaje personaje;
        private DireccionPersonaje direccionPersonaje = new DireccionPersonaje();
        private int doubleJump = 0;
        private float saltoActual = 0;
        //private TGCVector3 ultimoCheckpoint = new TGCVector3(0f, 0.1f, 0f);
        #endregion

        #region Estado

        private bool partidaReiniciada= false;
        #endregion


        #region APIGUI
        //Api gui
        private DXGui gui_primaria = new DXGui();
        private DXGui gui_secundaria = new DXGui();

        public const int IDOK = 0;

        public const int IDCANCEL = 1;
        public const int ID_JUGAR = 10;
        public const int ID_RESTART = 101;
        public const int ID_CONFIGURAR = 103;
        public const int ID_APP_EXIT = 105;
        public const int ID_PROGRESS1 = 107;
        public const int ID_RESET_CAMARA = 108;

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
        #endregion

        float ScreenRes_X;
        float ScreenRes_Y;

        #region Sprites
        public CustomSprite barraDeVida;
        public CustomSprite fruta;
        public CustomSprite mascara;
        public CustomSprite hoguera;
        //public CustomSprite sonido;
        public Drawer2D drawer2D;
        public TgcText2D textoFrutas;
        public TgcText2D textoMascaras;
        public TgcText2D textoSonido;
        public TgcText2D textoHoguera;
        #endregion

        #region Camara
        private TgcThirdPersonCamera camaraInterna;
        private float cameraOffsetHeight = 400;
        private float cameraOffsetForward = -800;
        private TGCVector3 traslacionFrustum = new TGCVector3(0f, -0, -2800f);
        #endregion

        //Debug -> Hay que borrar estas variables
        #region Desarrollo
        private TGCVector3 movimientoRealCaja = TGCVector3.Empty;
        //TGCVector3 movimientoPorPlataforma = new TGCVector3(0, 0, 0);

        float coeficienteDiferencialGlobal = 0f;
        TGCVector3 verticeMasAltoGlobal = new TGCVector3(0, 0, 0);
        bool colisionRampa = false;
        TGCVector3 vectorDiferenciaGlobal = new TGCVector3(0, 0, 0);
        float YPorDesnivelGlobal = 0f;
        float longitudRampaGlobal = 0f;
        float alturaRampaGlobal = 0f;
        #endregion

        

        Random generadorRandom = new Random(); //Generador de numeros aleatorios;


        public override void Init()
        {
            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;
            
            
            
            ScreenRes_X = d3dDevice.PresentationParameters.BackBufferWidth;
            ScreenRes_Y = d3dDevice.PresentationParameters.BackBufferHeight;
            
            directorio = new Directorio(MediaDir);
            Hoguera.texturesPath = directorio.TexturasPath;
            FuegoLuz.texturesPath = directorio.TexturasPath; ;
            Personaje.texturesPath = directorio.TexturasPath; ;
            //Cargo el SoundManager
            soundManager = new SoundManager(directorio, this.DirectSound.DsDevice);
            soundManager.playSonidoFondo();

            //Objeto que conoce todos los path de MediaDir
            if (!partidaReiniciada)
            {
                informador = new Informador(DrawText, ScreenRes_X, ScreenRes_Y);
                personaje = new Personaje(directorio);
                estadoJuego = new EstadoJuego();
                escenario = new Escenario(directorio.EscenaCrash, personaje);

            }
            else
            {
                personaje.reiniciar();
                estadoJuego.reiniciar();
            }
            
            
            //Le cambiamos la textura para diferenciarlo un poco
            personaje.changeDiffuseMaps(new[]
            {
                    TgcTexture.createTexture(D3DDevice.Instance.Device, directorio.RobotTextura)
                }); 
                
            //Inicializamos el collisionManager.
            ColisionadorEsferico = new SphereCollisionManager();
            ColisionadorEsferico.GravityEnabled = true;
            ColisionadorEsferico.GravityForce = new TGCVector3(0, -10, 0);
            ColisionadorEsferico.SlideFactor = 1.3f;

            //Obtenemos las plataformas segun su tipo de movimiento.
            plataformas = escenario.Plataformas();
            plataformasRotantes = escenario.PlataformasRotantes();

            //Posición de la camara.
            camaraInterna = new TgcThirdPersonCamera(personaje.esferaPersonaje.Center, 600, -1200);

            //Configuro donde esta la posicion de la camara y hacia donde mira.
            Camara = camaraInterna;

            

            var meshesSinPlatXZ = escenario.scene.Meshes.FindAll(mesh => mesh.Name != "PlataformaX" && mesh.Name != "PlataformaZ");

            octree = new Octree();
            octree.create(meshesSinPlatXZ, escenario.BoundingBox());
            octree.createDebugOctreeMeshes();// --> Para renderizar las "cajas" que genera

            Frustum.Color = Color.Black;


            inicializarGUIPrincipal();
            inicializarGUISecundaria();
            inicializarIluminacion();
            inicializarHUDS(d3dDevice);


            

            string compilationErrors;
            olasLava = Microsoft.DirectX.Direct3D.Effect.FromFile(d3dDevice, MediaDir + "OlasLava.fx",
                null, null, ShaderFlags.PreferFlowControl, null, out compilationErrors);
            if (olasLava == null)
            {
                throw new Exception("Error al cargar shader OlasLava. Errores: " + compilationErrors);
            }

            foreach (TgcMesh mesh in escenario.MeshesParaEfectoLava())
            {
                mesh.Effect = olasLava;
                mesh.Technique = "Olas";
            }

            olasLava.SetValue("screen_dx", ScreenRes_X);
            olasLava.SetValue("screen_dy", ScreenRes_Y);
           
            //Configurar animacion inicial
            personaje.playAnimation("Parado", true);
        }


           


        public override void Update()
        {
            PreUpdate();

           
            //TODO: Reificar estos valores.
            //Obtenemos los valores default

            if (escenario.colisionDeSalto()) doubleJump = 2;
            else doubleJump = 0;

            
            var coeficienteSalto = 30f;
            float saltoRealizado = 0;
            var moveForward = 0f;
            personaje.moving = false;
            var animacion = "";

            while (ElapsedTime > 1) ElapsedTime = ElapsedTime / 10; //Para evitar el error de que ElapsedTime es muy alto al inicio

            tiempoAcumulado += ElapsedTime;

            
            //Pausa
            if (Input.keyPressed(Key.P)) estadoJuego.partidaPausada = !estadoJuego.partidaPausada;
            

            //Menu
            if (Input.keyPressed(Key.M))
            {
                estadoJuego.menu = true;
                estadoJuego.partidaPausada = true;
            }
             
            //Bounding Box activos.
            if (Input.keyPressed(Key.F))boundingBoxActivate = !boundingBoxActivate;

            //Activo y desactivo Modo Dios
            if (Input.keyPressed(Key.I)) estadoJuego.godMode = !estadoJuego.godMode;

            if (Input.keyPressed(Key.Z)) soundManager.actualizarEstado();

            //Si el personaje se mantiene en caida, se pierda la partida.
            if (personaje.position().Y < -200)estadoJuego.partidaPerdida = true;
            

           

            //Si se sigue en juego, se continua con la logica del juego.
            if (!estadoJuego.partidaPausada && !estadoJuego.partidaPerdida)
            {
               
                if (Input.keyDown(Key.R)) solicitudInteraccionConCaja = true;
                else solicitudInteraccionConCaja = false;

                if (Input.keyDown(Key.Q))personaje.kicking = true;
                else personaje.kicking = false;

                if (personaje.VELOCIDAD_EXTRA > 0) personaje.running = true;
                else personaje.running = false;


                #region Salto
                // Para que no se pueda saltar cuando agarras algun objeto
                if (!solicitudInteraccionConCaja)
                {
                    if (Input.keyUp(Key.Space) && saltoActual < coeficienteSalto && (doubleJump > 0 || estadoJuego.godMode))
                    {
                        saltoActual = coeficienteSalto;
                        doubleJump -= 1;
                        personaje.jumping = true;
                        soundManager.playSonidoSaltar();
                    }
                    if (Input.keyUp(Key.Space) || saltoActual > 0 )
                    {
                        saltoActual -= coeficienteSalto * ElapsedTime;
                        saltoRealizado = saltoActual;
                    }
                    if (saltoRealizado == 0) personaje.jumping = false;
                   
                }
                #endregion

                #region Danio
                if (escenario.personajeSobreLava() && !estadoJuego.godMode)
                {
                    soundManager.playSonidoDanio();
                    escenario.quemarPersonaje();
                }
                #endregion

                #region BarraVida

                if (personaje.vivo())
                {

                    if (!personaje.vidaCompleta())
                    {
                        barraDeVida.Scaling = new TGCVector2(personaje.vida, 1);
                        barraDeVida.Color = Color.FromArgb(Convert.ToInt32((1 - personaje.vida) * 255), Convert.ToInt32(personaje.vida * 255), 0);
                    }
                    else
                    {
                        barraDeVida.Scaling = new TGCVector2(1, 1);
                        barraDeVida.Color = Color.FromArgb(0, 255, 0);
                    }
                }
                else
                {
                    estadoJuego.partidaPerdida = true;
                }
                #endregion

                #region Frutas
                if (escenario.personajeSobreFruta())
                {
                    personaje.aumentarFrutas();
                    soundManager.playSonidoFruta();
                    escenario.eliminarFrutaColisionada();
                 }
                 textoFrutas.Text = personaje.frutas.ToString();


                #endregion

                #region Mascaras
                if (escenario.personajeSobreMascara())
                {
                    personaje.aumentarMascaras();
                    soundManager.playSonidoMoneda();
                    escenario.eliminarMascaraColisionada();
                }
                textoMascaras.Text = personaje.mascaras.ToString();


                #endregion

                #region Hogueras
                var hoguera = escenario.getClosestBonfire(personaje.position(), 500f);
                if (hoguera != null)
                {
                    if (Input.keyUp(Key.E) && hoguera.afectar(personaje)) informador.nuevoCheckpoint();
                    else if(!hoguera.Encendida) informador.hogueraCerca();
                }
                textoHoguera.Text = personaje.hogueras.ToString();
                #endregion

                #region Sonido
                if (soundManager.sonido) textoSonido.Text = "Sound: on";
                else textoSonido.Text = "Sound: off";
                #endregion

                #region Movimientos

                personaje.moving = personaje.rotar(Input,new Key());
                personaje.actualizarValores(ElapsedTime);
                //Vector de movimiento
                var movimientoOriginal = new TGCVector3(0,0,0);
                float movX = 0;
                float movY = saltoRealizado;
                float movZ = 0;

                if (personaje.moving)
                {
                    animacion = "Caminando";
                    moveForward = personaje.Velocidad();
                    movX = FastMath.Sin(personaje.rotation().Y) * moveForward * ElapsedTime;
                    movZ = FastMath.Cos(personaje.rotation().Y) * moveForward * ElapsedTime;
                    soundManager.playSonidoCaminar();
                }
                else soundManager.stopSonidoCaminar();

                if (personaje.kicking) animacion = "Pateando";
                else if (personaje.running) animacion = "Corriendo";
                else if (solicitudInteraccionConCaja && personaje.moving) animacion = "Empujando";
                else if (!personaje.moving) animacion = "Parado";
                   
                
               
                movimientoOriginal = new TGCVector3(movX, movY, movZ);

                moverMundo(movimientoOriginal);
                #endregion

                //Ejecuta la animacion del personaje
                personaje.playAnimation(animacion, true);

                //Reajustamos la camara
                ajustarCamara();

               //Actualizo posición del Frustum
                Frustum.updateMesh(camaraInterna.Position + traslacionFrustum, camaraInterna.LookAt);
                PostUpdate();
            }
            
        }
 
        public void moverMundo(TGCVector3 movimientoOriginal)
        {
            TGCVector3 movimientoRealPersonaje = new TGCVector3(0, 0, 0);
            movimientoDePlataformas();
            movimientoDeCajas(movimientoOriginal);
            
            
            movimientoOriginal += movimientoPorSliding(movimientoOriginal);
            movimientoOriginal += movimientoPorPlataformas();

           
            //Busca una plataforma rotante con la que se este colisionando
            //NOTA: para estas plataformas se colisiona Esfera -> OBB y no Esfera -> AABB como las demás colisiones
            var plataformaRotante = plataformasRotantes.Find(plat => colliderOBB.colisionaEsferaOBB(personaje.esferaPersonaje,plat.OBB));
            //Si colisiona con una maneja la colision para las rotantes sino usa el metodo general
            if (plataformaRotante != null)
            {
                movimientoRealPersonaje = colliderOBB.manageColisionEsferaOBB(personaje.esferaPersonaje, movimientoOriginal, plataformaRotante.OBB);
                
                personaje.matrizTransformacionPlataformaRotante = plataformaRotante.transform();
                
            }
            else
            {
                movimientoRealPersonaje = ColisionadorEsferico.moveCharacter(personaje.esferaPersonaje, movimientoOriginal, escenario.MeshesColisionablesBB());
                personaje.matrizTransformacionPlataformaRotante = TGCMatrix.Identity;
            }
            personaje.MovimientoRealActual = movimientoRealPersonaje;
            float alturaPorDesnivel = 0f;
            if ((alturaPorDesnivel = movimientoPorDesnivel()) >= 0)
            {
                var xOriginal = personaje.esferaPersonaje.Center.X;
                var zOriginal = personaje.esferaPersonaje.Center.Z;
                var YActualizado = alturaPorDesnivel;

                personaje.esferaPersonaje.setCenter(new TGCVector3(xOriginal, YActualizado, zOriginal));
                
            }
            personaje.transformar();
            //personaje.move(movimientoRealPersonaje);
        }
       
        public TGCVector3 movimientoPorPlataformas()
        {

            Plataforma plataformaColisionante = plataformas.Find(plataforma => plataforma.colisionaConPersonaje(personaje.esferaPersonaje));
            if (plataformaColisionante != null) colisionPlataforma = true;
            else colisionPlataforma = false;

            if (colisionPlataforma) return plataformaColisionante.VectorMovimiento();
            else return TGCVector3.Empty;
        }

         public float movimientoPorDesnivel()
        {
            Rampa rampa = escenario.obtenerColisionRampaPersonaje();

            if (rampa == null || personaje.jumping)
            {
                colisionRampa = false;
                ColisionadorEsferico.GravityEnabled = true;
                return -1;
            }
            ColisionadorEsferico.GravityEnabled = false;

            colisionRampa = true;

            return rampa.obtenerAlturaInstantanea(personaje.position()) + personaje.esferaPersonaje.Radius;

        }

        public TGCVector3 movimientoPorSliding(TGCVector3 movimientoOriginal)
        {
            var vectorSlide = new TGCVector3(0, 0, 0);

            PisoInercia pisoInercia = escenario.obtenerColisionPisoInerciaPersonaje();
            if (pisoInercia == null)
            {
                personaje.sliding = false;
                return new TGCVector3(0, 0, 0);
            }
            
            var vectorSlideActual = pisoInercia.vectorEntrada();

            var versorMovimientoOriginal = movimientoOriginal * (1 / TGCVector3.Length(movimientoOriginal));

           
            if (vectorSlideActual == TGCVector3.Empty || ((versorMovimientoOriginal != pisoInercia.versorEntrada()) && TGCVector3.Length(movimientoOriginal) > 0))
            {
                pisoInercia.setVectorEntrante(movimientoOriginal);
            }
            else  vectorSlide = vectorSlideActual;
            
         
            return vectorSlide;
            
        }

        public void movimientoDePlataformas()
        {
            foreach (Plataforma plataforma in plataformas) plataforma.Update(tiempoAcumulado);
        }
        public void movimientoDeCajas(TGCVector3 movimientoOriginal)
        {
            
            Caja cajaColisionante = escenario.obtenerColisionCajaPersonaje();

            if (cajaColisionante != null) interaccionCaja = true;
            else
            {
                interaccionCaja = false;
                return;
            }

            cajaColisionante.afectar(personaje);
            if (cajaColisionante.esTNT()) soundManager.playSonidoDanio();
            
            
            if (!solicitudInteraccionConCaja)
            {
                interaccionCaja = false;
                return;
            }
            

            if (cajaColisionante == objetoMovibleGlobal) cajaColisionante = null;

            //Si es una caja nueva updatea la referencia global
            if (cajaColisionante != null && cajaColisionante != objetoEscenario) objetoEscenario = cajaColisionante;

            if (objetoEscenario != null) generarMovimiento(objetoEscenario, movimientoOriginal);
        }
        public void generarMovimiento(Caja objetoMovible, TGCVector3 movimiento)
        {
            if (objetoMovibleGlobal == null || objetoMovibleGlobal != objetoMovible) objetoMovibleGlobal = objetoMovible;

            esferaCaja = new TgcBoundingSphere(objetoMovible.boundingBox().calculateBoxCenter() + new TGCVector3(0f, 15f, 0f), objetoMovible.boundingBox().calculateBoxRadius() * 0.7f);

            movimientoRealCaja = ColisionadorEsferico.moveCharacter(esferaCaja, movimiento,  escenario.MeshesColisionablesBBSin(objetoMovible.cajaMesh));

            var testCol =personaje.colisionaConCaja(objetoMovible);
            
            if (solicitudInteraccionConCaja && testCol)
            {
                if (!escenario.colisionEscenario()) objetoMovible.Move(movimientoRealCaja);
                else if (escenario.colisionConPilar() || personaje.colisionaConCaja(objetoMovible)) movimientoRealCaja = TGCVector3.Empty;
                else objetoMovible.Move(-movimientoRealCaja);
                
            }
            else if (movimientoRealCaja.Y < 0) objetoMovible.Move(movimientoRealCaja);

        }
      
       
        public void ajustarCamara()
        {
            //Actualizar valores de camara segun modifiers
            
            camaraInterna.TargetDisplacement = new TGCVector3(0, 50, 0);
            camaraInterna.OffsetHeight = cameraOffsetHeight;
            camaraInterna.OffsetForward = cameraOffsetForward;

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
            camaraInterna.Target = personaje.esferaPersonaje.Center;
        }

        public override void Render()
        {
            PreRender();
            if (estadoJuego.menu)gui_render(ElapsedTime);
            else
            {
                Frustum.render();
                olasLava.SetValue("time", tiempoAcumulado);

                //int factorY = rnd.Next(-6, 6);
                //int factorX = rnd.Next(-6, 6);
                //int frecuencia = rnd.Next(1, 11);

                //olasLava.SetValue("factorY", factorY);
                //olasLava.SetValue("factorX", factorX);
                //olasLava.SetValue("frecuencia", frecuencia);


                if (!estadoJuego.partidaPerdida)
                {
                    renderizarSprites();

                    informador.informar(estadoJuego,personaje,ElapsedTime);

                    renderizarDebug();
                    //Renderizo OBB de las plataformas rotantes
                    plataformasRotantes.ForEach(plat => plat.Render(tiempoAcumulado));
                    
                    if (!estadoJuego.partidaPausada)
                    {
                        octree.render(Frustum, boundingBoxActivate);
                        renderizarRestantes();
                        personaje.animateAndRender(ElapsedTime);
                        if (personaje.emisorParticulas != null)
                        {
                            personaje.emisorParticulas.Position = personaje.position();
                            personaje.emisorParticulas.Speed= new TGCVector3(65, 20, 15);
                            personaje.emisorParticulas.render(ElapsedTime);
                        }
                        escenario.RenderAll();

                    }
                    else DrawText.drawText("EN PAUSA", 500, 500, Color.Red);


                    if (boundingBoxActivate)
                    {
                        personaje.boundingBox().Render();
                        personaje.esferaPersonaje.Render();
                        escenario.RenderizarBoundingBoxes();
                    }


                    TgcMesh closestLight = escenario.getClosestLight(personaje.position(), 2500f);
                    if(closestLight != null)
                    {
                        personaje.effect().SetValue("lightColor", ColorValue.FromColor(Color.White));
                        personaje.effect().SetValue("lightPosition", TGCVector3.Vector3ToFloat4Array(closestLight.Position));
                        personaje.effect().SetValue("eyePosition", TGCVector3.Vector3ToFloat4Array(Camara.Position));
                    }
                    personaje.effect().SetValue("materialEmissiveColor", ColorValue.FromColor(Color.White));
                    personaje.effect().SetValue("materialAmbientColor", ColorValue.FromColor(Color.FromArgb(50, 50, 50)));
                    personaje.effect().SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                    personaje.effect().SetValue("materialSpecularColor", ColorValue.FromColor(Color.DimGray));
                    personaje.effect().SetValue("materialSpecularExp", 500f);

                    personaje.effect().SetValue("lightIntensity", 20);
                    personaje.effect().SetValue("lightAttenuation", 25);

                    foreach(TgcMesh mesh in meshesConLuz)
                    {
                        //mesh.Effect.SetValue("lightPosition", TGCVector3.Vector3ToFloat4Array(luz.Position));
                        mesh.Effect.SetValue("eyePosition", TGCVector3.Vector3ToFloat4Array(Camara.Position));
                    }


                    //EMIRSORES DE PARTICULAS
                    D3DDevice.Instance.ParticlesEnabled = true;
                    D3DDevice.Instance.EnableParticles();
                    foreach(Hoguera s in escenario.hogueras)
                    {
                        s.renderParticles(ElapsedTime);
                    }
                    foreach (FuegoLuz s in escenario.fuegosLuz)
                    {
                        s.renderParticles(ElapsedTime);
                    }
                }
                else
                {
                    gui_partida_perdida_render(ElapsedTime);
                }
            }
           
            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }
        private void renderizarRestantes() => plataformas.ForEach(plat => { if (plat.plataformaMesh.Name == "PlataformaX" || plat.plataformaMesh.Name == "PlataformaZ") plat.plataformaMesh.Render(); });
        private void renderizarSprites()
        {
            drawer2D.BeginDrawSprite();
            drawer2D.DrawSprite(barraDeVida);
            drawer2D.DrawSprite(fruta);
            drawer2D.DrawSprite(mascara);
            drawer2D.DrawSprite(hoguera);
            drawer2D.EndDrawSprite();

            textoFrutas.render();
            textoMascaras.render();
            textoSonido.render();
            textoHoguera.render();

        }
        private void renderizarDebug()
        {
            DrawText.drawText("Posicion Actual: " + personaje.position() + "\n"
                              // + "Vector Movimiento Real Personaje: " + movimientoRealPersonaje + "\n"
                               + "Colision con Caja: " + interaccionCaja + "\n"
                               + "Solicitud interaccion con caja: " + solicitudInteraccionConCaja + "\n"
                               + "Moving: " + personaje.moving + "\n"
                               + "Jumping: " + personaje.jumping + "\n"
                               + "Sliding: " + personaje.sliding + "\n"
                               + "Kicking: " + personaje.kicking + "\n"
                               + "Elapsed Time: " + ElapsedTime +"\n"
                              /* + "Colision Con Rampa: " + colisionRampa + "\n"
                               + "Vertice mas alto: " + verticeMasAltoGlobal + "\n"
                               + "Vector diferencia: " + vectorDiferenciaGlobal + "\n"
                               + "Y Por desnivel: " + YPorDesnivelGlobal + "\n"
                               + "Longitud Rampa: " + longitudRampaGlobal + "\n"
                               + "Altura Rampa: " + alturaRampaGlobal + "\n"
                               + "Posicion bounding box: " + personaje.boundingBox().calculateBoxCenter() + "\n"
                               + "Coeficiente Diferencial: " + coeficienteDiferencialGlobal + "\n"
                               /*+ "Vector Movimiento Relativo Personaje" + movimientoRelativoPersonaje + "\n"
                               + "Vector Movimiento Real Caja" + movimientoRealCaja + "\n"
                               + "Interaccion Con Caja: " + interaccionConCaja + "\n"
                               + "Colision Plataforma: " + colisionPlataforma + "\n"
                               /*+ "Movimiento por plataforma: " + movimientoPorPlataforma*/, 0, 600, Color.GhostWhite);
        }
        

        /// <summary>
        ///     Se llama cuando termina la ejecución del ejemplo.
        ///     Hacer Dispose() de todos los objetos creados.
        ///     Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        /// </summary>

        public override void Dispose()
        {
            personaje.dispose();
            escenario.DisposeAll();
            barraDeVida.Dispose();
            fruta.Dispose();
            mascara.Dispose();
            textoFrutas.Dispose();
            textoMascaras.Dispose();
        }

        #region GUIMethods
        public void inicializarGUIPrincipal()
        {
            // levanto el GUI
            gui_primaria.Create(MediaDir);
                        
            // menu principal
            gui_primaria.InitDialog(false,false);
            int W = D3DDevice.Instance.Width;
            int H = D3DDevice.Instance.Height;
            int x0 = 70;
            int y0 = 10;
            int dy = 120;
            int dy2 = dy;
            int dx = 400;
            int item_epsilon = 50;
            gui_primaria.InsertImage("menu.png",1850,450, directorio.Menu);
            
            gui_primaria.InsertMenuItem(ID_JUGAR, "Jugar", "open.png", x0, y0, MediaDir, dx, dy);
            gui_primaria.InsertMenuItem(ID_CONFIGURAR, "Configurar", "navegar.png", x0+dx+item_epsilon, y0 , MediaDir, dx, dy);
            gui_primaria.InsertMenuItem(ID_APP_EXIT, "Salir", "salir.png", x0, y0 += dy2, MediaDir, dx, dy);
           
        }

        public void inicializarGUISecundaria()
        {
                         
            float W = D3DDevice.Instance.Width ;
            float H = D3DDevice.Instance.Height ;

            int dx = (int)(700.0f );
            int dy = (int)(450.0f );
            int x0 = (int)((W - dx) / 2);
            int y0 = (int)((H - dy) / 2);
            int r = 100;

            gui_secundaria.Create(MediaDir);
            gui_secundaria.InitDialog(false, false);
            gui_secundaria.InsertImage("menu_perdiste.png", 1850, 450, directorio.Menu);

            gui_secundaria.InsertFrame("Partida Perdida", x0, y0, dx, dy, Color.FromArgb(0, 0, 0));
            gui_secundaria.InsertItem("Desea reiniciar el juego?", x0 + 200, y0 + 200);
            gui_secundaria.InsertCircleButton(0, "OK", "ok.png", x0 + 70, y0 + dy - r - 90, mediaDir, r);
            gui_secundaria.InsertCircleButton(1, "CANCEL", "cancel.png", x0 + dx - r - 70, y0 + dy - r - 90, mediaDir, r);

        }

        public void gui_partida_perdida_render(float elapsedTime)
        {
            GuiMessage mensaje_gui = gui_secundaria.Update(elapsedTime, Input);
            soundManager.pauseSonidos();

            // proceso el msg
            switch (mensaje_gui.message)
            {
                case MessageType.WM_COMMAND:
                    switch (mensaje_gui.id)
                    {
                        case IDOK:
                            partidaReiniciada = true;
                            Init();
                            break;
                        case IDCANCEL:
                            System.Windows.Forms.Application.Exit();
                            break;
                        default:
                            break;
                    }
                    break;

                default:
                    break;
            }

            gui_secundaria.Render();
        }

        public void gui_render(float elapsedTime)
        {
            // ------------------------------------------------
            GuiMessage mensaje_gui = gui_primaria.Update(elapsedTime, Input);
            
            
            // proceso el msg
            switch (mensaje_gui.message)
            {
                case MessageType.WM_COMMAND:
                    switch (mensaje_gui.id)
                    {
                        case IDOK:

                        case IDCANCEL:
                            // Resultados OK, y CANCEL del ultimo messagebox
                            gui_primaria.EndDialog();
                            profiling = false;
                            if (msg_box_app_exit)
                            {
                                // Es la resupuesta a un messagebox de salir del sistema
                                if (mensaje_gui.id == IDOK)
                                {
                                    // Salgo del sistema
                                    System.Windows.Forms.Application.Exit();
                                }
                            }
                            msg_box_app_exit = false;
                            break;

                        case ID_JUGAR:
                            estadoJuego.menu=false;
                            estadoJuego.partidaPausada = false;
                            break;

                            
                        /*case ID_CONFIGURAR:
                            Configurar();
                            break;*/

                        case ID_APP_EXIT:
                            gui_primaria.Menu_Exit("Desea Salir del Juego?",directorio.Menu, "Crash Bandicoot");
                            msg_box_app_exit = true;
                            break;

                        default:
                            break;
                    }
                    break;

                default:
                    break;
            }
            gui_primaria.Render();
        }
        #endregion

        public void inicializarIluminacion()
        {
            meshesConLuz = new List<TgcMesh>();
            effectLuzComun = TgcShaders.Instance.TgcMeshPhongShader;
            effectLuzLava = effectLuzComun.Clone(effectLuzComun.Device);
            foreach (TgcMesh mesh in escenario.scene.Meshes)
            {
                Microsoft.DirectX.Direct3D.Effect defaultEffect = mesh.Effect;

                TgcMesh luz = escenario.getClosestLight(mesh.BoundingBox.calculateBoxCenter(), 2500f);

                if (luz == null)
                {
                    mesh.Effect = defaultEffect;
                }
                else
                {
                    if(mesh.Layer != "LAVA" && mesh.Layer != "FUEGO" && mesh.Layer != "HOGUERA")
                    {
                        if (luz.Layer == "LAVA")
                        {
                            mesh.Effect = effectLuzLava;
                            mesh.Technique = TgcShaders.Instance.getTgcMeshTechnique(mesh.RenderType);
                            mesh.Effect.SetValue("lightPosition", TGCVector3.Vector3ToFloat4Array(luz.Position));
                            mesh.Effect.SetValue("eyePosition", TGCVector3.Vector3ToFloat4Array(Camara.Position));
                            //mesh.Effect.SetValue("lightIntensity", 20f);
                            //mesh.Effect.SetValue("lightAttenuation", 0.3f);
                            mesh.Effect.SetValue("ambientColor", ColorValue.FromColor(Color.Red));
                            mesh.Effect.SetValue("diffuseColor", ColorValue.FromColor(Color.Red));
                            mesh.Effect.SetValue("specularColor", ColorValue.FromColor(Color.Orange));
                            mesh.Effect.SetValue("specularExp", 10000f);
                        }
                        else
                        {
                            if (mesh.Layer != "LAVA" && mesh.Layer != "FUEGO" && mesh.Layer != "HOGUERA")
                            {
                                mesh.Effect = effectLuzComun;
                                mesh.Technique = TgcShaders.Instance.getTgcMeshTechnique(mesh.RenderType);
                                mesh.Effect.SetValue("lightPosition", TGCVector3.Vector3ToFloat4Array(luz.Position));
                                mesh.Effect.SetValue("eyePosition", TGCVector3.Vector3ToFloat4Array(Camara.Position));
                                //mesh.Effect.SetValue("lightIntensity", 20f);
                                //mesh.Effect.SetValue("lightAttenuation", 0.3f);
                                mesh.Effect.SetValue("ambientColor", ColorValue.FromColor(Color.FromArgb(50, 50, 50)));
                                mesh.Effect.SetValue("diffuseColor", ColorValue.FromColor(Color.White));
                                mesh.Effect.SetValue("specularColor", ColorValue.FromColor(Color.DimGray));
                                mesh.Effect.SetValue("specularExp", 500f);
                            }
                        }
                        meshesConLuz.Add(mesh);
                    }
                }
                //mesh.Technique = "RenderScene2";
            }
            personajeLightShader = TgcShaders.Instance.TgcSkeletalMeshPointLightShader;
            personaje.effect(personajeLightShader);
            personaje.technique(TgcShaders.Instance.getTgcSkeletalMeshTechnique(personaje.renderType()));
        }

        public void inicializarHUDS(Microsoft.DirectX.Direct3D.Device d3dDevice)
        {
            drawer2D = new Drawer2D();
            barraDeVida = new CustomSprite();
            barraDeVida.Bitmap = new CustomBitmap(directorio.BarraVida, d3dDevice);
            barraDeVida.Position = new TGCVector2(10, 20);

            fruta = new CustomSprite();
            fruta.Bitmap = new CustomBitmap(directorio.Fruta, d3dDevice);
            fruta.Position = new TGCVector2(20, 70);

            textoFrutas = new TgcText2D();
            textoFrutas.Text = "0";
            textoFrutas.Color = Color.White;
            textoFrutas.Align = TgcText2D.TextAlign.LEFT;
            textoFrutas.Position = new Point(100, 80);
            textoFrutas.Size = new Size(350, 140);
            textoFrutas.changeFont(new System.Drawing.Font("TimesNewRoman", 30,FontStyle.Bold));


            mascara = new CustomSprite();
            mascara.Bitmap = new CustomBitmap(directorio.Mascara, d3dDevice);
            mascara.Position = new TGCVector2(25, 150);

            textoMascaras = new TgcText2D();
            textoMascaras.Text = "0";
            textoMascaras.Color = Color.White;
            textoMascaras.Align = TgcText2D.TextAlign.LEFT;
            textoMascaras.Position = new Point(100, 200);
            textoMascaras.Size = new Size(350, 140);
            textoMascaras.changeFont(new System.Drawing.Font("TimesNewRoman", 30, FontStyle.Bold));

            hoguera = new CustomSprite();
            hoguera.Bitmap = new CustomBitmap(directorio.Hoguera, d3dDevice);
            hoguera.Position = new TGCVector2(22, 290);

            textoHoguera = new TgcText2D();
            textoHoguera.Text = "0";
            textoHoguera.Color = Color.White;
            textoHoguera.Align = TgcText2D.TextAlign.LEFT;
            textoHoguera.Position = new Point(100, 330);
            textoHoguera.Size = new Size(350, 140);
            textoHoguera.changeFont(new System.Drawing.Font("TimesNewRoman", 30, FontStyle.Bold));

            textoSonido = new TgcText2D();
            textoSonido.Text = "Sound: on";
            textoSonido.Color = Color.Green;
            textoSonido.Align = TgcText2D.TextAlign.LEFT;
            textoSonido.Position = new Point(1400, 20);
            textoSonido.Size = new Size(200, 20);
            textoSonido.changeFont(new System.Drawing.Font("TimesNewRoman", 15, FontStyle.Bold));

        }

    }


}