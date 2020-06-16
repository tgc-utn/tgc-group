using BulletSharp;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.Terrain;
using TGC.Core.Text;
using TGC.Group.Model._2D;

namespace TGC.Group.Model
{
    class EscenaJuego : Escena
    {
        //Objetos de escena
        private TgcScene escena;
        private Pasto pasto;
        private TgcSkyBox skyBox;
        private Luz sol;

        //Objetos de juego
        private List<Jugador> jugadores = new List<Jugador>();
        private Jugador jugadorActivo;
        private CamaraJugador camara;
        private Pelota pelota;
        private List<Turbo> turbos;
        private Paredes paredes;
        private Arco[] arcos;

        private int golequipo1 = 0;
        private int golequipo2 = 0;
        private double tiempoRestante = 300;

        //Objetos de fisica
        protected DiscreteDynamicsWorld dynamicsWorld;
        protected CollisionDispatcher dispatcher;
        protected DefaultCollisionConfiguration collisionConfiguration;
        protected SequentialImpulseConstraintSolver constraintSolver;
        protected BroadphaseInterface overlappingPairCache;
        private RigidBody floorBody;

        // 2D
        private UIEscenaJuego ui;

        public EscenaJuego(TgcCamera Camera, string MediaDir, string ShadersDir, TgcText2D DrawText, float TimeBetweenUpdates, TgcD3dInput Input, List<Jugador> jugadores, Jugador jugadorActivo) : base(Camera, MediaDir, ShadersDir, DrawText, TimeBetweenUpdates, Input)
        {
            initFisica();

            initMeshes();

            this.jugadores = jugadores;
            this.jugadorActivo = jugadorActivo;
            initJugadores();

            sol = new Luz(Color.White, new TGCVector3(0, 70, -130));


            pelota = new Pelota(escena.getMeshByName("Pelota"), new TGCVector3(0f, 50f, 0f));
            pelota.Mesh.Effect = TGCShaders.Instance.LoadEffect(ShadersDir + "CustomShaders.fx");
            pelota.Mesh.Technique = "BlinnPhong";
            dynamicsWorld.AddRigidBody(pelota.Cuerpo);

            paredes = new Paredes(escena.getMeshByName("Box_5"));
            dynamicsWorld.AddRigidBody(paredes.Cuerpo);

            arcos = new Arco[2];

            arcos[0] = new Arco(escena.getMeshByName("Arco"), FastMath.PI);
            arcos[1] = new Arco(escena.getMeshByName("Arco"), 0);

            dynamicsWorld.AddRigidBody(arcos[0].Cuerpo);
            dynamicsWorld.AddRigidBody(arcos[1].Cuerpo);

            camara = new CamaraJugador(jugadorActivo, pelota, Camera, paredes.Mesh.createBoundingBox());

            ui = new UIEscenaJuego();
            ui.Init(MediaDir,drawer2D);
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
            StaticPlaneShape floorShape = new StaticPlaneShape(TGCVector3.Up.ToBulletVector3(), 0);
            DefaultMotionState floorMotionState = new DefaultMotionState();
            RigidBodyConstructionInfo floorInfo = new RigidBodyConstructionInfo(0, floorMotionState, floorShape);

            floorBody = new RigidBody(floorInfo);
            floorBody.Restitution = 1.25f;
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

            pasto = new Pasto(escena.Meshes[0], TGCShaders.Instance.LoadEffect(ShadersDir + "CustomShaders.fx"), 32, .5f);

            TgcMesh meshTurbo = escena.getMeshByName("Turbo");

            turbos = new List<Turbo>()
            {
                new Turbo(meshTurbo, new TGCVector3(80, -.2f, 100)),
                new Turbo(meshTurbo, new TGCVector3(-80, -.2f, -100)),
                new Turbo(meshTurbo, new TGCVector3(80, -.2f, -100)),
                new Turbo(meshTurbo, new TGCVector3(-80, -.2f, 100)),
                new Turbo(meshTurbo, new TGCVector3(0, -.2f, 130)),
                new Turbo(meshTurbo, new TGCVector3(0, -.2f, -130)),
                new Turbo(meshTurbo, new TGCVector3(0, -.2f, 250)),
                new Turbo(meshTurbo, new TGCVector3(0, -.2f, -250)),
                new Turbo(meshTurbo, new TGCVector3(220, -.2f, 0), 100),
                new Turbo(meshTurbo, new TGCVector3(-220, -.2f, 0), 100),
                new Turbo(meshTurbo, new TGCVector3(220, -.2f, 300), 100),
                new Turbo(meshTurbo, new TGCVector3(-220, -.2f, -300), 100),
                new Turbo(meshTurbo, new TGCVector3(-220, -.2f, 300), 100),
                new Turbo(meshTurbo, new TGCVector3(220, -.2f, -300), 100)
            };

        }

