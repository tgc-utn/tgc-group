using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Camara;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace TGC.Group.Model
{
    class Player
    {
        //Gameplay vars
        private List<Item> inventory;
        private float oxygen = 100f;
        private float health = 100f;

        private const float OXYGEN_LOSS_SPEED = 10f;
        private const float OXYGEN_RECOVER_SPEED = OXYGEN_LOSS_SPEED * 3.2f;
        private const float OXYGEN_DAMAGE = 5f;
        private const float WATER_LEVEL = 10f; //When players reaches a position above this level, then recovers oxygen.

        //Dev vars
        private bool godmode = false;

        
        //Transformations vars
        private TGCBox mesh { get; set; }
        private TGCVector3 size = new TGCVector3(2, 5, 2);
        private TGCQuaternion rotation = TGCQuaternion.Identity;

        //Config vars
        private float sensitivity = 53f;
        public TGCVector2 cam_angles = TGCVector2.Zero;
        private const float CAMERA_MAX_X_ANGLE = 1.6f;


        private float speed = 25f; //foward and horizontal speed
        private float vspeed = 10f; //vertical speed


        //Tgc functions
        public TGCVector3 Position() { return mesh.Position; }

        public void InitMesh() { mesh = TGCBox.fromSize(size, null); }

        public void Update(TgcD3dInput Input,TgcCamera Camara, float ElapsedTime) {
            CheckInputs(Input, Camara, ElapsedTime);
            GameplayUpdate(ElapsedTime);
            UpdateCamera(Input, Camara, ElapsedTime);
        }
        public void Render() { mesh.BoundingBox.Render(); }

        private void CheckInputs(TgcD3dInput Input,TgcCamera Camara, float ElapsedTime)
        {
            //Gameplay
            int w = Input.keyDown(Key.W) ? 1 : 0;
            int s = Input.keyDown(Key.S) ? 1 : 0;
            int d = Input.keyDown(Key.A) ? 1 : 0;
            int a = Input.keyDown(Key.D) ? 1 : 0;
            int space = Input.keyDown(Key.Space) ? 1 : 0;
            int ctrl = Input.keyDown(Key.LeftControl) ? 1 : 0;

            float fmov = w - s; //foward movement
            float hmov = d - a; //horizontal movement
            float vmov = space - ctrl; //vertical movement

            TGCVector3 movement = GetCameraLookDir(Camara) * fmov + GetCameraLeftDir(Camara) * hmov + TGCVector3.Up * vmov;
            movement *= speed * ElapsedTime;
            Move(movement);

            Rotate(new TGCVector2(Input.XposRelative, Input.YposRelative) * sensitivity * ElapsedTime);

            //Dev
            bool p = Input.keyPressed(Key.P);
            if (p) { godmode = !godmode; GodMode(godmode); }
        }

        private void Move(TGCVector3 movement) { mesh.Position += movement; }

        private void Rotate(TGCVector2 rotAmount) {
           TGCQuaternion rotationX = TGCQuaternion.RotationAxis(new TGCVector3(0.0f, 1.0f, 0.0f), rotAmount.X);
           TGCQuaternion rotationY = TGCQuaternion.RotationAxis(new TGCVector3(1.0f, 0.0f, 0.0f), rotAmount.Y);
           rotation *= rotationX * rotationY;
        }

        public void Dispose() { mesh.Dispose(); }

        public TGCMatrix Rotation() { return TGCMatrix.RotationTGCQuaternion(rotation); }

        public void UpdateTransform() { mesh.Transform = TGCMatrix.Scaling(mesh.Scale) * TGCMatrix.RotationTGCQuaternion(rotation) * TGCMatrix.Translation(mesh.Position); }

        // Camera functions
        private void UpdateCamera(TgcD3dInput Input, TgcCamera Camara, float ElapsedTime)
        {
            cam_angles += new TGCVector2(Input.YposRelative, Input.XposRelative) * sensitivity * ElapsedTime;
            cam_angles.X = FastMath.Clamp(cam_angles.X, -CAMERA_MAX_X_ANGLE, CAMERA_MAX_X_ANGLE);
            cam_angles.Y = cam_angles.Y > 2 * FastMath.PI || cam_angles.Y < -2 * FastMath.PI ? 0 : cam_angles.Y;
            TGCQuaternion rotationY = TGCQuaternion.RotationAxis(new TGCVector3(0f, 1f, 0f), cam_angles.Y);
            TGCQuaternion rotationX = TGCQuaternion.RotationAxis(new TGCVector3(1f, 0f, 0f), -cam_angles.X);
            TGCQuaternion rotation = rotationX * rotationY;

            var init_offset = new TGCVector3(0f,0f,1f);
            TGCMatrix camera_m = TGCMatrix.Translation(init_offset) * TGCMatrix.RotationTGCQuaternion(rotation) * TGCMatrix.Translation(Position());
            TGCVector3 pos = new TGCVector3(camera_m.M41, camera_m.M42, camera_m.M43);
            Camara.SetCamera(pos, Position());
        }

        private TGCVector3 GetCameraLookDir(TgcCamera Camara) { return TGCVector3.Normalize(Camara.LookAt - Camara.Position); }
        private TGCVector3 GetCameraLeftDir(TgcCamera Camara) { return TGCVector3.Cross(GetCameraLookDir(Camara), Camara.UpVector); }


        //Gameplay functions
        private void GameplayUpdate(float ElapsedTime)
        {
            if (!godmode)
            {
                if (IsOutsideWater()) RecoverOxygen(ElapsedTime); else LoseOxygen(ElapsedTime);
            }
        }

        private void LoseOxygen(float ElapsedTime) { 
            oxygen = Math.Max(0, oxygen - OXYGEN_LOSS_SPEED * ElapsedTime);
            if (oxygen == 0) GetDamage(OXYGEN_DAMAGE * ElapsedTime);
        }

        private void GetHeal(float amount) { health = Math.Min(100f, health + amount); }
        private void GetDamage(float amount) { health = Math.Max(0, health - amount); }
        private void RecoverOxygen(float ElapsedTime) { oxygen = Math.Min(100, oxygen + OXYGEN_RECOVER_SPEED * ElapsedTime); }
        private bool IsOutsideWater() { return mesh.Position.Y > WATER_LEVEL; }

        public float Oxygen() { return oxygen; }
        public float Health() { return health; }

        //Dev functions
        private void GodMode(bool enabled)
        {
            if (enabled)
            {
                health = 100f;
                oxygen = 100f;
            }
        }
    }
}
