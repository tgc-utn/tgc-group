//"Inclusion de librerias"
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

namespace TGC.Group.Model
{
    /// <summary>
    ///     Twisted Chano, juego de autos chocadores
    /// </summary>
    public class GameModel : TgcExample
    {
        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        /// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
        /// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        /// <summary>
        /// Inicializacion de variables y "definicion" de los objetos
        /// </summary>
        ///

        //Posicion del piso
        float piso;

        //Altura del salto
        private const float ALTURA_SALTO = 225f;

        //Posición Y de las barras de vida
        private const int POSICION_Y_BARRA_VIDA = 40;

        //Posición X de las barras de vida
        private const int POSICION_X_BARRA_VIDA = 110;

        //Velocidad de movimiento del auto
        private float MOVEMENT_SPEED = 0f;

        //Rozamiento del piso
        private float ROZAMIENTO = 100f;

        //Velocidad Maxima
        private const float MAX_SPEED = 1200f;

        //Velocidad de rotación del auto
        private const float ROTATION_SPEED = 120f;

        //Cantidad de filas
        private const int ROWS = 30;

        //Cantidad de columnas
        private const int COLUMNS = 30;

        //Tamaño cuadrante
        private const int CUADRANTE_SIZE = 600;

        //Posicion vertices
        private const int POSICION_VERTICE = 9000;

        //Scene principal
        private TgcScene ScenePpal;

        //Mesh del auto
        private TgcMesh Mesh { get; set; }

        //BoudingBox Obb del auto.
        private TgcBoundingOrientedBox ObbMesh;

        //Boleano para ver si dibujamos el boundingbox
        private bool BoundingBox { get; set; }

        //Nombre del jugador 1 que se dibujara en pantalla
        public string NombreJugador1 { get;  set; }

        //Cantidad de autos enemigos
        public int CantidadDeOponentes { get; set; }

        //Cantidad de autos enemigos
        public int TiempoDeJuego { get; set; }

        //Camara en tercera persona
        private TgcThirdPersonCamera CamaraInterna;

        //Tipo de cámara
        private int TipoCamara = 0;

        //Lista de Autos
        private List<TgcMesh> MeshAutos;

        //Lista de palmeras
        private List<TgcMesh> MeshPalmeras;
        private TgcMesh PalmeraOriginal;

        //Lista de pinos
        private List<TgcMesh> MeshPinos;
        private TgcMesh PinoOriginal;

        //Lista de rocas
        private List<TgcMesh> MeshRocas;
        private TgcMesh RocaOriginal;

        //Lista de rocas
        private List<TgcMesh> MeshArbolesBananas;
        private TgcMesh ArbolBananasOriginal;

        //Lista de barriles de polvora
        private List<TgcMesh> MeshBarrilesPolvora;
        private TgcMesh BarrilPolvoraOriginal;

        //Lista de carretillas
        private List<TgcMesh> MeshCarretillas;
        private TgcMesh CarretillaOriginal;

        //Lista de contenedores
        private List<TgcMesh> MeshContenedores;
        private TgcMesh ContenedorOriginal;

        //Lista de fuentes de agua
        //private List<TgcMesh> MeshFuentesAgua;
        //private TgcMesh FuenteAguaOriginal;

        //Lista de lockers
        private List<TgcMesh> MeshLockers;
        private TgcMesh LockerOriginal;

        //Lista de expendedores bebidas
        private List<TgcMesh> MeshExpendedoresBebidas;
        private TgcMesh ExpendedorBebidaOriginal;

        //Lista de cajas de municiones
        private List<TgcMesh> MeshCajasMuniciones;
        private TgcMesh CajaMunicionesOriginal;

        //Lista de objetos del mesh principal
        private List<TgcMesh> MeshPrincipal;

        //Fuente para los jugadores
        private TgcDrawText letraJugadores;

        //Sprites para las barras de vida
        private TgcDrawer2D drawerBarras;
        private TgcSprite spriteBarraJugador1;
        private TgcSprite spriteBarraJugador1Llena;

        private TgcSprite spriteBarraJugador2;
        private TgcSprite spriteBarraJugador2Llena;

        private TgcSprite spriteBarraJugador3;
        private TgcSprite spriteBarraJugador3Llena;

        private TgcSprite spriteBarraJugador4;
        private TgcSprite spriteBarraJugador4Llena;

        private TgcSprite spriteBarraJugador5;
        private TgcSprite spriteBarraJugador5Llena;

        //Vida Inicial de cada jugador
        private float cantVidaJugador1 = 100;
        private float cantVidaJugador2 = 100;
        private float cantVidaJugador3 = 100;
        private float cantVidaJugador4 = 100;
        private float cantVidaJugador5 = 100;

        //Tiempo de fin de juego
        private bool inicioReloj = true;
        private bool finReloj = false;
        public bool finModelo = false;
        private DateTime TiempoFin;

        //Ruedas
        TgcMesh ruedaDerechaDelanteraMesh;
        TgcMesh ruedaDerechaTraseraMesh;
        TgcMesh ruedaIzquierdaDelanteraMesh;
        TgcMesh ruedaIzquierdaTraseraMesh;
        List<TgcMesh> RuedasJugador1;
        float rotate = 0;
        float rotacionVertical = 0;
        List<float> dx = new List<float> { 23, -23, -23, 23 };
        List<float> dy = new List<float> { -30, 32, -31, 30 };


        private bool falling = false;
        private bool jumping = false;

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
            var loader = new TgcSceneLoader();
            letraJugadores = new TgcDrawText(d3dDevice, "Rock it", 10, MediaDir);
            drawerBarras = new TgcDrawer2D(d3dDevice);

            //Cargo las barras de vida
            CargarBarrasDeVida();

