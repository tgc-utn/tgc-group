using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.Utils;
using TGC.Camara;
using TGC.Core.Collision;
using TGC.UtilsGroup;
using TGC.Core.Geometry;
using System;
using TGC.Core.BoundingVolumes;
using TGC.Core.Shaders;
using Microsoft.DirectX.Direct3D;
using TGC.Core;
using TGC.Core.Sound;
using System.Windows.Forms;

namespace TGC.Group.Model
{
    //Twisted Chano, juego de autos chocadores
    public class GameModel : TgcExample
    {
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        //Variables de presentación de juego
        private const float VELOCIDAD_CAMARA_PRESENTACION = 3;
        private float camaraPosH = 0;
        private float camaraPosV = 0;
        private float camaraDireccion = 1;
        private float camaraActivaH = 1;
        private float camaraActivaV = 1;
        private CamaraTW CamaraInit;
        public bool ModoPresentacion = true;
        private HUDMenu claseMenu;

        //Cantidad de filas
        private const int ROWS = 15;

        //Cantidad de columnas
        private const int COLUMNS = 15;

        //Tamaño cuadrante
        private const int CUADRANTE_SIZE = 1200;

        //Posicion vertices
        private const int POSICION_VERTICE = 9000;

        //Boleano para ver si dibujamos el boundingbox
        private bool BoundingBox { get; set; }

        //Nombre del jugador 1 que se dibujara en pantalla
        public string NombreJugador1 = "humano";

        //Scene principal
        public static TgcScene ScenePpal;

        //Cantidad de autos enemigos
        public int CantidadDeOponentes = 1;

        //Cantidad de tiempo de juego
        public int TiempoDeJuego = 5;

        //Tipo de cámara
        private int TipoCamara = 0;

        //Lista de Autos
        public static List<TgcMesh> MeshAutos;
        public static List<Auto> ListaMeshAutos;

        //Lista de palmeras
        public static List<TgcMesh> MeshPalmeras;
        private TgcMesh PalmeraOriginal;

        //Lista de pinos
        public static List<TgcMesh> MeshPinos;
        private TgcMesh PinoOriginal;

        //Lista de rocas
        public static List<TgcMesh> MeshRocas;
        private TgcMesh RocaOriginal;

        //Lista de bananas
        public static List<TgcMesh> MeshArbolesBananas;
        private TgcMesh ArbolBananasOriginal;

        //Lista de Postes de luz
        public static List<TgcMesh> MeshPDL;
        private TgcMesh PDLOriginal;

        //Lista de power ups de vida
        public static List<TgcMesh> MeshPowerUpsVida;
        private TgcMesh PowerUpVidaOriginal;

        //Jugadores
        private List <Jugador> listaJugadores;

        //Tiempo
        private HUDTiempo claseTiempo;

        //Variable de fin de modelo
        public bool finModelo { get; set; }
        public bool restartModelo { get; set; }

        //Quadtree
        private Quadtree quadtree;

        //Gano alguien
        public static bool finReloj = false;
        public static int nroGanador = 0;

        //Luna
        private TgcSphere sphereLuna;
        private Microsoft.DirectX.Direct3D.Effect lunaEffect;
        private TgcBox luzLunaMesh;

        //Shadows
        private readonly int SHADOWMAP_SIZE = 1024;
        private float far_plane = 25000f;
        private readonly float near_plane = 1f;
        private Vector2 posicionLuzEnAuto = new Vector2(20, 0);
        private Vector3 g_LightPos;
        private Vector3 g_LightDir;
        private Microsoft.DirectX.Direct3D.Effect shadowEffect;
        private Matrix g_LightView;
        private Matrix g_mShadowProj;
        private Surface g_pDSShadow;
        private Texture g_pShadowMap;

        //Shadows Salto
        private Vector2 posicionLuzSobreAuto = new Vector2(20, 0);
        private Vector3 g_LightPosSalto = new Vector3 (0, 0, 0);
        private Vector3 g_LightDirSalto = new Vector3(0, 0, 0);
        private Matrix g_LightViewSalto;
        private Surface g_pDSShadowSalto;
        private Texture g_pShadowMapSalto;

        //Reflejo
        private Microsoft.DirectX.Direct3D.Effect envMapEffect;
        private CubeTexture g_pCubeMap;
        private float FPS_EnvMap = 0;

        //PowerUps
        private Microsoft.DirectX.Direct3D.Effect powerUpEffect;
        private float TiempoRotacion = 0;

        private float currentMoveDirPowerUpVida = 1f;

        public override void Init()
        {
            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;
            var loader = new TgcSceneLoader();

            ////////////////////////////////////////////////////////////////////////////////////////
            //Cargar Shader personalizado
            this.powerUpEffect = TgcShaders.loadEffect(MediaDir + "Shaders\\PowerUps.fx");

            //Cargar Shader personalizado
            this.shadowEffect = TgcShaders.loadEffect(MediaDir + "Shaders\\ShadowMap.fx");
            
            //Cargar Shader personalizado
            this.envMapEffect = TgcShaders.loadEffect(MediaDir + "Shaders\\EnvMap.fx");

            //Cargar Shader personalizado
            this.lunaEffect = TgcShaders.loadEffect(MediaDir + "Shaders\\MultiDiffuseLights.fx");

            //Cargar variables de shader de envmap
            this.envMapEffect.SetValue("fvLightPosition", new Vector4(0, 2150, 0, 0));
            this.envMapEffect.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat3Array(Camara.Position));
            this.envMapEffect.SetValue("kx", 0.5f);
            this.envMapEffect.SetValue("kc", 0.5f);
            this.envMapEffect.SetValue("usar_fresnel", true);

            //Creo la textura para envmap
            g_pCubeMap = new CubeTexture(D3DDevice.Instance.Device, 256, 1, Usage.RenderTarget, Format.A16B16G16R16F, Pool.Default);

