using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Examples.Camara;


namespace TGC.Group.Model
{
    public class GameModel : TGCExample
    {
        private EscenarioLoader escenarioLoader;
        private TieFighterSpawner tieFighterSpawner;
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }


        public override void Init()
        {
            //Scene = new TgcSceneLoader().loadSceneFromFile(MediaDir + "Xwing\\TRENCH_RUN-TgcScene.xml");
            /*
            Bloque bloque = new Bloque(MediaDir, new TGCVector3(0f,0f,1000f), "Xwing\\TRENCH_RUN-TgcScene.xml");
            GameManager.Instance.AgregarRenderizable(bloque);

            Bloque bloque1 = new Bloque(MediaDir, new TGCVector3(0f, 100f, 3000f), "Xwing\\death+star-TgcScene.xml");
            Bloque bloque2 = new Bloque(MediaDir, new TGCVector3(0f, 0f, 5000f), "Xwing\\TRENCH_RUN-TgcScene.xml");
            GameManager.Instance.AgregarRenderizable(bloque1);
            GameManager.Instance.AgregarRenderizable(bloque2);
            */
            var posicionInicialDeNave = new TGCVector3(100, -15, -250);

            InputDelJugador input = new InputDelJugador(Input);

            Nave naveDelJuego = new Nave(MediaDir, posicionInicialDeNave, input);
            GameManager.Instance.AgregarRenderizable(naveDelJuego);

            Camara camaraDelJuego = new Camara(posicionInicialDeNave, 10, -50, naveDelJuego);
            Camera = camaraDelJuego;
            GameManager.Instance.Camara = camaraDelJuego;

            escenarioLoader = new EscenarioLoader(MediaDir, naveDelJuego);
            tieFighterSpawner = new TieFighterSpawner(MediaDir, naveDelJuego);
            //Skybox skybox = new Skybox(MediaDir, camaraDelJuego);
            //GameManager.Instance.AgregarRenderizable(skybox);
            /*
            Torreta torreta = new Torreta(MediaDir, new TGCVector3(10,2, 15), naveDelJuego);
            GameManager.Instance.AgregarRenderizable(torreta);

            Torreta torreta2 = new Torreta(MediaDir, new TGCVector3(-10, 2, 15), naveDelJuego);
            GameManager.Instance.AgregarRenderizable(torreta2);
            
            TGCVector3 direccionDisparo = posicionInicialDeNave - new TGCVector3(10, 2, 15);
            torreta.Disparar(direccionDisparo);
            torreta2.Disparar(posicionInicialDeNave - new TGCVector3(-10, 2, 15));
            */
            //TieFighter tieFighter = new TieFighter(MediaDir, new TGCVector3(100, -15, 100), naveDelJuego);
            //GameManager.Instance.AgregarRenderizable(tieFighter);

        }

        public override void Update()
        {
            PreUpdate();
            GameManager.Instance.Update(ElapsedTime);
            escenarioLoader.Update(ElapsedTime);
            tieFighterSpawner.Update(ElapsedTime);
            //Scene.Meshes.ForEach(delegate (TgcMesh mesh) { mesh.Transform= TGCMatrix.Scaling(20f, 20f, 20f); });
            //Scene.BoundingBox.transform(TGCMatrix.Scaling(10f, 10f, 10f));
            PostUpdate();
        }


        public override void Render()
        {
            PreRender();
            //Scene.RenderAll();
            //Scene.Meshes.ForEach(delegate (TgcMesh mesh) { mesh.BoundingBox.transform(TGCMatrix.Scaling(20f, 20f, 20f)); });
           // Scene.Meshes.ForEach(delegate (TgcMesh mesh) { mesh.BoundingBox.Render(); });

            GameManager.Instance.Render();
            PostRender();
        }

        public override void Dispose()
        {
            //Scene.DisposeAll();
            GameManager.Instance.Dispose();
        }
    }
}