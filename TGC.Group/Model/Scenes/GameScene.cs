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
using TGC.Core.Terrain;
using Microsoft.DirectX;
using TGC.Group.TGCUtils;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.Model.Utils;
using Microsoft.DirectX.Direct3D;
using TGC.Core.SkeletalAnimation;
using TGC.Group.Model.Elements.RigidBodyFactories;
using TGC.Core.Terrain;

namespace TGC.Group.Model.Scenes
{
    class GameScene : Scene
    {
        readonly TgcText2D DrawText = new TgcText2D();
        private World World { get; }
        private bool BoundingBox { get; set; }

        public delegate void Callback();
        Callback onPauseCallback = () => {};
        TgcSkyBox skyBox;
        CustomSprite waterVision;
        Drawer2D drawer = new Drawer2D();

        public GameScene(TgcD3dInput input, string mediaDir) : base(input)
        { 
            backgroundColor = Color.FromArgb(1, 78, 129, 179);
<<<<<<< HEAD

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
            Box = new Element(new TGCVector3(-25, 0, 0), TGCBox.fromSize(size, texture).ToMesh("caja"));

            //Cargo el unico mesh que tiene la escena.
            TgcMesh Mesh = new TgcSceneLoader().loadSceneFromFile(mediaDir + "LogoTGC-TgcScene.xml").Meshes[0];
            //Defino una escala en el modelo logico del mesh que es muy grande.
            Mesh.Scale = new TGCVector3(0.5f, 0.5f, 0.5f);

            this.TgcLogo = new Element(TGCVector3.Empty, Mesh);

            this.Camera = new Camera(new TGCVector3(30, 30, 200), input);

            string baseDir = "../../../res/";

            skyBox = new TgcSkyBox();
            skyBox.SkyEpsilon = 0;
            //skyBox.Color = Color.FromArgb(188, 76, 100, 160);
            skyBox.Center = Camera.Position;
            skyBox.Size = new TGCVector3(30000, 30000, 30000);
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, baseDir +    "underwater_skybox-up.jpg"   );
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, baseDir + "underwater_skybox-down.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, baseDir + "underwater_skybox-left.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, baseDir + "underwater_skybox-right.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, baseDir + "underwater_skybox-front.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, baseDir + "underwater_skybox-back.jpg");
            skyBox.Init();
            D3DDevice.Instance.Device.Transform.Projection =
                Matrix.PerspectiveFovLH(
                    45,
                    D3DDevice.Instance.AspectRatio,
                    D3DDevice.Instance.ZNearPlaneDistance,
                    D3DDevice.Instance.ZFarPlaneDistance * 3f
                );

            waterVision = BitmapRepository.CreateSpriteFromPath(BitmapRepository.WaterRectangle);
            Screen.FitSpriteToScreen(waterVision);
            waterVision.Color = Color.FromArgb(120, 76, 100, 160);

            //for(int i = 0; i < 100; ++i)
            //{
            //    try
            //    {
            //        D3DDevice.Instance.Device.SamplerState[i].AddressU = TextureAddress.Clamp;
            //        D3DDevice.Instance.Device.SamplerState[i].AddressV = TextureAddress.Clamp;
            //        D3DDevice.Instance.Device.SamplerState[i].AddressW = TextureAddress.Clamp;
            //        D3DDevice.Instance.Device.SamplerState[i].BorderColor = Color.Black;
            //    } catch(Exception e)
            //    {

            //    }
            //}

            D3DDevice.Instance.Device.SamplerState[0].AddressU = TextureAddress.Clamp;
            D3DDevice.Instance.Device.SamplerState[0].AddressV = TextureAddress.Clamp;
            D3DDevice.Instance.Device.SamplerState[0].AddressW = TextureAddress.Clamp;
            D3DDevice.Instance.Device.SamplerState[0].MinFilter = TextureFilter.Point;
            D3DDevice.Instance.Device.SetRenderState(RenderStates.Lighting, false);
            World = new World(new TGCVector3(0, 0, 0));
            Camera = new Camera(new TGCVector3(30, 30, 200), input);
        }

        public override void Update(float elapsedTime)
        {

            AquaticPhysics.Instance.DynamicsWorld.StepSimulation(elapsedTime);

            CollisionManager.CheckCollitions(this.World.GetCollisionables());

            this.World.Update(this.Camera.Position);

            skyBox.Center = new TGCVector3(Camera.Position);
            skyBox.Center = Camera.Position;
            //Capturar Input teclado
            if (GameInput.Statistic.IsPressed(Input))
            {
                this.BoundingBox = !this.BoundingBox;
            }
            if (GameInput.Escape.IsPressed(Input))
            {
                onPauseCallback();
            }
        }
        public override void Render()
        {
            ClearScreen();
            this.DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.OrangeRed);
            this.DrawText.drawText("Con clic izquierdo subimos la camara [Actual]: " + TGCVector3.PrintVector3(this.Camera.Position), 0, 30, Color.OrangeRed);

            this.skyBox.Render();
            this.World.Render(this.Camera.Position);

            drawer.BeginDrawSprite();
            drawer.DrawSprite(waterVision);
            drawer.EndDrawSprite();

            this.World.Render(this.Camera.Position);

            if (this.BoundingBox) {
                this.World.RenderBoundingBox(this.Camera.Position);
            }
        }

        public override void Dispose()
        {
            this.World.Dispose();
        }


        public GameScene OnPause(Callback onPauseCallback)
        {
            this.onPauseCallback = onPauseCallback;
            return this;
        }
    }
}
