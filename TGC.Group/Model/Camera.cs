using Microsoft.DirectX.DirectInput;
using System.Drawing;
using System.Windows.Forms;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Group.Model.Input;

namespace TGC.Group.Model
{
    public class Camera : TgcCamera
    {
        private readonly Point mouseCenter;
        private TGCMatrix cameraRotation;
        private TGCVector3 initialDirectionView;
        private float leftrightRot;
        private float updownRot;

        delegate void CameraUpdateLogic(float elapsedTime);
        CameraUpdateLogic currentUpdateLogic;

        private TgcD3dInput Input { get; }
        public float MovementSpeed { get; set; }
        public float RotationSpeed { get; set; }

        public Camera(TGCVector3 position, TgcD3dInput input)
        {
            Input = input;
            Position = position;
            mouseCenter = GetMouseCenter();
            RotationSpeed = 0.1f;
            MovementSpeed = 500f;
            initialDirectionView = new TGCVector3(0, 0, -1);
            leftrightRot = 0;
            updownRot = 0;
            Cursor.Hide();
            currentUpdateLogic = MoveNormally;
        }

        private static Point GetMouseCenter()
        {

            return new Point(D3DDevice.Instance.Device.Viewport.Width / 2, D3DDevice.Instance.Device.Viewport.Height / 2);
        }

        public override void UpdateCamera(float elapsedTime)
        {
            currentUpdateLogic(elapsedTime);
        }

        public TGCMatrix CalculateCameraRotation()
        {
            leftrightRot += Input.XposRelative * RotationSpeed;
            updownRot = FastMath.Clamp( updownRot - Input.YposRelative * RotationSpeed, -FastMath.PI_HALF, FastMath.PI_HALF);

            return TGCMatrix.RotationX(updownRot) * TGCMatrix.RotationY(leftrightRot);
        }

        private TGCVector3 CalculateInputTranslation()
        {
            var moveVector = TGCVector3.Empty;

            if (GameInput.Up.IsDown(Input))
            {
                moveVector += new TGCVector3(0, 0, -1) * MovementSpeed;
            }

            if (GameInput.Down.IsDown(Input))
            {
                moveVector += new TGCVector3(0, 0, 1) * MovementSpeed;
            }

            if (GameInput.Right.IsDown(Input))
            {
                moveVector += new TGCVector3(-1, 0, 0) * MovementSpeed;
            }

            if (GameInput.Left.IsDown(Input))
            {
                moveVector += new TGCVector3(1, 0, 0) * MovementSpeed;
            }
            
            if (GameInput.Float.IsDown(Input))
            {
                moveVector += new TGCVector3(0, 1, 0) * MovementSpeed;
            }

            return moveVector;
        }
        void MoveNormally(float elapsedTime)
        {
            cameraRotation = CalculateCameraRotation();

            Position += TGCVector3.TransformNormal(CalculateInputTranslation() * elapsedTime, cameraRotation);

            LookAt = Position + TGCVector3.TransformNormal(initialDirectionView, cameraRotation);

            UpVector = TGCVector3.TransformNormal(DEFAULT_UP_VECTOR, cameraRotation);

            Cursor.Position = mouseCenter;
            base.SetCamera(Position, LookAt, UpVector);
        }

        public void Freeze()
        {
            currentUpdateLogic = (elapsedTime) => {};
        }

        public void Unfreeze()
        {
            currentUpdateLogic = MoveNormally;
        }
    }
}