            //Cargo el terreno
            ScenePpal = loader.loadSceneFromFile(MediaDir + "MAPA3-TgcScene.xml");

            TransformarMeshScenePpal(0, 3, POSICION_VERTICE);
            TransformarMeshScenePpal(1, 3, POSICION_VERTICE);
            TransformarMeshScenePpal(2, 3, POSICION_VERTICE);
            TransformarMeshScenePpal(3, 3, POSICION_VERTICE);
            TransformarMeshScenePpal(4, 3, POSICION_VERTICE);
            TransformarMeshScenePpal(5, 3, POSICION_VERTICE);

            //Cargo los objetos del mesh en una lista para después poder validar las colisiones
            MeshPrincipal = new List<TgcMesh>();

            foreach (TgcMesh unMesh in ScenePpal.Meshes)
            {
                if (unMesh.Name.IndexOf("Floor") != -1)
                    continue;

                if (unMesh.Name.IndexOf("Pasto") != -1)
                    continue;

                MeshPrincipal.Add(unMesh);
            }

            //Cargo el auto
            var SceneAuto = loader.loadSceneFromFile(MediaDir + "Vehiculos\\Auto\\Auto-TgcScene.xml");

            //Movemos el escenario un poco para arriba para que se pueda mover el auto
            Mesh = SceneAuto.Meshes[0];
        
            Mesh.AutoTransformEnable = true;
            Mesh.move(0, 0.5f, 0);
            piso = Mesh.Position.Y;

            //Cargo el bouding box obb del auto a partir de su AABB
            ObbMesh = TgcBoundingOrientedBox.computeFromAABB(Mesh.BoundingBox);

            //Camara por defecto
            CamaraInterna = new TgcThirdPersonCamera(Mesh.Position, 300, 600);
            Camara = CamaraInterna;

            //Creo los objetos del escenario
            CrearAutos(loader);
            CrearObjetos(loader);
        }

        private void CargarBarrasDeVida()
        {
            //Jugador 1
            spriteBarraJugador1 = new TgcSprite(drawerBarras);
            spriteBarraJugador1.Texture = TgcTexture.createTexture(MediaDir + "Textures\\Sprites\\barraVacia.png");
            spriteBarraJugador1.Position = new Vector2(D3DDevice.Instance.Width - POSICION_X_BARRA_VIDA, POSICION_Y_BARRA_VIDA);
            spriteBarraJugador1.Scaling = new Vector2(0.1f, 0.4f);

            spriteBarraJugador1Llena = new TgcSprite(drawerBarras);
            spriteBarraJugador1Llena.Texture = TgcTexture.createTexture(MediaDir + "Textures\\Sprites\\barraLlena.png");
            spriteBarraJugador1Llena.Position = new Vector2(D3DDevice.Instance.Width - POSICION_X_BARRA_VIDA, POSICION_Y_BARRA_VIDA);
            spriteBarraJugador1Llena.Scaling = new Vector2(0.1f, 0.4f);
            spriteBarraJugador1Llena.Color = Color.Blue;

            if (CantidadDeOponentes >= 1)
            {
                //Jugador 2
                spriteBarraJugador2 = new TgcSprite(drawerBarras);
                spriteBarraJugador2.Texture = TgcTexture.createTexture(MediaDir + "Textures\\Sprites\\barraVacia.png");
                spriteBarraJugador2.Position = new Vector2(D3DDevice.Instance.Width - POSICION_X_BARRA_VIDA, POSICION_Y_BARRA_VIDA + 30);
                spriteBarraJugador2.Scaling = new Vector2(0.1f, 0.4f);

                spriteBarraJugador2Llena = new TgcSprite(drawerBarras);
                spriteBarraJugador2Llena.Texture = TgcTexture.createTexture(MediaDir + "Textures\\Sprites\\barraLlena.png");
                spriteBarraJugador2Llena.Position = new Vector2(D3DDevice.Instance.Width - POSICION_X_BARRA_VIDA, POSICION_Y_BARRA_VIDA + 30);
                spriteBarraJugador2Llena.Scaling = new Vector2(0.1f, 0.4f);
                spriteBarraJugador2Llena.Color = Color.Blue;
            }

            if (CantidadDeOponentes >= 2)
            {
                //Jugador 3
                spriteBarraJugador3 = new TgcSprite(drawerBarras);
                spriteBarraJugador3.Texture = TgcTexture.createTexture(MediaDir + "Textures\\Sprites\\barraVacia.png");
                spriteBarraJugador3.Position = new Vector2(D3DDevice.Instance.Width - POSICION_X_BARRA_VIDA, POSICION_Y_BARRA_VIDA + 60);
                spriteBarraJugador3.Scaling = new Vector2(0.1f, 0.4f);

                spriteBarraJugador3Llena = new TgcSprite(drawerBarras);
                spriteBarraJugador3Llena.Texture = TgcTexture.createTexture(MediaDir + "Textures\\Sprites\\barraLlena.png");
                spriteBarraJugador3Llena.Position = new Vector2(D3DDevice.Instance.Width - POSICION_X_BARRA_VIDA, POSICION_Y_BARRA_VIDA + 60);
                spriteBarraJugador3Llena.Scaling = new Vector2(0.1f, 0.4f);
                spriteBarraJugador3Llena.Color = Color.Blue;
            }

            if (CantidadDeOponentes >= 3)
            {
                //Jugador 4
                spriteBarraJugador4 = new TgcSprite(drawerBarras);
                spriteBarraJugador4.Texture = TgcTexture.createTexture(MediaDir + "Textures\\Sprites\\barraVacia.png");
                spriteBarraJugador4.Position = new Vector2(D3DDevice.Instance.Width - POSICION_X_BARRA_VIDA, POSICION_Y_BARRA_VIDA + 90);
                spriteBarraJugador4.Scaling = new Vector2(0.1f, 0.4f);

                spriteBarraJugador4Llena = new TgcSprite(drawerBarras);
                spriteBarraJugador4Llena.Texture = TgcTexture.createTexture(MediaDir + "Textures\\Sprites\\barraLlena.png");
                spriteBarraJugador4Llena.Position = new Vector2(D3DDevice.Instance.Width - POSICION_X_BARRA_VIDA, POSICION_Y_BARRA_VIDA + 90);
                spriteBarraJugador4Llena.Scaling = new Vector2(0.1f, 0.4f);
                spriteBarraJugador4Llena.Color = Color.Blue;
            }

            if (CantidadDeOponentes >= 4)
            {
                //Jugador 5
                spriteBarraJugador5 = new TgcSprite(drawerBarras);
                spriteBarraJugador5.Texture = TgcTexture.createTexture(MediaDir + "Textures\\Sprites\\barraVacia.png");
                spriteBarraJugador5.Position = new Vector2(D3DDevice.Instance.Width - POSICION_X_BARRA_VIDA, POSICION_Y_BARRA_VIDA + 120);
                spriteBarraJugador5.Scaling = new Vector2(0.1f, 0.4f);

                spriteBarraJugador5Llena = new TgcSprite(drawerBarras);
                spriteBarraJugador5Llena.Texture = TgcTexture.createTexture(MediaDir + "Textures\\Sprites\\barraLlena.png");
                spriteBarraJugador5Llena.Position = new Vector2(D3DDevice.Instance.Width - POSICION_X_BARRA_VIDA, POSICION_Y_BARRA_VIDA + 120);
                spriteBarraJugador5Llena.Scaling = new Vector2(0.1f, 0.4f);
                spriteBarraJugador5Llena.Color = Color.Blue;
            }

        }

