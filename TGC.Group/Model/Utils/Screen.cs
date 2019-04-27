using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Utils
{
    class Screen
    {
        public static int Width
        {
            get { return D3DDevice.Instance.Device.DisplayMode.Width; }
        }
        public static int Height
        {
            get { return D3DDevice.Instance.Device.DisplayMode.Height; }
        }
        public static void CenterSprite(CustomSprite sprite)
        {
            sprite.Position = new TGCVector2(
                (Width - sprite.Bitmap.Width * sprite.Scaling.X) / 2,
                (Height - sprite.Bitmap.Height * sprite.Scaling.Y) / 2
            );
        }
        public static void FitSpriteToScreen(CustomSprite sprite)
        {
            sprite.Scaling = new TGCVector2(
                (float)Width / sprite.Bitmap.Width,
                (float)Height / sprite.Bitmap.Height
            );
            sprite.Position = new TGCVector2(0, 0);
        }
    }
}
