using BulletSharp;
using BulletSharp.Math;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System;
using System.Timers;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TGC.Core.BoundingVolumes;
using TGC.Core.BulletPhysics;
using TGC.Core.Collision;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Terrain;
using TGC.Core.Text;

namespace TGC.Group.Model
{
    public class GameModel : TGCExample
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

        //Objetos de escena
        private TgcScene  escena;
        private TgcMesh   cancha;
        private TgcSkyBox skyBox;

        //Objetos de juego
        private List<Jugador> jugadores = new List<Jugador>();
        private Jugador       jugadorActivo;
        private CamaraJugador camara;
        private Pelota        pelota;
        private List<Turbo>   turbos = new List<Turbo>();
        private Paredes       paredes;
        private Arco[]        arcos;

        private int golequipo1 = 0;
        private int golequipo2 = 0;
        private double contador   = 300;
        private double i = 0;

        //Objetos de fisica
        protected DiscreteDynamicsWorld             dynamicsWorld;
        protected CollisionDispatcher               dispatcher;
        protected DefaultCollisionConfiguration     collisionConfiguration;
        protected SequentialImpulseConstraintSolver constraintSolver;
        protected BroadphaseInterface               overlappingPairCache;
        private   RigidBody                         floorBody;

        // 2D
        private Drawer2D drawer2D;
        private Drawer2D rojo2D;
        private Drawer2D azul2D;
        private Drawer2D contador2D;
        private CustomSprite medidorTurbo;
        private CustomSprite equipoRojo;
        private CustomSprite equipoAzul;
        private CustomSprite contadorTiempo;
        private CustomSprite gameOver;
        private TgcText2D textoTurbo;
        private TgcText2D textoGolAzul;
        private TgcText2D textoGolRojo;
        private TgcText2D textoConTiempo;
        private double antElapse;

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

            FixedTickEnable = true;

            initFisica();

            initMeshes();

            initJugadores();
            
            pelota = new Pelota(escena.getMeshByName("Pelota"), new TGCVector3(0f, 50f, -100f));
            dynamicsWorld.AddRigidBody(pelota.Cuerpo);

            paredes = new Paredes(escena.getMeshByName("Box_5"));
            dynamicsWorld.AddRigidBody(paredes.Cuerpo);

            arcos = new Arco[2];

            arcos[0] = new Arco(escena.getMeshByName("Arco"),FastMath.PI);
            arcos[1] = new Arco(escena.getMeshByName("Arco"),0);

            dynamicsWorld.AddRigidBody(arcos[0].Cuerpo);
            dynamicsWorld.AddRigidBody(arcos[1].Cuerpo);

            camara = new CamaraJugador(jugadorActivo, pelota, Camera, paredes.Mesh.createBoundingBox());


            drawer2D = new Drawer2D();
            rojo2D = new Drawer2D();
            azul2D = new Drawer2D();
            contador2D = new Drawer2D();
            medidorTurbo = new CustomSprite();
            equipoRojo = new CustomSprite();
            equipoAzul = new CustomSprite();
            contadorTiempo = new CustomSprite();
            gameOver = new CustomSprite();
            
            medidorTurbo.Bitmap = new CustomBitmap(MediaDir + "\\Textures\\MedidorTurbo.png", D3DDevice.Instance.Device);
            equipoRojo.Bitmap = new CustomBitmap(MediaDir + "\\Textures\\EquipoRojo.png", D3DDevice.Instance.Device);
            equipoAzul.Bitmap = new CustomBitmap(MediaDir + "\\Textures\\EquipoAzul.png", D3DDevice.Instance.Device);
            contadorTiempo.Bitmap = new CustomBitmap(MediaDir + "\\Textures\\ContadorTiempo.png", D3DDevice.Instance.Device);
            gameOver.Bitmap = new CustomBitmap(MediaDir + "Textures\\GameOver.png", D3DDevice.Instance.Device);

            medidorTurbo.Scaling = new TGCVector2(.25f * D3DDevice.Instance.Height / medidorTurbo.Bitmap.Height, 
                .25f * D3DDevice.Instance.Height / medidorTurbo.Bitmap.Height);
            medidorTurbo.Position = new TGCVector2(D3DDevice.Instance.Width - medidorTurbo.Scaling.X * medidorTurbo.Bitmap.Height - .05f * D3DDevice.Instance.Width,
                D3DDevice.Instance.Height - medidorTurbo.Scaling.X * medidorTurbo.Bitmap.Height - .05f * D3DDevice.Instance.Height);

