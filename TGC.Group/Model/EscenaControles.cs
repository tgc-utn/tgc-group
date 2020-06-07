using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Text;

namespace TGC.Group.Model
{
    class EscenaControles : Escena
    {

        CustomSprite unSprite;

        public EscenaControles(TgcCamera Camera, string MediaDir, TgcText2D DrawText, float TimeBetweenUpdates, TgcD3dInput Input) : base(Camera, MediaDir, DrawText, TimeBetweenUpdates, Input)
        {
            unSprite = new CustomSprite();
            unSprite.Bitmap = new CustomBitmap(MediaDir + "Textures\\GameOver.png", D3DDevice.Instance.Device);

            unSprite.Scaling = new TGCVector2((float)D3DDevice.Instance.Width / unSprite.Bitmap.Width, (float)D3DDevice.Instance.Height / unSprite.Bitmap.Height);
            unSprite.Position = new TGCVector2(0, 0);
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override void Render()
        {
            drawer2D.BeginDrawSprite();
            drawer2D.DrawSprite(unSprite);
            drawer2D.EndDrawSprite();
        }

        public override Escena Update(float ElapsedTime)
        {
            return this;
        }
    }
}
