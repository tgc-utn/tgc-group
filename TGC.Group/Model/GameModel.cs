using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Terrain;
using TGC.Core.Textures;

namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo para implementar el TP.
    ///     Inicialmente puede ser renombrado o copiado para hacer m�s ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar el modelo que instancia GameForm <see cref="Form.GameForm.InitGraphics()" />
    ///     line 97.
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

        //Definimos estructuras
        private TgcPlane suelo;

        private TgcPlane[] suelos1;
        private TgcPlane[] paredes11;
        private TgcPlane[] paredes12;
        private TgcPlane[] suelos2;
        private TgcPlane[] paredes21;
        private TgcPlane[] paredes22;
        private TgcPlane[] suelos3;
        private TgcPlane[] paredes31;
        private TgcPlane[] paredes32;
        private TgcPlane pared;
        private TgcPlane pared2;
        TgcSkyBox skyBox;

        private TgcPlane[] completarLineaDeSuelosX( int cantidadDeSuelo, string DireccionTextura, float X, float Y,float Z, float escala) {


            TgcPlane[] suelos = new TgcPlane[cantidadDeSuelo];


            for (int i = 0; i < cantidadDeSuelo; i++) {


                suelos[i] = new TgcPlane(new TGCVector3(X, Y, Z), new TGCVector3(escala, escala, escala), TgcPlane.Orientations.XZplane, TgcTexture.createTexture(DireccionTextura), 10f, 10f);
                
                X += escala;


            }  

            return suelos;

        }

        private TgcPlane[] completarLineaDeSuelosZ(int cantidadDeSuelo, string DireccionTextura, float X, float Y, float Z, float escala)
        {


            TgcPlane[] suelos = new TgcPlane[cantidadDeSuelo];


            for (int i = 0; i < cantidadDeSuelo; i++)
            {


                suelos[i] = new TgcPlane(new TGCVector3(X, Y, Z), new TGCVector3(escala, escala, escala), TgcPlane.Orientations.XZplane, TgcTexture.createTexture(DireccionTextura), 10f, 10f);

                Z += escala;


            }

            return suelos;

        }

        private TgcPlane[] completarParedZ(int cantidadDeParedes, string DireccionTextura, float X, float Y, float Z, float escala)
        {


            TgcPlane[] Paredes = new TgcPlane[cantidadDeParedes];


            for (int i = 0; i < cantidadDeParedes; i++)
            {


                Paredes[i] = new TgcPlane(new TGCVector3(X, Y, Z), new TGCVector3(escala, escala, escala), TgcPlane.Orientations.XYplane, TgcTexture.createTexture(DireccionTextura), 10f, 10f);

                X += escala;


            }

            return Paredes;

        }


        private TgcPlane[] completarParedX(int cantidadDeParedes, string DireccionTextura, float X, float Y, float Z, float escala)
        {


            TgcPlane[] Paredes = new TgcPlane[cantidadDeParedes];


            for (int i = 0; i < cantidadDeParedes; i++)
            {


                Paredes[i] = new TgcPlane(new TGCVector3(X, Y, Z), new TGCVector3(escala, escala, escala), TgcPlane.Orientations.YZplane, TgcTexture.createTexture(DireccionTextura), 10f, 10f);

                Z += escala;


            }

            return Paredes;

        }



        private TgcSkyBox crearcielo() {

            //Crear SkyBox
            skyBox = new TgcSkyBox();
            skyBox.Center = TGCVector3.Empty;
            skyBox.Size = new TGCVector3(10000, 10000, 10000);

            //Configurar color
            //skyBox.Color = Color.OrangeRed;

            //var texturesPath = MediaDir + "Texturas\\Quake\\SkyBox1\\";

            //Configurar las texturas para cada una de las 6 caras
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, MediaDir + "Color A05.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, MediaDir + "Color A05.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, MediaDir + "Color A05.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, MediaDir + "Color A05.png");

            //Hay veces es necesario invertir las texturas Front y Back si se pasa de un sistema RightHanded a uno LeftHanded
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, MediaDir + "Color A05.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, MediaDir + "Color A05.png");
            skyBox.SkyEpsilon = 25f;
            //Inicializa todos los valores para crear el SkyBox
            skyBox.Init();



            return skyBox;
        
        }






        private void mostrarArrayPlano(TgcPlane[] planos) {

            foreach (TgcPlane plano in planos) {


                plano.Render();


            }
                           
        }




        public override void Init()
        {





           

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;




            //Crear SkyBox
            skyBox = crearcielo();

            //Textura de la carperta Media. Game.Default es un archivo de configuracion (Game.settings) util para poner cosas.
            //Pueden abrir el Game.settings que se ubica dentro de nuestro proyecto para configurar.
            //Esta textura despues la cambiamos.
            var pathTexturaCaja = MediaDir + Game.Default.TexturaCaja;
            var texturaPisoDeMetal = MediaDir + "//Metal-floor_.png";
            var pathTexturaCaja3 = MediaDir + "//Piso2.jpg";
            var texturaEstrella = MediaDir + "//Color_002.jpg";
            
            //var pathTexturaCaja3 = MediaDir + "//stones.bpn";
            // var pathTexturaCaja3 = MediaDir + Game.Default.TexturaCaja;

            //Cargamos una textura, tener en cuenta que cargar una textura significa crear una copia en memoria.
            //Es importante cargar texturas en Init, si se hace en el render loop podemos tener grandes problemas si instanciamos muchas.
            var pisoTexture = TgcTexture.createTexture(pathTexturaCaja);

            //Definimos caracteristicas del suelo
            //suelo = new TgcPlane(new TGCVector3(0, 0, 0), new TGCVector3(50, 50, 50), TgcPlane.Orientations.XZplane, pisoTexture, 10f, 10f);

            int cantidadEspacio1 = 4;
            int escala = 10;
            // Pasillo 1 (0,0,0) -> (6x10,0,0)

            suelos1 = completarLineaDeSuelosX(6, pathTexturaCaja, 0, 0, 0, escala);
            paredes11 = completarParedZ((6 + 1), pathTexturaCaja3, 0, 0, 0, escala);
            paredes12 = completarParedZ(6, pathTexturaCaja3, 0, 0, 1*escala, escala);

            //Pasillo 2 (6x10,0,0) -> (6x10,0,6x10)

            suelos2 = completarLineaDeSuelosZ(6, texturaPisoDeMetal, 6* escala, 0, 0, escala);
            paredes21= completarParedX(6, texturaEstrella, (1 + 6) * escala, 0, 0, escala);
            paredes22 = completarParedX(6, texturaEstrella, (0 + 6) * escala, 0, 1 * escala, escala);

            //Pasillo 3 (6x10,0,6x10) -> (6x10,0,12x10)
            //int posz = 8* escala;

            suelos3 = completarLineaDeSuelosX(6, pathTexturaCaja, 6*escala, 0, 6*escala, escala);
            paredes31 = completarParedZ(5, pathTexturaCaja3, (1+6) * escala, 0, 6 * escala, escala);
            paredes32 = completarParedZ((1+5), pathTexturaCaja3, (0 + 6) * escala, 0, (1+6) * escala, escala);


            pared = new TgcPlane(new TGCVector3(0, 0, 0), new TGCVector3(10, 10, 10), TgcPlane.Orientations.YZplane, TgcTexture.createTexture(pathTexturaCaja), 10f, 10f);
            pared2 = new TgcPlane(new TGCVector3(0, 0, 0), new TGCVector3(10, 10, 10), TgcPlane.Orientations.XYplane, TgcTexture.createTexture(pathTexturaCaja), 10f, 10f);
            
           // paredes2 = completarParedX(8, texturaEstrella, posx+escala, 0, 0, escala);


            //Suelen utilizarse objetos que manejan el comportamiento de la camara.
            //Lo que en realidad necesitamos gr�ficamente es una matriz de View.
            //El framework maneja una c�mara est�tica, pero debe ser inicializada.
            //Posici�n de la camara.
            var cameraPosition = new TGCVector3(0, 10, 125);
            //Quiero que la camara mire hacia el origen (0,0,0).
            var lookAt = TGCVector3.Empty;
            //Configuro donde esta la posicion de la camara y hacia donde mira.
            Camara.SetCamera(cameraPosition, lookAt);
            //Internamente el framework construye la matriz de view con estos dos vectores.
            //Luego en nuestro juego tendremos que crear una c�mara que cambie la matriz de view con variables como movimientos o animaciones de escenas.
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la l�gica de computo del modelo, as� como tambi�n verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        public override void Update()
        {
            PreUpdate();

            if (Input.keyPressed(Key.W))
            {
                Camara.SetCamera(Camara.Position + new TGCVector3(10f, 0, 0), Camara.LookAt);
            }

            if (Input.keyPressed(Key.S))
            {
                Camara.SetCamera(Camara.Position + new TGCVector3(-10f, 0, 0), Camara.LookAt);
            }

            if (Input.keyPressed(Key.A))
            {
                Camara.SetCamera(Camara.Position + new TGCVector3(0, 0, 10f), Camara.LookAt);
            }

            if (Input.keyPressed(Key.D))
            {
                Camara.SetCamera(Camara.Position + new TGCVector3(0, 0, -10), Camara.LookAt);
            }

            if (Input.keyPressed(Key.Space))
            {
                Camara.SetCamera(Camara.Position + new TGCVector3(0f, 10f, 0), Camara.LookAt);
            }

            if (Input.keyPressed(Key.LeftControl))
            {
                Camara.SetCamera(Camara.Position + new TGCVector3(0, -10f, 0), Camara.LookAt);
            }

            PostUpdate();
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aqu� todo el c�digo referido al renderizado.
        ///     Borrar todo lo que no haga falta.
        /// </summary>
        public override void Render()
        {
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones seg�n nuestra conveniencia.
            PreRender();

            //Dibuja un texto por pantalla
            DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.OrangeRed);
            DrawText.drawText("Con clic izquierdo subimos la camara [Actual]: " + TGCVector3.PrintVector3(Camara.Position), 0, 30, Color.OrangeRed);


            mostrarArrayPlano(suelos1);
            mostrarArrayPlano(suelos2);
            mostrarArrayPlano(suelos3);
            pared.Render();
            pared2.Render();
            mostrarArrayPlano(paredes11);
            mostrarArrayPlano(paredes12);
            mostrarArrayPlano(paredes21);
            mostrarArrayPlano(paredes22);
            mostrarArrayPlano(paredes31);
            mostrarArrayPlano(paredes32);
            skyBox.Render();




            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        /// <summary>
        ///     Se llama cuando termina la ejecuci�n del ejemplo.
        ///     Hacer Dispose() de todos los objetos creados.
        ///     Es muy importante liberar los recursos, sobretodo los gr�ficos ya que quedan bloqueados en el device de video.
        /// </summary>
        public override void Dispose()
        {
            //Dispose del mesh.
            //suelo.Dispose();
        }
    }
}