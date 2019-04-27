﻿using System;
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
        public static CustomBitmap SubnauticaPortrait = new CustomBitmap("../../../res/subnautica-portada.png", D3DDevice.Instance.Device);
        public static CustomSprite CreateSpriteFromPath(CustomBitmap bitmap)
        {
            CustomSprite ret = new CustomSprite();
            ret.Bitmap = bitmap;
            return ret;
        }
    }
}
