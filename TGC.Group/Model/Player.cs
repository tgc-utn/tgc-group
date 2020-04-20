using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace TGC.Group.Model
{
    class Player
    {
        private List<Item> inventory;
        private float oxygen;
        private float health;

        private TGCBox mesh { get; set; }
        private TGCVector3 size = new TGCVector3(2, 5, 2);
        private TGCQuaternion rotation = TGCQuaternion.Identity;

        private float sensitivity = 5.5f; //mouse sensitivity
        private float speed = 25f; //foward and horizontal speed
        private float vspeed = 10f; //vertical speed

        public TGCVector3 Position() { return mesh.Position; }

        public void InitMesh() { mesh = TGCBox.fromSize(size, null); }
        public void Render() { mesh.BoundingBox.Render(); mesh.Render(); }

        public void CheckInputs(TgcD3dInput Input, float ElapsedTime)
        {
            int w = Input.keyDown(Key.W) ? 1 : 0;
            int s = Input.keyDown(Key.S) ? 1 : 0;
            int d = Input.keyDown(Key.A) ? 1 : 0;
            int a = Input.keyDown(Key.D) ? 1 : 0;
            int space = Input.keyDown(Key.Space) ? 1 : 0;
            int ctrl = Input.keyDown(Key.LeftControl) ? 1 : 0;

            int fmov = s - w; //foward movement
            int hmov = d - a; //horizontal movement
            int vmov = space - ctrl; //vertical movement

            Move(new TGCVector3(hmov * speed, vmov * vspeed, fmov * speed) * ElapsedTime);
            Rotate(new TGCVector2(Input.XposRelative, Input.YposRelative) * sensitivity * ElapsedTime);
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
    }
}
