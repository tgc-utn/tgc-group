using Microsoft.DirectX.DirectInput;
using TGC.Core.Camara;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model
{
    class CamaraJugador
    {

        public TGCVector3 CameraDirection
        {
            get
            {
                TGCVector3 cameraDirection = cameraPosition - lookAt;
                cameraDirection.Normalize();
                return cameraDirection;
            }
        }

        private TGCVector3 lookAt;
        private TGCVector3 cameraPosition;
        private TgcCamera  camara;

        TgcBoundingAxisAlignBox limites;

        private ObjetoJuego jugador;
        private ObjetoJuego pelota;

        public CamaraJugador(ObjetoJuego jugador, ObjetoJuego pelota, TgcCamera camara, TgcBoundingAxisAlignBox limites)
        {
            this.jugador = jugador;
            this.pelota = pelota;
            this.camara = camara;
            this.limites = limites;

            lookAt = new TGCVector3(TGCVector3.Empty);
            cameraPosition = new TGCVector3(0, 100, 225);
        }

        public void Update(float ElapsedTime)
        {
            TGCVector3 viewDirection = jugador.Translation - pelota.Translation;
            TGCVector3 jugadorTranslation = jugador.Translation;

            viewDirection.Normalize();

            cameraPosition = jugadorTranslation  + viewDirection * 25f;

            float minTranslateY = jugadorTranslation.Y + 2f;

            if (cameraPosition.Y < minTranslateY)
            {
                cameraPosition.Y = minTranslateY;
            }
            
            if (cameraPosition.X < limites.PMin.X)
            {
                cameraPosition.X = limites.PMin.X;
            }
            else if (cameraPosition.X > limites.PMax.X)
            {
                cameraPosition.X = limites.PMax.X;
            }
            
            if (cameraPosition.Z < limites.PMin.Z)
            {
                cameraPosition.Z = limites.PMin.Z;
            }
            else if (cameraPosition.Z > limites.PMax.Z)
            {
                cameraPosition.Z = limites.PMax.Z;
            }

            camara.SetCamera(cameraPosition, pelota.Translation);
        }

        public void HandleInput(TgcD3dInput input, float ElapsedTime)
        {
            lookAt.X -= input.XposRelative * 3f;
            lookAt.Y -= input.YposRelative * 3f;

            if (input.keyDown(Key.W))
            {
                cameraPosition -= TGCVector3.Normalize(cameraPosition - lookAt) * ElapsedTime * 100f;
                //CameraPosition.Z -= 100f * ElapsedTime;
            }
            if (input.keyDown(Key.S))
            {
                cameraPosition += TGCVector3.Normalize(cameraPosition - lookAt) * ElapsedTime * 100f;
                //CameraPosition.Z += 100f * ElapsedTime;
            }
        }
    }
}
