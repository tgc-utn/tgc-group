using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.Model.Utils;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Scenes
{
    class InventoryScene : Scene
    {
        enum State
        {
            IN,
            INVENTORY,
            OUT
        }

        CommandList.Command[] stateSetters = new CommandList.Command[3];

        private GameScene gameScene;

        public delegate void UpdateLogic(float elapsedTime);
        public delegate void RenderLogic();
        public UpdateLogic updateLogic = time => {};
        public RenderLogic renderLogic = () => {};

        private TgcText2D text = new TgcText2D();
        private Drawer2D drawer = new Drawer2D();
        private CustomSprite PDA, darknessCover, cursor;
        float PDAPositionX, finalPDAPositionX, PDAMoveCoefficient;
        int PDATransparency;

        public InventoryScene(TgcD3dInput input, GameScene gameScene) : base(input)
        {
            this.gameScene = gameScene;

            InitPDA();
            InitDarknessCover();
            InitCursor();

            BindStateWithStateSetter(State.IN, TakePDAIn, State.OUT);
            BindStateWithStateSetter(State.INVENTORY, InventoryInteraction, State.OUT);
            BindStateWithStateSetter(State.OUT, TakePDAOut, State.IN);

            SetState(State.IN);
        }
        private void SetState(State newState)
        {
            stateSetters[(int)newState]();
        }
        private void BindStateWithStateSetter(State state, UpdateLogic newUpdateLogic, State nextState)
        {
            stateSetters[(int)state] = () => {
                this.updateLogic = newUpdateLogic;
                pressed[Key.I] = () => SetState(nextState);
            };
        }
        private void InitPDA()
        {
            PDA = BitmapRepository.CreateSpriteFromPath(BitmapRepository.PDA);
            PDA.Scaling = new TGCVector2(.5f, .35f);
            Screen.CenterSprite(PDA);
            finalPDAPositionX = PDA.Position.X;
            PDAPositionX = GetPDAInitialPosition();
            PDAMoveCoefficient = (finalPDAPositionX - GetPDAInitialPosition()) * 4;
        }
        private void InitDarknessCover()
        {
            darknessCover = BitmapRepository.CreateSpriteFromPath(BitmapRepository.BlackRectangle);
            Screen.FitSpriteToScreen(darknessCover);
            darknessCover.Color = Color.FromArgb(0, 0, 0, 0);
        }
        private void InitCursor()
        {
            cursor = BitmapRepository.CreateSpriteFromPath(BitmapRepository.Hand);
            Screen.CenterSprite(cursor);
        }
        public override void Update(float elapsedTime)
        {
            updateLogic(elapsedTime);
        }
        public override void Render()
        {
            drawer.BeginDrawSprite();
            drawer.DrawSprite(darknessCover);
            drawer.DrawSprite(PDA);
            if(updateLogic == InventoryInteraction) drawer.DrawSprite(cursor);
            drawer.EndDrawSprite();
        }
        private float GetPDAInitialPosition() { return -PDA.Bitmap.Width * PDA.Scaling.X; }
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
        public void ChangeUpdateLogic(UpdateLogic updateLogic)
        {
            this.updateLogic = updateLogic;
        }
        public void TakePDAIn(float elapsedTime)
        {
            PDAPositionX += PDAMoveCoefficient * elapsedTime;
            PDATransparency = CalculatePDATransparency();

            if (PDAPositionX > finalPDAPositionX)
            {
                PDAPositionX = finalPDAPositionX;
                SetState(State.INVENTORY);
            }
            PDA.Position = new TGCVector2(PDAPositionX, PDA.Position.Y);
            PDA.Color = Color.FromArgb(PDATransparency, PDA.Color.R, PDA.Color.G, PDA.Color.B);
            darknessCover.Color = Color.FromArgb(CalculaterBlacknessTransparency(), darknessCover.Color.R, darknessCover.Color.G, darknessCover.Color.B);
        }
        public void InventoryInteraction(float elapsedTime)
        {

        }
        public void TakePDAOut(float elapsedTime)
        {
            PDAPositionX -= PDAMoveCoefficient * elapsedTime;
            PDATransparency = CalculatePDATransparency();

            if (PDAPositionX + PDA.Bitmap.Width * PDA.Scaling.X < 0)
            {
                PDAPositionX = GetPDAInitialPosition();
                SetState(State.IN);
                gameScene.CloseInventory();

            }
            PDA.Position = new TGCVector2(PDAPositionX, PDA.Position.Y);
            PDA.Color = Color.FromArgb(PDATransparency, PDA.Color.R, PDA.Color.G, PDA.Color.B);
            darknessCover.Color = Color.FromArgb(CalculaterBlacknessTransparency(), darknessCover.Color.R, darknessCover.Color.G, darknessCover.Color.B);
        }
    }
}
