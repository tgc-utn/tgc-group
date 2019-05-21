using System.Drawing;
using TGC.Core.Input;
using TGC.Core.Text;
using TGC.Core.Mathematica;
using TGC.Group.Model.Elements;
using TGC.Group.Model.Input;
using TGC.Core.Terrain;
using Microsoft.DirectX;
using TGC.Group.TGCUtils;
using TGC.Group.Model.Resources.Sprites;
using Microsoft.DirectX.Direct3D;
using TGC.Group.Model.Items;
using TGC.Group.Model.Items.Equipment;
using TGC.Group.Model.Player;
using TGC.Group.Model.Elements.RigidBodyFactories;
using TGC.Core.Direct3D;
using Key = Microsoft.DirectX.DirectInput.Key;
using Screen = TGC.Group.Model.Utils.Screen;
using System;
using TGC.Group.Model.UI;

namespace TGC.Group.Model.Scenes
{
    class GameScene : Scene
    {
        private readonly TgcText2D DrawText = new TgcText2D();
        private World World { get; }
        private bool BoundingBox { get; set; }

        string baseDir = "../../../res/";

        public delegate void Callback();
        Callback onPauseCallback = () => {};

        Scene subScene;
        InventoryScene inventoryScene;

        TgcSkyBox skyBoxUnderwater, skyBoxOutside;
        CustomSprite waterVision, mask, aim, hand, cursor, dialogBox;
        Drawer2D drawer = new Drawer2D();
        string dialogName, dialogDescription;
        private Character character = new Character();
        internal Character Character { get { return character; } }

        private bool gaveOxygenTank = false; //TODO remove
        private bool aimFired = false;

        delegate void InteractionLogic(float elapsedTime);

        InteractionLogic currentInteractionLogic, newUpdateLogic;

        delegate void RenderLogic();

        RenderLogic stateDependentRenderLogic, newRenderLogic;

        private StatsIndicators statsIndicators = new StatsIndicators(115, 550);
        
        public GameScene(TgcD3dInput input, string mediaDir) : base(input)
        {
            backgroundColor = Color.FromArgb(255, 78, 129, 179);

            this.World = new World(new TGCVector3(0, 0, 0));

            SetCamera(input);

            IncrementFarPlane(3f);
            SetClampTextureAddressing();
            InitInventoryScene();
            InitSkyBoxes();
            InitWaterVision();
            InitMask();
            InitAim();
            InitHand();
            InitDialogBox();

            this.statsIndicators.init();
            
            World = new World(new TGCVector3(0, 0, 0));

            cursor = aim;

            subScene = Scene.Empty;
            
            pressed[Key.Escape] = () => {
                onPauseCallback();
            };

            pressed[Key.F] = () => {
                this.BoundingBox = !this.BoundingBox;
            };

            TurnExploreCommandsOn();
        }

        private void TurnExploreCommandsOn()
        {
            pressed[Key.I] = OpenInventory;
            pressed[GameInput._Enter] = () => aimFired = true;
        }
        private void TurnExploreCommandsOff()
        {
            pressed[Key.I] = pressed[GameInput._Enter] = () => { };
        }
        private void OpenInventory()
        {
            ((Camera)Camera).Freeze();
            subScene = inventoryScene;
            Input.update();
            TurnExploreCommandsOff();
        }

