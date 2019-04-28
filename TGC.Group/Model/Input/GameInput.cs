using System.Collections.Generic;
using System.Linq;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Input;

namespace TGC.Group.Model
{
    public class GameInput
    {
        public static readonly GameInput Up = new GameInput(new List<Key> {Key.Up, Key.W});
        public static readonly GameInput Down = new GameInput(new List<Key>{Key.Down, Key.S});
        public static readonly GameInput Left = new GameInput(new List<Key>{Key.Left, Key.A});
        public static readonly GameInput Right = new GameInput(new List<Key>{Key.Right, Key.D});
        public static readonly GameInput Float = new GameInput(new List<Key>{Key.Space});
        public static readonly GameInput Escape = new GameInput(new List<Key>{Key.Escape});
        public static readonly GameInput Enter = new GameInput(new List<Key>{Key.Return});
        public static readonly GameInput Statictis = new GameInput(new List<Key>{Key.F});

        private readonly List<Key> keys;

        private GameInput(List<Key> keys)
        {
            this.keys = keys;
        }

        public bool IsPressed(TgcD3dInput input)
        {
            return this.keys.Any(input.keyPressed);
        }

        public bool IsDown(TgcD3dInput input)
        {
            return this.keys.Any(input.keyDown);
        }

        public bool IsUp(TgcD3dInput input)
        {
            return this.keys.Any(input.keyUp);
        }
    }
}