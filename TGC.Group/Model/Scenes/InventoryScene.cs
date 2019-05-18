using System.Drawing;
using System.Windows.Forms;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Scenes
{
    partial class InventoryScene : Scene
    {
        private GameScene gameScene;
        private int count;

        private TgcText2D text = new TgcText2D();
        private Drawer2D drawer = new Drawer2D();
        private CustomSprite PDA, darknessCover, cursor, bubble, fish, plant;
        private Color cursorDefaultColor;
        float PDAPositionX, finalPDAPositionX, PDAMoveCoefficient;
        int PDATransparency;

        private TGCVector2 GetScaleForSpriteByPixels(CustomSprite sprite, int xPixels, int yPixels)
        {
            float pixelWidth = sprite.Bitmap.Width;
            float pixelHeight = sprite.Bitmap.Height;
            float xScale = xPixels / pixelWidth;
            float yScale = yPixels / pixelHeight;
            return new TGCVector2(xScale, yScale);
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
            drawer.EndDrawSprite();
            if (stateID == StateID.INVENTORY)
            {
                bool hovering = false;
                TGCVector2 baseVector = PDA.Position + new TGCVector2(375, 175);
                drawer.BeginDrawSprite();
                byte xOffset = 110;
                byte yOffset = 110;
                byte maxItemsPerLine = 5;
                byte i = 0;
                foreach (var item in gameScene.Character.Inventory.Items)
                {
                    int x = i % maxItemsPerLine;
                    int y = i / maxItemsPerLine;
                    //text.drawText("-" + i++ + ": " + item.Name + " | " + item.Description + " | " + item.type.ToString(), 500, 300 + 30 * i, Color.White);
                    bubble.Position = baseVector + new TGCVector2(xOffset * x, yOffset * y);
                    if(
                        Cursor.Position.X >= bubble.Position.X &&
                        Cursor.Position.X <= bubble.Position.X + bubble.Bitmap.Width * bubble.Scaling.X &&
                        Cursor.Position.Y >= bubble.Position.Y &&
                        Cursor.Position.Y <= bubble.Position.Y + bubble.Bitmap.Height * bubble.Scaling.Y
                       )
                    {
                        bubble.Scaling = bubbleDefaultScale + GetScaleForSpriteByPixels(bubble, 10, 10);
                        item.Icon.Scaling = item.DefaultScale + GetScaleForSpriteByPixels(item.Icon, 10, 10);
                        hovering = true;
                    }
                    else
                    {
                        bubble.Scaling = bubbleDefaultScale;
                        item.Icon.Scaling = item.DefaultScale;
                    }
                    item.Icon.Position = bubble.Position + new TGCVector2(7, 19);
                    drawer.DrawSprite(bubble);
                    drawer.DrawSprite(item.Icon);
                    ++i;
                }

                cursor.Color = hovering ? Color.Yellow : cursorDefaultColor;
                drawer.DrawSprite(cursor);
                drawer.EndDrawSprite();
            }

            //if (stateID == StateID.INVENTORY)
            //{
            //    //text.drawText("count: " + count, 500, 270, Color.White);
            //    drawer.BeginDrawSprite();
            //    //int i = 1;
            //    foreach (var item in gameScene.Character.Inventory.Items)
            //    {
            //        //text.drawText("-" + i++ + ": " + item.Name + " | " + item.Description + " | " + item.type.ToString(), 500, 300 + 30 * i, Color.White);
            //        drawer.DrawSprite(bubble);
            //    }
            //    drawer.DrawSprite(PDA);
            //}

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
            cursor.Position = new TGCVector2(Cursor.Position.X, Cursor.Position.Y);
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
