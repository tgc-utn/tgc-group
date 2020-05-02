using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Camara;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class FPSCamara
    {
        private TgcCamera Camara;
        private Player Player;
        //Internal vars
        public TGCVector2 cam_angles = TGCVector2.Zero;

        //Configuration
        private const float CAMERA_MAX_X_ANGLE = 1.5f;
        private float sensitivity = 10f;

        public FPSCamara(TgcCamera Camara, Player Player)
        {
            this.Camara = Camara;
            this.Player = Player;
        }

        public void Update(TgcD3dInput Input, float ElapsedTime)
        {
            cam_angles += new TGCVector2(Input.YposRelative, Input.XposRelative) * sensitivity * ElapsedTime;
            cam_angles.X = FastMath.Clamp(cam_angles.X, -CAMERA_MAX_X_ANGLE, CAMERA_MAX_X_ANGLE);
            cam_angles.Y = cam_angles.Y > 2 * FastMath.PI || cam_angles.Y < -2 * FastMath.PI ? 0 : cam_angles.Y;
            TGCQuaternion rotationY = TGCQuaternion.RotationAxis(new TGCVector3(0f, 1f, 0f), cam_angles.Y);
            TGCQuaternion rotationX = TGCQuaternion.RotationAxis(new TGCVector3(1f, 0f, 0f), -cam_angles.X);
            TGCQuaternion rotation = rotationX * rotationY;

            var init_offset = new TGCVector3(0f, 0f, 1f);
            TGCMatrix camera_m = TGCMatrix.Translation(init_offset) * TGCMatrix.RotationTGCQuaternion(rotation) * TGCMatrix.Translation(Player.Position());
            TGCVector3 pos = new TGCVector3(camera_m.M41, camera_m.M42, camera_m.M43);
            Camara.SetCamera(pos, Player.Position());
        }

        public TGCVector3 LookDir() { return TGCVector3.Normalize(Camara.LookAt - Camara.Position); }
        public TGCVector3 LeftDir() { return TGCVector3.Cross(LookDir(), Camara.UpVector); }

        //Configuration Functions
        public void SetSensitivity(float newSensitivity) { sensitivity = newSensitivity; }
    }
}