        private void IncrementFarPlane(float scale)
        {
            D3DDevice.Instance.Device.Transform.Projection =
                Matrix.PerspectiveFovLH(
                    D3DDevice.Instance.FieldOfView,
                    D3DDevice.Instance.AspectRatio,
                    D3DDevice.Instance.ZNearPlaneDistance,
                    D3DDevice.Instance.ZFarPlaneDistance * scale
                );
        }
        private void SetFOV(int fov)
        {
            D3DDevice.Instance.Device.Transform.Projection =
                Matrix.PerspectiveFovLH(
                    fov,
                    D3DDevice.Instance.AspectRatio,
                    D3DDevice.Instance.ZNearPlaneDistance,
                    D3DDevice.Instance.ZFarPlaneDistance
                );
        }
        private void InitInventoryScene()
        {
            inventoryScene = new InventoryScene(Input, this);
        }
        private void InitWaterVision()
        {
            waterVision = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.WaterRectangle);
            Screen.FitSpriteToScreen(waterVision);
            waterVision.Color = Color.FromArgb(120, 10, 70, 164);
        }
        private void InitMask()
        {
            mask = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.Mask);
            Screen.FitSpriteToScreen(mask);
        }
        private void InitAim()
        {
            aim = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.Aim);
            Screen.CenterSprite(aim);
        }
        private void InitHand()
        {
            hand = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.Hand);
            hand.Scaling = new TGCVector2(.75f, .75f);
            Screen.CenterSprite(hand);
        }
        private void InitDialogBox()
        {
            dialogBox = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.BlackRectangle);
            dialogBox.Scaling = new TGCVector2(.35f, .05f);
            dialogBox.Color = Color.FromArgb(188, dialogBox.Color.R, dialogBox.Color.G, dialogBox.Color.B);
            Screen.CenterSprite(dialogBox);
            dialogBox.Position = new TGCVector2(dialogBox.Position.X + 120, dialogBox.Position.Y + 80);
        }

        private void InitSkyBoxes()
        {
            skyBoxUnderwater = new TgcSkyBox();
            skyBoxUnderwater.Color = backgroundColor;
            skyBoxUnderwater.SkyEpsilon = 30;
            skyBoxUnderwater.Center = Camera.Position;
            skyBoxUnderwater.Size = new TGCVector3(30000, 8000, 30000);
            skyBoxUnderwater.setFaceTexture(TgcSkyBox.SkyFaces.Up   , baseDir + "underwater_skybox-up.jpg"    );
            skyBoxUnderwater.setFaceTexture(TgcSkyBox.SkyFaces.Down , baseDir + "underwater_skybox-down.jpg"  );
            skyBoxUnderwater.setFaceTexture(TgcSkyBox.SkyFaces.Left , baseDir + "underwater_skybox-left.jpg"  );
            skyBoxUnderwater.setFaceTexture(TgcSkyBox.SkyFaces.Right, baseDir + "underwater_skybox-right.jpg" );
            skyBoxUnderwater.setFaceTexture(TgcSkyBox.SkyFaces.Front, baseDir + "underwater_skybox-front.jpg" );
            skyBoxUnderwater.setFaceTexture(TgcSkyBox.SkyFaces.Back , baseDir + "underwater_skybox-back.jpg"  );
            skyBoxUnderwater.Init();

            skyBoxOutside = new TgcSkyBox();
            skyBoxOutside.Color = Color.FromArgb(255, 71, 96, 164);
            skyBoxOutside.SkyEpsilon = 50;
            skyBoxOutside.Size = new TGCVector3(30000, 8000, 30000);
            skyBoxOutside.Center = new TGCVector3(
                skyBoxUnderwater.Center.X,
                skyBoxUnderwater.Center.Y + skyBoxUnderwater.Size.Y / 2 + 30,
                skyBoxUnderwater.Center.Z
                );
            skyBoxOutside.setFaceTexture(TgcSkyBox.SkyFaces.Up, baseDir +    "skybox-up.jpg");
            skyBoxOutside.setFaceTexture(TgcSkyBox.SkyFaces.Down, baseDir +  "skybox-down.jpg");
            skyBoxOutside.setFaceTexture(TgcSkyBox.SkyFaces.Left, baseDir +  "skybox-left-middle.jpg");
            skyBoxOutside.setFaceTexture(TgcSkyBox.SkyFaces.Right, baseDir + "skybox-right-middle.jpg");
            skyBoxOutside.setFaceTexture(TgcSkyBox.SkyFaces.Front, baseDir + "skybox-front-middle.jpg");
            skyBoxOutside.setFaceTexture(TgcSkyBox.SkyFaces.Back, baseDir + "skybox-back-middle.jpg");
            skyBoxOutside.Init();
        }

        private void SetClampTextureAddressing()
        {
            D3DDevice.Instance.Device.SamplerState[0].AddressU = TextureAddress.Clamp;
            D3DDevice.Instance.Device.SamplerState[0].AddressV = TextureAddress.Clamp;
            D3DDevice.Instance.Device.SamplerState[0].AddressW = TextureAddress.Clamp;
        }
        private void SetCamera(TgcD3dInput input)
        {
            var position = new TGCVector3(30, 30, 200);
            var rigidBody = new CapsuleFactory().Create(position, 100, 60);
            AquaticPhysics.Instance.Add(rigidBody);
            Camera = new Camera(position, input, rigidBody);
        }
        private IItem manageSelectableElement(Element element)
        {
            if (element == null)
            {
                cursor = aim;
                dialogName = dialogDescription = "";
                return null;
            }
            cursor = hand;
            IItem item = null;

            element.Selectable = true;

            dialogName = element.item.Name;
            dialogDescription = element.item.Description;

            if (aimFired)
            {
                this.World.Remove(element);
                item = element.item;
                aimFired = false;
            }

            return item;
        }
        public override void Update(float elapsedTime)
        {
            AquaticPhysics.Instance.DynamicsWorld.StepSimulation(elapsedTime);

            CollisionManager.CheckCollitions(this.World.GetCollisionables());

            this.World.Update((Camera) this.Camera);

            var item = this.manageSelectableElement(this.World.SelectableElement); // Important: get this AFTER updating the world
            
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

            skyBoxUnderwater.Center = new TGCVector3(Camera.Position.X, skyBoxUnderwater.Center.Y, Camera.Position.Z);
            skyBoxOutside.Center = new TGCVector3(Camera.Position.X, skyBoxOutside.Center.Y, Camera.Position.Z);

            this.character.UpdateStats(new Stats(-elapsedTime, 0));

            if(Camera.Position.Y > skyBoxUnderwater.Center.Y + skyBoxUnderwater.Size.Y / 2)
            {
                character.UpdateStats(new Stats(character.MaxStats.Oxygen, 0));
            }

            subScene.Update(elapsedTime);
            aimFired = false;
        }

        public override void Render()
        {
            ClearScreen();

            this.skyBoxUnderwater.Render();
            this.skyBoxOutside.Render();
            this.World.Render(this.Camera);

            if (this.BoundingBox)
            {
                this.World.RenderBoundingBox(this.Camera);
            }

            drawer.BeginDrawSprite();
            //drawer.DrawSprite(waterVision);
            drawer.DrawSprite(cursor);
            if (dialogName != "")
            {
                drawer.DrawSprite(dialogBox);
            }
            drawer.EndDrawSprite();

            subScene.Render();


            if (dialogName != "")
            {
                DrawText.drawText(dialogName, (int)dialogBox.Position.X, (int)dialogBox.Position.Y, Color.White);
                DrawText.drawText(dialogDescription, (int)dialogBox.Position.X, (int)dialogBox.Position.Y + 15, Color.White);
            }

            drawer.BeginDrawSprite();
            drawer.DrawSprite(mask);
            drawer.EndDrawSprite();
            
            this.statsIndicators.render(this.character);
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
        public void CloseInventory()
        {
            subScene = Scene.Empty;
            TurnExploreCommandsOn();
            ((Camera)Camera).Unfreeze();
        }
        public override void ReactToInput()
        {
            base.ReactToInput();
            subScene.ReactToInput();
        }
    }
}