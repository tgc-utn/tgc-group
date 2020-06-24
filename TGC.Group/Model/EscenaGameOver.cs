using System.Drawing;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Text;

namespace TGC.Group.Model
{
    class EscenaGameOver : Escena
    {
        CustomSprite gameOver;

        public EscenaGameOver(TgcCamera Camera, string MediaDir, string ShadersDir, TgcText2D DrawText, float TimeBetweenUpdates, TgcD3dInput Input) : base(Camera, MediaDir, ShadersDir, DrawText, TimeBetweenUpdates, Input)
        {
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
            D3DDevice.Instance.Device.Clear(Microsoft.DirectX.Direct3D.ClearFlags.Target | Microsoft.DirectX.Direct3D.ClearFlags.ZBuffer, Color.White, 1.0f, 0);
            D3DDevice.Instance.Device.BeginScene();
            drawer2D.BeginDrawSprite();
            drawer2D.DrawSprite(gameOver);
            drawer2D.EndDrawSprite();
        }

        public override Escena Update(float ElapsedTime)
        {
            if (Input.keyDown(Microsoft.DirectX.DirectInput.Key.Space))
            {
                return CambiarEscena(new EscenaMenu(Camera, MediaDir, ShadersDir, DrawText, TimeBetweenUpdates, Input));
            }
            return this;
        }
    }
}
