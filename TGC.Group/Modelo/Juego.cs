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
using TGC.Core.Geometry;

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

        private bool boundingBoxActivate = false;




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
        private DXGui gui_menu_principal = new DXGui();
        private DXGui gui_partida_perdida = new DXGui();
        private DXGui gui_partida_ganada = new DXGui();

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

        #region Sprites
        public CustomSprite barraDeVida;
        public CustomSprite fruta;
        public CustomSprite mascara;
        public CustomSprite hoguera;
        public CustomSprite pauseSprite;
        public CustomSprite soundOnSprite;
        public CustomSprite soundOffSprite;
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
        private TGCVector3 traslacionFrustum = new TGCVector3(0f, 0, -2800f);
        #endregion

        #region Resolucion

        float ScreenRes_X;
        float ScreenRes_Y;
        #endregion

        //Debug -> Hay que borrar estas variables
        #region Desarrollo
        private TGCVector3 movimientoRealCaja = TGCVector3.Empty;
        //TGCVector3 movimientoPorPlataforma = new TGCVector3(0, 0, 0);

        TGCVector3 verticeMasAltoGlobal = new TGCVector3(0, 0, 0);
       
        TGCVector3 vectorDiferenciaGlobal = new TGCVector3(0, 0, 0);

        
        #endregion

        

        Random generadorRandom = new Random(); //Generador de numeros aleatorios;

        private int cant_pasadas = 2;

        public static List<TgcMesh> meshesConLuz;
        private Microsoft.DirectX.Direct3D.Effect effectLuzComun;
        private Microsoft.DirectX.Direct3D.Effect effectLuzLava;
        private Microsoft.DirectX.Direct3D.Effect personajeLightShader;

        private Microsoft.DirectX.Direct3D.Effect olasLavaEffect;
        private Microsoft.DirectX.Direct3D.Effect postProcessBloom;


        private Surface g_pDepthStencil; // Depth-stencil buffer
        private Texture g_pRenderTarget, g_pGlowMap, g_pRenderTarget4, g_pRenderTarget4Aux;
        private VertexBuffer g_pVBV3D;
        Microsoft.DirectX.Direct3D.Device d3dDevice;

        private readonly int SHADOWMAP_SIZE = 1024;
        private Microsoft.DirectX.Direct3D.Effect efectoShadow;
        private TGCVector3 g_LightDir; // direccion de la luz actual
        private TGCVector3 g_LightPos; // posicion de la luz actual (la que estoy analizando)
        private TGCMatrix g_LightView; // matriz de view del light
        private TGCMatrix g_mShadowProj; // Projection matrix for shadow map
        private Surface g_pDSShadow; // Depth-stencil buffer for rendering to shadow map

        private Texture g_pShadowMap; // Texture to which the shadow map is rendered

        public override void Init()
        {
            //Device de DirectX para crear primitivas.
            d3dDevice = D3DDevice.Instance.Device;
            ScreenRes_X = d3dDevice.PresentationParameters.BackBufferWidth;
            ScreenRes_Y = d3dDevice.PresentationParameters.BackBufferHeight;
            
            directorio = new Directorio(MediaDir);
            Hoguera.texturesPath = directorio.TexturasPath;
            FuegoLuz.texturesPath = directorio.TexturasPath; 
            Personaje.texturesPath = directorio.TexturasPath; 
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

           

            //Posición de la camara.
            camaraInterna = new TgcThirdPersonCamera(personaje.esferaPersonaje.Center, 600, -1200);

            //Configuro donde esta la posicion de la camara y hacia donde mira.
            Camara = camaraInterna;


            octree = new Octree();
            octree.create(escenario.scene.Meshes, escenario.BoundingBox());
            octree.createDebugOctreeMeshes();// --> Para renderizar las "cajas" que genera

            Frustum.Color = Color.Black;


            inicializarGUIPrincipal();
            inicializarGUISecundaria();
            inicializarGUITerciaria();
            inicializarIluminacion();
            inicializarSprites(d3dDevice);

            
            string compilationErrors;
            olasLavaEffect = Microsoft.DirectX.Direct3D.Effect.FromFile(d3dDevice, MediaDir + "OlasLava.fx",
                null, null, ShaderFlags.PreferFlowControl, null, out compilationErrors);
            if (olasLavaEffect == null)
            {
                throw new Exception("Error al cargar shader OlasLava. Errores: " + compilationErrors);
            }

            postProcessBloom = Microsoft.DirectX.Direct3D.Effect.FromFile(D3DDevice.Instance.Device, MediaDir + "GaussianBlur.fx", null, null, ShaderFlags.PreferFlowControl, null, out compilationErrors);
            if (postProcessBloom == null)
            {
                throw new Exception("Error al cargar shader postProcessBloom. Errores: " + compilationErrors);
            }

            foreach (TgcMesh mesh in escenario.LavaMesh())
            {
                mesh.Effect = olasLavaEffect;
                mesh.Technique = "Olas";
            }

            foreach (TgcMesh mesh in escenario.FuegosMesh())
            {
                mesh.Effect = olasLavaEffect;
                mesh.Technique = "Olas";
            }

            postProcessBloom.Technique = "DefaultTechnique";

            g_pDepthStencil = d3dDevice.CreateDepthStencilSurface(d3dDevice.PresentationParameters.BackBufferWidth, d3dDevice.PresentationParameters.BackBufferHeight,
                DepthFormat.D24S8, MultiSampleType.None, 0, true);

            // inicializo el render target
            g_pRenderTarget = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth, d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget,
                Format.X8R8G8B8, Pool.Default);

            g_pGlowMap = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth, d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget,
                Format.X8R8G8B8, Pool.Default);

            g_pRenderTarget4 = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth / 4, d3dDevice.PresentationParameters.BackBufferHeight / 4, 1, Usage.RenderTarget,
                Format.X8R8G8B8, Pool.Default);

            g_pRenderTarget4Aux = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth / 4, d3dDevice.PresentationParameters.BackBufferHeight / 4, 1, Usage.RenderTarget,
                Format.X8R8G8B8, Pool.Default);

            postProcessBloom.SetValue("g_RenderTarget", g_pRenderTarget);

            // Resolucion de pantalla
            postProcessBloom.SetValue("screen_dx", ScreenRes_X);
            postProcessBloom.SetValue("screen_dy", ScreenRes_Y);
            olasLavaEffect.SetValue("screen_dx", ScreenRes_X);
            olasLavaEffect.SetValue("screen_dy", ScreenRes_Y);

            CustomVertex.PositionTextured[] vertices =
            {
                new CustomVertex.PositionTextured(-1, 1, 1, 0, 0),
                new CustomVertex.PositionTextured(1, 1, 1, 1, 0),
                new CustomVertex.PositionTextured(-1, -1, 1, 0, 1),
                new CustomVertex.PositionTextured(1, -1, 1, 1, 1)
            };
            //vertex buffer de los triangulos
            g_pVBV3D = new VertexBuffer(typeof(CustomVertex.PositionTextured), 4, d3dDevice, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);
            g_pVBV3D.SetData(vertices, 0, LockFlags.None);


            efectoShadow = TgcShaders.loadEffect(directorio.ShadowMap);

            // Creo el shadowmap.
            // Format.R32F
            // Format.X8R8G8B8
            g_pShadowMap = new Texture(D3DDevice.Instance.Device, SHADOWMAP_SIZE, SHADOWMAP_SIZE, 1, Usage.RenderTarget, Format.R32F, Pool.Default);

            // tengo que crear un stencilbuffer para el shadowmap manualmente
            // para asegurarme que tenga el mismo tamano que el shadowmap, y que no tenga
            // multisample, etc etc.
            g_pDSShadow = D3DDevice.Instance.Device.CreateDepthStencilSurface(SHADOWMAP_SIZE, SHADOWMAP_SIZE, DepthFormat.D24S8, MultiSampleType.None, 0, true);
            // por ultimo necesito una matriz de proyeccion para el shadowmap, ya
            // que voy a dibujar desde el pto de vista de la luz.
            // El angulo tiene que ser mayor a 45 para que la sombra no falle en los extremos del cono de luz
            // de hecho, un valor mayor a 90 todavia es mejor, porque hasta con 90 grados es muy dificil
            // lograr que los objetos del borde generen sombras
            var aspectRatio = D3DDevice.Instance.AspectRatio;
            g_mShadowProj = TGCMatrix.PerspectiveFovLH(Geometry.DegreeToRadian(80), aspectRatio, 50, 5000);
             D3DDevice.Instance.Device.Transform.Projection = TGCMatrix.PerspectiveFovLH(Geometry.DegreeToRadian(45.0f), aspectRatio, 2f, 4000f).ToMatrix();


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
                TgcMesh fruta;
                if ((fruta = escenario.obtenerFrutaColisionada()) != null) 
                {
                    if (fruta.Name == "PODRIDA")
                    {
                        personaje.comerFrutaPodrida();
                        soundManager.playSonidoDanio();
                    }
                    else
                    {
                        personaje.aumentarFrutas();
                        soundManager.playSonidoFruta();
                    }
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

                #region Metas
                if (escenario.colisionaConMeta()) estadoJuego.partidaGanada = true;
                #endregion

                #region Sonido
                if (soundManager.estado_sonido) textoSonido.Text = "Sound: on";
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
        #region MovimientosMundo
        public void moverMundo(TGCVector3 movimientoOriginal)
        {
            TGCVector3 movimientoRealPersonaje = new TGCVector3(0, 0, 0);
            movimientoDePlataformas();
            movimientoDeCajas(movimientoOriginal);
            
            
            movimientoOriginal += movimientoPorSliding(movimientoOriginal);
            movimientoOriginal += movimientoPorPlataformas();

           
            //Busca una plataforma rotante con la que se este colisionando
            //NOTA: para estas plataformas se colisiona Esfera -> OBB y no Esfera -> AABB como las demás colisiones
            var plataformaRotante = escenario.plataformasRotantes.Find(plat => colliderOBB.colisionaEsferaOBB(personaje.esferaPersonaje,plat.OBB));
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

            Plataforma plataformaColisionante = escenario.plataformas.Find(plataforma => plataforma.colisionaConPersonaje(personaje.esferaPersonaje));
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
               
                ColisionadorEsferico.GravityEnabled = true;
                return -1;
            }
            ColisionadorEsferico.GravityEnabled = false;
            
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
            foreach (Plataforma plataforma in escenario.plataformas) plataforma.Update(tiempoAcumulado);
            foreach (PlataformaRotante plataforma in escenario.plataformasRotantes) plataforma.Update(tiempoAcumulado);
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

        #endregion

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
            if (estadoJuego.menu) gui_principal_render(ElapsedTime);
            else if(estadoJuego.partidaGanada) gui_partida_ganada_render(ElapsedTime);
            else if (estadoJuego.partidaPerdida) gui_partida_perdida_render(ElapsedTime);
            else
            {
                Surface pSurf, pOldRT, pOldDS;
                var modelosAnterior = octree.modelos;

                #region ShadowMap
                g_LightPos = personaje.position() + new TGCVector3(-300f,400f,-300f);
                g_LightDir = personaje.position() - g_LightPos;
                g_LightDir.Normalize();

              
                D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

                //Genero el shadow map
                RenderShadowMap();

                D3DDevice.Instance.Device.BeginScene();
                // dibujo la escena pp dicha
                D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                RenderScene(false);

                octree.modelos = modelosAnterior;
                D3DDevice.Instance.Device.EndScene();


                #endregion


                #region Principales
                
                // guardo el Render target anterior y seteo la textura como render target
                pOldRT = D3DDevice.Instance.Device.GetRenderTarget(0);
                pSurf = g_pRenderTarget.GetSurfaceLevel(0);
                D3DDevice.Instance.Device.SetRenderTarget(0, pSurf);
                // hago lo mismo con el depthbuffer, necesito el que no tiene multisampling
                pOldDS = D3DDevice.Instance.Device.DepthStencilSurface;
                D3DDevice.Instance.Device.DepthStencilSurface = g_pDepthStencil;
                D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);


                D3DDevice.Instance.Device.BeginScene();

                olasLavaEffect.SetValue("time", tiempoAcumulado);
                
            
                Frustum.render();
                octree.render(Frustum, boundingBoxActivate);
                renderizarSprites();
                renderizarDebug();
                personaje.renderizarEmisorParticulas(ElapsedTime);
                informador.renderizarInforme(estadoJuego, personaje, ElapsedTime);

                    
                if (!estadoJuego.partidaPausada)
                {
                    personaje.animateAndRender(ElapsedTime);
                }
                else
                {
                    drawer2D.BeginDrawSprite();
                    drawer2D.DrawSprite(pauseSprite);
                    drawer2D.EndDrawSprite();
                }
                  


                if (boundingBoxActivate)
                {
                    personaje.esferaPersonaje.Render();
                    escenario.RenderizarBoundingBoxes();
                }



                /*TgcMesh luzCercana = escenario.obtenerFuenteLuzCercana(personaje.position(), 2500f);
                if (luzCercana != null)
                {
                    personaje.effect().SetValue("lightColor", ColorValue.FromColor(Color.Red));
                    personaje.effect().SetValue("lightPosition", TGCVector3.Vector3ToFloat4Array(luzCercana.Position));
                    personaje.effect().SetValue("eyePosition", TGCVector3.Vector3ToFloat4Array(Camara.Position));
                }
                personaje.effect().SetValue("materialEmissiveColor", ColorValue.FromColor(Color.White));
                personaje.effect().SetValue("materialAmbientColor", ColorValue.FromColor(Color.FromArgb(50, 50, 50)));
                personaje.effect().SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                personaje.effect().SetValue("materialSpecularColor", ColorValue.FromColor(Color.DimGray));
                personaje.effect().SetValue("materialSpecularExp", 500f);

                personaje.effect().SetValue("lightIntensity", 20);
                personaje.effect().SetValue("lightAttenuation", 25);

               foreach (TgcMesh mesh in meshesConLuz)
                {
                    mesh.Effect.SetValue("eyePosition", TGCVector3.Vector3ToFloat4Array(Camara.Position));
                }*/


                //Emisores de Particulas
                D3DDevice.Instance.ParticlesEnabled = true;
                D3DDevice.Instance.EnableParticles();
                escenario.hogueras.ForEach(hoguera => hoguera.renderParticles(ElapsedTime));
                escenario.fuegosLuz.ForEach(hoguera => hoguera.renderParticles(ElapsedTime));


                D3DDevice.Instance.Device.EndScene();
                #endregion



                #region GlowMap
                // dibujo el glow map


                // DefaultTechnique no cambia nada de lo visible (Mientras KLum = 1)
                postProcessBloom.Technique = "DefaultTechnique";
                pSurf = g_pGlowMap.GetSurfaceLevel(0);
                D3DDevice.Instance.Device.SetRenderTarget(0, pSurf);
                D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                D3DDevice.Instance.Device.BeginScene();


                //Dibujamos SOLO los meshes que tienen glow brillantes
                
                octree.modelos = escenario.MeshesLuminosos();
                octree.render(Frustum, boundingBoxActivate);

                Dictionary<int, Microsoft.DirectX.Direct3D.Effect> meshEffect = new Dictionary<int, Microsoft.DirectX.Direct3D.Effect>();
                Dictionary<int, string> meshTechnique = new Dictionary<int, string>();

                List<TgcMesh> opacos = escenario.MeshesOpacos();
                // El resto opacos
                foreach (var m in opacos)
                {
                    meshEffect.Add(m.GetHashCode(), m.Effect);
                    meshTechnique.Add(m.GetHashCode(), m.Technique);
                    m.Effect = postProcessBloom;
                    m.Technique = "DibujarObjetosOscuros";
                }

                octree.modelos = opacos;
                octree.render(Frustum, boundingBoxActivate);

                octree.modelos = modelosAnterior;

                foreach (var m in opacos)
                {
                    Microsoft.DirectX.Direct3D.Effect e = null;
                    string tec = "";
                    meshEffect.TryGetValue(m.GetHashCode(), out e);
                    meshTechnique.TryGetValue(m.GetHashCode(), out tec);
                    m.Effect = e;
                    m.Technique = tec;
                }


                D3DDevice.Instance.Device.EndScene();

                pSurf.Dispose();
                #endregion

                #region Blur
                // Hago un blur sobre el glow map
                // 1er pasada: downfilter x 4
                // -----------------------------------------------------
                pSurf = g_pRenderTarget4.GetSurfaceLevel(0);
                D3DDevice.Instance.Device.SetRenderTarget(0, pSurf);

                D3DDevice.Instance.Device.BeginScene();
                postProcessBloom.Technique = "DownFilter4";
                D3DDevice.Instance.Device.VertexFormat = CustomVertex.PositionTextured.Format;
                D3DDevice.Instance.Device.SetStreamSource(0, g_pVBV3D, 0);
                postProcessBloom.SetValue("g_RenderTarget", g_pGlowMap);

                D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                postProcessBloom.Begin(FX.None);
                postProcessBloom.BeginPass(0);
                D3DDevice.Instance.Device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                postProcessBloom.EndPass();
                postProcessBloom.End();
                pSurf.Dispose();

                D3DDevice.Instance.Device.EndScene();

                D3DDevice.Instance.Device.DepthStencilSurface = pOldDS;

                // Pasadas de blur
                for (var P = 0; P < cant_pasadas; ++P)
                {
                    // Gaussian blur Horizontal
                    // -----------------------------------------------------
                    pSurf = g_pRenderTarget4Aux.GetSurfaceLevel(0);
                    D3DDevice.Instance.Device.SetRenderTarget(0, pSurf);
                    // dibujo el quad pp dicho :

                    D3DDevice.Instance.Device.BeginScene();
                    postProcessBloom.Technique = "GaussianBlurSeparable";
                    D3DDevice.Instance.Device.VertexFormat = CustomVertex.PositionTextured.Format;
                    D3DDevice.Instance.Device.SetStreamSource(0, g_pVBV3D, 0);
                    postProcessBloom.SetValue("g_RenderTarget", g_pRenderTarget4);

                    D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                    postProcessBloom.Begin(FX.None);
                    postProcessBloom.BeginPass(0);
                    D3DDevice.Instance.Device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                    postProcessBloom.EndPass();
                    postProcessBloom.End();
                    pSurf.Dispose();

                    D3DDevice.Instance.Device.EndScene();

                    pSurf = g_pRenderTarget4.GetSurfaceLevel(0);
                    D3DDevice.Instance.Device.SetRenderTarget(0, pSurf);
                    pSurf.Dispose();

                    //  Gaussian blur Vertical
                    // -----------------------------------------------------

                    D3DDevice.Instance.Device.BeginScene();
                    postProcessBloom.Technique = "GaussianBlurSeparable";
                    D3DDevice.Instance.Device.VertexFormat = CustomVertex.PositionTextured.Format;
                    D3DDevice.Instance.Device.SetStreamSource(0, g_pVBV3D, 0);
                    postProcessBloom.SetValue("g_RenderTarget", g_pRenderTarget4Aux);

                    D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                    postProcessBloom.Begin(FX.None);
                    postProcessBloom.BeginPass(1);
                    D3DDevice.Instance.Device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                    postProcessBloom.EndPass();
                    postProcessBloom.End();

                    D3DDevice.Instance.Device.EndScene();
                }

                //  To Gray Scale
                // -----------------------------------------------------
                // Ultima pasada vertical va sobre la pantalla pp dicha
                D3DDevice.Instance.Device.SetRenderTarget(0, pOldRT);
                //pSurf = g_pRenderTarget4Aux.GetSurfaceLevel(0);
                //device.SetRenderTarget(0, pSurf);

                D3DDevice.Instance.Device.BeginScene();

                postProcessBloom.Technique = "GrayScale";
                D3DDevice.Instance.Device.VertexFormat = CustomVertex.PositionTextured.Format;
                D3DDevice.Instance.Device.SetStreamSource(0, g_pVBV3D, 0);
                postProcessBloom.SetValue("g_RenderTarget", g_pRenderTarget);
                postProcessBloom.SetValue("g_GlowMap", g_pRenderTarget4Aux);
                D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                postProcessBloom.Begin(FX.None);
                postProcessBloom.BeginPass(0);
                D3DDevice.Instance.Device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                postProcessBloom.EndPass();
                postProcessBloom.End();

                D3DDevice.Instance.Device.EndScene();

                #endregion


                D3DDevice.Instance.Device.BeginScene();
                RenderFPS();
                RenderAxis();
                D3DDevice.Instance.Device.EndScene();
                D3DDevice.Instance.Device.Present();
            }
        }

        public void RenderShadowMap()
        {
            // Calculo la matriz de view de la luz
            efectoShadow.SetValue("g_vLightPos", new TGCVector4(g_LightPos.X, g_LightPos.Y, g_LightPos.Z, 1));
            efectoShadow.SetValue("g_vLightDir", new TGCVector4(g_LightDir.X, g_LightDir.Y, g_LightDir.Z, 1));
            g_LightView = TGCMatrix.LookAtLH(g_LightPos, g_LightPos + g_LightDir, new TGCVector3(0, 0, 1));

            // inicializacion standard:
            efectoShadow.SetValue("g_mProjLight", g_mShadowProj.ToMatrix());
            efectoShadow.SetValue("g_mViewLightProj", (g_LightView * g_mShadowProj).ToMatrix());

            // Primero genero el shadow map, para ello dibujo desde el pto de vista de luz
            // a una textura, con el VS y PS que generan un mapa de profundidades.
            var pOldRT = D3DDevice.Instance.Device.GetRenderTarget(0);
            var pShadowSurf = g_pShadowMap.GetSurfaceLevel(0);
            D3DDevice.Instance.Device.SetRenderTarget(0, pShadowSurf);
            var pOldDS = D3DDevice.Instance.Device.DepthStencilSurface;
            D3DDevice.Instance.Device.DepthStencilSurface = g_pDSShadow;
            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            D3DDevice.Instance.Device.BeginScene();

            // Hago el render de la escena pp dicha
            efectoShadow.SetValue("g_txShadow", g_pShadowMap);
            RenderScene(true);
            // Termino
            D3DDevice.Instance.Device.EndScene();

            // restuaro el render target y el stencil
            D3DDevice.Instance.Device.DepthStencilSurface = pOldDS;
            D3DDevice.Instance.Device.SetRenderTarget(0, pOldRT);
        }

        public void RenderScene(bool shadow)
        {
            List<TgcMesh> meshes = escenario.scene.Meshes;
            meshes.ForEach(mesh =>
            {
                if (shadow) mesh.Technique = "RenderShadow";
                else mesh.Technique = "RenderScene";
                mesh.Effect = efectoShadow;
            });
           
              octree.modelos = meshes;
              octree.render(Frustum, boundingBoxActivate);
        }

        private void renderizarSprites()
        {
            drawer2D.BeginDrawSprite();
            drawer2D.DrawSprite(barraDeVida);
            drawer2D.DrawSprite(fruta);
            drawer2D.DrawSprite(mascara);
            drawer2D.DrawSprite(hoguera);

            if (soundManager.estado_sonido) drawer2D.DrawSprite(soundOnSprite);
            else drawer2D.DrawSprite(soundOffSprite);

            drawer2D.EndDrawSprite();

            textoFrutas.render();
            textoMascaras.render();
            //textoSonido.render();
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
            gui_menu_principal.Create(MediaDir);
                        
            // menu principal
            gui_menu_principal.InitDialog(false,false);
            int W = D3DDevice.Instance.Width;
            int H = D3DDevice.Instance.Height;
            int x0 = 70;
            int y0 = 10;
            int dy = 120;
            int dy2 = dy;
            int dx = 400;
            int item_epsilon = 50;
            gui_menu_principal.InsertImage("menu.png",1850,450, directorio.Menu);
            
            gui_menu_principal.InsertMenuItem(ID_JUGAR, "Jugar", "open.png", x0, y0, MediaDir, dx, dy);
            gui_menu_principal.InsertMenuItem(ID_CONFIGURAR, "Configurar", "navegar.png", x0+dx+item_epsilon, y0 , MediaDir, dx, dy);
            gui_menu_principal.InsertMenuItem(ID_APP_EXIT, "Salir", "salir.png", x0, y0 += dy2, MediaDir, dx, dy);
           
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

            gui_partida_perdida.Create(MediaDir);
            gui_partida_perdida.InitDialog(false, false);
            gui_partida_perdida.InsertImage("menu_perdiste.png", 1850, 450, directorio.Menu);

            gui_partida_perdida.InsertFrame("Partida Perdida", x0, y0, dx, dy, Color.FromArgb(0, 0, 0));
            gui_partida_perdida.InsertItem("Desea reiniciar el juego?", x0 + 200, y0 + 200);
            gui_partida_perdida.InsertCircleButton(0, "OK", "ok.png", x0 + 70, y0 + dy - r - 90, mediaDir, r);
            gui_partida_perdida.InsertCircleButton(1, "CANCEL", "cancel.png", x0 + dx - r - 70, y0 + dy - r - 90, mediaDir, r);

        }

        public void inicializarGUITerciaria()
        {
            float W = D3DDevice.Instance.Width;
            float H = D3DDevice.Instance.Height;

            int dx = (int)(700.0f);
            int dy = (int)(450.0f);
            int x0 = (int)((W - dx) / 2);
            int y0 = (int)((H - dy) / 2);
            int r = 100;

            gui_partida_ganada.Create(MediaDir);
            gui_partida_ganada.InitDialog(false, false);
            gui_partida_ganada.InsertImage("menu_partida_ganada.jpg", 1850, 450, directorio.Menu);

            gui_partida_ganada.InsertFrame("Partida Ganada", x0, y0, dx, dy, Color.FromArgb(0, 0, 0));
            gui_partida_ganada.InsertCircleButton(0, "OK", "ok.png", x0 + 70, y0 + dy - r - 90, mediaDir, r);
        }


        public void gui_partida_ganada_render(float elapsedTime)
        {
            PreRender();
            GuiMessage mensaje_gui = gui_partida_ganada.Update(elapsedTime, Input);
            soundManager.pauseSonidos();

            // proceso el msg
            switch (mensaje_gui.message)
            {
                case MessageType.WM_COMMAND:
                    switch (mensaje_gui.id)
                    {
                        case IDOK:
                            System.Windows.Forms.Application.Exit(); ;
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

            gui_partida_ganada.Render();
            PostRender();
        }
        public void gui_partida_perdida_render(float elapsedTime)
        {
            PreRender();
            GuiMessage mensaje_gui = gui_partida_perdida.Update(elapsedTime, Input);
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

            gui_partida_perdida.Render();
            PostRender();
        }

        public void gui_principal_render(float elapsedTime)
        {
            PreRender();
            GuiMessage mensaje_gui = gui_menu_principal.Update(elapsedTime, Input);
            
            
            // proceso el msg
            switch (mensaje_gui.message)
            {
                case MessageType.WM_COMMAND:
                    switch (mensaje_gui.id)
                    {
                        case IDOK:

                        case IDCANCEL:
                            // Resultados OK, y CANCEL del ultimo messagebox
                            gui_menu_principal.EndDialog();
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
                            gui_menu_principal.Menu_Exit("Desea Salir del Juego?",directorio.Menu, "Crash Bandicoot");
                            msg_box_app_exit = true;
                            break;

                        default:
                            break;
                    }
                    break;

                default:
                    break;
            }
            gui_menu_principal.Render();
            PostRender();
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

                TgcMesh luz = escenario.obtenerFuenteLuzCercana(mesh.BoundingBox.calculateBoxCenter(), 2500f);

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
                                /*mesh.Effect = effectLuzComun;
                                
                                mesh.Technique = TgcShaders.Instance.getTgcMeshTechnique(mesh.RenderType);
                                mesh.Effect.SetValue("lightPosition", TGCVector3.Vector3ToFloat4Array(luz.Position));
                                mesh.Effect.SetValue("eyePosition", TGCVector3.Vector3ToFloat4Array(Camara.Position));
                                //mesh.Effect.SetValue("lightIntensity", 20f);
                                //mesh.Effect.SetValue("lightAttenuation", 0.3f);
                                mesh.Effect.SetValue("ambientColor", ColorValue.FromColor(Color.FromArgb(50, 50, 50)));
                                mesh.Effect.SetValue("diffuseColor", ColorValue.FromColor(Color.White));
                                mesh.Effect.SetValue("specularColor", ColorValue.FromColor(Color.DimGray));
                                mesh.Effect.SetValue("specularExp", 500f);*/
                            }
                        }
                        meshesConLuz.Add(mesh);
                    }
                }
                //mesh.Technique = "RenderScene2";
            }
            //personajeLightShader = TgcShaders.Instance.TgcSkeletalMeshPointLightShader;
           // personaje.effect(personajeLightShader);
            //personaje.technique(TgcShaders.Instance.getTgcSkeletalMeshTechnique(personaje.renderType()));
        }

        public void inicializarSprites(Microsoft.DirectX.Direct3D.Device d3dDevice)
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


            pauseSprite = new CustomSprite();
            pauseSprite.Bitmap = new CustomBitmap(directorio.Paused, d3dDevice);
            pauseSprite.Position = new TGCVector2(ScreenRes_X/2- pauseSprite.Bitmap.Size.Width/2, ScreenRes_Y/2 - pauseSprite.Bitmap.Size.Height/2);

            soundOnSprite = new CustomSprite();
            soundOnSprite.Bitmap = new CustomBitmap(directorio.SoundOn, d3dDevice);
            soundOnSprite.Position = new TGCVector2(ScreenRes_X - soundOnSprite.Bitmap.Size.Width, 0f);
            

            soundOffSprite = new CustomSprite();
            soundOffSprite.Bitmap = new CustomBitmap(directorio.SoundOff, d3dDevice);
            soundOffSprite.Position = new TGCVector2(ScreenRes_X - soundOffSprite.Bitmap.Size.Width, 0f);
            soundOffSprite.Scaling = new TGCVector2(1f, 1.1f);

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