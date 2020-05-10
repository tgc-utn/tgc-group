using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Coral : TGCExample
    {
        private TgcMesh mesh;
        private TGCMatrix escalaBase;
        public Coral(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }
        public void actualizarPosicion(TGCVector3 posicion)
        {
            // como hacer que toque el piso?
            mesh.Position = posicion;
        }

        public override void Init()
        {
            var loader = new TgcSceneLoader();
            var scene = loader.loadSceneFromFile(MediaDir + "coral-TgcScene.xml");
            mesh = scene.Meshes[0];
            escalaBase = TGCMatrix.Scaling(new TGCVector3(0.2f, 0.2f, 0.2f));
        }
        public override void Update()
        {
           mesh.Transform = escalaBase * TGCMatrix.Identity * TGCMatrix.Translation(mesh.Position);
        }
        public override void Render()
        {
            mesh.Render();
        }
        public override void Dispose()
        {
            mesh.Dispose();
        }
    }
}
