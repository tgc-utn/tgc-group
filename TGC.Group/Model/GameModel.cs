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

        private TGCBox CajaTroll { get; set; }
        private TgcScene Scene;

        public override void Init()
        {

            Scene = new TgcSceneLoader().loadSceneFromFile(MediaDir + "Ciudad\\Ciudad-TgcScene.xml");

            var pathTexturaTroll = MediaDir + "troll.jpg";
            var texturaTroll = TgcTexture.createTexture(pathTexturaTroll);
            var size = new TGCVector3(4, 4, 10);
            CajaTroll = TGCBox.fromSize(size, texturaTroll);

            var posicionInicial = new TGCVector3(0, 200, 0); //Asumiendo que la camara tiene que empezar en el mismo lugar que la cajaTroll
            CajaTroll.Position = posicionInicial;

            Camara unaCamara = new Camara(posicionInicial, 20, 80);
            Camara = unaCamara;
            GameManager.Instance.Camara = unaCamara;

            var posicionDeLaPrimeraCajaMeme = posicionInicial + new TGCVector3(5, 0, 0);
            PicaraCaja UnaCajameme = new PicaraCaja(MediaDir, posicionDeLaPrimeraCajaMeme);
            GameManager.Instance.AgregarRenderizable(UnaCajameme);

            var posicionDeLaSegundaCajaMeme = posicionInicial + new TGCVector3(-5, 0, 0);
            PicaraCaja OtraCajaMeme = new PicaraCaja(MediaDir, posicionDeLaSegundaCajaMeme);
            GameManager.Instance.AgregarRenderizable(OtraCajaMeme);
        }

        public override void Update()
        {
            PreUpdate();
            //Obtenemos acceso al objeto que maneja input de mouse y teclado del framework
            var input = Input;

            //Declaramos un vector de movimiento inicializado en cero.
            //El movimiento sobre el suelo es sobre el plano XZ.
            //Sobre XZ nos movemos con las flechas del teclado o con las letas WASD.
            var movement = new TGCVector3(0,0,-1);

            //Movernos de izquierda a derecha, sobre el eje X.
            if (input.keyDown(Key.Left) || input.keyDown(Key.A))
            {
                movement.X = 1;
            }
            else if (input.keyDown(Key.Right) || input.keyDown(Key.D))
            {
                movement.X = -1;
            }

            //Movernos adelante y atras, sobre el eje Z.
            if (input.keyDown(Key.Up) || input.keyDown(Key.W))
            {
                movement.Y = 1;
            }
            else if (input.keyDown(Key.Down) || input.keyDown(Key.S))
            {
                movement.Y = -1;
            }

            movement *= 50f * ElapsedTime;
            CajaTroll.Position = CajaTroll.Position + movement;
            CajaTroll.Transform = TGCMatrix.Translation(CajaTroll.Position);

            GameManager.Instance.Update(ElapsedTime);
 
            PostUpdate();
        }


        public override void Render()
        {
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
            PreRender();

            Scene.RenderAll();
            CajaTroll.Render();

            GameManager.Instance.Render();



            PostRender();
        }

        public override void Dispose()
        {
            CajaTroll.Dispose();
            Scene.DisposeAll();

            GameManager.Instance.Render();
        }
    }
}