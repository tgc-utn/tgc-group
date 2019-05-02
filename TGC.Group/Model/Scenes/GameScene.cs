using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Input;
using TGC.Core.Text;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Group.Model.Input;
using TGC.Core.SkeletalAnimation;

namespace TGC.Group.Model.Scenes
{
    class GameScene : Scene
    {
        TgcText2D DrawText = new TgcText2D();
      
        //Scenary
        private World World { get; }

        //Boleano para ver si dibujamos el boundingbox
        private bool BoundingBox { get; set; }

        public delegate void Callback();
        Callback onEscapeCallback = () => {};

        public GameScene(TgcD3dInput input, string mediaDir) : base(input)
        { 
            PhysicsWorld.Init();
            backgroundColor = Color.FromArgb(1, 78, 129, 179);

            this.World = new World(new TGCVector3(0, 0, 0));
            
            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;

            //Textura de la carperta Media. Game.Default es un archivo de configuracion (Game.settings) util para poner cosas.
            //Pueden abrir el Game.settings que se ubica dentro de nuestro proyecto para configurar.
            var pathTexturaCaja = mediaDir + Game.Default.TexturaCaja;

            //Cargamos una textura, tener en cuenta que cargar una textura significa crear una copia en memoria.
            //Es importante cargar texturas en Init, si se hace en el render loop podemos tener grandes problemas si instanciamos muchas.
            var texture = TgcTexture.createTexture(pathTexturaCaja);

            //Creamos una caja 3D ubicada de dimensiones (5, 10, 5) y la textura como color.
            var size = new TGCVector3(5, 10, 5);
            //Construimos una caja según los parámetros, por defecto la misma se crea con centro en el origen y se recomienda así para facilitar las transformaciones.
            //Posición donde quiero que este la caja, es común que se utilicen estructuras internas para las transformaciones.
            //Entonces actualizamos la posición lógica, luego podemos utilizar esto en render para posicionar donde corresponda con transformaciones.
           // Box = new Element(new TGCVector3(-25, 0, 0), TGCBox.fromSize(size, texture).ToMesh("caja"));

            //Cargo el unico mesh que tiene la escena.
            TgcMesh Mesh = new TgcSceneLoader().loadSceneFromFile(mediaDir + "LogoTGC-TgcScene.xml").Meshes[0];
            //Defino una escala en el modelo logico del mesh que es muy grande.
            Mesh.Scale = new TGCVector3(0.5f, 0.5f, 0.5f);

            //this.TgcLogo = new Element(TGCVector3.Empty, Mesh);
            this.Camera = new Camera(new TGCVector3(30, 30, 200), input);


        }

        public override void Update(float elapsedTime)
        {
            PhysicsWorld.DynamicsWorld.StepSimulation(elapsedTime);

            CollisionManager.CheckCollitions(this.World.GetCollisionables());

            this.World.Update(this.Camera.Position);

            if (GameInput.Statistic.IsPressed(Input))
            {
                this.BoundingBox = !this.BoundingBox;
            }
            if (GameInput.Escape.IsPressed(Input))
            {
                onEscapeCallback();
            }
        }
        public override void Render()
        {
            ClearScreen();
            this.DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.OrangeRed);
            this.DrawText.drawText("Con clic izquierdo subimos la camara [Actual]: " + TGCVector3.PrintVector3(this.Camera.Position), 0, 30, Color.OrangeRed);

            //this.Box.Render();
            this.World.Render(this.Camera.Position);


            //Render de BoundingBox, muy útil para debug de colisiones.
            if (this.BoundingBox) {
                this.World.RenderBoundingBox(this.Camera.Position);
            }
        }

        public override void Dispose()
        {
            //this.Box.Dispose();
            this.World.Dispose();
        }


        public GameScene OnEscape(Callback onEscapeCallback)
        {
            this.onEscapeCallback = onEscapeCallback;
            return this;
        }
    }
}
