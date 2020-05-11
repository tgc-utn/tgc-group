using Microsoft.DirectX.DirectInput;
using TGC.Core.Camara;
using TGC.Core.Input;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class CamaraJugador
    {

        private TGCVector3 LookAt;
        private TGCVector3 CameraPosition;
        private TgcCamera  camara;

        private ObjetoJuego jugador;
        private ObjetoJuego pelota;

        public CamaraJugador(ObjetoJuego jugador, ObjetoJuego pelota, TgcCamera camara)
        {
            this.jugador = jugador;
            this.pelota = pelota;
            this.camara = camara;

            LookAt = new TGCVector3(TGCVector3.Empty);
            CameraPosition = new TGCVector3(0, 100, 225);
        }

        public void Update(float ElapsedTime)
        {

            LookAt = pelota.Translation;
            CameraPosition = jugador.Translation;
            CameraPosition.Y += 10;
            CameraPosition.Z += 100;

            camara.SetCamera(CameraPosition, LookAt);
        }

        public void HandleInput(TgcD3dInput input, float ElapsedTime)
        {
            LookAt.X -= input.XposRelative * 3f;
            LookAt.Y -= input.YposRelative * 3f;

            if (input.keyDown(Key.W))
            {
                CameraPosition -= TGCVector3.Normalize(CameraPosition - LookAt) * ElapsedTime * 100f;
                //CameraPosition.Z -= 100f * ElapsedTime;
            }
            if (input.keyDown(Key.S))
            {
                CameraPosition += TGCVector3.Normalize(CameraPosition - LookAt) * ElapsedTime * 100f;
                //CameraPosition.Z += 100f * ElapsedTime;
            }
        }
    }
}
