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
    class EscenaGameOver : Escena
    {
        Drawer2D drawer;
        CustomSprite gameOver;

        public EscenaGameOver(TgcCamera Camera, string MediaDir, TgcText2D DrawText, float TimeBetweenUpdates, TgcD3dInput Input) : base(Camera, MediaDir, DrawText, TimeBetweenUpdates, Input)
        {
            drawer = new Drawer2D();
            gameOver = new CustomSprite();
            gameOver.Bitmap = new CustomBitmap(MediaDir + "Textures\\GameOver.png", D3DDevice.Instance.Device);

            gameOver.Scaling = new TGCVector2((float)D3DDevice.Instance.Width / gameOver.Bitmap.Width, (float)D3DDevice.Instance.Height / gameOver.Bitmap.Height);
            gameOver.Position = new TGCVector2(0, 0);
        }
        public override void Dispose()
        {
            gameOver.Dispose();
        }

        public override void Render()
        {
            drawer.BeginDrawSprite();
            drawer.DrawSprite(gameOver);
            drawer.EndDrawSprite();
        }

        public override Escena Update(float ElapsedTime)
        {
            if (Input.keyDown(Microsoft.DirectX.DirectInput.Key.Space))
            {
                return CambiarEscena(new EscenaMenu(Camera, MediaDir, DrawText, TimeBetweenUpdates, Input));
            }
            return this;
        }
    }
}
