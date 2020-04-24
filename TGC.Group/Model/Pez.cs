using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Terrain;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    public class Pez : TgcExample
    {

        private const float VELOCIDAD = 0.05f;
        private TgcMesh mesh;
        private float movidoEnX = 0f;
        private bool sentidoEnXEsPositivo = true;

        public Pez(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public void actualizarPosicion(TGCVector3 posicion)
        {
            mesh.Position = posicion;
        }

        public override void Init()
        {
            var loader = new TgcSceneLoader();
            var scene = loader.loadSceneFromFile(MediaDir + "yellow_fish-TgcScene.xml");
            mesh = scene.Meshes[0];
            mesh.Transform = TGCMatrix.Translation(-25, 0, 0);
            mesh.Scale = new TGCVector3(0.3f, 0.3f, 0.3f);
        }
        public override void Update()
        {
            //Declaramos un vector de movimiento inicializado en cero.
            //El movimiento sobre el suelo es sobre el plano XZ.
            //Sobre XZ nos movemos con las flechas del teclado o con las letas WASD.
            var movement = TGCVector3.Empty;
            var direccionX = sentidoEnXEsPositivo ? 1f : -1f;
            if (movidoEnX > 1250f || movidoEnX < -500f)
            {
                // tengo que girar mi pez y moverme para el otro lado

                mesh.Rotation -= new TGCVector3(0, FastMath.PI, 0);
                mesh.Transform = TGCMatrix.RotationY(mesh.Rotation.Y);
                sentidoEnXEsPositivo = movidoEnX < -500f;
            }


            //Movernos de izquierda a derecha, sobre el eje X.
            movement.X = direccionX;
            //movement.Z = -1;

            movidoEnX += direccionX;

            //Multiplicar movimiento por velocidad y elapsedTime
            movement *= VELOCIDAD * ElapsedTime;
            mesh.Position = mesh.Position + movement;
            mesh.Transform = TGCMatrix.Translation(mesh.Position);            
        }
        public override void Render()
        {
            //PreRender();

            //Dibujar objeto principal
            //Siempre primero hacer todos los calculos de logica e input y luego al final dibujar todo (ciclo update-render)
            mesh.Render();

            //PostRender();
        }
        public override void Dispose()
        {
            mesh.Dispose();
        }

    }
}
