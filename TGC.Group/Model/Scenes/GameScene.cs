using System.Drawing;
using System.Windows.Forms;
using TGC.Core.Input;
using TGC.Core.Text;
using TGC.Core.Mathematica;
using TGC.Group.Model.Elements;
using TGC.Group.Model.Input;
using TGC.Core.Terrain;
using Microsoft.DirectX;
using TGC.Group.TGCUtils;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.Model.Utils;
using Microsoft.DirectX.Direct3D;
using TGC.Core.SkeletalAnimation;
using TGC.Group.Model.Items;
using TGC.Group.Model.Utils;
using TGC.Group.Model.Items.Equipment;
using TGC.Group.Model.Player;
using TGC.Group.Model.Elements.RigidBodyFactories;
using TGC.Core.Direct3D;
using Microsoft.DirectX.DirectInput;
using Screen = TGC.Group.Model.Utils.Screen;

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
        CustomSprite waterVision, darknessCover, mask, aim, hand, cursor;
        Drawer2D drawer = new Drawer2D();
        CustomSprite PDA;
        float PDAPositionX, finalPDAPositionX, PDAMoveCoefficient;
        int PDATransparency;
        
        private Character character = new Character();

        private float oneSecond = 0; //TODO remove
        private bool gaveOxygenTank = false; //TODO remove

        delegate void InteractionLogic(float elapsedTime);
        InteractionLogic currentInteractionLogic, newUpdateLogic;
        delegate void RenderLogic();
        RenderLogic stateDependentRenderLogic, newRenderLogic;
        public GameScene(TgcD3dInput input, string mediaDir) : base(input)
        {
            backgroundColor = Color.FromArgb(1, 78, 129, 179);

            this.World = new World(new TGCVector3(0, 0, 0));

            SetCamera(input);

            IncrementFarPlane(3f);
            SetClampTextureAddressing();

            InitSkyBox();
            InitWaterVision();
            InitDarknessCover();
            InitMask();
            InitAim();
            InitHand();

            World = new World(new TGCVector3(0, 0, 0));

            PDA = BitmapRepository.CreateSpriteFromPath(BitmapRepository.PDA);
            PDA.Scaling = new TGCVector2(.5f, .35f);
            Screen.CenterSprite(PDA);
            finalPDAPositionX = PDA.Position.X;
            PDAMoveCoefficient = (finalPDAPositionX - GetPDAInitialPosition()) * 4;

            cursor = aim;

            currentInteractionLogic = WorldInteractionLogic;
            stateDependentRenderLogic = () => {};

            // This will be useful for the fog effect
            D3DDevice.Instance.Device.RenderState.FogEnable = true;
            D3DDevice.Instance.Device.RenderState.RangeFogEnable = true;
            D3DDevice.Instance.Device.RenderState.FogColor = Color.FromArgb(255, 10, 70, 164);
            D3DDevice.Instance.Device.RenderState.FogTableMode = FogMode.Exp;
            D3DDevice.Instance.Device.RenderState.FogVertexMode = FogMode.Exp;
            D3DDevice.Instance.Device.RenderState.FogDensity = .66f;
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
        private void InitHand()
        {
            hand = BitmapRepository.CreateSpriteFromPath(BitmapRepository.Hand);
            Screen.CenterSprite(hand);
        }

        private void InitSkyBox()
        {
            skyBox = new TgcSkyBox();
            skyBox.SkyEpsilon = 50;
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
        private void SetCamera(TgcD3dInput input)
        {
            var position = new TGCVector3(30, 30, 200);
            var rigidBody = new CapsuleFactory().Create(position, 100, 60);
            AquaticPhysics.Instance.Add(rigidBody);
            Camera = new Camera(position, input, rigidBody);
        }

        public override void Update(float elapsedTime)
        {
            UpdateLogic();

            this.oneSecond += elapsedTime;
            
            AquaticPhysics.Instance.DynamicsWorld.StepSimulation(elapsedTime);

            CollisionManager.CheckCollitions(this.World.GetCollisionables());

            this.World.Update((Camera)this.Camera);

            var item = manageSelectableElement(this.World.SelectableElement); // Important: get this AFTER updating the world
            
            if(item != null)
                this.character.GiveItem(item);

            //TODO crafter logic, move to crafter when coded
            if (OxygenTank.Recipe.CanCraft(this.character.Inventory.AsIngredients()) && !this.gaveOxygenTank)
            {
                this.character.RemoveIngredients(OxygenTank.Recipe.Ingredients);
                var oxygenTank = new OxygenTank();
                this.character.GiveItem(oxygenTank);
                
                ///////TODO when UI is ready, the selected element will be equipped
                this.character.Equip(oxygenTank);

                this.gaveOxygenTank = true;
            }
            //***********************************************

            skyBox.Center = Camera.Position;
            if (this.oneSecond > 1.0f)
            {
                this.oneSecond = 0;
                this.character.UpdateStats(new Stats(-1,0));
            }
            
            if (GameInput.Statistic.IsPressed(this.Input))
            {
                this.BoundingBox = !this.BoundingBox;
            }
            if (GameInput.Escape.IsPressed(this.Input))
            {
                onPauseCallback();
            }

            currentInteractionLogic(elapsedTime);
        }

        private IItem manageSelectableElement(Element element)
        {
            if (element == null) return null;
            IItem item = null;

            element.Selectable = true;
            
            if (GameInput.Enter.IsPressed(this.Input))
            {
                this.World.Remove(element);
                item = element.item;
            }

            return item;
        }

        public override void Render()
        {
            ClearScreen();

            this.skyBox.Render();
            this.World.Render(this.Camera);


            if (this.BoundingBox)
            {
                this.DrawText.drawText("Oxygen = " + this.character.ActualStats.Oxygen + "/" + this.character.MaxStats.Oxygen, 0, 60, Color.Bisque);
                this.World.RenderBoundingBox(this.Camera);
            }

            drawer.BeginDrawSprite();
            drawer.DrawSprite(waterVision);
            drawer.EndDrawSprite();

            stateDependentRenderLogic();

            drawer.BeginDrawSprite();
            drawer.DrawSprite(mask);
            drawer.DrawSprite(cursor);
            drawer.EndDrawSprite();

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
                cursor = aim;
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
                cursor = hand;
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
