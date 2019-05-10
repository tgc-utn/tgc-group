using TGC.Core.Input;
using TGC.Core.Camara;
using System.Drawing;
using TGC.Core.Direct3D;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using static TGC.Core.Input.TgcD3dInput;

namespace TGC.Group.Model.Scenes
{
    abstract class Scene
    {
        protected CommandList pressed = new CommandList();
        protected CommandList down = new CommandList();

        private static Scene emptySceneSingleInstance;
        public static Scene Empty => emptySceneSingleInstance ?? (emptySceneSingleInstance = new EmptyScene(null));
        protected Color backgroundColor = Color.Black;
        protected bool _uses3DCamera = true;
        public bool Uses3DCamera { get { return _uses3DCamera; } }
        private TgcCamera _camera = null;
        public TgcCamera Camera
        {
            set { _camera = value; }
            get { return _camera ?? new TgcCamera(); }
        }
        protected TgcD3dInput Input { get; set; }

        protected Scene(TgcD3dInput Input)
        {
            this.Input = Input;
        }

        abstract public void Update(float elapsedTime);
        abstract public void Render();
        virtual public void Dispose() {}
        protected void ClearScreen()
        {
            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, backgroundColor, 1.0f, 0);
        }
        public virtual void ReactToInput()
        {
            foreach(Key key in pressed.keys)
            {
                if(Input.keyPressed(key))
                {
                    pressed[key]();
                }
            }
            foreach (Key key in down.keys)
            {
                if (Input.keyDown(key))
                {
                    down[key]();
                }
            }
            foreach (MouseButtons button in pressed.mouseButtons)
            {
                if (Input.buttonPressed(button))
                {
                    pressed[button]();
                }
            }
            foreach (MouseButtons button in down.mouseButtons)
            {
                if (Input.buttonDown(button))
                {
                    down[button]();
                }
            }
        }
    }

    class EmptyScene : Scene
    {
        public EmptyScene(TgcD3dInput input) : base(input) {}
        public override void Render() {}
        public override void Update(float elapsedTime) {}
    }

    class CommandList
    {
        private static byte NumberOfKeys = 237, NumberOfMouseButtons = 3;
        public delegate void Command();
        Command[] commandsForKeys = new Command[NumberOfKeys];
        Command[] commandsForMouseButtons = new Command[NumberOfMouseButtons];
        public List<Key> keys = new List<Key>();
        public List<MouseButtons> mouseButtons = new List<MouseButtons>();
        public Command this[Key key]
        {
            get { return commandsForKeys[(int)key]; }
            set
            {
                commandsForKeys[(int)key] = value;
                if(!keys.Contains(key))
                {
                    keys.Add(key);
                }
            }
        }
        public Command this[MouseButtons mouseButton]
        {
            get { return commandsForMouseButtons[(int)mouseButton]; }
            set
            {
                commandsForMouseButtons[(int)mouseButton] = value;
                if (!mouseButtons.Contains(mouseButton))
                {
                    mouseButtons.Add(mouseButton);
                }
            }
        }
        public Command this[List<object> list]
        {
            set
            {
                foreach(object mouseButtonOrKey in list)
                {
                    if( mouseButtonOrKey.GetType() == typeof(Key))
                        this[(Key)mouseButtonOrKey] = value;

                    if (mouseButtonOrKey.GetType() == typeof(MouseButtons))
                        this[(MouseButtons)mouseButtonOrKey] = value;
                }
                
            }
        }
    }
}
