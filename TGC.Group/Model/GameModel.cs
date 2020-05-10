using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using BulletSharp;
using TGC.Core.BulletPhysics;
using BulletSharp.Math;
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

        private TgcScene Escena { get; set; }
        private TgcMesh Cancha { get; set; }
        private TgcMesh Arco { get; set; }
        private TgcMesh Pelota { get; set; }

        private List<Jugador> jugadores = new List<Jugador>();

        private TgcMesh paredes { get; set; }

        private TgcSkyBox skyBox;

        private TGCVector3 LookAt;
        private TGCVector3 CameraPosition;
        protected DiscreteDynamicsWorld dynamicsWorld;
        protected CollisionDispatcher dispatcher;
        protected DefaultCollisionConfiguration collisionConfiguration;
        protected SequentialImpulseConstraintSolver constraintSolver;
        protected BroadphaseInterface overlappingPairCache;

        private RigidBody floorBody;

        RigidBody ballBody;

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

            FixedTickEnable = false;

            //Creamos el mundo fisico por defecto.
            collisionConfiguration = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConfiguration);
            GImpactCollisionAlgorithm.RegisterAlgorithm(dispatcher);
            constraintSolver = new SequentialImpulseConstraintSolver();
            overlappingPairCache = new DbvtBroadphase(); //AxisSweep3(new BsVector3(-5000f, -5000f, -5000f), new BsVector3(5000f, 5000f, 5000f), 8192);
            dynamicsWorld = new DiscreteDynamicsWorld(dispatcher, overlappingPairCache, constraintSolver, collisionConfiguration);
            dynamicsWorld.Gravity = new TGCVector3(0, -10f, 0).ToBulletVector3();


            var floorShape = new StaticPlaneShape(TGCVector3.Up.ToBulletVector3(), 0);
            var floorMotionState = new DefaultMotionState();
            var floorInfo = new RigidBodyConstructionInfo(0, floorMotionState, floorShape);
            floorBody = new RigidBody(floorInfo);
            dynamicsWorld.AddRigidBody(floorBody);

            ballBody = BulletRigidBodyFactory.Instance.CreateBall(1f, 1f, new TGCVector3(0f, 50f, 0f));
            dynamicsWorld.AddRigidBody(ballBody);


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

            Escena = new TgcSceneLoader().loadSceneFromFile(MediaDir + "Cancha2-TgcScene.xml");
            Cancha = Escena.Meshes[0];
            Arco = Escena.Meshes[1];
            Jugador Auto = new Jugador(Escena.Meshes[2], new TGCVector3(-20, 0, 100), new TGCVector3(0, 0, 0));
            Jugador Tractor = new Jugador(Escena.Meshes[5], new TGCVector3(0, 0, -30), new TGCVector3(0, FastMath.PI, 0));
            Jugador Patrullero = new Jugador(Escena.Meshes[3], new TGCVector3(0, 0, 30), new TGCVector3(0, 0, 0));
            Jugador Tanque = new Jugador(Escena.Meshes[4], new TGCVector3(20, 0, -100), new TGCVector3(0, FastMath.PI, 0));

            jugadores.Add(Auto);
            jugadores.Add(Tanque);
            jugadores.Add(Patrullero);
            jugadores.Add(Tractor);
            dynamicsWorld.AddRigidBody(Auto.CocheBody);
            dynamicsWorld.AddRigidBody(Tanque.CocheBody);
            dynamicsWorld.AddRigidBody(Patrullero.CocheBody);
            dynamicsWorld.AddRigidBody(Tractor.CocheBody);

            Pelota = Escena.getMeshByName("Pelota");

            paredes = Escena.getMeshByName("Box_5");


            LookAt = new TGCVector3(TGCVector3.Empty);
            CameraPosition = new TGCVector3(0, 100, 225);
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

            LookAt.X -= Input.XposRelative * 1.5f;
            LookAt.Y -= Input.YposRelative * 1.5f;
            Camera.SetCamera(CameraPosition, LookAt);

            System.Windows.Forms.Cursor.Position = new Point(Form.GameForm.ActiveForm.Width / 2, Form.GameForm.ActiveForm.Height / 2);

            if (Input.keyDown(Key.W))
            {
                
                CameraPosition -= TGCVector3.Normalize(CameraPosition - LookAt) * ElapsedTime * 100f;
                //CameraPosition.Z -= 100f * ElapsedTime;
            }
            if (Input.keyDown(Key.S))
            {
                CameraPosition += TGCVector3.Normalize(CameraPosition - LookAt) * ElapsedTime * 100f;
                //CameraPosition.Z += 100f * ElapsedTime;
            }

            PostUpdate();
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

            skyBox.Render();

            Arco.Rotation = new TGCVector3(0, 0, 0); 
            Arco.UpdateMeshTransform();
            Arco.Render();

            Arco.Rotation = new TGCVector3(0, FastMath.PI, 0);
            Arco.UpdateMeshTransform();
            Arco.Render();

            Pelota.Transform = TGCMatrix.Scaling(1, 1, 1) * new TGCMatrix(ballBody.InterpolationWorldTransform);
            Pelota.Render();

            //Cuando tenemos modelos mesh podemos utilizar un método que hace la matriz de transformación estándar.
            //Es útil cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones jerárquicas o complicadas.
            Cancha.UpdateMeshTransform();
            Cancha.Render();

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
            Escena.DisposeAll();
        }
    }
}