            //--------------------------------------------------------------------------------------
            // Creo el shadowmap.
            // Format.R32F
            // Format.X8R8G8B8
            g_pShadowMap = new Texture(D3DDevice.Instance.Device, SHADOWMAP_SIZE, SHADOWMAP_SIZE,
                1, Usage.RenderTarget, Format.R32F,
                Pool.Default);

            // tengo que crear un stencilbuffer para el shadowmap manualmente
            // para asegurarme que tenga la el mismo tamano que el shadowmap, y que no tenga
            // multisample, etc etc.
            g_pDSShadow = D3DDevice.Instance.Device.CreateDepthStencilSurface(SHADOWMAP_SIZE,
                SHADOWMAP_SIZE,
                DepthFormat.D24S8,
                MultiSampleType.None,
                0,
                true);
            // por ultimo necesito una matriz de proyeccion para el shadowmap, ya
            // que voy a dibujar desde el pto de vista de la luz.
            // El angulo tiene que ser mayor a 45 para que la sombra no falle en los extremos del cono de luz
            // de hecho, un valor mayor a 90 todavia es mejor, porque hasta con 90 grados es muy dificil
            // lograr que los objetos del borde generen sombras
            var aspectRatio = D3DDevice.Instance.AspectRatio;
            g_mShadowProj = Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(80), aspectRatio, 50, 5000);
            D3DDevice.Instance.Device.Transform.Projection =
                Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(45.0f), aspectRatio, near_plane, far_plane);
            ////////////////////////////////////////////////////////////////////////////////////////

            //--------------------------------------------------------------------------------------
            // Creo el shadowmap.
            // Format.R32F
            // Format.X8R8G8B8
            g_pShadowMapSalto = new Texture(D3DDevice.Instance.Device, SHADOWMAP_SIZE, SHADOWMAP_SIZE,
                1, Usage.RenderTarget, Format.R32F,
                Pool.Default);
            // tengo que crear un stencilbuffer para el shadowmap manualmente
            // para asegurarme que tenga la el mismo tamano que el shadowmap, y que no tenga
            // multisample, etc etc.
            g_pDSShadowSalto = D3DDevice.Instance.Device.CreateDepthStencilSurface(SHADOWMAP_SIZE,
                SHADOWMAP_SIZE,
                DepthFormat.D24S8,
                MultiSampleType.None,
                0,
                true);
            ////////////////////////////////////////////////////////////////////////////////////////

            //Cargo la clase de Tiempo
            this.claseTiempo = new HUDTiempo(MediaDir, this.TiempoDeJuego);

            //Cargo el terreno
            ScenePpal = loader.loadSceneFromFile(MediaDir + "MAPA2-TgcScene.xml");

            TransformarMeshScenePpal(0, 3, POSICION_VERTICE);
            TransformarMeshScenePpal(1, 3, POSICION_VERTICE);
            TransformarMeshScenePpal(2, 3, POSICION_VERTICE);
            TransformarMeshScenePpal(3, 3, POSICION_VERTICE);
            TransformarMeshScenePpal(4, 3, POSICION_VERTICE);
            TransformarMeshScenePpal(5, 3, POSICION_VERTICE);

            //Crear caja para luz de PDL
            GameModel.MeshPDL = new List<TgcMesh>();
            this.luzLunaMesh = TgcBox.fromSize(new Vector3(0, 10, 0));
            this.luzLunaMesh.AutoTransformEnable = true;
            this.luzLunaMesh.Color = Color.White;
            this.luzLunaMesh.move(new Vector3(0, 2000, 0));

            //var pisoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Textures\\block10d.jpg");
            TgcMesh instanciaPDL_1;

            PDLOriginal = loader.loadSceneFromFile(MediaDir + "Objetos\\PosteDeLuz\\PosteDeLuz-TgcScene.xml").Meshes[0];
            instanciaPDL_1 = PDLOriginal.createMeshInstance(PDLOriginal.Name + "_1");
            instanciaPDL_1.AutoTransformEnable = true;
            instanciaPDL_1.move(new Vector3(32, 0, 0));
            instanciaPDL_1.BoundingBox.setExtremes(new Vector3(0, 0, 0), new Vector3(10, 70, 10));
            instanciaPDL_1.BoundingBox.move(new Vector3(50, 0, 0));
            //GameModel.MeshPDL.Add(instanciaPDL_1);            
            ////////////////////////////////////////////////////////////////////////////////////////

            //Cargo los jugadores y sus autos
            this.CrearJugadores(loader);

            //Creo los objetos del escenario
            this.CrearObjetos(loader);

            foreach (TgcMesh unMesh in GameModel.ScenePpal.Meshes)
            {
                unMesh.Effect = this.shadowEffect;
            }

            foreach (TgcMesh unMesh in GameModel.MeshPDL)
            {
                unMesh.Effect = this.shadowEffect;
            }

            foreach (TgcMesh unMesh in GameModel.MeshPowerUpsVida)
            {
                unMesh.Effect = this.powerUpEffect;
                unMesh.Technique = "RenderScene";
            }

            /*
            foreach (var unJugador in this.listaJugadores)
            {
                unJugador.claseAuto.GetMesh().D3dMesh.ComputeNormals();
            }
            */

            //Crear Quadtree
            List<TgcMesh> listaMeshesQ = new List<TgcMesh>();
            listaMeshesQ.AddRange(MeshPalmeras);
            listaMeshesQ.AddRange(MeshPinos);
            listaMeshesQ.AddRange(MeshRocas);
            listaMeshesQ.AddRange(MeshArbolesBananas);
            //listaMeshesQ.AddRange(MeshPDL);
            listaMeshesQ.AddRange(GameModel.ScenePpal.Meshes);
            listaMeshesQ.AddRange(MeshPowerUpsVida);

            quadtree = new Quadtree();
            quadtree.create(listaMeshesQ, ScenePpal.BoundingBox);

            //Inicio camara de presentacion
            CamaraInit = new CamaraTW (this.listaJugadores[0].claseAuto.GetPosition());
            Camara = CamaraInit.GetCamera();

            //Crear caja para indicar ubicacion de la luz luna
            this.sphereLuna = new TgcSphere();
            this.sphereLuna.AutoTransformEnable = true;
            this.sphereLuna.Radius = 500;
            this.sphereLuna.Position = new Vector3(0, 2400, 0);
            this.sphereLuna.LevelOfDetail = 4;
            this.sphereLuna.BasePoly = TgcSphere.eBasePoly.ICOSAHEDRON;
            this.sphereLuna.setTexture(TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "\\Textures\\luna.jpg"));
            this.sphereLuna.updateValues();
            ////////////////////////////////////////////////////////////////////////////////////////

            //Inicio clase de menu
            this.claseMenu = new HUDMenu(MediaDir);
            GameModel.finReloj = false;
        }

        private void TransformarMeshScenePpal (int index, float escala, float desplazamiento)
        {
            var unMesh = ScenePpal.Meshes[index];

            unMesh.AutoTransformEnable = false;

            unMesh.Transform = unMesh.Transform * Matrix.Scaling(new Vector3(escala, 1, escala))
                                                                * Matrix.Translation(new Vector3((-1) * desplazamiento, 0, (-1) * desplazamiento));
            unMesh.BoundingBox.transform(unMesh.Transform);

            //unMesh.AlphaBlendEnable = true;
        }

        private void CrearObjetos(TgcSceneLoader loader)
        {
            int[,] MatrizPoblacion;

            //Creo palmeras
            MatrizPoblacion = RandomMatrix(0,2);
            PalmeraOriginal = loader.loadSceneFromFile(MediaDir + "Vegetacion\\Palmera\\Palmera-TgcScene.xml").Meshes[0];
            GameModel.MeshPalmeras = CrearInstancias(PalmeraOriginal, 0.75f, 0.25f, 1, MatrizPoblacion);

            //Creo pinos
            MatrizPoblacion = RandomMatrix(0, 2);
            PinoOriginal = loader.loadSceneFromFile(MediaDir + "Vegetacion\\Pino\\Pino-TgcScene.xml").Meshes[0];
            GameModel.MeshPinos = CrearInstancias(PinoOriginal, 0.90f, -0.05f, 1, MatrizPoblacion);

            //Creo rocas
            MatrizPoblacion = RandomMatrix(0, 2);
            RocaOriginal = loader.loadSceneFromFile(MediaDir + "Vegetacion\\Roca\\Roca-TgcScene.xml").Meshes[0];
            GameModel.MeshRocas = CrearInstancias(RocaOriginal, 0.75f, 0.30f, 1, MatrizPoblacion);

            //Creo arboles bananas
            MatrizPoblacion = RandomMatrix(0, 2);
            ArbolBananasOriginal = loader.loadSceneFromFile(MediaDir + "Vegetacion\\ArbolBananas\\ArbolBananas-TgcScene.xml").Meshes[0];
            GameModel.MeshArbolesBananas = CrearInstancias(ArbolBananasOriginal, 1.50f, 0.15f, 1, MatrizPoblacion);

            //Creo PowerUps
            MatrizPoblacion = RandomMatrix(0, 6);
            PowerUpVidaOriginal = loader.loadSceneFromFile(MediaDir + "PowerUps\\PowerUpVida\\PowerUpVida-TgcScene.xml").Meshes[0];
            GameModel.MeshPowerUpsVida = CrearInstancias(PowerUpVidaOriginal, 1, 50f, 1, MatrizPoblacion);
        }

        private int[,] RandomMatrix(int minValue, int maxValue)
        {
            int[,] MatrizPoblacion = new int[ROWS, COLUMNS];
            System.Random randomNumber = new System.Random();

            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLUMNS; j++)
                {
                    MatrizPoblacion[i, j] = randomNumber.Next(minValue, maxValue);
                }
            }

            return MatrizPoblacion;
        }

        private void CrearJugadores(TgcSceneLoader loader)
        {
            System.Random randomNumber = new System.Random();
            //TgcMesh instanciaPDL_1, instanciaPDL_2, instanciaPDL_3, instanciaPDL_4, instanciaPDL_5;

            //Creo la lista de jugadores y sus autos
            GameModel.MeshAutos = new List<TgcMesh>();
            GameModel.ListaMeshAutos = new List<Auto>();
            this.listaJugadores = new List<Jugador>();
            this.listaJugadores.Add(new Jugador(this.NombreJugador1, MediaDir, 0, DirectSound));
            this.listaJugadores[0].claseAuto.SetMesh(loader.loadSceneFromFile(MediaDir + "Vehiculos\\Auto\\Auto-TgcScene.xml").Meshes[0]);
            this.listaJugadores[0].claseAuto.SetRuedas(loader);
            this.listaJugadores[0].CreateCamera();

            GameModel.MeshAutos.Add(this.listaJugadores[0].claseAuto.GetMesh());
            GameModel.ListaMeshAutos.Add(this.listaJugadores[0].claseAuto);

            if (CantidadDeOponentes >= 1)
            {
                //Crear instancia de modelo
                listaJugadores.Add(new Jugador("gris", MediaDir, 1, DirectSound));
                this.listaJugadores[1].claseAuto.SetMesh(loader.loadSceneFromFile(MediaDir + "Vehiculos\\AutoGris\\Auto-TgcScene.xml").Meshes[0]);
                this.listaJugadores[1].claseAuto.SetPositionMesh(new Vector3((-1) * (POSICION_VERTICE - CUADRANTE_SIZE * 2), 0, (POSICION_VERTICE - CUADRANTE_SIZE * 2)), false);
                this.listaJugadores[1].claseAuto.SetRuedas(loader);
                this.listaJugadores[1].CreateCamera();
                GameModel.MeshAutos.Add(this.listaJugadores[1].claseAuto.GetMesh());
                GameModel.ListaMeshAutos.Add(this.listaJugadores[1].claseAuto);

                /*
                instanciaPDL_2 = PDLOriginal.createMeshInstance(PDLOriginal.Name + "_2");
                instanciaPDL_2.AutoTransformEnable = true;
                instanciaPDL_2.move(new Vector3(((-1) * (POSICION_VERTICE - CUADRANTE_SIZE * 4)) + 20, 0, (POSICION_VERTICE - CUADRANTE_SIZE * 4)));
                instanciaPDL_2.BoundingBox.setExtremes(new Vector3(0, 0, 0), new Vector3(10, 70, 10));
                instanciaPDL_2.BoundingBox.move(new Vector3(((-1) * (POSICION_VERTICE - CUADRANTE_SIZE * 4)) + 45, 0, (POSICION_VERTICE - CUADRANTE_SIZE * 4)));
                GameModel.MeshPDL.Add(instanciaPDL_2);
                */
            }

            if (CantidadDeOponentes >= 2)
            {
                listaJugadores.Add(new Jugador("verde", MediaDir, 2, DirectSound));
                this.listaJugadores[2].claseAuto.SetMesh(loader.loadSceneFromFile(MediaDir + "Vehiculos\\AutoVerde\\Auto-TgcScene.xml").Meshes[0]);
                this.listaJugadores[2].claseAuto.SetPositionMesh(new Vector3(POSICION_VERTICE - CUADRANTE_SIZE * 2, 0, POSICION_VERTICE - CUADRANTE_SIZE * 2), false);
                this.listaJugadores[2].claseAuto.SetRuedas(loader);
                this.listaJugadores[2].CreateCamera();
                GameModel.MeshAutos.Add(this.listaJugadores[2].claseAuto.GetMesh());
                GameModel.ListaMeshAutos.Add(this.listaJugadores[2].claseAuto);
            }

            if (CantidadDeOponentes >= 3)
            {
                listaJugadores.Add(new Jugador("rojo", MediaDir, 3, DirectSound));
                this.listaJugadores[3].claseAuto.SetMesh(loader.loadSceneFromFile(MediaDir + "Vehiculos\\AutoRojo\\Auto-TgcScene.xml").Meshes[0]);
                this.listaJugadores[3].claseAuto.SetPositionMesh(new Vector3((POSICION_VERTICE - CUADRANTE_SIZE * 2), 0, (-1) * (POSICION_VERTICE - CUADRANTE_SIZE * 2)), true);
                this.listaJugadores[3].claseAuto.SetRuedas(loader);
                this.listaJugadores[3].CreateCamera();
                GameModel.MeshAutos.Add(this.listaJugadores[3].claseAuto.GetMesh());
                GameModel.ListaMeshAutos.Add(this.listaJugadores[3].claseAuto);
            }

            if (CantidadDeOponentes >= 4)
            {
                listaJugadores.Add(new Jugador("marrón", MediaDir, 4, DirectSound));
                this.listaJugadores[4].claseAuto.SetMesh(loader.loadSceneFromFile(MediaDir + "Vehiculos\\AutoMarron\\Auto-TgcScene.xml").Meshes[0]);
                this.listaJugadores[4].claseAuto.SetPositionMesh(new Vector3((-1) * (POSICION_VERTICE - CUADRANTE_SIZE * 2), 0, (-1) * (POSICION_VERTICE - CUADRANTE_SIZE * 2)), true);
                this.listaJugadores[4].claseAuto.SetRuedas(loader);
                this.listaJugadores[4].CreateCamera();
                GameModel.MeshAutos.Add(this.listaJugadores[4].claseAuto.GetMesh());
                GameModel.ListaMeshAutos.Add(this.listaJugadores[4].claseAuto);
            }
        }

        private List<TgcMesh> CrearInstancias(TgcMesh unObjeto, float scale, float ejeZ, int cantidadObjetos, int[,] MatrizPoblacion)
        {
            List<TgcMesh> ListaMesh = new List<TgcMesh>();
            System.Random randomNumber = new System.Random();
            Matrix unaBoundingBoxMatrix;
            Matrix unaTranslation;

            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLUMNS; j++)
                {
                    if (MatrizPoblacion[i, j] == 1)
                    {
                        for (int z = 0; z < cantidadObjetos; z++)
                        {
                            //Crear instancia de modelo
                            var instance = unObjeto.createMeshInstance(unObjeto.Name + i + "_" + j);

                            instance.AutoTransformEnable = false;
                            instance.AutoUpdateBoundingBox = true;

                            //Roto el objeto aleatoriamente
                            instance.Transform = Matrix.RotationY((randomNumber.Next(1, 180)) * FastMath.PI / 180);

                            //Calculo el tamaño del bounding box
                            unaBoundingBoxMatrix = instance.Transform * Matrix.Scaling(new Vector3(0.15f, 0.8f, 0.15f)) *
                                                                       Matrix.Translation(new Vector3(POSICION_VERTICE, ejeZ, (-1) * POSICION_VERTICE));

                            //Lo agrando y traslado al borde del terreno
                            instance.Transform = instance.Transform * Matrix.Scaling(new Vector3(scale, scale, scale)) *
                                                                       Matrix.Translation(new Vector3(POSICION_VERTICE, ejeZ, (-1) * POSICION_VERTICE));

                            //Calculo la matriz de traslación aleatoria
                            unaTranslation = Matrix.Translation(new Vector3((-1) * randomNumber.Next(j * CUADRANTE_SIZE, (j + 1) * CUADRANTE_SIZE), 0,
                                                                 randomNumber.Next(i * CUADRANTE_SIZE, (i + 1) * CUADRANTE_SIZE)));

                            //Lo posiciono en una posición aleatoria
                            instance.Transform = instance.Transform * unaTranslation;

                            //Posiciono el bounding box alterado en el mismo lugar que el mesh
                            unaBoundingBoxMatrix = unaBoundingBoxMatrix * unaTranslation;

                            //Muevo el bounding box
                            instance.BoundingBox.transform(instance.Transform);

                            //Valido si pisa a otro objeto que ya existe
                            if (ValidarColisionCrearInstancias(instance, ListaMesh))
                            {
                                //Hubo colision, no creo el objeto
                                instance.dispose();
                                continue;
                            }
                            ///////////////////////////////////////////////////////////////////////////////////

                            //Para determinados objetos, le meto el bounding alterado
                            if ((unObjeto.Name == "Palmera") || (unObjeto.Name == "Pino") || (unObjeto.Name == "ArbolBananas"))
                            {
                                instance.BoundingBox.transform(unaBoundingBoxMatrix);

                                //Le activo el alpha para que se vea mejor
                                instance.AlphaBlendEnable = true;
                            }

                            //Cargo el efecto de sombra
                            instance.Effect = this.shadowEffect;

                            //Lo agrego a la lista para después renderizarlo
                            ListaMesh.Add(instance);
                        }
                    }
                }
            }

            return ListaMesh;
        }

        private bool ValidarColisionCrearInstancias(TgcMesh unaInstancia, List<TgcMesh> ListaMeshActual)
        {
            //Valido la colisión para cada lista de objetos que tenga
            if (AccionarListaMesh(ListaMeshActual, 2, unaInstancia) ||
                AccionarListaMesh(MeshPalmeras, 2, unaInstancia) ||
                AccionarListaMesh(MeshPinos, 2, unaInstancia) ||
                AccionarListaMesh(MeshRocas, 2, unaInstancia) ||
                AccionarListaMesh(MeshArbolesBananas, 2, unaInstancia) ||
                AccionarListaMesh(MeshAutos, 2, unaInstancia) ||
                AccionarListaMesh(GameModel.ScenePpal.Meshes, 2, unaInstancia) ||
                AccionarListaMesh(MeshPowerUpsVida,2,unaInstancia)
                )
            {
                return true;
            }

            return false;
        }

        public override void Update()
        {
            bool MoverRuedas = false, Avanzar = false, Frenar = false, Izquierda = false, Derecha = false, Saltar = false;

            PreUpdate();

            if (this.ModoPresentacion)
            {
                if (Input.keyPressed(Key.Up))
                {
                    this.claseMenu.SetPosicionMenu(-1);
                }

                if (Input.keyPressed(Key.Down))
                {
                    this.claseMenu.SetPosicionMenu(1);
                }

                if (Input.keyPressed(Key.NumPadEnter) || Input.keyPressed(Key.Return))
                {
                    if (this.claseMenu.GetEstadoMenu() == "E")
                    {
                        this.ModoPresentacion = false;
                        this.listaJugadores[0].ActualizarNombreJugador(this.claseMenu.GetNombreJugador());
                        this.NombreJugador1 = this.claseMenu.GetNombreJugador();
                        Camara = this.listaJugadores[0].claseCamara.GetCamera();
                        this.listaJugadores[0].claseAuto.ReproducirSonidoArranque();
                        System.Threading.Thread.Sleep(2000);
                        this.listaJugadores[0].claseAuto.ReproducirSonidoMotor();

                    }

                    if (this.claseMenu.GetEstadoMenu() == "S")
                    {
                        this.finModelo = true;
                    }

                    if (this.claseMenu.GetEstadoMenu() == "I")
                    {
                        this.claseMenu.SetNroMenu(2);
                    }
                }

                this.claseMenu.EscribirTeclasEnPantalla (Input);
                this.TiempoRotacion = 0;
            }
            else
            {
                //Achico el farplane para mejorar un poco los FPS
                this.far_plane = 3000f;

                //Valido las teclas que se presionaron
                if ((Input.keyDown(Key.Up) || Input.keyDown(Key.W)))
                {
                    Avanzar = true;
                }

                if (Input.keyDown(Key.Left) || Input.keyDown(Key.A))
                {
                    Izquierda = true;
                    MoverRuedas = true;
                }
                else
                {
                    if (Input.keyDown(Key.Right) || Input.keyDown(Key.D))
                    {
                        Derecha = true;
                        MoverRuedas = true;
                    }
                }

                if ((Input.keyDown(Key.Down) || Input.keyDown(Key.S)))
                {
                    Frenar = true;
                }

                if (Input.keyPressed(Key.Space))
                {
                    Saltar = true;
                }

                //Activo bounding box para debug
                ActivarBoundingBox();

                //Chequea si se solicitó cambiar el tipo de camara
                CambiarDeCamara();

                //Actualizo los jugadores
                foreach (var unJugador in this.listaJugadores)
                {
                    if (unJugador.GetNroJugador() == 0)
                    {
                        unJugador.Update(MoverRuedas, Avanzar, Frenar, Izquierda, Derecha, Saltar, (ElapsedTime + 0.01f));
                    }
                    else
                    {
                        //IA
                        unJugador.Seguir(listaJugadores, ElapsedTime);
                        unJugador.Update(false, false, false, false, false, false, ElapsedTime);
                    }
                }
                
                //Actualizo el tiempo
                this.claseTiempo.Update();

                //Calculo quien va ganando hasta ahora
                if (!GameModel.finReloj)
                    this.CalcularGanador();

                //Chequeo el fin del modelo
                if ((GameModel.finReloj || this.claseTiempo.GetFinDeJuego()) && Input.keyDown(Key.X))
                {
                    this.finModelo = true;
                    this.restartModelo = true;
                }
            }

            this.CalcularPosicionLuzAuto();
            this.CalcularPosicionLuzAutoSalto();

        }

        public void RenderShadowMap()
        {
            TgcMesh unMesh;

            // Calculo la matriz de view de la luz
            shadowEffect.SetValue("g_vLightPos", new Vector4(g_LightPos.X, g_LightPos.Y, g_LightPos.Z, 1));
            shadowEffect.SetValue("g_vLightDir", new Vector4(g_LightDir.X, g_LightDir.Y, g_LightDir.Z, 1));
            g_LightView = Matrix.LookAtLH(g_LightPos, g_LightPos + g_LightDir, new Vector3(0, 0, 1));

            shadowEffect.SetValue("g_vLightPosSalto", new Vector4(g_LightPosSalto.X, g_LightPosSalto.Y, g_LightPosSalto.Z, 1));
            shadowEffect.SetValue("g_vLightDirSalto", new Vector4(g_LightDirSalto.X, g_LightDirSalto.Y, g_LightDirSalto.Z, 1));
            g_LightViewSalto = Matrix.LookAtLH(g_LightPosSalto, g_LightPosSalto + g_LightDirSalto, new Vector3(0, 0, 1));

            // inicializacion standard:
            shadowEffect.SetValue("g_mProjLight", g_mShadowProj);
            shadowEffect.SetValue("g_mViewLightProj", g_LightView * g_mShadowProj);
            shadowEffect.SetValue("g_mViewLightProjSalto", g_LightViewSalto * g_mShadowProj);

            // Primero genero el shadow map, para ello dibujo desde el pto de vista de luz
            // a una textura, con el VS y PS que generan un mapa de profundidades.
            var pOldRT = D3DDevice.Instance.Device.GetRenderTarget(0);
            var pShadowSurf = g_pShadowMap.GetSurfaceLevel(0);

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            D3DDevice.Instance.Device.SetRenderTarget(0, pShadowSurf);
            var pOldDS = D3DDevice.Instance.Device.DepthStencilSurface;
            D3DDevice.Instance.Device.DepthStencilSurface = g_pDSShadow;

            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            D3DDevice.Instance.Device.BeginScene();

            //Hago el render de la escena pp dicha
            shadowEffect.SetValue("g_txShadow", g_pShadowMap);

            //Renderizo todos las sombras de todos los objetos del mapa
            quadtree.render(Frustum, false, "RenderShadow", 0, this.shadowEffect);

            //Dibujo las sombras de los jugadores
            foreach (var unJugador in this.listaJugadores)
            {
                unMesh = unJugador.claseAuto.GetMesh();
                unMesh.Effect = this.shadowEffect;
                unMesh.Technique = "RenderShadow";
                unJugador.claseAuto.Render();
            }

            // Termino
            D3DDevice.Instance.Device.EndScene();

            // restuaro el render target y el stencil oara las sombras
            D3DDevice.Instance.Device.DepthStencilSurface = pOldDS;
            pShadowSurf.Dispose();
            D3DDevice.Instance.Device.SetRenderTarget(0, pOldRT);

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            var pShadowSurfSalto = g_pShadowMapSalto.GetSurfaceLevel(0);
            D3DDevice.Instance.Device.SetRenderTarget(0, pShadowSurfSalto);
            var pOldDSSalto = D3DDevice.Instance.Device.DepthStencilSurface;
            D3DDevice.Instance.Device.DepthStencilSurface = g_pDSShadowSalto;

            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            D3DDevice.Instance.Device.BeginScene();

            //Hago el render de la escena pp dicha
            shadowEffect.SetValue("g_txShadowSalto", g_pShadowMapSalto);

            //Renderizo todos las sombras de todos los objetos del mapa
            quadtree.render(Frustum, false, "RenderShadowSalto", 2, this.shadowEffect);

            //Dibujo las sombras del jugador principal
            unMesh = this.listaJugadores[0].claseAuto.GetMesh();
            unMesh.Effect = this.shadowEffect;
            unMesh.Technique = "RenderShadowSalto";
            unMesh.render();

            // Termino
            D3DDevice.Instance.Device.EndScene();

            // restuaro el render target y el stencil
            D3DDevice.Instance.Device.DepthStencilSurface = pOldDS;
            pShadowSurfSalto.Dispose();

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            D3DDevice.Instance.Device.SetRenderTarget(0, pOldRT);
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        }

        public void CalcularPosicionLuzAuto()
        {
            float rohumo, alfa_luz;
            float posicion_xluz;
            float posicion_yluz;

            rohumo = FastMath.Sqrt(this.posicionLuzEnAuto.X * this.posicionLuzEnAuto.X + this.posicionLuzEnAuto.Y * this.posicionLuzEnAuto.Y);

            alfa_luz = FastMath.Asin(this.posicionLuzEnAuto.X / rohumo);
            posicion_xluz = FastMath.Sin(alfa_luz + this.listaJugadores[0].claseAuto.GetMesh().Rotation.Y) * rohumo;
            posicion_yluz = FastMath.Cos(alfa_luz + this.listaJugadores[0].claseAuto.GetMesh().Rotation.Y) * rohumo;

            this.g_LightPos = this.listaJugadores[0].claseAuto.ObbArribaDer.Position + new Vector3(posicion_xluz, 0, posicion_yluz);
            this.g_LightDir = new Vector3((-1) * FastMath.Sin(this.listaJugadores[0].claseAuto.GetMesh().Rotation.Y), 0, (-1) * FastMath.Cos(this.listaJugadores[0].claseAuto.GetMesh().Rotation.Y));
            this.g_LightDir.Normalize();
        }

        public void CalcularPosicionLuzAutoSalto()
        {
            this.g_LightPosSalto = this.listaJugadores[0].claseAuto.ObbMesh.Position + new Vector3(0, 250, 0);
            this.g_LightDirSalto = this.listaJugadores[0].claseAuto.ObbMesh.Position - this.g_LightPosSalto;
            this.g_LightDirSalto.Normalize();
        }

        public override void Render()
        {
            TgcMesh unMesh;
            Surface pOldRT, pFace;

            //Habilito el render de particulas para el humo
            D3DDevice.Instance.ParticlesEnabled = true;
            D3DDevice.Instance.EnableParticles();
            this.TiempoRotacion += ElapsedTime;
            this.powerUpEffect.SetValue("time", this.TiempoRotacion);

            ClearTextures();

            #region RenderEnvMap

            if (this.FPS_EnvMap == float.MaxValue)
                this.FPS_EnvMap = 0;
            else
                this.FPS_EnvMap++;

            pOldRT = D3DDevice.Instance.Device.GetRenderTarget(0);
            // ojo: es fundamental que el fov sea de 90 grados.
            // asi que re-genero la matriz de proyeccion
            D3DDevice.Instance.Device.Transform.Projection = Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(90.0f), 1f, 1f, 200f);

            if (!this.ModoPresentacion)
            {
                if ((this.FPS_EnvMap % 3) == 0)
                {
                    // Genero las caras del enviroment map
                    for (var nFace = CubeMapFace.PositiveX; nFace <= CubeMapFace.NegativeZ; ++nFace)
                    {
                        pFace = g_pCubeMap.GetCubeMapSurface(nFace, 0);
                        D3DDevice.Instance.Device.SetRenderTarget(0, pFace);
                        Vector3 Dir, VUP;
                        Color color;
                        switch (nFace)
                        {
                            case CubeMapFace.PositiveX:
                                // Left
                                Dir = new Vector3(1, 0, 0);
                                VUP = new Vector3(0, 1, 0);
                                color = Color.Black;
                                break;

                            case CubeMapFace.NegativeX:
                                // Right
                                Dir = new Vector3(-1, 0, 0);
                                VUP = new Vector3(0, 1, 0);
                                color = Color.Red;
                                break;

                            case CubeMapFace.PositiveY:
                                // Up
                                Dir = new Vector3(0, 1, 0);
                                VUP = new Vector3(0, 0, -1);
                                color = Color.Gray;
                                break;
                            
                            /*case CubeMapFace.NegativeY:
                                // Down
                                Dir = new Vector3(0, -1, 0);
                                VUP = new Vector3(0, 0, 1);
                                color = Color.Yellow;
                                break;*/

                            case CubeMapFace.PositiveZ:
                                // Front
                                Dir = new Vector3(0, 0, 1);
                                VUP = new Vector3(0, 1, 0);
                                color = Color.Green;
                                break;
                            
                            case CubeMapFace.NegativeZ:
                                // Back
                                Dir = new Vector3(0, 0, -1);
                                VUP = new Vector3(0, 1, 0);
                                color = Color.Blue;
                                break;

                            default:
                                continue;
                        }

                        D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, color, 1.0f, 0);
                        D3DDevice.Instance.Device.BeginScene();

                        this.quadtree.render(Frustum, false, "", 1, null);
                        this.sphereLuna.render();

                        D3DDevice.Instance.Device.EndScene();
                        pFace.Dispose();
                    }
                }
            }

            D3DDevice.Instance.Device.SetRenderTarget(0, pOldRT);

            // Restauro el estado de las transformaciones
            D3DDevice.Instance.Device.Transform.View = Camara.GetViewMatrix();
            D3DDevice.Instance.Device.Transform.Projection = Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(45.0f), D3DDevice.Instance.AspectRatio, near_plane, far_plane);

            //Dibujo el auto espejado
            D3DDevice.Instance.Device.BeginScene();
            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            this.envMapEffect.SetValue("g_txCubeMap", g_pCubeMap);

            foreach (var unJugador in this.listaJugadores)
            {
                unMesh = unJugador.claseAuto.GetMesh();
                unMesh.Effect = this.envMapEffect;
                unMesh.Technique = "RenderCubeMap";
                unJugador.Render();
            }

            D3DDevice.Instance.Device.EndScene();

            #endregion

            #region RenderShadowMap

            //Genero el shadow map
            this.RenderShadowMap();           

            #endregion

            #region RenderEscenaConLuna

            D3DDevice.Instance.Device.BeginScene();

            //Dibuja un texto por pantalla
            this.RenderFPS();
            DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.Red);
            DrawText.drawText("Con la tecla F1 se cambia el tipo de camara. Pos [Actual]: " + TgcParserUtils.printVector3(Camara.Position), 0, 30, Color.Red);
            DrawText.drawText("Direccion: " + TgcParserUtils.printVector3(listaJugadores[1].claseAuto.direccionSeguir), 0, 60, Color.Red);
            DrawText.drawText("Direccion: " + TgcParserUtils.printVector3(listaJugadores[1].claseAuto.direccionASeguir), 0, 80, Color.Red);

            //Renderizo los objetos/bounding box cargados de las listas
            if (BoundingBox)
            {
                RenderizarObjetos(1);

                foreach (var unJugador in this.listaJugadores)
                {
                    unJugador.claseAuto.RenderObb();
                }
            }

            //Renderizo todos los objetos
            quadtree.render(Frustum, false, "RenderScene", 0, this.shadowEffect);

            //Dibujo el reloj o solo muevo la camara para la presentación del juego
            if (!this.ModoPresentacion)
                this.claseTiempo.Render();
            else
            {
                this.ActualizarCamaraPresentacionJuego();
                this.claseMenu.Render();
            }

            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            //PostRender();
            D3DDevice.Instance.Device.EndScene();

            #endregion

            #region Luz de Luna

            D3DDevice.Instance.Device.BeginScene();

            var lightDir = new Vector3(0, -10, 0);
            lightDir.Normalize();

            this.sphereLuna.Effect = this.lunaEffect;
            this.sphereLuna.Technique = "MultiDiffuseLightsTechnique";

            var lightColors = new ColorValue[1];
            var pointLightPositions = new Vector4[1];
            var pointLightIntensity = new float[1];
            var pointLightAttenuation = new float[1];

            lightColors[0] = ColorValue.FromColor(Color.Blue);
            pointLightPositions[0] = TgcParserUtils.vector3ToVector4(this.luzLunaMesh.Position);
            pointLightIntensity[0] = 150;
            pointLightAttenuation[0] = 0.1f;

            this.sphereLuna.Effect.SetValue("lightColor", lightColors);
            this.sphereLuna.Effect.SetValue("lightPosition", pointLightPositions);
            this.sphereLuna.Effect.SetValue("lightIntensity", pointLightIntensity);
            this.sphereLuna.Effect.SetValue("lightAttenuation", pointLightAttenuation);
            this.sphereLuna.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Yellow));
            this.sphereLuna.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));

            this.sphereLuna.render();
            this.luzLunaMesh.render();

            D3DDevice.Instance.Device.EndScene();
            #endregion

            D3DDevice.Instance.Device.Present();
        }

        private void CalcularGanador()
        {
            float cantVida = 0, cantVidaTmp = 0, cantVidaTotal_0 = 0, cantVidaTotal_1 = 0, cantVidaTotal_2 = 0, cantVidaTotal_3 = 0, cantVidaTotal_4 = 0;
            int ganador = 0;

            foreach (Jugador unJugador in this.listaJugadores)
            {
                cantVidaTmp = unJugador.claseHUD.GetVidaJugador();

                if (cantVidaTmp > cantVida)
                {
                    ganador = unJugador.GetNroJugador();
                    cantVida = cantVidaTmp;
                }

                switch (unJugador.GetNroJugador())
                {
                    case 0:
                        cantVidaTotal_0 = cantVidaTmp;
                        break;

                    case 1:
                        cantVidaTotal_1 = cantVidaTmp;
                        break;

                    case 2:
                        cantVidaTotal_2 = cantVidaTmp;
                        break;

                    case 3:
                        cantVidaTotal_3 = cantVidaTmp;
                        break;

                    case 4:
                        cantVidaTotal_4 = cantVidaTmp;
                        break;

                }
            }

            //Actualizo el ganador actual
            GameModel.nroGanador = ganador;

            //Calculo si ganó alguien o el principal perdió
            if ((cantVidaTotal_0 <= 0) || (cantVidaTotal_1 <= 0 && cantVidaTotal_2 <= 0 && cantVidaTotal_3 <= 0 && cantVidaTotal_4 <= 0))
                GameModel.finReloj = true;
        }
        
        private void ActualizarCamaraPresentacionJuego()
        {
            this.camaraPosH += VELOCIDAD_CAMARA_PRESENTACION * camaraDireccion * camaraActivaH;
            this.camaraPosV += VELOCIDAD_CAMARA_PRESENTACION * camaraDireccion * camaraActivaV;

            if ((Math.Abs(camaraPosV) > (POSICION_VERTICE - 2500)) || (Math.Abs(camaraPosH) > (POSICION_VERTICE - 2500)))
            {
                this.camaraDireccion *= -1;
                
                this.camaraActivaV = 0;

                if (camaraPosH > 0)
                {
                    this.camaraPosH -= 5;
                    this.camaraPosV -= 5;
                }
                else
                {
                    this.camaraPosH += 5;
                    this.camaraPosV += 5;
                }
            }

            this.CamaraInit.AjustarPosicionDeCamara(new Vector3(camaraPosH, 1200, camaraPosV), 0);
        }

        //0: render
        //1: render y render bounding box
        //2: control de colisiones
        //3: render bounding box
        private bool AccionarListaMesh(List<TgcMesh> unaListaMesh, int unaAccion, TgcMesh unaInstancia)
        {
            //Si no está sin inicializar, realizo una acción
            if (unaListaMesh != null)
            {
                foreach (var mesh in unaListaMesh)
                {
                    switch (unaAccion)
                    {
                        case 0:
                            {
                                mesh.render(); 
                            }
                            break;

                        case 1:
                            {
                                mesh.BoundingBox.render();
                            }
                            break;

                        case 2:
                            {

                                if (mesh != unaInstancia)
                                {
                                    if (TgcCollisionUtils.classifyBoxBox(mesh.BoundingBox, unaInstancia.BoundingBox) != TgcCollisionUtils.BoxBoxResult.Afuera)
                                    {
                                        return true;
                                    }
                                }
                            }
                            break;
                    }
                }
            }

            return false;
        }

        private void RenderizarObjetos(int unaAccion)
        {
            //Renderizar objetos del escenario
            AccionarListaMesh(GameModel.ScenePpal.Meshes, unaAccion, null);

            //Renderizar palmeras
            AccionarListaMesh(MeshPalmeras, unaAccion, null);

            //Renderizar arbol bananas
            AccionarListaMesh(MeshArbolesBananas, unaAccion, null);
            
            //Renderizar pinos
            AccionarListaMesh(MeshPinos, unaAccion, null);

            //Renderizar rocas
            AccionarListaMesh(MeshRocas, unaAccion, null);

            //Renderizar PDL
            AccionarListaMesh(MeshPDL, unaAccion, null);

            //Rederizar Power ups de vida
            AccionarListaMesh(MeshPowerUpsVida, unaAccion, null);
        }

        public void ActivarBoundingBox()
        {
            //Capturar Input teclado
            if (Input.keyPressed(Key.F))
            {
                //Activa o desactiva el Bounding Box
                BoundingBox = !BoundingBox;
            }
        }

        public void CambiarDeCamara()
        {
            if (Input.keyPressed(Key.F1))
            {
                TipoCamara++;

                if (TipoCamara >= 2)
                    TipoCamara = 0;

                switch (TipoCamara)
                {
                    case 0:
                        {
                            Camara = this.listaJugadores[0].claseCamara.GetCamera();
                        }
                        break;

                    case 1:
                        {
                            Camara = new TgcRotationalCamera (this.listaJugadores[0].claseAuto.GetBBCenter(), this.listaJugadores[0].claseAuto.GetBBRadius() * 2, Input);
                        }
                        break;
                }
            }
        }

        public override void Dispose()
        {
            //Borro los objetos de los jugadores
            foreach (var unJugador in this.listaJugadores)
            {
                unJugador.Dispose();
            }

            PalmeraOriginal.dispose();
            PinoOriginal.dispose();
            RocaOriginal.dispose();
            ArbolBananasOriginal.dispose();
            PDLOriginal.dispose();
            PowerUpVidaOriginal.dispose();
            this.g_pCubeMap.Dispose();
            this.g_pShadowMap.Dispose();
            this.g_pDSShadow.Dispose();
            this.lunaEffect.Dispose();
            this.luzLunaMesh.dispose();
            this.shadowEffect.Dispose();
            this.envMapEffect.Dispose();
            ScenePpal.disposeAll();
        }
    }
}