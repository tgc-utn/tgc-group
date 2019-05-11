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
    partial class InventoryScene : Scene
    {
        private GameScene gameScene;
        private int count;

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
            if(nextStateID != StateID.NULL)
            {
                SetState(nextStateID);
                nextStateID = StateID.NULL;
            }
            updateLogic(elapsedTime);
        }
        public override void Render()
        {
            drawer.BeginDrawSprite();
            drawer.DrawSprite(darknessCover);
            drawer.DrawSprite(PDA);
            if (stateID == StateID.INVENTORY)
            {
                drawer.DrawSprite(cursor);
            }
            drawer.EndDrawSprite();

            if (stateID == StateID.INVENTORY)
                text.drawText("count: " + count, 500, 300, Color.White);

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
        public void TakePDAIn(float elapsedTime)
        {
            PDAPositionX += PDAMoveCoefficient * elapsedTime;
            PDATransparency = CalculatePDATransparency();

            if (PDAPositionX > finalPDAPositionX)
            {
                PDAPositionX = finalPDAPositionX;
                SetNextState(StateID.INVENTORY);
            }
            PDA.Position = new TGCVector2(PDAPositionX, PDA.Position.Y);
            PDA.Color = Color.FromArgb(PDATransparency, PDA.Color.R, PDA.Color.G, PDA.Color.B);
            darknessCover.Color = Color.FromArgb(CalculaterBlacknessTransparency(), darknessCover.Color.R, darknessCover.Color.G, darknessCover.Color.B);
        }
        public void InventoryInteraction(float elapsedTime)
        {
            count = gameScene.Character.Inventory.Items.Count;
        }
        public void TakePDAOut(float elapsedTime)
        {
            PDAPositionX -= PDAMoveCoefficient * elapsedTime;
            PDATransparency = CalculatePDATransparency();

            if (PDAPositionX + PDA.Bitmap.Width * PDA.Scaling.X < 0)
            {
                PDAPositionX = GetPDAInitialPosition();
                SetNextState(StateID.IN);
                gameScene.CloseInventory();

            }
            PDA.Position = new TGCVector2(PDAPositionX, PDA.Position.Y);
            PDA.Color = Color.FromArgb(PDATransparency, PDA.Color.R, PDA.Color.G, PDA.Color.B);
            darknessCover.Color = Color.FromArgb(CalculaterBlacknessTransparency(), darknessCover.Color.R, darknessCover.Color.G, darknessCover.Color.B);
        }
    }
}
