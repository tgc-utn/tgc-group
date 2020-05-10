using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.Input;
using TGC.Core.Camara;
using Microsoft.DirectX.DirectInput;


namespace TGC.Group.Model
{
    class CamaraJugador
    {

        private TGCVector3 LookAt;
        private TGCVector3 CameraPosition;
        private TgcCamera camara;

        private Jugador jugador;
        private BulletSharp.RigidBody pelota;

        public CamaraJugador(Jugador jugador, BulletSharp.RigidBody pelota, TgcCamera camara)
        {
            this.jugador = jugador;
            this.pelota = pelota;
            this.camara = camara;

            LookAt = new TGCVector3(TGCVector3.Empty);
            CameraPosition = new TGCVector3(0, 100, 225);
        }

        public void Update(float ElapsedTime)
        {
            BulletSharp.Math.Vector3 scale = new BulletSharp.Math.Vector3();
            BulletSharp.Math.Vector3 translation = new BulletSharp.Math.Vector3();
            BulletSharp.Math.Quaternion rotation = new BulletSharp.Math.Quaternion();

            pelota.InterpolationWorldTransform.Decompose(out scale, out rotation, out translation);

            LookAt = new TGCVector3(translation);
            CameraPosition = jugador.Position;
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
