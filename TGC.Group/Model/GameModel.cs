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
    ///     Inicialmente puede ser renombrado o copiado para hacer más ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar el modelo que instancia GameForm <see cref="Form.GameForm.InitGraphics()" />
    ///     line 97.
    /// </summary>
    public class GameModel : TgcExample
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
            Scene = new TgcSceneLoader().loadSceneFromFile(MediaDir + "Ciudad\\Ciudad-TgcScene.xml");

            var posicionInicial = new TGCVector3(0, 200, -100); //Asumiendo que la camara tiene que empezar en el mismo lugar que la cajaTroll

            Camara unaCamara = new Camara(posicionInicial, 20, -120);
            Camara = unaCamara;
            GameManager.Instance.Camara = unaCamara;

            Nave nave = new Nave(MediaDir, posicionInicial,Input);
            GameManager.Instance.AgregarRenderizable(nave);
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