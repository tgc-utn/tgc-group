using Microsoft.DirectX.DirectInput;
using System.Drawing;
using System.Collections;
using System.Linq;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using System.Collections.Generic;
using System;
using TGC.Group.Model.Scenes;
using TGC.Core.Text;
using TGC.Group.Form;
using System.Windows.Forms;

namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo para implementar el TP.
    ///     Inicialmente puede ser renombrado o copiado para hacer más ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar el modelo que instancia GameForm <see cref="Form.GameForm.InitGraphics()" />
    ///     line 97.
    /// </summary>
    public class GameModel : TgcExample
    {
        private GameScene gameScene;
        private StartMenu startMenu;
        private PauseMenu pauseMenu;

        private Scene _curentScene = null;
        private Scene CurrentScene
        {
            set
            {
                _curentScene = value;
                Camara = _curentScene.Camera;
                nextScene = null;
            }
            get { return _curentScene; }
        }

        private Scene nextScene;

        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        /// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
        /// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Console.WriteLine("Mediadir: " + mediaDir);
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public override void Init()
        {
            //note(fede): Only at this point the Input field has been initialized by the form

            startMenu = new StartMenu(Input)
                    .onGameStart(() => SetNextScene(gameScene))
                    .onGameExit(StopGame);

            pauseMenu = new PauseMenu(Input)
                .OnGoToStartMenu(() => {
                    CreateNewGameScene();
                    SetNextScene(startMenu);
                })
                .OnReturnToGame(() => SetNextScene(gameScene));

            CreateNewGameScene();

            SetNextScene(startMenu);
        }

        public override void Update()
        {
            if (hasToChangeScene()) CurrentScene = nextScene;

            PreUpdate();

            CurrentScene.Update();

            if(CurrentScene.Uses3DCamera)
            {
                PostUpdate();
            }
        }

        public override void Render()
        {
            D3DDevice.Instance.Device.BeginScene();
            TexturesManager.Instance.clearAll();

            CurrentScene.Render();

            PostRender();
        }

        private bool hasToChangeScene() { return nextScene != null; }

        public override void Dispose()
        {
            CurrentScene.Dispose();
        }


        /**
         * This methods anounces the INTENTION of changing the current scene, but doesn't actually changes it
         * The ChangeScene() method will change the scene at a safe time and should not be called by any 
         * other than the GameModel
         */
        private void SetNextScene(Scene newScene)
        {
            nextScene = newScene;
        }

        private void StopGame()
        {
            GameForm.Stop();
            Application.Exit();
        }

        private void CreateNewGameScene()
        {
            gameScene = new GameScene(Input, MediaDir)
                    .OnEscape(() => SetNextScene(
                        pauseMenu.WithPreRender(gameScene.Render)
                    ));
        }
    }
}