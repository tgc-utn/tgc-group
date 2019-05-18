using Microsoft.DirectX.DirectInput;
using TGC.Core.Input;
using System.Drawing;
using TGC.Core.Mathematica;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model.Scenes
{
    partial class InventoryScene
    {
        public delegate void UpdateLogic(float elapsedTime);
        public UpdateLogic updateLogic = time => { };
        enum StateID
        {

            IN,
            INVENTORY,
            OUT,
            NULL
        }
        struct State
        {
            public StateID nextStateID;
            public UpdateLogic updateLogic;
            public State(UpdateLogic updateLogic, StateID nextStateID)
            {
                this.updateLogic = updateLogic;
                this.nextStateID = nextStateID;
            }
        }
        private State[] states = new State[3];
        private StateID stateID, nextStateID;

        private TGCVector2 bubbleDefaultScale = new TGCVector2(.5f, .5f);
        public InventoryScene(TgcD3dInput input, GameScene gameScene) : base(input)
        {
            this.gameScene = gameScene;

            InitPDA();
            InitDarknessCover();
            InitCursor();
            InitBubble();
            InitFish();
            InitPlant();

            BindState(StateID.IN, TakePDAIn, StateID.OUT);
            BindState(StateID.INVENTORY, InventoryInteraction, StateID.OUT);
            BindState(StateID.OUT, TakePDAOut, StateID.IN);

            SetState(StateID.IN);
        }
        private void SetState(StateID newStateID)
        {
            State newState = states[(int)newStateID];

            this.stateID = newStateID;
            this.updateLogic = newState.updateLogic;
            pressed[Key.I] = () => SetNextState(newState.nextStateID);
        }
        private void SetNextState(StateID newStateID)
        {
            nextStateID = newStateID;
        }
        private void BindState(StateID stateID, UpdateLogic stateUpdateLogic, StateID nextStateID)
        {
            states[(int)stateID] = new State(stateUpdateLogic, nextStateID);
        }
        private void InitPDA()
        {
            PDA = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.PDA);
            PDA.Scaling = new TGCVector2(.5f, .35f);
            Screen.CenterSprite(PDA);
            finalPDAPositionX = PDA.Position.X;
            PDAPositionX = GetPDAInitialPosition();
            PDAMoveCoefficient = (finalPDAPositionX - GetPDAInitialPosition()) * 4;
        }
        private void InitDarknessCover()
        {
            darknessCover = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.BlackRectangle);
            Screen.FitSpriteToScreen(darknessCover);
            darknessCover.Color = Color.FromArgb(0, 0, 0, 0);
        }
        private void InitCursor()
        {
            cursor = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.ArrowPointer);
            cursorDefaultColor = cursor.Color;
            Screen.CenterSprite(cursor);
        }
        private void InitBubble()
        {
            bubble = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.Bubble);
            bubble.Scaling = bubbleDefaultScale;
        }
        private void InitFish()
        {
            fish = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.Fish);
            fish.Scaling = new TGCVector2(.1f, .05f);
        }
        private void InitPlant()
        {
            plant = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.Plant);
            plant.Scaling = new TGCVector2(.1f, .05f);
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
    }
}
