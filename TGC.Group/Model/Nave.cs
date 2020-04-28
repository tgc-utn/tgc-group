using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Input;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Text;
using System.Drawing;

namespace TGC.Group.Model
{
    public class Nave : IRenderizable
    {
        private readonly string mediaDir;
        private readonly TGCVector3 posicionInicial; 
        private TgcMesh mainMesh;
        private TgcD3dInput input;

        public Nave(string mediaDir, TGCVector3 posicionInicial, TgcD3dInput input)
        {
            this.mediaDir = mediaDir;
            this.posicionInicial = posicionInicial;
            this.input = input;
        }

        public void Init()
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene2 = loader.loadSceneFromFile(mediaDir + "StarFox\\Nave+de+foxV7-TgcScene.xml");

            //Solo nos interesa el primer modelo de esta escena (tiene solo uno)
            mainMesh = scene2.Meshes[0];
            mainMesh.Position = posicionInicial;
            mainMesh.Transform = TGCMatrix.Translation(0, 5, 0);
        }

        private void Moverse(TGCVector3 versorDirector, float elapsedTime)
        {
            TGCVector3 movimientoDelFrame = new TGCVector3(0, 0, 0);
            TGCVector3 movimientoAdelante = new TGCVector3(0, 0, 1);

            movimientoDelFrame += versorDirector + movimientoAdelante;
            movimientoDelFrame *= 10f * elapsedTime;

            mainMesh.Position += movimientoDelFrame;
            mainMesh.Transform = TGCMatrix.Translation(mainMesh.Position);

        }


        public void Update(float elapsedTime)
        {
            TGCVector3 direccionDelInput = new TGCVector3(0, 0, 0); //A "direccion" se refiere a direccion y sentido.

            if (input.keyDown(Key.Left) || input.keyDown(Key.A))
            {
                direccionDelInput.X = -1;
            }
            else if (input.keyDown(Key.Right) || input.keyDown(Key.D))
            {
                direccionDelInput.X = 1;
            }

            if (input.keyDown(Key.Up) || input.keyDown(Key.W))
            {
                direccionDelInput.Y = 1;
            }
            else if (input.keyDown(Key.Down) || input.keyDown(Key.S))
            {
                direccionDelInput.Y = -1;
            }

            Moverse(direccionDelInput, elapsedTime);

        }


        public void Render()
        {
            mainMesh.Render();
            new TgcText2D().drawText("Posicion de la nave:\n" + mainMesh.Position.ToString(), 5, 20, Color.White);
            new TgcText2D().drawText("Rotacion de la nave:\n" + mainMesh.Rotation.ToString(), 5, 100, Color.White);//    Esto aparece un por un rato pero despues crashea todo :(
        }
        public void Dispose()
        {
            mainMesh.Dispose();
        }
    }
}

