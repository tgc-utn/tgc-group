using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    public class Pez : TGCExample
    {

        private const float VELOCIDAD = 0.15f;
        private TgcMesh mesh;
        private float movidoEnX = 0f;
        private bool sentidoEnXEsPositivo = true;
        private TGCMatrix escalaBase;
        private TGCMatrix traslacionBase = TGCMatrix.Translation(new TGCVector3(0.0f, 0.0f, 0.0f));
        private TGCVector3 rotacion = new TGCVector3(0, 0, 0);

        public Pez(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public void actualizarPosicion(TGCVector3 posicion)
        {
            traslacionBase = TGCMatrix.Translation(posicion);
        }

        public override void Init()
        {
            var loader = new TgcSceneLoader();
            var scene = loader.loadSceneFromFile(MediaDir + "yellow_fish-TgcScene.xml");
            mesh = scene.Meshes[0];
            escalaBase = TGCMatrix.Scaling(new TGCVector3(0.3f, 0.3f, 0.3f));
        }

        public override void Update()
        {
            //Movimiento de derecha a izquierda, va y vuelve
            var direccionX = sentidoEnXEsPositivo ? 1f : -1f;
            if (movidoEnX > 20f || movidoEnX < -70f)
            {
                // tengo que girar mi pez y moverme para el otro lado
                rotacion -= new TGCVector3(0, FastMath.PI, 0);
                sentidoEnXEsPositivo = movidoEnX > -70f;
            }

            //Movernos de izquierda a derecha, sobre el eje X.
            movidoEnX += direccionX * VELOCIDAD * ElapsedTime;

            //Multiplicar movimiento por velocidad y elapsedTime
            TGCMatrix traslacion = traslacionBase * TGCMatrix.Translation(movidoEnX, 0, 0);
            mesh.Transform = escalaBase * TGCMatrix.RotationY(rotacion.Y) * traslacion;
        }
        public override void Render()
        {
            //PreRender();

            //Dibujar objeto principal
            //Siempre primero hacer todos los calculos de logica e input y luego al final dibujar todo (ciclo update-render)

            
            mesh.Render();

            mesh.BoundingBox.Render();

            //PostRender();
        }
        public override void Dispose()
        {
            mesh.Dispose();
        }

    }
}
