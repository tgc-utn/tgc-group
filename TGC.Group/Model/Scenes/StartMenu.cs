using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using System;
using TGC.Group.TGCUtils;
using Microsoft.DirectX.Direct3D;
using TGC.Group.Model.Utils;
using TGC.Group.Model.Resources.Sprites;

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
        CustomSprite spriteSubnautica, spriteBlackRectangle;
        private double x;
        private int yBase;
        private Pointer pointer = Pointer.UP;
        public StartMenu(TgcD3dInput Input) : base(Input)
        {
            onGameStartCallback = onGameExitCallback = () => {};
            DrawTextBig = new TgcText2D();
            DrawTextSmall = new TgcText2D();

            DrawTextBig.changeFont(new System.Drawing.Font("Arial Black", 40f));
            DrawTextSmall.changeFont(new System.Drawing.Font("Arial Black", 25f));

            drawer = new Drawer2D();

            spriteSubnautica = BitmapRepository.CreateSpriteFromPath(BitmapRepository.SubnauticaPortrait);
            spriteBlackRectangle = BitmapRepository.CreateSpriteFromPath(BitmapRepository.BlackRectangle);
            spriteBlackRectangle.Color = Color.FromArgb(188, 0, 0, 0);

            Screen.FitSpriteToScreen(spriteSubnautica);
            spriteBlackRectangle.Scaling = new TGCVector2(1, .1f);
            Screen.CenterSprite(spriteBlackRectangle);
            spriteBlackRectangle.Position = new TGCVector2(
                spriteBlackRectangle.Position.X,
                Screen.Height * (3f / 5)
            );

            x = spriteBlackRectangle.Position.X + 200;
            yBase = (int)(spriteBlackRectangle.Position.Y + 10);
        }

        override public void Update()
        {
            if (Input.keyPressed(Key.DownArrow)) pointer = Pointer.DOWN;
            if (Input.keyPressed(Key.UpArrow)) pointer = Pointer.UP;
            if (Input.keyPressed(Key.Return)) fireAction();
        }
        override public void Render()
        {
            ClearScreen();

            drawer.BeginDrawSprite();
            drawer.DrawSprite(spriteSubnautica);
            drawer.DrawSprite(spriteBlackRectangle);
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
