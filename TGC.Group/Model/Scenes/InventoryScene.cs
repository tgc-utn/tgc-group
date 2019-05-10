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
        enum StateID
        {
            IN,
            INVENTORY,
            OUT
        }
        struct State
        {
            public StateID id, nextStateID;
            public UpdateLogic updateLogic;
            public State(StateID id, UpdateLogic updateLogic, StateID nextStateID)
            {
                this.id = id;
                this.updateLogic = updateLogic;
                this.nextStateID = nextStateID;
            }
        }
        private State[] states = new State[3];
        private StateID stateID;

        private GameScene gameScene;

        public delegate void UpdateLogic(float elapsedTime);
        public UpdateLogic updateLogic = time => {};

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

            BindState(StateID.IN, TakePDAIn, StateID.OUT);
            BindState(StateID.INVENTORY, InventoryInteraction, StateID.OUT);
            BindState(StateID.OUT, TakePDAOut, StateID.IN);

            SetState(StateID.IN);
        }
        private void SetState(StateID newStateID)
        {
            State newState = states[(int)newStateID];

            this.stateID = newState.id;
            this.updateLogic = newState.updateLogic;
            pressed[Key.I] = () => SetState(newState.nextStateID);
        }
        private void BindState(StateID stateID, UpdateLogic stateUpdateLogic, StateID nextStateID)
        {
            states[(int)stateID] = new State(stateID, stateUpdateLogic, nextStateID);
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
            if(stateID == StateID.INVENTORY) drawer.DrawSprite(cursor);
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
                SetState(StateID.INVENTORY);
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
                SetState(StateID.IN);
                gameScene.CloseInventory();

            }
            PDA.Position = new TGCVector2(PDAPositionX, PDA.Position.Y);
            PDA.Color = Color.FromArgb(PDATransparency, PDA.Color.R, PDA.Color.G, PDA.Color.B);
            darknessCover.Color = Color.FromArgb(CalculaterBlacknessTransparency(), darknessCover.Color.R, darknessCover.Color.G, darknessCover.Color.B);
        }
    }
}
