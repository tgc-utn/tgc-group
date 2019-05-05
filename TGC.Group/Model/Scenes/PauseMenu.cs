using System.Drawing;
using TGC.Core.Input;
using TGC.Core.Text;
using TGC.Core.Mathematica;
using TGC.Group.TGCUtils;
using TGC.Group.Model.Utils;
using TGC.Group.Model.Input;

namespace TGC.Group.Model.Scenes
{
    class PauseMenu : Scene
    {
        public delegate void Callback();
        private Callback preRender = () => {};
        private Callback onReturnToGameCallback = () => {}, onGoToStartMenuCallback = () => {};

        private int x, y;

        TgcText2D textBig = new TgcText2D(), textSmall = new TgcText2D();

        enum Pointer
        {
            UP,
            DOWN
        }
        Pointer pointer = Pointer.UP;

        Drawer2D drawer;
        CustomSprite sprite;
        public PauseMenu(TgcD3dInput input, Drawer2D drawer, CustomSprite sprite) : base(input)
        {
            this.drawer = drawer;
            this.sprite = sprite;
            sprite.Color = Color.FromArgb(188, 0, 50, 200);
            sprite.Scaling = new TGCVector2(.8f, .5f);

            Screen.CenterSprite(sprite);

            x = (int)(sprite.Position.X + 60);
            y = (int)(sprite.Position.Y + 80);

            _uses3DCamera = false;
            textBig.changeFont(new System.Drawing.Font("Arial Black", 40f));
            textSmall.changeFont(new System.Drawing.Font("Arial Black", 20f));
        }
        public override void Update(float elapsedTime)
        {
            if (GameInput.Enter.IsPressed(Input))
            {
                Decide();
                pointer = Pointer.UP;
            }
            if (GameInput.Escape.IsPressed(Input))
            {
                onReturnToGameCallback();
                pointer = Pointer.UP;
            }
            if (GameInput.Up.IsPressed(Input))
            {
                pointer = Pointer.UP;
            }
            if (GameInput.Down.IsPressed(Input))
            {
                pointer = Pointer.DOWN;
            }
        }
        public override void Render()
        {
            preRender();

            drawer.BeginDrawSprite();
            drawer.DrawSprite(sprite);
            drawer.EndDrawSprite();

            textBig.drawText("PAUSE", x + 50, y, Color.DarkGray);
            textSmall.drawText("BACK TO THE GAME", x, y + 250, pointer == Pointer.UP ? Color.White : Color.DarkGray);
            textSmall.drawText("GO TO START MENU", x, y + 300, pointer == Pointer.UP ? Color.DarkGray : Color.White);
            textSmall.drawText("->", x - 45, y + 250 + (int)pointer * 50, Color.White);
        }
        public PauseMenu OnReturnToGame(Callback onEnterCallback)
        {
            this.onReturnToGameCallback = onEnterCallback;
            return this;
        }
        public PauseMenu OnGoToStartMenu(Callback onEscapeCallback)
        {
            this.onGoToStartMenuCallback = onEscapeCallback;
            return this;
        }
        private void Decide()
        {
            (pointer == Pointer.UP ? onReturnToGameCallback : onGoToStartMenuCallback)();
        }
        public PauseMenu WithPreRender(Callback preRender)
        {
            this.preRender = preRender;
            return this;
        }
    }
}