        private void TransformarMeshScenePpal (int index, float escala, float desplazamiento)
        {
            var unMesh = ScenePpal.Meshes[index];

            unMesh.AutoTransformEnable = false;

            unMesh.Transform = unMesh.Transform * Matrix.Scaling(new Vector3(escala, 1, escala))
                                                                * Matrix.Translation(new Vector3((-1) * desplazamiento, 0, (-1) * desplazamiento));
            unMesh.BoundingBox.transform(unMesh.Transform);

            unMesh.AlphaBlendEnable = true;
            //unMesh.updateBoundingBox();
        }

        private void CrearObjetos(TgcSceneLoader loader)
        {
            int[,] MatrizPoblacion;

            //Creo palmeras
            MatrizPoblacion = RandomMatrix();
            PalmeraOriginal = loader.loadSceneFromFile(MediaDir + "Vegetacion\\Palmera\\Palmera-TgcScene.xml").Meshes[0];
            MeshPalmeras = CrearInstancias(PalmeraOriginal, 0.75f, 0.25f, 2, MatrizPoblacion);

            //Creo pinos
            MatrizPoblacion = RandomMatrix();
            PinoOriginal = loader.loadSceneFromFile(MediaDir + "Vegetacion\\Pino\\Pino-TgcScene.xml").Meshes[0];
            MeshPinos = CrearInstancias(PinoOriginal, 0.90f, -0.05f, 2, MatrizPoblacion);

            //Creo rocas
            MatrizPoblacion = RandomMatrix();
            RocaOriginal = loader.loadSceneFromFile(MediaDir + "Vegetacion\\Roca\\Roca-TgcScene.xml").Meshes[0];
            MeshRocas = CrearInstancias(RocaOriginal, 0.75f, 0.30f, 2, MatrizPoblacion);

            //Creo arboles bananas
            MatrizPoblacion = RandomMatrix();
            ArbolBananasOriginal = loader.loadSceneFromFile(MediaDir + "Vegetacion\\ArbolBananas\\ArbolBananas-TgcScene.xml").Meshes[0];
            MeshArbolesBananas = CrearInstancias(ArbolBananasOriginal, 1.50f, 0.15f, 1, MatrizPoblacion);
            /*
            //Creo barriles de polvora
            MatrizPoblacion = RandomMatrix();
            BarrilPolvoraOriginal = loader.loadSceneFromFile(MediaDir + "Objetos\\BarrilPolvora\\BarrilPolvora-TgcScene.xml").Meshes[0];
            MeshBarrilesPolvora = CrearInstancias(BarrilPolvoraOriginal, 0.75f, 1.15f, 1, MatrizPoblacion);

            //Creo carretillas
            MatrizPoblacion = RandomMatrix();
            CarretillaOriginal = loader.loadSceneFromFile(MediaDir + "Objetos\\Carretilla\\Carretilla-TgcScene.xml").Meshes[0];
            MeshCarretillas = CrearInstancias(CarretillaOriginal, 0.20f, 1.15f, 1, MatrizPoblacion);

            //Creo contenedores
            MatrizPoblacion = RandomMatrix();
            ContenedorOriginal = loader.loadSceneFromFile(MediaDir + "Objetos\\Contenedor\\Contenedor-TgcScene.xml").Meshes[0];
            MeshContenedores = CrearInstancias(ContenedorOriginal, 1.5f, 1.15f, 1, MatrizPoblacion);

            //Creo fuentes de agua
            //MatrizPoblacion = RandomMatrix();
            //FuenteAguaOriginal = loader.loadSceneFromFile(MediaDir + "Objetos\\FuenteAgua\\FuenteAgua-TgcScene.xml").Meshes[0];
            //MeshFuentesAgua = CrearInstancias(FuenteAguaOriginal, 1, 25, 1, MatrizPoblacion);

            //Creo lockers
            MatrizPoblacion = RandomMatrix();
            LockerOriginal = loader.loadSceneFromFile(MediaDir + "Muebles\\LockerMetal\\LockerMetal-TgcScene.xml").Meshes[0];
            MeshLockers = CrearInstancias(LockerOriginal, 1, 1.15f, 1, MatrizPoblacion);

            //Creo expendedores bebidas
            MatrizPoblacion = RandomMatrix();
            ExpendedorBebidaOriginal = loader.loadSceneFromFile(MediaDir + "Muebles\\ExpendedorDeBebidas\\ExpendedorDeBebidas-TgcScene.xml").Meshes[0];
            MeshExpendedoresBebidas = CrearInstancias(ExpendedorBebidaOriginal, 0.50f, 1.15f, 1, MatrizPoblacion);

            //Creo cajas de municiones
            MatrizPoblacion = RandomMatrix();
            CajaMunicionesOriginal = loader.loadSceneFromFile(MediaDir + "Armas\\CajaMuniciones\\CajaMuniciones-TgcScene.xml").Meshes[0];
            MeshCajasMuniciones = CrearInstancias(CajaMunicionesOriginal, 1, 1.15f, 1, MatrizPoblacion);*/
        }

