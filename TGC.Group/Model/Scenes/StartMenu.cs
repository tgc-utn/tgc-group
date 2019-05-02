using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.Model.Input;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Scenes
{
    enum Pointer
    {
        UP,
        DOWN
    }

    class StartMenu : Scene
    {
        public delegate void Callback();
        private Callback onGameStartCallback, onGameExitCallback;
        TgcText2D DrawTextBig, DrawTextSmall;
        Drawer2D drawer;
        CustomSprite sprite;
        private double x = 600;
        private int yBase = 600;
        private Pointer pointer = Pointer.UP;
        public StartMenu(TgcD3dInput Input) : base(Input)
        {
            onGameStartCallback = onGameExitCallback = () => {};
            DrawTextBig = new TgcText2D();
            DrawTextSmall = new TgcText2D();

            DrawTextBig.changeFont(new System.Drawing.Font("Arial Black", 40f));
            DrawTextSmall.changeFont(new System.Drawing.Font("Arial Black", 25f));

            drawer = new Drawer2D();
            sprite = new CustomSprite();

            sprite.Bitmap = new CustomBitmap("../../../res/subnautica-portada.png", D3DDevice.Instance.Device);

            float size = .3335f;
            sprite.Scaling = new TGCVector2(size, size);
        }

        override public void Update(float elapsedTime)
        {
            if (GameInput.Down.IsPressed(Input)) pointer = Pointer.DOWN;
            if (GameInput.Up.IsPressed(Input)) pointer = Pointer.UP;
            if (GameInput.Enter.IsPressed(Input)) fireAction();
        }
        override public void Render()
        {
            ClearScreen();

            drawer.BeginDrawSprite();
            drawer.DrawSprite(sprite);
            drawer.EndDrawSprite();

            DrawTextSmall.drawText("Start", (int)x, yBase, pointer == Pointer.DOWN ? Color.AliceBlue : Color.OrangeRed);
            DrawTextSmall.drawText("Exit", (int)x, yBase + 30, pointer == Pointer.DOWN ? Color.OrangeRed : Color.AliceBlue);
            DrawTextSmall.drawText("->", (int)x - 40, yBase + (int)pointer * 30, Color.OrangeRed);
        }
        private void fireAction()
        {
            if (pointer == Pointer.UP) onGameStartCallback();
            if (pointer == Pointer.DOWN) onGameExitCallback();
        }

        public StartMenu onGameStart(Callback onGameStartCallback)
        {
            this.onGameStartCallback = onGameStartCallback;
            return this;
        }
        public StartMenu onGameExit(Callback onGameExitCallback)
        {
            this.onGameExitCallback = onGameExitCallback;
            return this;
        }
    }
}
