using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.Utils;
using TGC.Core.Text;
using System.Collections.Generic;
using TGC.Core.Terrain;
using TGC.Core.UserControls;
using TGC.Core.UserControls.Modifier;
using TGC.Core.Sound;
using TGC.Core.Shaders;
using Microsoft.DirectX.Direct3D;

namespace TGC.Group.Model
{
    public class GameModel : TgcExample
    {
        public TgcScene currentScene;
        private TgcSceneLoader loader;
        private bool ShowBoundingBox { get; set; }

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;

            loader = new TgcSceneLoader();
        }

        public override void Init()
        {
            LoadScene();
            InitCamera();
        }

        public override void Update()
        {
            PreUpdate();
            Camara.UpdateCamera(ElapsedTime);
        }

        public override void Render()
        {
            PreRender();
            currentScene.renderAll(ShowBoundingBox);
            RenderHelpText();
            PostRender();
        }

        public override void Dispose()
        {
            currentScene.disposeAll();
        }

        private void RenderHelpText()
        {
            DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.OrangeRed);
            DrawText.drawText("Camera position: \n" + Camara.Position, 0, 40, Color.OrangeRed);
            DrawText.drawText("Camera LookAt: \n" + Camara.LookAt, 0, 120, Color.OrangeRed);
        }

        private void LoadScene()
        {
            currentScene = loader.loadSceneFromFile(MediaDir + "Isla\\isla-TgcScene.xml");
        }

        private void InitCamera()
        {
            var cameraPosition = new Vector3(-60, 70, 0);
            Camara = new TgcFpsCamera(cameraPosition, Input);
        }
    }
}