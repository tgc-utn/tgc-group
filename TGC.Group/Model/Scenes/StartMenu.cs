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
using TGC.Core.Terrain;
using TGC.Core.Camara;

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
        CustomSprite spriteSubnautica, spriteBlackRectangle, title;
        private double x;
        private int yBase;
        private Pointer pointer = Pointer.UP;
        private TgcSkyBox skyBox;

        private TGCVector3 viewDirectionStart = new TGCVector3(-1, 0.25f, 0);

        private float rotation = 0;

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
            title = BitmapRepository.CreateSpriteFromPath(BitmapRepository.Title);
            title.Scaling = new TGCVector2(.15f, .25f);
            title.Position = new TGCVector2(200, 250);
            spriteBlackRectangle.Color = Color.FromArgb(188, 0, 0, 0);

            Screen.FitSpriteToScreen(spriteSubnautica);
            spriteBlackRectangle.Scaling = new TGCVector2(1, .1f);
            Screen.CenterSprite(spriteBlackRectangle);
            spriteBlackRectangle.Position = new TGCVector2(
                spriteBlackRectangle.Position.X,
                Screen.Height * (3f / 4)
            );

            x = spriteBlackRectangle.Position.X + 200;
            yBase = (int)(spriteBlackRectangle.Position.Y + 10);

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

        override public void Update()
        {
            if (Input.keyPressed(Key.DownArrow)) pointer = Pointer.DOWN;
            if (Input.keyPressed(Key.UpArrow)) pointer = Pointer.UP;
            if (Input.keyPressed(Key.Return)) fireAction();

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
        public override void Dispose()
        {
            skyBox.Dispose();
        }
    }
}
