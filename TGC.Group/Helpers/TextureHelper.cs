using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Helpers
{
    public static class TextureHelper
    {
        public static Texture LoadTerrainTexture(Device d3dDevice, string path)
        {
            //Rotar e invertir textura
            var b = (Bitmap)Image.FromFile(path);
            b.RotateFlip(RotateFlipType.Rotate90FlipX);

            return Texture.FromBitmap(d3dDevice, b, Usage.None, Pool.Managed);
        }
    }
}
