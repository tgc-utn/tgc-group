using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.TGCUtils;
using TGC.Group.Model.Utils;
using TGC.Group.Model.Resources.Sprites;
using TGC.Core.Terrain;
using TGC.Core.Camara;
using TGC.Group.Model.Input;

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
        CustomSprite spriteBlackRectangle, title;
        private double x;
        private int yStartWord, yOffset = 40;
        private Pointer pointer = Pointer.UP;
        private TgcSkyBox skyBox;
        private Color[] colors = { Color.OrangeRed, Color.AliceBlue };

        private TGCVector3 viewDirectionStart = new TGCVector3(-1, 0.25f, 0);

        private float rotation = 0;

        public StartMenu(TgcD3dInput Input) : base(Input)
        {
            onGameStartCallback = onGameExitCallback = () => {};

            drawer = new Drawer2D();

            InitFonts();
            InitTitle();
            InitBlackRectangle();

            x = spriteBlackRectangle.Position.X + 200;
            yStartWord = (int)(spriteBlackRectangle.Position.Y + 10);

            Screen.CenterSprite(title);
            title.Position = new TGCVector2(
                title.Position.X,
                Screen.Height * (1f / 5)
            );

            skyBox = new TgcSkyBox();
            skyBox.Center = new TGCVector3(0, 500, 0);
            skyBox.Size = new TGCVector3(10000, 10000, 10000);
            string baseDir = "../../../res/";
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up   , baseDir +  "skybox-up.jpg"    );
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down , baseDir +  "skybox-down.jpg"  );
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left , baseDir +  "skybox-left.jpg"  );
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, baseDir +  "skybox-right.jpg" );
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, baseDir +  "skybox-front.jpg" );
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back , baseDir +  "skybox-back.jpg"  );
            skyBox.Init();
            Camera = new TgcCamera();
        }

        private void InitFonts()
        {
            DrawTextBig = new TgcText2D();
            DrawTextSmall = new TgcText2D();
            DrawTextBig.changeFont(new System.Drawing.Font("Arial Black", 40f));
            DrawTextSmall.changeFont(new System.Drawing.Font("Arial Black", 25f));
        }

        private void InitTitle()
        {
            title = BitmapRepository.CreateSpriteFromPath(BitmapRepository.Title);
            title.Scaling = new TGCVector2(.15f, .25f);
            title.Position = new TGCVector2(200, 250);
        }

        private void InitBlackRectangle()
        {
            spriteBlackRectangle = BitmapRepository.CreateSpriteFromPath(BitmapRepository.BlackRectangle);
            spriteBlackRectangle.Color = Color.FromArgb(188, 0, 0, 0);
            spriteBlackRectangle.Scaling = new TGCVector2(1, .1f);
            Screen.CenterSprite(spriteBlackRectangle);

            spriteBlackRectangle.Position = new TGCVector2(
                spriteBlackRectangle.Position.X,
                Screen.Height * (3f / 4)
            );
        }

        override public void Update(float elapsedTime)
        {
            if (GameInput.Down.IsPressed(Input)) pointer = Pointer.DOWN;
            if (GameInput.Up.IsPressed(Input)) pointer = Pointer.UP;
            if (GameInput.Enter.IsPressed(Input)) fireAction();

            TGCVector3 lookAt  = skyBox.Center + TGCVector3.TransformNormal(viewDirectionStart, TGCMatrix.RotationY(rotation));
            rotation += .0001f;
            Camera.SetCamera(skyBox.Center, lookAt);
        }
        override public void Render()
        {
            ClearScreen();

            skyBox.Render();

            drawer.BeginDrawSprite();
            //drawer.DrawSprite(spriteSubnautica);
            drawer.DrawSprite(spriteBlackRectangle);
            drawer.DrawSprite(title);
            drawer.EndDrawSprite();

            DrawTextSmall.drawText("Start", (int)x, yStartWord, colors[(int)pointer]);
            DrawTextSmall.drawText("Exit", (int)x, yStartWord + yOffset, colors[(((int)pointer) + 1) % 2]);
            DrawTextSmall.drawText("->", (int)x - 40, yStartWord + (int)pointer * yOffset, Color.OrangeRed);
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
        public override void Dispose()
        {
            skyBox.Dispose();
        }
    }
}
