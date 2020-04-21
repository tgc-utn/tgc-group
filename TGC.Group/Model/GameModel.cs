using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
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
        private TgcPlane[] paredes1;
        private TgcPlane[] suelos2;
        private TgcPlane[] paredes2;
        private TgcPlane[] suelos3;
        private TgcPlane pared;
        private TgcPlane pared2;

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






        private void mostrarArrayPlano(TgcPlane[] planos) {

            foreach (TgcPlane plano in planos) {


                plano.Render();


            }
                           
        }




        public override void Init()
        {

            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;

            //Textura de la carperta Media. Game.Default es un archivo de configuracion (Game.settings) util para poner cosas.
            //Pueden abrir el Game.settings que se ubica dentro de nuestro proyecto para configurar.
            //Esta textura despues la cambiamos.
            var pathTexturaCaja = MediaDir + Game.Default.TexturaCaja;
            var pathTexturaCaja2 = MediaDir + "//rocks.jpg";
            var pathTexturaCaja3 = MediaDir + "//Piso2.jpg";
            var pathTexturaCaja4 = MediaDir + "//cartelera2.jpg";
            
            //var pathTexturaCaja3 = MediaDir + "//stones.bpn";
            // var pathTexturaCaja3 = MediaDir + Game.Default.TexturaCaja;

            //Cargamos una textura, tener en cuenta que cargar una textura significa crear una copia en memoria.
            //Es importante cargar texturas en Init, si se hace en el render loop podemos tener grandes problemas si instanciamos muchas.
            var pisoTexture = TgcTexture.createTexture(pathTexturaCaja);

            //Definimos caracteristicas del suelo
            //suelo = new TgcPlane(new TGCVector3(0, 0, 0), new TGCVector3(50, 50, 50), TgcPlane.Orientations.XZplane, pisoTexture, 10f, 10f);

            int cantidad = 4;
            int escala = 10;
            suelos1 = completarLineaDeSuelosX(cantidad, pathTexturaCaja, 0, 0, 0, escala);
            int posx =cantidad* escala;
            suelos2 = completarLineaDeSuelosZ(8, pathTexturaCaja2, posx, 0, 0, escala);
            int posz = 8* escala;
            suelos3 = completarLineaDeSuelosX(cantidad, pathTexturaCaja, posx, 0, posz, escala);
            pared = new TgcPlane(new TGCVector3(0, 0, 0), new TGCVector3(10, 10, 10), TgcPlane.Orientations.YZplane, TgcTexture.createTexture(pathTexturaCaja), 10f, 10f);
            pared2 = new TgcPlane(new TGCVector3(0, 0, 0), new TGCVector3(10, 10, 10), TgcPlane.Orientations.XYplane, TgcTexture.createTexture(pathTexturaCaja), 10f, 10f);
            paredes1= completarParedZ(cantidad, pathTexturaCaja3, 0, 0, 0, escala);
            paredes2 = completarParedX(8, pathTexturaCaja4, posx+escala, 0, 0, escala);


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

            //suelo.Render();
            //suelos1[0].Render();
            //suelos1[1].Render();
            //suelos1[2].Render();
            //suelos1[3].Render();
            mostrarArrayPlano(suelos1);
            mostrarArrayPlano(suelos2);
            mostrarArrayPlano(suelos3);
            pared.Render();
            pared2.Render();
            mostrarArrayPlano(paredes1);
            mostrarArrayPlano(paredes2);




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