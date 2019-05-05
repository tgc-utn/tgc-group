using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Textures;
using System;
using TGC.Group.Model.Scenes;
using TGC.Group.Form;
using System.Windows.Forms;
using TGC.Group.TGCUtils;
using TGC.Group.Model.Resources.Sprites;

namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo para implementar el TP.
    ///     Inicialmente puede ser renombrado o copiado para hacer m�s ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar el modelo que instancia GameForm <see cref="Form.GameForm.InitGraphics()" />
    ///     line 97.
    /// </summary>
    public class GameModel : TgcExample
    {
        private GameScene gameScene;
        private StartMenu startMenu;
        private PauseMenu pauseMenu;
        private ShipScene shipScene;

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

        Drawer2D drawerPause;
        CustomSprite spritePause;

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

            drawerPause = new Drawer2D();
            spritePause = BitmapRepository.CreateSpriteFromPath(BitmapRepository.BlackRectangle);
        }

        public override void Init()
        {
            //note(fede): Only at this point the Input field has been initialized by the form

            startMenu = new StartMenu(Input)
                    .onGameStart(() => SetNextScene(shipScene))
                    .onGameExit(StopGame);

            pauseMenu = new PauseMenu(Input, drawerPause, spritePause)
                .OnGoToStartMenu(() => {
                    ResetGame();
                    SetNextScene(startMenu);
                });

            ResetGame();

            SetNextScene(startMenu);

        }

        public override void Update()
        {
            if (hasToChangeScene()) CurrentScene = nextScene;

            PreUpdate();

            CurrentScene.Update(this.ElapsedTime);

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

        private void ResetGame()
        {
            gameScene = new GameScene(Input, MediaDir)
                    .OnPause(() => PauseScene(gameScene));

            shipScene = new ShipScene(Input)
                .OnGoToWater(() => SetNextScene(gameScene))
                .OnPause(() => PauseScene(shipScene));
        }

        private void PauseScene(Scene scene)
        {
            SetNextScene(pauseMenu
                    .WithPreRender(scene.Render)
                    .OnReturnToGame(() => SetNextScene(scene))
                 );
        }
    }
}