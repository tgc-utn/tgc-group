using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Nave : TGCExample
    {
        private TgcScene escenaNave;
        //private TGCMatrix escalaBase;
        public Nave(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public override void Init()
        {
            var loader = new TgcSceneLoader();
            escenaNave = loader.loadSceneFromFile(MediaDir + "ship-TgcScene.xml");
            //escalaBase = TGCMatrix.Scaling(new TGCVector3(0.2f, 0.2f, 0.2f));
        }
        public override void Update()
        {
        }
        public override void Render()
        {
            foreach (var mesh in escenaNave.Meshes)
            {
                mesh.Render();
            }
        }
        public override void Dispose()
        {
            escenaNave.DisposeAll();
        }
    }
}
