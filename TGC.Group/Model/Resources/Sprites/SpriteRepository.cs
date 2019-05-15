using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Resources.Sprites
{
    class BitmapRepository
    {
        public static CustomBitmap BlackRectangle = new CustomBitmap("../../../res/rectPause.png", D3DDevice.Instance.Device);
        public static CustomBitmap WaterRectangle = new CustomBitmap("../../../res/rectWater.png", D3DDevice.Instance.Device);
        public static CustomBitmap Title = new CustomBitmap("../../../res/title.png", D3DDevice.Instance.Device);
        public static CustomBitmap PDA = new CustomBitmap("../../../res/PDA.png", D3DDevice.Instance.Device);
        public static CustomBitmap Mask = new CustomBitmap("../../../res/mask.png", D3DDevice.Instance.Device);
        public static CustomBitmap Aim = new CustomBitmap("../../../res/aim.png", D3DDevice.Instance.Device);
        public static CustomBitmap Hand = new CustomBitmap("../../../res/hand.png", D3DDevice.Instance.Device);
        public static CustomBitmap ArrowPointer = new CustomBitmap("../../../res/arrow-pointer.png", D3DDevice.Instance.Device);
        public static CustomBitmap Bubble = new CustomBitmap("../../../res/bubble.png", D3DDevice.Instance.Device);
        public static CustomBitmap Fish = new CustomBitmap("../../../res/fish.png", D3DDevice.Instance.Device);
        public static CustomBitmap Plant = new CustomBitmap("../../../res/plant.png", D3DDevice.Instance.Device);
        public static CustomSprite CreateSpriteFromBitmap(CustomBitmap bitmap)
        {
            CustomSprite ret = new CustomSprite();
            ret.Bitmap = bitmap;
            return ret;
        }
    }
}
