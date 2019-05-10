using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Camara;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Terrain;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Text;

namespace TGC.Group.Model.Scenes
{
    class ShipScene : Scene
    {
        TgcSkyBox walls;
        float rotation = 0;
        readonly TgcText2D DrawText = new TgcText2D();
        TGCVector3 viewDirectionStart = new TGCVector3(-1, 0.25f, 0);
        public delegate void Callback();
        private Callback onGoToWaterCallback = () => {}, onPauseCallback = () => {};

        public ShipScene(TgcD3dInput input) : base(input)
        {
            this.backgroundColor = Color.DarkOrange;

            walls = new TgcSkyBox();
            walls.SkyEpsilon = 0;
            walls.Center = new TGCVector3(0, 500, 0);
            walls.Size = new TGCVector3(500, 500, 1000);

            string baseDir = "../../../res/";
            walls.setFaceTexture(TgcSkyBox.SkyFaces.Back, baseDir +  "wall-1.jpg");
            walls.setFaceTexture(TgcSkyBox.SkyFaces.Down, baseDir +  "wall-1.jpg");
            walls.setFaceTexture(TgcSkyBox.SkyFaces.Front, baseDir + "wall-1.jpg");
            walls.setFaceTexture(TgcSkyBox.SkyFaces.Left, baseDir +  "wall-1.jpg");
            walls.setFaceTexture(TgcSkyBox.SkyFaces.Right, baseDir + "wall-1.jpg");
            walls.setFaceTexture(TgcSkyBox.SkyFaces.Up, baseDir +    "ceiling.jpg");

            walls.Init();
            Camera = new CameraFPSGravity(walls.Center + new TGCVector3(0, 400, 0), Input);
        }

        public override void Render()
        {
            ClearScreen();

            walls.Render();
        }

        public override void Update(float elapsedTime)
        {
            if(Input.keyPressed(Key.Return))
            {
                onGoToWaterCallback();
            }
            if (Input.keyPressed(Key.Escape))
            {
                onPauseCallback();
            }
        }

        public ShipScene OnGoToWater(Callback onGoToWaterCallback)
        {
            this.onGoToWaterCallback = onGoToWaterCallback;
            return this;
        }
        public ShipScene OnPause(Callback onPauseCallback)
        {
            this.onPauseCallback = onPauseCallback;
            return this;
        }
    }
}
