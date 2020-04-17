using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Input;
using Microsoft.DirectX.DirectInput;
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
            var loader = new TgcSceneLoader();
            var scene2 = loader.loadSceneFromFile(mediaDir + "StarFox\\Nave+de+foxV7-TgcScene.xml");

            //Solo nos interesa el primer modelo de esta escena (tiene solo uno)
            mainMesh = scene2.Meshes[0];
            mainMesh.Position = posicionInicial;
            mainMesh.Transform = TGCMatrix.Translation(0, 5, 0);
        }

        /// <summary>
        /// Estos 3 metodos de Update, Render y Dispose se llaman igual que los de GameModel pero no son llamados cada frame automaticamente.
        /// Lo que quiero decir es que lo unico que tienen en comun es el nombre y la logica que por convencion llevan adentro.
        /// El Update y el Render NO tienen que usar PreUpdate() y PostUpdate() ( o PreRender() y PostRender() ) al principio y al final de los metodos.
        /// <summary>


        private void Moverse(TGCVector3 versorDirector, float elapsedTime)
        {
            TGCVector3 movimientoDelFrame = new TGCVector3(0, 0, 0);
            TGCVector3 movimientoAdelante = new TGCVector3(0, 0, 1);

            movimientoDelFrame += versorDirector + movimientoAdelante;
            movimientoDelFrame *= 50f * elapsedTime;

            mainMesh.Position += movimientoDelFrame;
            mainMesh.Transform = TGCMatrix.Translation(mainMesh.Position);
            mainMesh.Transform = TGCMatrix.RotationY(9.0f);

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
        }
        public void Dispose()
        {
            mainMesh.Dispose();
        }
    }
}