        private void initJugadores()
        {
            jugadores[0].Reubicar(new TGCVector3(-20, 0, 100), new TGCVector3(0, 0, 0));
            jugadores[1].Reubicar(new TGCVector3(0, 0, -30), new TGCVector3(FastMath.PI, 0, 0));
            jugadores[2].Reubicar(new TGCVector3(0, 0, 30), new TGCVector3(0, 0, 0));
            jugadores[3].Reubicar(new TGCVector3(20, 0, -100), new TGCVector3(FastMath.PI, 0, 0));
            foreach(var jugador in jugadores)
            {
                dynamicsWorld.AddRigidBody(jugador.Cuerpo);
            }
        }

        private void Reubicar()
        {
            pelota.ReiniciarPelota();
            jugadores.ForEach(jugador => jugador.ReiniciarJugador());
            turbos.ForEach(turbo => turbo.Reiniciar());
        }

        public override Escena Update(float ElapsedTime)
        {
            tiempoRestante -= ElapsedTime;

            if (tiempoRestante <= 0)
            {
                return CambiarEscena(new EscenaGameOver(Camera, MediaDir, ShadersDir, DrawText, TimeBetweenUpdates, Input));
            }

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
                Reubicar();
            }

            if (pelota.CheckCollideWith(arcos[1]))
            {
                golequipo2++;
                Reubicar();
            }

            pasto.Update(ElapsedTime);

            camara.Update(ElapsedTime);

            jugadorActivo.HandleInput(Input);
            if (Input.keyDown(Key.Escape))
            {
                return CambiarEscena(new EscenaMenu(Camera, MediaDir, ShadersDir, DrawText, TimeBetweenUpdates, Input));
            }

            ui.TextoTurbo = jugadorActivo.Turbo.ToString();
            ui.ColorTextoTurbo = Color.FromArgb(255, 255 - (int)(jugadorActivo.Turbo * 2.55), 255 - (int)(Math.Min(jugadorActivo.Turbo, 50) * 4.55));
            ui.TextoGolAzul = golequipo1.ToString();
            ui.TextoGolRojo = golequipo2.ToString();
            ui.TextoReloj = String.Format("{0:0}:{1:00}", Math.Floor(tiempoRestante / 60), tiempoRestante % 60);

            return this;
        }

        public override void Render()
        {
            skyBox.Render();

            pelota.Mesh.Effect.SetValue("normal_map", TextureLoader.FromFile(D3DDevice.Instance.Device, MediaDir + "Textures\\pelotaNormalMap.png"));
            pelota.Render(sol);

            pasto.Render();

            foreach (var jugador in jugadores)
            {
                jugador.Mesh.Effect.SetValue("eyePosition", TGCVector3.TGCVector3ToFloat3Array(Camera.Position));
                jugador.Render(sol);
            }

            arcos[0].Render();
            arcos[1].Render();

            DrawText.drawText("posicion del jugador: " + jugadorActivo.Translation.ToString(), 0, 20, Color.Red);

            foreach (var turbo in turbos)
            {
                turbo.Render();
            }

            paredes.Render();

            ui.Render();
            sol.Render();
        }
        public override void Dispose()
        {
            dynamicsWorld.Dispose();
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