        private int[,] RandomMatrix()
        {
            int[,] MatrizPoblacion = new int[ROWS, COLUMNS];
            System.Random randomNumber = new System.Random();

            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLUMNS; j++)
                {
                    MatrizPoblacion[i, j] = randomNumber.Next(0, 2);
                }
            }

            return MatrizPoblacion;
        }

        private void CrearAutos(TgcSceneLoader loader)
        {
            System.Random randomNumber = new System.Random();
            var meshEnemigos = new List<TgcMesh>();

            var tanque = loader.loadSceneFromFile(MediaDir + "Vehiculos\\TanqueFuturistaRuedas\\TanqueFuturistaRuedas-TgcScene.xml").Meshes[0].createMeshInstance("tanque");
            var hummer = loader.loadSceneFromFile(MediaDir + "Vehiculos\\Hummer\\Hummer-TgcScene.xml").Meshes[0].createMeshInstance("hummer");
            var camioneta = loader.loadSceneFromFile(MediaDir + "Vehiculos\\Camioneta\\Camioneta-TgcScene.xml").Meshes[0].createMeshInstance("camioneta");
            var patrullero = loader.loadSceneFromFile(MediaDir + "Vehiculos\\Patrullero\\Patrullero-TgcScene.xml").Meshes[0].createMeshInstance("buggy");

            tanque.Transform = tanque.Transform * Matrix.Translation(new Vector3((-1) * (POSICION_VERTICE - CUADRANTE_SIZE * 4), 0, (POSICION_VERTICE - CUADRANTE_SIZE * 4)));
            tanque.BoundingBox.transform(tanque.Transform);
            meshEnemigos.Add(tanque);

            hummer.Transform = hummer.Transform * Matrix.Translation(new Vector3(POSICION_VERTICE - CUADRANTE_SIZE * 4, 0, POSICION_VERTICE - CUADRANTE_SIZE * 4));
            hummer.BoundingBox.transform(hummer.Transform);
            meshEnemigos.Add(hummer);

            camioneta.Transform = camioneta.Transform * Matrix.RotationY(180 * (FastMath.PI / 180));
            camioneta.Transform = camioneta.Transform * Matrix.Translation(new Vector3((POSICION_VERTICE - CUADRANTE_SIZE * 4), 0,(-1) * (POSICION_VERTICE - CUADRANTE_SIZE * 4)));
            camioneta.BoundingBox.transform(camioneta.Transform);
            meshEnemigos.Add(camioneta);

            patrullero.Transform = patrullero.Transform * Matrix.RotationY(180 * (FastMath.PI / 180));
            patrullero.Transform = patrullero.Transform * Matrix.Translation(new Vector3((-1) * (POSICION_VERTICE - CUADRANTE_SIZE * 4), 0, (-1) * (POSICION_VERTICE - CUADRANTE_SIZE * 4)));
            patrullero.BoundingBox.transform(patrullero.Transform);
            meshEnemigos.Add(patrullero);

            //Cargo las ruedas de los autos
            TgcScene RuedaDerechaDelJ1 = loader.loadSceneFromFile(MediaDir + "Vehiculos\\Auto_Rueda_Derecha-TgcScene.xml");
            TgcScene RuedaDerechaTrasJ1 = loader.loadSceneFromFile(MediaDir + "Vehiculos\\Auto_Rueda_Derecha-TgcScene.xml");
            TgcScene RuedaIzquierdaDelJ1 = loader.loadSceneFromFile(MediaDir  + "Vehiculos\\Auto_Rueda_Izquierda-TgcScene.xml");
            TgcScene RuedaIzquierdaTrasJ1 = loader.loadSceneFromFile(MediaDir + "Vehiculos\\Auto_Rueda_Izquierda-TgcScene.xml");

            ruedaDerechaDelanteraMesh = RuedaDerechaDelJ1.Meshes[0];
            ruedaDerechaTraseraMesh = RuedaDerechaTrasJ1.Meshes[0];
            ruedaIzquierdaDelanteraMesh = RuedaIzquierdaDelJ1.Meshes[0];
            ruedaIzquierdaTraseraMesh = RuedaIzquierdaTrasJ1.Meshes[0];
            
            
            ruedaDerechaDelanteraMesh.AutoTransformEnable = true;
            //ruedaDerechaDelanteraMesh.move(new Vector3(10f, 100f, 100f));
            ruedaDerechaDelanteraMesh.Scale = new Vector3(0.5f, 0.5f, 0.5f);
            

            ruedaDerechaTraseraMesh.AutoTransformEnable = true;
            ruedaDerechaTraseraMesh.Scale = new Vector3(0.5f, 0.5f, 0.5f);

            ruedaIzquierdaDelanteraMesh.AutoTransformEnable = true;
            ruedaIzquierdaDelanteraMesh.Scale = new Vector3(0.5f, 0.5f, 0.5f);

            ruedaIzquierdaTraseraMesh.AutoTransformEnable = true;
            ruedaIzquierdaTraseraMesh.Scale = new Vector3(0.5f, 0.5f, 0.5f);
            /*
            ruedaDerechaDelanteraMesh.Transform = ruedaDerechaDelanteraMesh.Transform * Matrix.Scaling(new Vector3(1f, 1, 1f))
                                                                * Matrix.Translation(new Vector3(1f, 20f, 1f));
            ruedaDerechaDelanteraMesh.BoundingBox.transform(ruedaDerechaDelanteraMesh.Transform);
            */

            RuedasJugador1 = new List<TgcMesh> { ruedaDerechaDelanteraMesh, ruedaDerechaTraseraMesh, ruedaIzquierdaDelanteraMesh, ruedaIzquierdaTraseraMesh };

            //Inicio la lista de autos
            MeshAutos = new List<TgcMesh>();
           
            //Auto principal
            MeshAutos.Add(Mesh);

            //Autos enemigos
            for (int i = 0; i< CantidadDeOponentes; i++)
            {
               var autoEnemigo = meshEnemigos[i];
               MeshAutos.Add(autoEnemigo);

            }

            //hago dispose de cada auto que no se va a dibujar
            foreach (var meshEnemigo in meshEnemigos)
            {
                if (MeshAutos.Contains(meshEnemigo) == false)
                {
                    meshEnemigo.dispose();
                }
            }
        }

        //Crea instancias de un objeto
        private List<TgcMesh> CrearInstancias(TgcMesh unObjeto, float scale, float ejeZ, int cantidadObjetos, int[,] MatrizPoblacion)
        {
            List<TgcMesh> ListaMesh = new List<TgcMesh>();
            System.Random randomNumber = new System.Random();

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

                            //Roto el objeto aleatoriamente
                            instance.Transform = Matrix.RotationY((randomNumber.Next(1, 180)) * FastMath.PI / 180);

                            /*
                            instance.Transform = instance.Transform * Matrix.Scaling(new Vector3(scale, scale, scale)) *
                                                                       Matrix.Translation(new Vector3(200, ejeZ, -10));
                            */
                            
                            //Lo agrando y traslado al borde del terreno
                            instance.Transform = instance.Transform * Matrix.Scaling(new Vector3(scale, scale, scale)) *
                                                                       Matrix.Translation(new Vector3(POSICION_VERTICE, ejeZ, (-1) * POSICION_VERTICE));


                            
                            //Lo posiciono en una posición aleatoria
                            instance.Transform = instance.Transform * Matrix.Translation(
                                                                        new Vector3((-1) * randomNumber.Next(j * CUADRANTE_SIZE, (j + 1) * CUADRANTE_SIZE), 0,
                                                                                    randomNumber.Next(i * CUADRANTE_SIZE, (i + 1) * CUADRANTE_SIZE)));
                            
                            instance.BoundingBox.transform(instance.Transform);
                            
                            //Valido si pisa a otro objeto que ya existe
                            if (ValidarColisionCrearInstancias(instance, ListaMesh))
                            {
                                //Hubo colision, no creo el objeto
                                instance.dispose();
                                continue;                                
                            }
                            ///////////////////////////////////////////////////////////////////////////////////

                            //Le activo el alpha para que se vea mejor
                            instance.AlphaBlendEnable = true;

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
                AccionarListaMesh(MeshBarrilesPolvora, 2, unaInstancia) ||
                AccionarListaMesh(MeshCarretillas, 2, unaInstancia) ||
                AccionarListaMesh(MeshContenedores, 2, unaInstancia) ||
                AccionarListaMesh(MeshLockers, 2, unaInstancia) ||
                AccionarListaMesh(MeshExpendedoresBebidas, 2, unaInstancia) ||
                AccionarListaMesh(MeshCajasMuniciones, 2, unaInstancia) ||
                AccionarListaMesh(MeshAutos, 2, unaInstancia) ||
                AccionarListaMesh(MeshPrincipal, 2, unaInstancia)
                )
            {
                return true;
            }

            return false;
        }

        private bool ValidarColisionObjetos(TgcMesh unaInstancia)
        {
            //Valido la colisión para cada lista de objetos que tenga
            if (AccionarListaMesh(MeshPalmeras, 2, unaInstancia) ||
                AccionarListaMesh(MeshPinos, 2, unaInstancia) ||
                AccionarListaMesh(MeshRocas, 2, unaInstancia) ||
                AccionarListaMesh(MeshArbolesBananas, 2, unaInstancia) ||
                AccionarListaMesh(MeshAutos, 2, unaInstancia) ||
                AccionarListaMesh(MeshPrincipal, 2, unaInstancia)
                )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        public override void Update()
        {
            PreUpdate();

            //Activo bounding box para debug
            ActivarBoundingBox();

            //Chequea si se solicitó cambiar el tipo de camara
            CambiarDeCamara();

            //Acciona la vista con el movimiento del mouse
            MoverCamaraConMouse();

            //Maneja el movimiento del auto teniendo en cuenta la posición de los otros objetos
            MoverAutoConColisiones();

            //Salgo
            if (finReloj && Input.keyDown(Key.X))
            {
                this.finModelo = true;
            }
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
            DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.Red);
            DrawText.drawText("Con la tecla F1 se cambia el tipo de camara. Pos [Actual]: " + TgcParserUtils.printVector3(Camara.Position), 0, 30, Color.Red);

            DrawText.drawText("Jugador 1: " + this.NombreJugador1, 0, 40, Color.LightYellow);
            DrawText.drawText("Velocidad: " + this.MOVEMENT_SPEED, 0, 50, Color.LightYellow);

            //Siempre antes de renderizar el modelo necesitamos actualizar la matriz de transformacion.
            //Debemos recordar el orden en cual debemos multiplicar las matrices, en caso de tener modelos jerárquicos, tenemos control total.
            /*Box.Transform = Matrix.Scaling(Box.Scale) *
                            Matrix.RotationYawPitchRoll(Box.Rotation.Y, Box.Rotation.X, Box.Rotation.Z) *
                            Matrix.Translation(Box.Position);
            //A modo ejemplo realizamos toda las multiplicaciones, pero aquí solo nos hacia falta la traslación.
            //Finalmente invocamos al render de la caja
            Box.render();
            */
            //Cuando tenemos modelos mesh podemos utilizar un método que hace la matriz de transformación estándar.
            //Es útil cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones jerárquicas o complicadas.
            Mesh.UpdateMeshTransform();

            //Render del mesh
            Mesh.render();

            //Dibujamos la escena
            ScenePpal.renderAll();

            //Render de BoundingBox, muy útil para debug de colisiones.
            if (BoundingBox)
            {
                foreach (var mesh in ScenePpal.Meshes)
                {
                    if(mesh.Equals(Mesh) == false) mesh.BoundingBox.render(); //agrego este if asi no dibujamos al bounding box AABB
                }

                ObbMesh.render();
                RenderizarObjetos(1);
            }

            //Renderizo los objetos cargados de las listas
            RenderizarObjetos(0);

            //Dibujo las barras de vida de todos los jugadores
            DibujarBarrasDeVida();

            //Dibujo las ruedas
            DibujarRuedas();

            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        private void DibujarRuedas()
        {
            float ro, alfa_rueda, rotacionRueda;
            float posicion_x;
            float posicion_y;

            //Posiciono las ruedas
            for (int i = 0; i < 4; i++)
            {
                ro = FastMath.Sqrt(dx[i] * dx[i] + dy[i] * dy[i]);

                alfa_rueda = FastMath.Asin(dx[i] / ro);

                if (i == 0 || i == 2)
                {
                    alfa_rueda += FastMath.PI;
                }

                posicion_x = FastMath.Sin(alfa_rueda + Mesh.Rotation.Y) * ro;
                posicion_y = FastMath.Cos(alfa_rueda + Mesh.Rotation.Y) * ro;

                RuedasJugador1[i].Position = (new Vector3(posicion_x, 7.5f, posicion_y) + Mesh.Position);

                rotacionRueda = 0;

                if (i == 0 || i == 2)
                {
                    rotacionRueda = 0.5f * Math.Sign (rotate);
                }

                //Si no aprieta para los costados, dejo la rueda derecha (por ahora, esto se puede modificar)
                if (Input.keyDown(Key.Left) || Input.keyDown(Key.A) || Input.keyDown(Key.Right) || Input.keyDown(Key.D))
                    RuedasJugador1[i].Rotation = new Vector3(rotacionVertical, Mesh.Rotation.Y + rotacionRueda, 0f);
                else
                    RuedasJugador1[i].Rotation = new Vector3(rotacionVertical, Mesh.Rotation.Y, 0f);
            }

            for (int i = 0; i < 4; i++)
            {
                RuedasJugador1[i].render();
            }
        }

        private void DibujarBarrasDeVida()
        {
            //Inicio el tiempo de juego
            if (inicioReloj)
            {
                TiempoFin = DateTime.Now.AddMinutes(this.TiempoDeJuego);
                inicioReloj = false;
            }

            //Dibujo el reloj
            string Tiempo = "";

            Tiempo = (TiempoFin - DateTime.Now).Minutes.ToString().PadLeft (2, '0') + ":" + (TiempoFin - DateTime.Now).Seconds.ToString().PadLeft(2, '0');

            if (((TiempoFin - DateTime.Now).Minutes < 0) || ((TiempoFin - DateTime.Now).Seconds < 0))
                finReloj = true;
                

            //Dibujo el nombre del Jugador y de los competidores
            letraJugadores.drawText(this.NombreJugador1, Convert.ToInt32(D3DDevice.Instance.Width) - POSICION_X_BARRA_VIDA - 115, POSICION_Y_BARRA_VIDA - 10, Color.DeepSkyBlue);
            letraJugadores.drawText("jugador 22", Convert.ToInt32(D3DDevice.Instance.Width) - POSICION_X_BARRA_VIDA - 115, POSICION_Y_BARRA_VIDA + 20, Color.DeepSkyBlue);
            letraJugadores.drawText("jugador 33", Convert.ToInt32(D3DDevice.Instance.Width) - POSICION_X_BARRA_VIDA - 115, POSICION_Y_BARRA_VIDA + 50, Color.DeepSkyBlue);
            letraJugadores.drawText("jugador 44", Convert.ToInt32(D3DDevice.Instance.Width) - POSICION_X_BARRA_VIDA - 115, POSICION_Y_BARRA_VIDA + 80, Color.DeepSkyBlue);
            letraJugadores.drawText("jugador 55", Convert.ToInt32(D3DDevice.Instance.Width) - POSICION_X_BARRA_VIDA - 115, POSICION_Y_BARRA_VIDA + 110, Color.DeepSkyBlue);

            //Calculo la vida de cada auto
            spriteBarraJugador1Llena.Scaling = new Vector2(cantVidaJugador1 * 0.001f, 0.4f);
            spriteBarraJugador2Llena.Scaling = new Vector2(cantVidaJugador2 * 0.001f, 0.4f);
            spriteBarraJugador3Llena.Scaling = new Vector2(cantVidaJugador3 * 0.001f, 0.4f);
            spriteBarraJugador4Llena.Scaling = new Vector2(cantVidaJugador4 * 0.001f, 0.4f);
            spriteBarraJugador5Llena.Scaling = new Vector2(cantVidaJugador5 * 0.001f, 0.4f);

            //Calculo si ganó alguien o el principal perdió
            if ( (cantVidaJugador1 <= 0) || (cantVidaJugador2 <= 0 && cantVidaJugador3 <= 0 && cantVidaJugador4 <= 0 && cantVidaJugador5 <= 0))
                finReloj = true;

            //Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
            drawerBarras.beginDrawSprite();

            //Dibujo las barras
            spriteBarraJugador1.render();
            spriteBarraJugador1Llena.render();

            spriteBarraJugador2.render();
            spriteBarraJugador2Llena.render();

            spriteBarraJugador3.render();
            spriteBarraJugador3Llena.render();

            spriteBarraJugador4.render();
            spriteBarraJugador4Llena.render();

            spriteBarraJugador5.render();
            spriteBarraJugador5Llena.render();

            //Finalizar el dibujado de Sprites
            drawerBarras.endDrawSprite();
            ///////////////////////

            if (finReloj)
            {
                letraJugadores.drawText("00:00", Convert.ToInt32(D3DDevice.Instance.Width) - POSICION_X_BARRA_VIDA + 35, POSICION_Y_BARRA_VIDA - 40, Color.OrangeRed);
                TgcDrawText letraGanador = new TgcDrawText(D3DDevice.Instance.Device, "Rock it", 36, MediaDir);
                letraGanador.drawText("FIN DEL JUEGO", (Convert.ToInt32(D3DDevice.Instance.Width) / 2) - 200, (Convert.ToInt32(D3DDevice.Instance.Height) / 2) - 150, Color.Green);
                letraGanador.drawText("GANÓ ...", (Convert.ToInt32(D3DDevice.Instance.Width) / 2) - 200, (Convert.ToInt32(D3DDevice.Instance.Height) / 2), Color.Green);
                letraJugadores.drawText("Presione la letra X para volver al menú inicial...", (Convert.ToInt32(D3DDevice.Instance.Width) / 2) - 200, (Convert.ToInt32(D3DDevice.Instance.Height) / 2) + 150, Color.OrangeRed);
            }
            else
            {
                letraJugadores.drawText(Tiempo, Convert.ToInt32(D3DDevice.Instance.Width) - POSICION_X_BARRA_VIDA + 35, POSICION_Y_BARRA_VIDA - 40, Color.OrangeRed);
            }
}

        //0: render
        //1: render bounding box
        //2: control de colisiones
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
            //Renderizar Autos
            AccionarListaMesh(MeshAutos, unaAccion, null);

            //Renderizar palmeras
            AccionarListaMesh(MeshPalmeras, unaAccion, null);

            //Renderizar arbol bananas
            AccionarListaMesh(MeshArbolesBananas, unaAccion, null);
            
            //Renderizar pinos
            AccionarListaMesh(MeshPinos, unaAccion, null);

            //Renderizar rocas
            AccionarListaMesh(MeshRocas, unaAccion, null);
            /*
            //Renderizar barriles de polvora
            AccionarListaMesh(MeshBarrilesPolvora, unaAccion, null);

            //Renderizar Carretillas
            AccionarListaMesh(MeshCarretillas, unaAccion, null);

            //Renderizar Contenedores
            AccionarListaMesh(MeshContenedores, unaAccion, null);

            //Renderizar Fuentes de Agua
            //RenderListaMesh(MeshPinos, unaAccion, null);

            //Renderizar Lockers
            AccionarListaMesh(MeshLockers, unaAccion, null);

            //Renderizar Expendedores Bebidas
            AccionarListaMesh(MeshExpendedoresBebidas, unaAccion, null);

            //Renderizar Cajas Municiones
            AccionarListaMesh(MeshCajasMuniciones, unaAccion, null);*/
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
                            //CamaraInterna = new TgcThirdPersonCamera(Mesh.Position, 300, 400);
                            Camara = CamaraInterna;
                        }
                        break;

                    case 1:
                        {
                            Camara = new TgcRotationalCamera(Mesh.BoundingBox.calculateBoxCenter(), Mesh.BoundingBox.calculateBoxRadius() * 2, Input);
                        }
                        break;
                }
            }
        }

        public void MoverCamaraConMouse()
        {
            //Capturar Input Mouse
            if (Input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Ver....
                /*
                //En este caso le sumamos un valor en Y
                Camara.SetCamera(Camara.Position + new Vector3(0, 10f, 0), Camara.LookAt);
                //Ver ejemplos de cámara para otras operaciones posibles.

                //Si superamos cierto Y volvemos a la posición original.
                if (Camara.Position.Y > 300f)
                {
                    Camara.SetCamera(new Vector3(Camara.Position.X, 0f, Camara.Position.Z), Camara.LookAt);
                }
                */
            }
        }

        public void MoverAutoConColisiones()
        {
            //Declaramos un vector de movimiento inicializado en cero.
            //El movimiento sobre el suelo es sobre el plano XZ.
            //Sobre XZ nos movemos con las flechas del teclado o con las letas WASD.
            var movement = new Vector3(0, 0, 0);
            var moveForward = 0f;
            rotate = 0;
            var moving = false;
            var rotating = false;

            //Movernos de izquierda a derecha, sobre el eje X.
            if (Input.keyDown(Key.Left) || Input.keyDown(Key.A))
            {
                rotate = -ROTATION_SPEED;
                rotating = true;
            }
            else if (Input.keyDown(Key.Right) || Input.keyDown(Key.D))
            {
                rotate = ROTATION_SPEED;
                rotating = true;
            }

            //Movernos adelante y atras, sobre el eje Z.
            if ((Input.keyDown(Key.Up) || Input.keyDown(Key.W)))
            {
                moveForward += -Acelerar(200f);
                moving = true;
            }
            if ((Input.keyDown(Key.Down) || Input.keyDown(Key.S)))
            {
                moveForward += -Acelerar(-250f);
                moving = true;
            }

            //////////El auto dejo de acelerar e ira frenando de apoco 
            if (!Input.keyDown(Key.Down) && !Input.keyDown(Key.S) && !Input.keyDown(Key.Up) && !Input.keyDown(Key.W))
            {
                moveForward = -Acelerar(0f);
                moving = true;
            }

            if (rotating)
            {
                var rotAngle = (MOVEMENT_SPEED * 0.2f * Math.Sign (rotate) * ElapsedTime) * (FastMath.PI / 180);
                Mesh.rotateY(rotAngle);
                CamaraInterna.rotateY(rotAngle);
                ObbMesh.rotate(new Vector3(0, rotAngle, 0));
            }

            if (Input.keyPressed(Key.Space) && !falling)
            {
                jumping = true;
            }

            if (jumping)
            {
                Mesh.move(0, 100 * ElapsedTime * 2, 0);

                if (Mesh.Position.Y >= ALTURA_SALTO)
                {
                    jumping = false;
                    falling = true;
                }
            }

            if (falling)
            {
                Mesh.move(0, -100 * ElapsedTime * 3, 0);

                if (Mesh.Position.Y < 0.5f)
                {
                    Mesh.move(0, 0.5f - Mesh.Position.Y, 0);
                    falling = false;
                }
            }

            if (moving)
            {
                //Guardar posicion original antes de cambiarla
                var originalPos = Mesh.Position;

                //Multiplicar movimiento por velocidad y elapsedTime

                movement *= MOVEMENT_SPEED * ElapsedTime;

                rotacionVertical -= MOVEMENT_SPEED * ElapsedTime / 60;

                Mesh.moveOrientedY(moveForward * ElapsedTime);
                ObbMesh.Center = Mesh.Position;

                //El framework posee la clase TgcCollisionUtils con muchos algoritmos de colisión de distintos tipos de objetos.
                //Por ejemplo chequear si dos cajas colisionan entre sí, o dos esferas, o esfera con caja, etc.
                var collisionFound = false;

                foreach (var mesh in ScenePpal.Meshes)
                {
                    //Los dos BoundingBox que vamos a testear
                    var mainMeshBoundingBox = Mesh.BoundingBox;
                    var sceneMeshBoundingBox = mesh.BoundingBox;

                    if (mesh.Name == "Room-1-Roof-0")
                    {
                        continue;
                    }

                    //Ejecutar algoritmo de detección de colisiones
                    var collisionResult = TgcCollisionUtils.classifyBoxBox(mainMeshBoundingBox, sceneMeshBoundingBox);

                    //Hubo colisión con un objeto. Guardar resultado y abortar loop.
                    if ((collisionResult != TgcCollisionUtils.BoxBoxResult.Encerrando) && (collisionResult != TgcCollisionUtils.BoxBoxResult.Afuera))
                    {
                        collisionFound = true;
                        break;
                    }
                }

                //Si hubo alguna colisión, entonces restaurar la posición original del mesh
                if (collisionFound)
                {
                    Mesh.Position = originalPos;
                }

                if (ValidarColisionObjetos(Mesh))
                {
                    Mesh.Position = originalPos;
                }

                //Hacer que la camara en 3ra persona se ajuste a la nueva posicion del objeto
                CamaraInterna.Target = Mesh.Position;
            }
        }

        private float Acelerar(float aceleracion) 
        {
            if ((MOVEMENT_SPEED < MAX_SPEED))
            {
                MOVEMENT_SPEED = MOVEMENT_SPEED + ((aceleracion + ObtenerRozamiento()) * ElapsedTime);

                if (MOVEMENT_SPEED > MAX_SPEED)
                    MOVEMENT_SPEED = MAX_SPEED - 1;

                return MOVEMENT_SPEED;
            }
            else return MOVEMENT_SPEED;
        }

        private float ObtenerRozamiento()
        {
            if (MOVEMENT_SPEED > 0) return (-ROZAMIENTO);
            if (MOVEMENT_SPEED < 0) return ROZAMIENTO;
            else return 0;
        }

        private float Desacelerar()
        {
            if (MOVEMENT_SPEED <= 0) return 0;
            else return MOVEMENT_SPEED -= ROZAMIENTO * ElapsedTime;

        }

        /// <summary>
        ///     Se llama cuando termina la ejecución del ejemplo.
        ///     Hacer Dispose() de todos los objetos creados.
        ///     Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        /// </summary>
        public override void Dispose()
        {
            foreach (var mesh in MeshAutos)
            {
                mesh.dispose();
            }

            PalmeraOriginal.dispose();
            PinoOriginal.dispose();
            RocaOriginal.dispose();
            ArbolBananasOriginal.dispose();

            foreach (var mesh in RuedasJugador1)
            {
                mesh.dispose();
            }

            /*
            BarrilPolvoraOriginal.dispose();
            CarretillaOriginal.dispose();
            ContenedorOriginal.dispose();
            LockerOriginal.dispose();
            ExpendedorBebidaOriginal.dispose();
            CajaMunicionesOriginal.dispose();
            */
            //FuenteAguaOriginal.dispose();
            ScenePpal.disposeAll();
        }
    }
}