            equipoRojo.Scaling = new TGCVector2(.06f * D3DDevice.Instance.Height / equipoRojo.Bitmap.Height,
                .1f * D3DDevice.Instance.Height / equipoRojo.Bitmap.Height);
            //equipoRojo.Position = new TGCVector2(D3DDevice.Instance.Width - equipoRojo.Scaling.Y * equipoRojo.Bitmap.Height - .7f * D3DDevice.Instance.Width,
            //  D3DDevice.Instance.Height - equipoRojo.Scaling.X * equipoRojo.Bitmap.Height - .69f * D3DDevice.Instance.Height);
            equipoRojo.Position = new TGCVector2(725, 10);

            equipoAzul.Scaling = new TGCVector2(.06f * D3DDevice.Instance.Height / equipoAzul.Bitmap.Height,
                .1f * D3DDevice.Instance.Height / equipoAzul.Bitmap.Height);
            //equipoAzul.Position = new TGCVector2(D3DDevice.Instance.Width - equipoAzul.Scaling.Y * equipoAzul.Bitmap.Height - .5f * D3DDevice.Instance.Width,
            //    D3DDevice.Instance.Height - equipoAzul.Scaling.X * equipoAzul.Bitmap.Height - .69f * D3DDevice.Instance.Height);
            equipoAzul.Position = new TGCVector2(510, 10);

            contadorTiempo.Scaling = new TGCVector2(.60f * D3DDevice.Instance.Height / contadorTiempo.Bitmap.Height,
                .60f * D3DDevice.Instance.Height / contadorTiempo.Bitmap.Height);
            //contadorTiempo.Position = new TGCVector2(D3DDevice.Instance.Width - contadorTiempo.Scaling.X * contadorTiempo.Bitmap.Height - .46f * D3DDevice.Instance.Width,
            //    D3DDevice.Instance.Height - contadorTiempo.Scaling.X * contadorTiempo.Bitmap.Height - .60f * D3DDevice.Instance.Height);
            contadorTiempo.Position = new TGCVector2(440,-160);

            gameOver.Scaling = new TGCVector2(.30f * D3DDevice.Instance.Height / gameOver.Bitmap.Height,
                .30f * D3DDevice.Instance.Height / gameOver.Bitmap.Height);
            //gameOver.Position = new TGCVector2(D3DDevice.Instance.Width - gameOver.Scaling.X * gameOver.Bitmap.Height - .46f * D3DDevice.Instance.Width,
            //    D3DDevice.Instance.Height - gameOver.Scaling.X * gameOver.Bitmap.Height - .60f * D3DDevice.Instance.Height);
            gameOver.Position = new TGCVector2(480, 180);

            textoTurbo = new TgcText2D();
            textoTurbo.Align = TgcText2D.TextAlign.CENTER;
            textoTurbo.Size = new Size((int)(250 * medidorTurbo.Scaling.X), (int)(250 * medidorTurbo.Scaling.X));
            textoTurbo.Position = new Point((int)medidorTurbo.Position.X, (int)medidorTurbo.Position.Y + (int)(90 * medidorTurbo.Scaling.X));
            textoTurbo.Color = Color.White;
            textoTurbo.changeFont(new Font("TimesNewRoman", 50, FontStyle.Bold));

            textoGolAzul = new TgcText2D();
            textoGolAzul.Align = TgcText2D.TextAlign.CENTER;
            textoGolAzul.Size = new Size((int)(250 *equipoAzul.Scaling.X), (int)(250 * equipoAzul.Scaling.X));
            //textoGolAzul.Position = new Point((int)medidorTurbo.Position.X, (int)medidorTurbo.Position.Y + (int)(90 * medidorTurbo.Scaling.X));
            textoGolAzul.Position = new Point(470, 20);
            textoGolAzul.Color = Color.Black;
            textoGolAzul.changeFont(new Font("TimesNewRoman", 30, FontStyle.Bold));

            textoGolRojo = new TgcText2D();
            textoGolRojo.Align = TgcText2D.TextAlign.CENTER;
            textoGolRojo.Size = new Size((int)(250 * equipoRojo.Scaling.X), (int)(250 * equipoRojo.Scaling.X));
            //textoGolAzul.Position = new Point((int)medidorTurbo.Position.X, (int)medidorTurbo.Position.Y + (int)(90 * medidorTurbo.Scaling.X));
            textoGolRojo.Position = new Point(685, 20);
            textoGolRojo.Color = Color.Black;
            textoGolRojo.changeFont(new Font("TimesNewRoman", 30, FontStyle.Bold));

            textoConTiempo = new TgcText2D();
            textoConTiempo.Align = TgcText2D.TextAlign.CENTER;
            textoConTiempo.Size = new Size((int)(90 * contadorTiempo.Scaling.X), (int)(90 * contadorTiempo.Scaling.X));
            //textoGolAzul.Position = new Point((int)medidorTurbo.Position.X, (int)medidorTurbo.Position.Y + (int)(90 * medidorTurbo.Scaling.X));
            textoConTiempo.Position = new Point(620, 40);
            textoConTiempo.Color = Color.Black;
            textoConTiempo.changeFont(new Font("TimesNewRoman", 30, FontStyle.Bold));

