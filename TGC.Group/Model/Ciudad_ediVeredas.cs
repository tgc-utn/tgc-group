using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.Utils;
using TGC.Core.Camara;
using System.Collections.Generic;
using TGC.Examples.Camara;

namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo para implementar el TP.
    ///     Inicialmente puede ser renombrado o copiado para hacer más ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar el modelo que instancia GameForm <see cref="Form.GameForm.InitGraphics()" />
    ///     line 97.
    /// </summary>
    public class Ciudad : TgcExample
    {

        private TgcPlane suelo;
        private TgcMesh edificio;
        private TgcPlane calle;
        private List<TgcPlane> veredas;
        private List<TgcPlane> cordones;
        private List<TgcPlane> paredes;
        private List<TgcMesh> meshes;
        private TgcSceneLoader loader;

        private int CameraX, CameraY, CameraZ;
        List<int> ListaRandom = new List<int>(7);
        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        /// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
        /// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
        public Ciudad(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }



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
            CameraX = -890;
            CameraY = 460;
            CameraZ = 110;
            //Cargamos una textura, tener en cuenta que cargar una textura significa crear una copia en memoria.
            //Es importante cargar texturas en Init, si se hace en el render loop podemos tener grandes problemas si instanciamos muchas.
            //Crea el piso de fondo

            loader = new TgcSceneLoader();
            crearPisoDeFondo();
            crearCamara();
            meshes = new System.Collections.Generic.List<TgcMesh>();
            veredas = new System.Collections.Generic.List<TgcPlane>();
            cordones = new System.Collections.Generic.List<TgcPlane>();
            paredes = new System.Collections.Generic.List<TgcPlane>();
            crearEdificios();
            crearVeredas();
            crearParedes();
            //crearCalles();
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        public override void Update()
        {
            PreUpdate();

            //Capturar Input teclado
            //desplazar();
            //Capturar Input Mouse
            //movimientosCamara();

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
            DrawText.drawText(
                "Con clic izquierdo subimos la camara [Actual]: " + TgcParserUtils.printVector3(Camara.Position), 0, 30,
                Color.OrangeRed);
            //Renderizar suelo
            suelo.render();
            //calle.render();
            //Renderizar instancias
            foreach (var mesh in meshes)
            {
                mesh.render();
            }

            //Renderizado de cordones
            foreach (var cordon in cordones)
            {
                cordon.render();
            }

            //Renderizado de veredas
            foreach (var v in veredas)
            {
                v.render();
            }

            //Renderizado de paredes
            foreach (var p in paredes)
            {
                p.render();
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
            suelo.dispose();
            calle.dispose();
            //Al hacer dispose del original, se hace dispose automaticamente de todas las instancias
            edificio.dispose();
        }
        private void movimientosCamara()
        {
            if (Input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Como ejemplo podemos hacer un movimiento simple de la cámara.
                //En este caso le sumamos un valor en Y
                if (Camara.Position.Y <= 450f)
                    Camara.SetCamera(Camara.Position + new Vector3(0, 10f, 0), Camara.LookAt);
            }

            if (Input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_RIGHT))
            {
                //Como ejemplo podemos hacer un movimiento simple de la cámara.
                //En este caso le sumamos un valor en Y
                Camara.SetCamera(Camara.Position + new Vector3(0, -10f, 0), Camara.LookAt);
                //Ver ejemplos de cámara para otras operaciones posibles.

                //Si superamos cierto Y volvemos a la posición original.
                if (Camara.Position.Y <= 5f)
                {
                    Camara.SetCamera(new Vector3(Camara.Position.X, 5f, Camara.Position.Z), Camara.LookAt);
                }
            }
        }
        private void desplazar()
        {
            if (Input.keyPressed(Key.A))
            {
                Camara.SetCamera(Camara.Position + new Vector3(10f, 0, 0), Camara.LookAt);
            }
            if (Input.keyPressed(Key.S))
            {
                Camara.SetCamera(Camara.Position + new Vector3(0, 0, 10f), Camara.LookAt);
            }
            if (Input.keyPressed(Key.W))
            {
                Camara.SetCamera(Camara.Position + new Vector3(0, 0, -10f), Camara.LookAt);
            }
            if (Input.keyPressed(Key.D))
            {
                Camara.SetCamera(Camara.Position + new Vector3(-10f, 0, 0), Camara.LookAt);
            }
        }
        private void crearPisoDeFondo()
        {
            var pisoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Texturas\\f1\\f1piso2.png"); //"Texturas\\piso2.jpg");
                                                                                                                           //var pisoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "MeshCreator\\Scenes\\Ciudad\\Textures\\Grass.jpg"); //"Texturas\\piso2.jpg");

            suelo = new TgcPlane(new Vector3(-500, 0, -500), new Vector3(6000, 0, 6000), TgcPlane.Orientations.XZplane, pisoTexture, 10f, 10f);

        }

        private void crearCamara()
        {
            //Suelen utilizarse objetos que manejan el comportamiento de la camara.
            //Lo que en realidad necesitamos gráficamente es una matriz de View.
            //El framework maneja una cámara estática, pero debe ser inicializada.
            //Posición de la camara.
            var cameraPosition = new Vector3(CameraX, CameraY, CameraZ);
            //Quiero que la camara mire hacia el origen (0,0,0).
            var lookAt = Vector3.Empty;

            //Configuro donde esta la posicion de la camara y hacia donde mira.
            //Camara.SetCamera(cameraPosition, lookAt);
            //Camara en 1ra persona
            Camara = new TgcFpsCamera(new Vector3(-400, 50, -400), Input);
        }
        private void crearEdificios()
        {
            //Internamente el framework construye la matriz de view con estos dos vectores.
            //Luego en nuestro juego tendremos que crear una cámara que cambie la matriz de view con variables como movimientos o animaciones de escenas.


            var scene =
                loader.loadSceneFromFile(MediaDir + "MeshCreator\\Scenes\\Ciudad\\Ciudad-TgcScene.xml");

            //Crear varias instancias del modelo original, pero sin volver a cargar el modelo entero cada vez
            var rows = 6;
            var cols = 6;
            generarGrilla(rows, cols, scene);

        }

        private void generarGrilla(int rows, int cols, TgcScene scene)
        {
            int modMesh = 0;
            for (var i = 1; i <= rows; i++)
            {
                for (var j = 1; j <= cols; j++)
                {
                    if ((((j * 2) + 1) % 2 != 0) && (((i * 2) + 1) % 2 != 0))
                        if (dibujarEdificio(getNMesh(modMesh), i, j, scene))
                            modMesh++;
                }
            }

        }
        private int getNMesh(int modMesh)
        {
            //  0,1,2,3,4,5,6,7

            System.Random generator = new System.Random();
            int n = generator.Next(0, 8);

            if (ListaRandom.Count == 7)
                ListaRandom.Clear();

            while (ListaRandom.Contains(n))
                n = generator.Next(0, 8);

            ListaRandom.Add(n);

            if (n == 1)
                return 0;

            if (n >= 0 && n <= 7)
                return n;


            //     if (modMesh % 2==0)
            //       return 7;
            //   else
            //     return 4;
            return 4;
        }
        private bool dibujarEdificio(int nMesh, int i, int j, TgcScene scene)
        {

            float offset_row = 300;
            float offset_Col = 100;
            if (nMesh == 0)
            { //Edificio Blanco Espejado
                offset_row = 300 + ((i - 1) * 700);
                offset_Col = 300 + ((j - 1) * 600);
            }
            if (nMesh == 2)
            {//Edicio Ladrillos
                offset_row = -100 + ((i - 1) * 700);
                offset_Col = 850 + ((j - 1) * 600);
            }
            if (nMesh == 3)
            { //edifcio amarillo
                offset_row = 1000 + ((i - 1) * 700);
                offset_Col = 1290 + ((j - 1) * 600);
            }

            if (nMesh == 4)
            { //Edificio espejado
                offset_row = 395 + ((i - 1) * 700);
                offset_Col = 1290 + ((j - 1) * 600);
            }
            if (nMesh == 5)
            { //Edificio alto blanco finito espejado
                offset_row = 1000 + ((i - 1) * 700);
                offset_Col = -70 + ((j - 1) * 600);
            }
            if (nMesh == 6)
            { //Edificio alto blanco finito espejado
                offset_row = -100 + ((i - 1) * 700);
                offset_Col = -70 + ((j - 1) * 600);
            }
            if (nMesh == 7)
            { //Edificio alto blanco finito espejado
                offset_row = 1100 + ((i - 1) * 700);
                offset_Col = 600 + ((j - 1) * 600);
            }
            edificio = scene.Meshes[nMesh];
            var instance = edificio.createMeshInstance(edificio.Name + i + "_" + j);
            //No recomendamos utilizar AutoTransform, en juegos complejos se pierde el control. mejor utilizar Transformaciones con matrices.
            instance.AutoTransformEnable = true;
            //Desplazarlo
            // instance.move(i * offset_row, 0, j * offset_Col);
            instance.move(offset_row, 0, offset_Col);
            if (nMesh == 0)
                instance.Scale = new Vector3(0.70f, 1f, 1f);

            if (nMesh == 4)
                instance.Scale = new Vector3(0.40f, 1f, 1f);

            meshes.Add(instance);

            return true;
        }
        private void crearCalles()
        {

            var CalleTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Texturas\\f1\\f1piso2.png");
            calle = new TgcPlane(new Vector3(-100, 0, -100), new Vector3(50, 2, 600), TgcPlane.Orientations.XZplane, CalleTexture, 10f, 10f);
            //calle = new TgcPlane();
            //calle.setTexture(CalleTexture);
            //Crear varias instancias del modelo original, pero sin volver a cargar el modelo entero cada vez
            //  meshes = new System.Collections.Generic.List<TgcMesh>();
            //  generarGrillaCalles(rows, cols, scene);
        }

        private void crearVeredas()
        {

            var cordonTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Texturas\\granito.jpg");
            var veredaTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Texturas\\piso2.jpg");

            cordones.Add(new TgcPlane(new Vector3(-450, 5, -450), new Vector3(5900, 0, 5), TgcPlane.Orientations.XZplane, cordonTexture, 40, 1));
            cordones.Add(new TgcPlane(new Vector3(-450, 0, -445), new Vector3(5895, 5, 0), TgcPlane.Orientations.XYplane, cordonTexture, 40, 1));
            veredas.Add(new TgcPlane(new Vector3(-500, 5, -500), new Vector3(6000, 0, 50), TgcPlane.Orientations.XZplane, veredaTexture, 60, 1));

            cordones.Add(new TgcPlane(new Vector3(-450, 5, -445), new Vector3(5, 0, 5890), TgcPlane.Orientations.XZplane, cordonTexture, 1, 40));
            cordones.Add(new TgcPlane(new Vector3(-445, 0, -445), new Vector3(0, 5, 5890), TgcPlane.Orientations.YZplane, cordonTexture, 1, 40));
            veredas.Add(new TgcPlane(new Vector3(-500, 5, -450), new Vector3(50, 0, 5950), TgcPlane.Orientations.XZplane, veredaTexture, 1, 60));

            cordones.Add(new TgcPlane(new Vector3(-450, 5, 5445), new Vector3(5900, 0, 5), TgcPlane.Orientations.XZplane, cordonTexture, 40, 1));
            cordones.Add(new TgcPlane(new Vector3(-450, 0, 5445), new Vector3(5900, 5, 0), TgcPlane.Orientations.XYplane, cordonTexture, 40, 1));
            veredas.Add(new TgcPlane(new Vector3(-450, 5, 5500), new Vector3(5950, 0, -50), TgcPlane.Orientations.XZplane, veredaTexture, 60, 1));

            cordones.Add(new TgcPlane(new Vector3(5445, 5, -445), new Vector3(5, 0, 5890), TgcPlane.Orientations.XZplane, cordonTexture, 1, 40));
            cordones.Add(new TgcPlane(new Vector3(5445, 0, -445), new Vector3(0, 5, 5890), TgcPlane.Orientations.YZplane, cordonTexture, 1, 40));
            veredas.Add(new TgcPlane(new Vector3(5450, 5, -450), new Vector3(50, 0, 5900), TgcPlane.Orientations.XZplane, veredaTexture, 1, 60));

			var rows = 10;
            var cols = 6;
            for (var i = 0; i <= rows; i++)
            {
                for (var j = 0; j <= cols; j++)
                {
                    if (((j % 2) == 0) && (i % 2 == 0))
                    {
                        veredas.Add(new TgcPlane(new Vector3((150 + (i * 350)), 5, 390+ (j * 350)), new Vector3(550, 0, 550), TgcPlane.Orientations.XZplane, veredaTexture, 60, 60));
                    }
                }
            }
        }

        private void crearParedes()
        {

            var paredTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "MeshCreator\\Textures\\Ladrillo\\brick1_2.jpg");
            //var veredaTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Texturas\\piso2.jpg");

            paredes.Add(new TgcPlane(new Vector3(-500, 0, -500), new Vector3(0, 100, 6000), TgcPlane.Orientations.YZplane, paredTexture, 60, 1));
            paredes.Add(new TgcPlane(new Vector3(-500, 0, -500), new Vector3(6000, 100, 0), TgcPlane.Orientations.XYplane, paredTexture, 60, 1));
            paredes.Add(new TgcPlane(new Vector3(5500, 0, 5500), new Vector3(0, 100, -6000), TgcPlane.Orientations.YZplane, paredTexture, 60, 1));
            paredes.Add(new TgcPlane(new Vector3(-500, 0, 5500), new Vector3(6000, 100, 0), TgcPlane.Orientations.XYplane, paredTexture, 60, 1));

        }

        private void crearManzanas()
        {

            var cordonTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Texturas\\granito.jpg");
            calle = new TgcPlane(new Vector3(-100, 0, -100), new Vector3(50, 2, 600), TgcPlane.Orientations.XZplane, cordonTexture, 10f, 10f);
            //calle = new TgcPlane();
            //calle.setTexture(CalleTexture);
            //Crear varias instancias del modelo original, pero sin volver a cargar el modelo entero cada vez
            //  meshes = new System.Collections.Generic.List<TgcMesh>();
            //  generarGrillaCalles(rows, cols, scene);
        }
        /*    private void generarGrillaCalles(int rows, int cols, TgcScene scene)
            {
                int modMesh = 0;
                for (var i = 1; i <= rows; i++)
                {
                    for (var j = 1; j <= cols; j++)
                    {
                        if ((j % 2 == 0) && (i  % 2 == 0))
                            if (dibujarCalle(getNMeshCalle(modMesh), i, j, scene))
                                modMesh++;
                    }
                }

            }
            private int getNMeshCalle(int modMesh)
            {
                return 0;
            }
            private bool dibujarCalle(int nMesh, int i, int j, TgcScene scene)
            {
                float offset = 250;
            //    calle = scene.Meshes[nMesh];
                //Crear instancia de modelo
              //  var instance = calle.createMeshInstance(calle.Name + i + "_" + j);
                //No recomendamos utilizar AutoTransform, en juegos complejos se pierde el control. mejor utilizar Transformaciones con matrices.
               // instance.AutoTransformEnable = true;
                //Desplazarlo
               // instance.move(i * offset, 0, j * offset);
                //instance.Scale = new Vector3(0.25f, 0.25f, 0.25f);

                //meshes.Add(instance);
                return true;
            }*/
    }
}