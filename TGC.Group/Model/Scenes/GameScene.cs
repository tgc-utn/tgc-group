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
using static TGC.Core.Input.TgcD3dInput;
using System.Collections.Generic;
using Microsoft.DirectX.DirectInput;

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

        Scene subScene;
        InventoryScene inventoryScene;

        TgcSkyBox skyBox;
        CustomSprite waterVision, mask, aim, hand, cursor, dialogBox;
        Drawer2D drawer = new Drawer2D();
        string dialogName, dialogDescription;
        private Character character = new Character();
        internal Character Character { get { return character; } }

        private float oneSecond = 0; //TODO remove
        private bool gaveOxygenTank = false; //TODO remove
        private bool aimFired = false;

        delegate void InteractionLogic(float elapsedTime);

        InteractionLogic currentInteractionLogic, newUpdateLogic;

        delegate void RenderLogic();

        RenderLogic stateDependentRenderLogic, newRenderLogic;

        public GameScene(TgcD3dInput input, string mediaDir) : base(input)
        {
            backgroundColor = Color.FromArgb(1, 78, 129, 179);

            this.World = new World(new TGCVector3(0, 0, 0));

            //DrawText.changeFont(new System.Drawing.Font("Arial Black", 10f));

            SetCamera(input);

            IncrementFarPlane(3f);
            SetClampTextureAddressing();
            InitInventoryScene();
            InitSkyBox();
            InitWaterVision();
            InitMask();
            InitAim();
            InitHand();
            InitDialogBox();

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
                    45,
                    D3DDevice.Instance.AspectRatio,
                    D3DDevice.Instance.ZNearPlaneDistance,
                    D3DDevice.Instance.ZFarPlaneDistance * scale
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
            this.oneSecond += elapsedTime;

            AquaticPhysics.Instance.DynamicsWorld.StepSimulation(elapsedTime);

            CollisionManager.CheckCollitions(this.World.GetCollisionables());

            this.World.Update((Camera) this.Camera);

            var item = manageSelectableElement(this.World
                .SelectableElement); // Important: get this AFTER updating the world

            if (item != null)
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
                this.character.UpdateStats(new Stats(-1, 0));
                this.character.UpdateStats(new Stats(-1, 0));
            }

            subScene.Update(elapsedTime);
            aimFired = false;
        }

        public override void Render()
        {
            ClearScreen();

            this.skyBox.Render();
            this.World.Render(this.Camera);

            if (this.BoundingBox)
            {
                this.World.RenderBoundingBox(this.Camera);
            }

            drawer.BeginDrawSprite();
            drawer.DrawSprite(waterVision);
            drawer.DrawSprite(cursor);
            if(dialogName != "")
            {
                drawer.DrawSprite(dialogBox);
            }
            drawer.EndDrawSprite();

            subScene.Render();

            drawer.BeginDrawSprite();
            drawer.DrawSprite(mask);
            drawer.EndDrawSprite();
            if (dialogName != "")
            {
                DrawText.drawText(dialogName, (int)dialogBox.Position.X, (int)dialogBox.Position.Y, Color.White);
                DrawText.drawText(dialogDescription, (int)dialogBox.Position.X, (int)dialogBox.Position.Y + 15, Color.White);
            }
            this.DrawText.drawText(
                "Oxygen = " + this.character.ActualStats.Oxygen + "/" + this.character.MaxStats.Oxygen, 0, 60,
                Color.Bisque);
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