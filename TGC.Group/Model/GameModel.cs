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
    /// <summary>
    ///     Ejemplo para implementar el TP.
    ///     Inicialmente puede ser renombrado o copiado para hacer m�s ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar el modelo que instancia GameForm <see cref="Form.GameForm.InitGraphics()" />
    ///     line 97.
    /// </summary>
    public class GameModel : TGCExample
    {
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        private TgcScene Scene;

        public override void Init()
        {
            Scene = new TgcSceneLoader().loadSceneFromFile(MediaDir + "Xwing\\TRENCH_RUN-TgcScene.xml");


            var posicionInicialDeNave = new TGCVector3(0, 5, -50); 

            Nave naveDelJuego = new Nave(MediaDir, posicionInicialDeNave,Input);
            GameManager.Instance.AgregarRenderizable(naveDelJuego);

            Camara camaraDelJuego = new Camara(posicionInicialDeNave, 30, -150, naveDelJuego);
            Camera = camaraDelJuego;
            GameManager.Instance.Camara = camaraDelJuego;

            Skybox skybox = new Skybox(MediaDir, camaraDelJuego);
            GameManager.Instance.AgregarRenderizable(skybox);

            Torreta torreta = new Torreta(MediaDir, new TGCVector3(0,10, 15));
            GameManager.Instance.AgregarRenderizable(torreta);

        }

        public override void Update()
        {
            PreUpdate();
            GameManager.Instance.Update(ElapsedTime);
            PostUpdate();
        }


        public override void Render()
        {
            PreRender();
            Scene.RenderAll();
            GameManager.Instance.Render();
            PostRender();
        }

        public override void Dispose()
        {
            Scene.DisposeAll();
            GameManager.Instance.Dispose();
        }
    }
}