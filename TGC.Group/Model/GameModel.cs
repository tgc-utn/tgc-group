using BulletSharp;
using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Terrain;

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
        private TgcMesh   arco;
        private TgcMesh   paredes;
        private TgcSkyBox skyBox;

        //Objetos de juego
        private List<Jugador> jugadores = new List<Jugador>();
        private Jugador       jugadorActivo;
        private CamaraJugador camara;
        private Pelota        pelota;

        //Objetos de fisica
        protected DiscreteDynamicsWorld             dynamicsWorld;
        protected CollisionDispatcher               dispatcher;
        protected DefaultCollisionConfiguration     collisionConfiguration;
        protected SequentialImpulseConstraintSolver constraintSolver;
        protected BroadphaseInterface               overlappingPairCache;
        private   RigidBody                         floorBody;

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
            
            pelota = new Pelota( escena.getMeshByName("Pelota"), new TGCVector3(0f, 50f, 0f));
            dynamicsWorld.AddRigidBody(pelota.Cuerpo);

            camara = new CamaraJugador(jugadorActivo, pelota, Camera);
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

            arco = escena.Meshes[1];

            paredes = escena.getMeshByName("Box_5");
        }

        private void initJugadores()
        {
            Jugador Auto = new Jugador(escena.Meshes[2], new TGCVector3(-20, 0, 100), new TGCVector3(0, 0, 0));
            Jugador Tractor = new Jugador(escena.Meshes[5], new TGCVector3(0, 0, -30), new TGCVector3(0, FastMath.PI, 0));
            Jugador Patrullero = new Jugador(escena.Meshes[3], new TGCVector3(0, 0, 30), new TGCVector3(0, 0, 0));
            Jugador Tanque = new Jugador(escena.Meshes[4], new TGCVector3(20, 0, -100), new TGCVector3(0, FastMath.PI, 0));

            jugadorActivo = Auto;

            jugadores.Add(Auto);
            jugadores.Add(Tractor);
            jugadores.Add(Patrullero);
            jugadores.Add(Tanque);
            dynamicsWorld.AddRigidBody(Auto.Cuerpo);
            dynamicsWorld.AddRigidBody(Tanque.Cuerpo);
            dynamicsWorld.AddRigidBody(Patrullero.Cuerpo);
            dynamicsWorld.AddRigidBody(Tractor.Cuerpo);
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

            foreach (Jugador jugador in jugadores)
            {
                jugador.Update(ElapsedTime);
            }

            pelota.Update(ElapsedTime);

            camara.Update(ElapsedTime);

            handleInput();

            PostUpdate();
        }

        private void handleInput()
        {
            jugadorActivo.HandleInput(Input);

            System.Windows.Forms.Cursor.Position = new Point(Form.GameForm.ActiveForm.Width / 2, Form.GameForm.ActiveForm.Height / 2);

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

            DrawText.drawText("Turbo: " + jugadorActivo.Turbo, 0, 20, Color.Red);

            skyBox.Render();

            arco.Rotation = new TGCVector3(0, 0, 0); 
            arco.UpdateMeshTransform();
            arco.Render();

            arco.Rotation = new TGCVector3(0, FastMath.PI, 0);
            arco.UpdateMeshTransform();
            arco.Render();
            
            pelota.Render();
            
            cancha.Transform = new TGCMatrix(floorBody.InterpolationWorldTransform);
            cancha.Render();

            foreach (var jugador in jugadores)
            {
                jugador.Render();
            }

            paredes.Transform = TGCMatrix.Identity;
            paredes.Render();

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
            escena.DisposeAll();
        }
    }
}