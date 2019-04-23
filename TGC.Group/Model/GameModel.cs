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
        private GameScene _gameScene;
        private GameScene GameScene
        {
            get
            {
                return _gameScene ??
                    new GameScene(Input, MediaDir)
                    .onEscape(() => {
                        newScene = StartMenu;
                    });
            }
            set
            {
                if (_gameScene  != null) _gameScene.Dispose();

                _gameScene = value;
            }
        }
        private StartMenu _startMenu;
        private StartMenu StartMenu
        {
            get // Call this getter only from Init() onwards
            {
                return _startMenu;
            }
            set
            {
                if (_startMenu != null) _startMenu.Dispose();
                _startMenu = value;
            }
        }

        private Scene _curentScene = null;
        private Scene CurrentScene
        {
            set
            {
                _curentScene = value;
                Camara = _curentScene.Camera;
            }
            get { return _curentScene; }
        }

        private Scene newScene;

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
            _startMenu = new StartMenu(Input)
                    .onGameStart(() => {
                        newScene = GameScene;
                    })
                    .onGameExit(() => {
                        GameForm.Stop();
                        Application.Exit();
                    });
            CurrentScene = StartMenu;
        }

        public override void Update()
        {
            ChangeSceneIfNecessary();

            PreUpdate();

            CurrentScene.Update();

            PostUpdate();
        }

        public override void Render()
        {
            PreRender();

            CurrentScene.Render();

            PostRender();
        }

        public override void Dispose()
        {
            CurrentScene.Dispose();
        }

        private void ChangeSceneIfNecessary()
        {
            if (newScene != null) CurrentScene = newScene;
            newScene = null;
        }
    }
}