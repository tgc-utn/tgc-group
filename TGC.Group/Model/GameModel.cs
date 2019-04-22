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
        private Scene _scene = null;
        private Scene Scene
        {
            set
            {
                if(_scene != null)
                {
                    _scene.Dispose();
                }
                _scene = value;
                Camara = _scene.Camera;
            }
            get { return _scene; }
        }

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
            Scene = new StartMenu(Input)
                .onGameStart(() => {
                    Scene = new GameScene(Input, MediaDir);
                })
                .onGameExit(() => {
                    GameForm.Stop();
                    Application.Exit();
                });
        }

        public override void Update()
        {
            PreUpdate();

            Scene.Update();

            PostUpdate();
        }

        public override void Render()
        {
            PreRender();

            Scene.Render();

            PostRender();
        }

        public override void Dispose()
        {
            Scene.Dispose();
        }
    }
}