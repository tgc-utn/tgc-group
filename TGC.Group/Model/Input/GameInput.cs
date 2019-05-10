using System.Collections.Generic;
using System.Linq;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Input;

using static TGC.Core.Input.TgcD3dInput;

namespace TGC.Group.Model.Input
{
    public class GameInput
    {
        public static readonly GameInput Up = new GameInput(new List<Key> {Key.Up, Key.W});
        public static readonly GameInput Down = new GameInput(new List<Key>{Key.Down, Key.S});
        public static readonly GameInput Left = new GameInput(new List<Key>{Key.Left, Key.A});
        public static readonly GameInput Right = new GameInput(new List<Key>{Key.Right, Key.D});
        public static readonly GameInput Float = new GameInput(new List<Key>{Key.Space});
        public static readonly GameInput Escape = new GameInput(new List<Key>{Key.Escape});
        public static readonly GameInput Enter = new GameInput(new List<Key>{Key.Return}, new List<MouseButtons> {MouseButtons.BUTTON_LEFT});
        public static readonly GameInput Statistic = new GameInput(new List<Key>{Key.F});

        public static readonly List<object> _Up = new List<object> { Key.Up, Key.W };
        public static readonly List<object> _Down = new List<object> { Key.Down, Key.S };
        public static readonly List<object> _Enter = new List<object> { Key.Return, MouseButtons.BUTTON_LEFT };
        public static readonly List<object> _Escape = new List<object> { Key.Escape };

        private readonly IEnumerable<Key> keys;
        private readonly IEnumerable<MouseButtons> buttons;

        private GameInput(IEnumerable<Key> keys, IEnumerable<MouseButtons> buttons)
        {
            this.keys = keys;
            this.buttons = buttons;
        }
        
        private GameInput(IEnumerable<Key> keys) : this(keys, Enumerable.Empty<MouseButtons>())
        {

        }


        public bool IsPressed(TgcD3dInput input)
        {
            return this.keys.Any(input.keyPressed) ||
                   this.buttons.Any(input.buttonPressed);
        }

        public bool IsDown(TgcD3dInput input)
        {
            return this.keys.Any(input.keyDown) ||
                   this.buttons.Any(input.buttonDown);
        }

        public bool IsUp(TgcD3dInput input)
        {
            return this.keys.Any(input.keyUp) ||
                   this.buttons.Any(input.buttonUp);
        }
    }
}