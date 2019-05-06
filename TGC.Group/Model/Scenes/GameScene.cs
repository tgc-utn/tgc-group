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

        string baseDir = "../../../res/";

        public delegate void Callback();
        Callback onPauseCallback = () => {};
        TgcSkyBox skyBox;
        CustomSprite waterVision, darknessCover, mask, aim, cursor;
        Drawer2D drawer = new Drawer2D();
        CustomSprite PDA;
        float PDAPositionX, finalPDAPositionX, PDAMoveCoefficient;
        int PDATransparency;

        delegate void InteractionLogic(float elapsedTime);
        InteractionLogic currentInteractionLogic, newUpdateLogic;
        delegate void RenderLogic();
        RenderLogic stateDependentRenderLogic, newRenderLogic;
        public GameScene(TgcD3dInput input, string mediaDir) : base(input)
        { 
            backgroundColor = Color.FromArgb(1, 78, 129, 179);

            this.World = new World(new TGCVector3(0, 0, 0));

            this.Camera = new Camera(new TGCVector3(30, 30, 200), input);

            IncrementFarPlane(3f);
            SetClampTextureAddressing();

            InitSkyBox();
            InitWaterVision();
            InitDarknessCover();
            InitMask();
            InitAim();

            World = new World(new TGCVector3(0, 0, 0));
            Camera = new Camera(new TGCVector3(30, 30, 200), input);

            PDA = BitmapRepository.CreateSpriteFromPath(BitmapRepository.PDA);
            PDA.Scaling = new TGCVector2(.5f, .35f);
            Screen.CenterSprite(PDA);
            finalPDAPositionX = PDA.Position.X;
            PDAMoveCoefficient = (finalPDAPositionX - GetPDAInitialPosition()) * 4;

            cursor = aim;

            currentInteractionLogic = WorldInteractionLogic;
            stateDependentRenderLogic = () => {};
        }

        private void IncrementFarPlane(float scale)
        {
            D3DDevice.Instance.Device.Transform.Projection =
                Matrix.PerspectiveFovLH(
                    45,
                    D3DDevice.Instance.AspectRatio,
                    D3DDevice.Instance.ZNearPlaneDistance,
                    D3DDevice.Instance.ZFarPlaneDistance * scale
                );
        }

        private void InitWaterVision()
        {
            waterVision = BitmapRepository.CreateSpriteFromPath(BitmapRepository.WaterRectangle);
            Screen.FitSpriteToScreen(waterVision);
            waterVision.Color = Color.FromArgb(120, 10, 70, 164);
        }
        private void InitDarknessCover()
        {
            darknessCover = BitmapRepository.CreateSpriteFromPath(BitmapRepository.BlackRectangle);
            Screen.FitSpriteToScreen(darknessCover);
            darknessCover.Color = Color.FromArgb(188, 0, 0, 0);
        }
        private void InitMask()
        {
            mask = BitmapRepository.CreateSpriteFromPath(BitmapRepository.Mask);
            Screen.FitSpriteToScreen(mask);
        }
        private void InitAim()
        {
            aim = BitmapRepository.CreateSpriteFromPath(BitmapRepository.Aim);
            Screen.CenterSprite(aim);
        }

        private void InitSkyBox()
        {
            skyBox = new TgcSkyBox();
            skyBox.SkyEpsilon = 0;
            skyBox.Center = Camera.Position;
            skyBox.Size = new TGCVector3(30000, 30000, 30000);
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up   , baseDir + "underwater_skybox-up.jpg"    );
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down , baseDir + "underwater_skybox-down.jpg"  );
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left , baseDir + "underwater_skybox-left.jpg"  );
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, baseDir + "underwater_skybox-right.jpg" );
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, baseDir + "underwater_skybox-front.jpg" );
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back , baseDir + "underwater_skybox-back.jpg"  );
            skyBox.Init();
        }

        private void SetClampTextureAddressing()
        {
            D3DDevice.Instance.Device.SamplerState[0].AddressU = TextureAddress.Clamp;
            D3DDevice.Instance.Device.SamplerState[0].AddressV = TextureAddress.Clamp;
            D3DDevice.Instance.Device.SamplerState[0].AddressW = TextureAddress.Clamp;
        }

        private bool HasToChangeInteractionLogic()
        {
            return newUpdateLogic != null;
        }
        private bool HasToChangeStateDependentRenderLogic()
        {
            return newRenderLogic != null;
        }
        private void UpdateLogic()
        {
            if (HasToChangeStateDependentRenderLogic())
            {
                stateDependentRenderLogic = newRenderLogic;
                newRenderLogic = null;
            }
            if (HasToChangeInteractionLogic())
            {
                currentInteractionLogic = newUpdateLogic;
                newUpdateLogic = null;
            }
        }
        public override void Update(float elapsedTime)
        {
            UpdateLogic();

            AquaticPhysics.Instance.DynamicsWorld.StepSimulation(elapsedTime);

            CollisionManager.CheckCollitions(this.World.GetCollisionables());

            this.World.Update(this.Camera.Position);

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

            drawer.BeginDrawSprite();
            drawer.DrawSprite(waterVision);
            drawer.EndDrawSprite();

            stateDependentRenderLogic();

            drawer.BeginDrawSprite();
            drawer.DrawSprite(mask);
            drawer.DrawSprite(cursor);
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
        private void ChangeInteractionLogic(InteractionLogic newLogic)
        {
            newUpdateLogic = newLogic;
        }
        private void ChangeStateDependentRenderLogic(RenderLogic newLogic)
        {
            newRenderLogic = newLogic;
        }

        private float GetPDAInitialPosition() { return -PDA.Bitmap.Width * PDA.Scaling.X; }
        private void WorldInteractionLogic(float elapsedTime)
        {
            if (Input.keyPressed(Key.I))
            {
                ChangeInteractionLogic(TakePDAIn);
                ChangeStateDependentRenderLogic(RenderInventory);
                PDAPositionX = GetPDAInitialPosition();
                ((Camera)Camera).Freeze();
                return;
            }
        }
        private void InventoryInteractionLogic(float elapsedTime)
        {
            if (Input.keyPressed(Key.I))
            {
                ChangeInteractionLogic(TakePDAOut);
                return;
            }
        }
        private int CalculateTransparency(int limit)
        {
            return FastMath.Max(
                FastMath.Min((int)
                ((
                    1 - (
                            (finalPDAPositionX - PDAPositionX) / (finalPDAPositionX - GetPDAInitialPosition())
                        )
                ) * limit), 255), 0);
        }
        private int CalculatePDATransparency()
        {
            return CalculateTransparency(140);
        }
        private int CalculaterBlacknessTransparency()
        {
            return CalculateTransparency(188);
        }
        private void TakePDAIn(float elapsedTime)
        {
            if (Input.keyPressed(Key.I))
            {
                ChangeInteractionLogic(TakePDAOut);
                return;
            }

            PDAPositionX += PDAMoveCoefficient * elapsedTime;
            PDATransparency = CalculatePDATransparency();

            if (PDAPositionX > finalPDAPositionX)
            {
                PDAPositionX = finalPDAPositionX;
                ChangeInteractionLogic(InventoryInteractionLogic);
            }
            PDA.Position = new TGCVector2(PDAPositionX, PDA.Position.Y);
            PDA.Color = Color.FromArgb(PDATransparency, PDA.Color.R, PDA.Color.G, PDA.Color.B);
            darknessCover.Color = Color.FromArgb(CalculaterBlacknessTransparency(), darknessCover.Color.R, darknessCover.Color.G, darknessCover.Color.B);
        }
        private void TakePDAOut(float elapsedTime)
        {
            if (Input.keyPressed(Key.I))
            {
                ChangeInteractionLogic(TakePDAIn);
                return;
            }

            PDAPositionX -= PDAMoveCoefficient * elapsedTime;
            PDATransparency = CalculatePDATransparency();

            if (PDAPositionX + PDA.Bitmap.Width * PDA.Scaling.X < 0)
            {
                PDAPositionX = finalPDAPositionX;
                ChangeInteractionLogic(WorldInteractionLogic);
                stateDependentRenderLogic = () => {};
                ((Camera)Camera).Unfreeze();
            }
            PDA.Position = new TGCVector2(PDAPositionX, PDA.Position.Y);
            PDA.Color = Color.FromArgb(PDATransparency, PDA.Color.R, PDA.Color.G, PDA.Color.B);
            darknessCover.Color = Color.FromArgb(CalculaterBlacknessTransparency(), darknessCover.Color.R, darknessCover.Color.G, darknessCover.Color.B);
        }
        private void RenderInventory()
        {
            drawer.BeginDrawSprite();
            drawer.DrawSprite(darknessCover);
            drawer.DrawSprite(PDA);
            drawer.EndDrawSprite();
        }
    }
}
