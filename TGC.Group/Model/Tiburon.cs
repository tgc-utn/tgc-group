using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Camara;

namespace TGC.Group.Model
{
    class Tiburon : TGCExample
    {
        private const float VELOCIDAD = 0.15f;
        private TgcMesh mesh;
        private TGCMatrix escalaBase;
        private TGCMatrix traslacionBase = TGCMatrix.Translation(new TGCVector3(0.0f, 0.0f, 0.0f));
        private TGCVector3 rotacion = new TGCVector3(0, 0, 0);
        public Tiburon(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }
        public override void Init()
        {
            var loader = new TgcSceneLoader();
            var scene = loader.loadSceneFromFile(MediaDir + "shark-TgcScene.xml");
            mesh = scene.Meshes[0];
            escalaBase = TGCMatrix.Scaling(new TGCVector3(0.2f, 0.2f, 0.2f));
        }

        public override void Update()
        {
            float movimientoX = 1;
            movimientoX *= VELOCIDAD * ElapsedTime;
            mesh.Position = mesh.Position + new TGCVector3(movimientoX, 0, 0);

            //Multiplicar movimiento por velocidad y elapsedTime
            TGCMatrix traslacion = traslacionBase * TGCMatrix.Translation(mesh.Position.X, 0, 0);
            mesh.Transform = escalaBase * TGCMatrix.RotationY(rotacion.Y) * traslacion;

        }

        public void aparecer(TgcCamera Camara)
        {
            // tengo que buscar la camara y pasar, cambio posicion
            // falta que tambien coincida con donde esta mirando el player??
            TGCVector3 camaraPosicion = Camara.Position + new TGCVector3(0, -5, -40);
            mesh.Position = camaraPosicion;
            traslacionBase = TGCMatrix.Translation(camaraPosicion);
            mesh.Transform = escalaBase * TGCMatrix.RotationY(rotacion.Y) * traslacionBase;
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
