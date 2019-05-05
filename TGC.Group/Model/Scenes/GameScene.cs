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
        CustomSprite PDA;
        float PDAPositionX, finalPDAPositionX;

        delegate void InteractionLogic(float elapsedTime);
        InteractionLogic currentInteractionLogic;
        delegate void RenderLogic();
        RenderLogic stateDependentRenderLogic;
        public GameScene(TgcD3dInput input, string mediaDir) : base(input)
        { 
            backgroundColor = Color.FromArgb(1, 78, 129, 179);

            this.World = new World(new TGCVector3(0, 0, 0));

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

            D3DDevice.Instance.Device.SamplerState[0].AddressU = TextureAddress.Clamp;
            D3DDevice.Instance.Device.SamplerState[0].AddressV = TextureAddress.Clamp;
            D3DDevice.Instance.Device.SamplerState[0].AddressW = TextureAddress.Clamp;
            D3DDevice.Instance.Device.SamplerState[0].MinFilter = TextureFilter.Point;
            D3DDevice.Instance.Device.SetRenderState(RenderStates.Lighting, false);
            World = new World(new TGCVector3(0, 0, 0));
            Camera = new Camera(new TGCVector3(30, 30, 200), input);

            PDA = BitmapRepository.CreateSpriteFromPath(BitmapRepository.PDA);
            PDA.Scaling = new TGCVector2(.5f, .35f);
            Screen.CenterSprite(PDA);
            finalPDAPositionX = PDA.Position.X;

            currentInteractionLogic = WorldInteractionLogic;
            stateDependentRenderLogic = () => {};
        }

        public override void Update(float elapsedTime)
        {

            AquaticPhysics.Instance.DynamicsWorld.StepSimulation(elapsedTime);

            CollisionManager.CheckCollitions(this.World.GetCollisionables());

            this.World.Update(this.Camera.Position);

            skyBox.Center = new TGCVector3(Camera.Position);
            skyBox.Center = Camera.Position;

            if (GameInput.Statistic.IsPressed(Input))
            {
                this.BoundingBox = !this.BoundingBox;
            }
            if (GameInput.Escape.IsPressed(Input))
            {
                onPauseCallback();
            }

            currentInteractionLogic(elapsedTime);
        }
        public override void Render()
        {
            ClearScreen();

            this.skyBox.Render();
            this.World.Render(this.Camera.Position);

            this.World.Render(this.Camera.Position);

            stateDependentRenderLogic();

            drawer.BeginDrawSprite();
            drawer.DrawSprite(waterVision);
            drawer.EndDrawSprite();

            if (this.BoundingBox)
            {
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
        private void WorldInteractionLogic(float elapsedTime)
        {
            if (Input.keyPressed(Key.I))
            {
                currentInteractionLogic = TakePDAIn;
                stateDependentRenderLogic = RenderInventory;
                PDAPositionX = -PDA.Bitmap.Width * PDA.Scaling.X;
                PDA.Position = new TGCVector2(PDAPositionX, PDA.Position.Y);
                ((Camera)Camera).Freeze();
                return;
            }
        }
        private void InventoryInteractionLogic(float elapsedTime)
        {
            if (Input.keyPressed(Key.I))
            {
                currentInteractionLogic = TakePDAOut;
                return;
            }
        }
        private void TakePDAIn(float elapsedTime)
        {
            if (Input.keyPressed(Key.I))
            {
                currentInteractionLogic = TakePDAOut;
                return;
            }

            PDAPositionX += 4000f * elapsedTime;

            if (PDAPositionX > finalPDAPositionX)
            {
                PDAPositionX = finalPDAPositionX;
                currentInteractionLogic = InventoryInteractionLogic;
            }
            PDA.Position = new TGCVector2(PDAPositionX, PDA.Position.Y);
        }
        private void TakePDAOut(float elapsedTime)
        {
            if (Input.keyPressed(Key.I))
            {
                currentInteractionLogic = TakePDAIn;
                return;
            }

            PDAPositionX -= 4000f * elapsedTime;

            if (PDAPositionX + PDA.Bitmap.Width < 0)
            {
                PDAPositionX = finalPDAPositionX;
                currentInteractionLogic = WorldInteractionLogic;
                stateDependentRenderLogic = () => {};
                ((Camera)Camera).Unfreeze();
            }
            PDA.Position = new TGCVector2(PDAPositionX, PDA.Position.Y);
        }
        private void RenderInventory()
        {
            drawer.BeginDrawSprite();
            drawer.DrawSprite(PDA);
            drawer.EndDrawSprite();
        }
    }
}