            System.Timers.Timer objTimer = new System.Timers.Timer();
            objTimer.Interval = 300;
            objTimer.Elapsed += new System.Timers.ElapsedEventHandler(objTimer_Elapsed);
            objTimer.Start();
            objTimer.BeginInit();

            
        }

        private void objTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
            
        }

        private void initFisica()
        {
            //Creamos el mundo fisico por defecto.
            collisionConfiguration = new DefaultCollisionConfiguration();

            dispatcher = new CollisionDispatcher(collisionConfiguration);
            GImpactCollisionAlgorithm.RegisterAlgorithm(dispatcher);

            constraintSolver = new SequentialImpulseConstraintSolver();

            overlappingPairCache = new DbvtBroadphase();

            dynamicsWorld = new DiscreteDynamicsWorld(dispatcher, overlappingPairCache, constraintSolver, collisionConfiguration);
            dynamicsWorld.Gravity = new TGCVector3(0, -10f, 0).ToBulletVector3();

            //Creamos el cuerpo del piso
            StaticPlaneShape          floorShape = new StaticPlaneShape(TGCVector3.Up.ToBulletVector3(), 0);
            DefaultMotionState        floorMotionState = new DefaultMotionState();
            RigidBodyConstructionInfo floorInfo = new RigidBodyConstructionInfo(0, floorMotionState, floorShape);

            floorBody = new RigidBody(floorInfo);
            dynamicsWorld.AddRigidBody(floorBody);
        }

        private void initMeshes()
        {

            //Crear SkyBox
            skyBox = new TgcSkyBox();
            skyBox.Center = new TGCVector3(0, 500, 0);
            skyBox.Size = new TGCVector3(10000, 10000, 10000);
            var texturesPath = MediaDir + "Textures\\SkyBox LostAtSeaDay\\";
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "lostatseaday_up.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "lostatseaday_dn.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "lostatseaday_lf.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "lostatseaday_rt.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "lostatseaday_bk.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "lostatseaday_ft.jpg");
            skyBox.Init();

            //cargar escena
            escena = new TgcSceneLoader().loadSceneFromFile(MediaDir + "Cancha-TgcScene.xml");

            cancha = escena.Meshes[0];
            

            TgcMesh meshTurbo = escena.getMeshByName("Turbo");
            turbos.Add(new Turbo(meshTurbo, new TGCVector3(80, 0, 100)));
            turbos.Add(new Turbo(meshTurbo, new TGCVector3(-80, 0, -100)));
            turbos.Add(new Turbo(meshTurbo, new TGCVector3(80, 0, -100)));
            turbos.Add(new Turbo(meshTurbo, new TGCVector3(-80, 0, 100)));

            turbos.Add(new Turbo(meshTurbo, new TGCVector3(0, 0, 130)));
            turbos.Add(new Turbo(meshTurbo, new TGCVector3(0, 0, -130)));
            turbos.Add(new Turbo(meshTurbo, new TGCVector3(0, 0, 250)));
            turbos.Add(new Turbo(meshTurbo, new TGCVector3(0, 0, -250)));

            turbos.Add(new Turbo(meshTurbo, new TGCVector3(220, 0, 0), 100));
            turbos.Add(new Turbo(meshTurbo, new TGCVector3(-220, 0, 0), 100));

            turbos.Add(new Turbo(meshTurbo, new TGCVector3(220, 0, 300), 100));
            turbos.Add(new Turbo(meshTurbo, new TGCVector3(-220, 0, -300), 100));
            turbos.Add(new Turbo(meshTurbo, new TGCVector3(-220, 0, 300), 100));
            turbos.Add(new Turbo(meshTurbo, new TGCVector3(220, 0, -300), 100));
        }

        private void initJugadores()
        {
            Jugador auto       = new Jugador(escena.Meshes[2], new TGCVector3(-20, 0, 100), new TGCVector3(0, 0, 0));
            Jugador tractor    = new Jugador(escena.Meshes[5], new TGCVector3(0, 0, -30), new TGCVector3(FastMath.PI, 0, 0));
            Jugador patrullero = new Jugador(escena.Meshes[3], new TGCVector3(0, 0, 30), new TGCVector3(0, 0, 0));
            Jugador tanque     = new Jugador(escena.Meshes[4], new TGCVector3(20, 0, -100), new TGCVector3(FastMath.PI, 0, 0));

            jugadorActivo = auto;

            jugadores.Add(auto);
            jugadores.Add(tractor);
            jugadores.Add(patrullero);
            jugadores.Add(tanque);

            dynamicsWorld.AddRigidBody(auto.Cuerpo);
            dynamicsWorld.AddRigidBody(tanque.Cuerpo);
            dynamicsWorld.AddRigidBody(patrullero.Cuerpo);
            dynamicsWorld.AddRigidBody(tractor.Cuerpo);
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        public override void Update()
        {
            PreUpdate();

            dynamicsWorld.StepSimulation(ElapsedTime, 10, TimeBetweenUpdates);

            foreach (var turbo in turbos)
            {
                turbo.Update(ElapsedTime);
            }

            foreach (Jugador jugador in jugadores)
            {
                jugador.Update(ElapsedTime);

                // Esto quedo medio feo, capaz estaria bueno trasladar esta logica a Turbo? O a Jugador?:
                Turbo turboEnContacto = turbos.Find(turbo => turbo.CheckCollideWith(jugador)); // Nunca vamos a tocar mas de 1 turbo en simultaneo
                if (turboEnContacto != null)
                    jugador.RecogerTurbo(turboEnContacto);
            }

            pelota.Update(ElapsedTime);

            arcos[0].Update(ElapsedTime);
            arcos[1].Update(ElapsedTime);

            if (pelota.CheckCollideWith(arcos[0]))
            {
                golequipo1++;
                pelota.ReiniciarPelota();
                jugadores.ForEach(jugador => jugador.ReiniciarJugador());
            }

            if (pelota.CheckCollideWith(arcos[1]))
            {
                golequipo2++;
                pelota.ReiniciarPelota();
                jugadores.ForEach(jugador => jugador.ReiniciarJugador());
            }

            antElapse += ElapsedTime;

            if (antElapse > 1)
            {
                contador -= 1;
                antElapse = 0;
            }
            //contador -= ElapsedTime;
            camara.Update(ElapsedTime);

            handleInput();

            textoTurbo.Text = jugadorActivo.Turbo.ToString();
            textoTurbo.Color = Color.FromArgb(255, 255 - (int)(jugadorActivo.Turbo * 2.55), 255 - (int)(Math.Min(jugadorActivo.Turbo, 50) * 4.55));

            
            PostUpdate();
        }


        private void handleInput()
        {
            jugadorActivo.HandleInput(Input);

            // Queremos esto todavia?:
            //System.Windows.Forms.Cursor.Position = new Point(Form.GameForm.ActiveForm.Width / 2, Form.GameForm.ActiveForm.Height / 2);

            if (Input.keyDown(Key.Escape))
            {
                TGC.Group.Form.GameForm.ActiveForm.Close();
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

            DrawText.drawText("Turbo: " + jugadorActivo.Turbo, 1800, 20, Color.Red);

            skyBox.Render();

            pelota.Mesh.Technique = "DIFFUSE_MAP_AND_LIGHTMAP";
            pelota.Render();

            cancha.Transform = new TGCMatrix(floorBody.InterpolationWorldTransform);
            cancha.Render();

            foreach (var jugador in jugadores)
            {
                jugador.Render();
            }

            arcos[0].Render();
            arcos[1].Render();

            DrawText.drawText("posicion del jugador: " + jugadorActivo.Translation.ToString(), 0, 20, Color.Red);

            foreach (var turbo in turbos)
            {
                turbo.Render();
            }

            paredes.Render();

            //DrawText.drawText("GOL Equipo 1: " + golequipo1, 140, 120, Color.DarkRed);
            //DrawText.drawText("GOL Equipo 2: " + golequipo2, 140, 140, Color.DarkRed);
            textoGolAzul.Text = golequipo1.ToString();
            textoGolRojo.Text = golequipo2.ToString();
            textoConTiempo.Text = contador.ToString();

            drawer2D.BeginDrawSprite();
            drawer2D.DrawSprite(medidorTurbo);
            drawer2D.EndDrawSprite();

            rojo2D.BeginDrawSprite();
            rojo2D.DrawSprite(equipoRojo);
            rojo2D.EndDrawSprite();

            azul2D.BeginDrawSprite();
            azul2D.DrawSprite(equipoAzul);
            azul2D.EndDrawSprite();

            contador2D.BeginDrawSprite();
            contador2D.DrawSprite(contadorTiempo);
            contador2D.EndDrawSprite();

           
           
           

            if (contador < 0)
              { 
               contador2D.BeginDrawSprite();
               contador2D.DrawSprite(gameOver);
               contador2D.EndDrawSprite();
                
               }

            textoGolAzul.render();
            textoGolRojo.render();
            textoConTiempo.render();
            textoTurbo.render();

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

            foreach (Jugador jugador in jugadores)
            {
                jugador.Dispose();
            }

            pelota.Dispose();

            escena.DisposeAll();   
            
            foreach (Turbo turbo in turbos)
            {
                turbo.Dispose();
            }
        }
    }
}