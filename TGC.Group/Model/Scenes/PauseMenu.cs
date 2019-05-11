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

        private int xTitle, yTitle, yOffsetFirstOption = 250, ySecondOption = 50;

        TgcText2D textBig = new TgcText2D(), textSmall = new TgcText2D();

        enum Pointer
        {
            UP,
            DOWN
        }
        Pointer pointer = Pointer.UP;
        Color[] colors = { Color.White, Color.DarkGray };

        Drawer2D drawer;
        CustomSprite sprite;
        public PauseMenu(TgcD3dInput input, Drawer2D drawer, CustomSprite sprite) : base(input)
        {
            this.drawer = drawer;
            this.sprite = sprite;
            sprite.Color = Color.FromArgb(188, 0, 50, 200);
            sprite.Scaling = new TGCVector2(.8f, .5f);

            Screen.CenterSprite(sprite);

            xTitle = (int)(sprite.Position.X + 60);
            yTitle = (int)(sprite.Position.Y + 80);

            _uses3DCamera = false;
            textBig.changeFont(new System.Drawing.Font("Arial Black", 40f));
            textSmall.changeFont(new System.Drawing.Font("Arial Black", 20f));

            InitInput();
        }
        private void InitInput()
        {
            pressed[GameInput._Enter] = () => {
                Decide();
                pointer = Pointer.UP;
            };
            pressed[GameInput._Escape] = () => {
                onReturnToGameCallback();
                pointer = Pointer.UP;
            };
            pressed[GameInput._Up] = () => {
                pointer = Pointer.UP;
            };
            pressed[GameInput._Down] = () => {
                pointer = Pointer.DOWN;
            };
        }
        public override void Update(float elapsedTime)
        {
            
        }
        public override void Render()
        {
            preRender();

            drawer.BeginDrawSprite();
            drawer.DrawSprite(sprite);
            drawer.EndDrawSprite();

            textBig.drawText("PAUSE", xTitle + 50, yTitle, Color.DarkGray);
            textSmall.drawText("BACK TO THE GAME", xTitle, yTitle + yOffsetFirstOption, colors[(int)pointer]);
            textSmall.drawText("GO TO START MENU", xTitle, yTitle + yOffsetFirstOption + ySecondOption, colors[(((int)pointer) + 1) % 2]);
            textSmall.drawText("->", xTitle - 45, yTitle + yOffsetFirstOption + (int)pointer * ySecondOption, Color.White);
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
