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

        private float speed = 25f; //foward and horizontal speed
        private float vspeed = 10f; //vertical speed

        public TGCVector3 Position() { return mesh.Position; }

        public void InitMesh() { mesh = TGCBox.fromSize(size, null); }
        public void Render() { mesh.BoundingBox.Render(); }

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
        }
        private void Move(TGCVector3 movement) { mesh.Position += movement; }

        public void UpdateTransform() { mesh.Transform = TGCMatrix.Scaling(mesh.Scale) * TGCMatrix.RotationYawPitchRoll(mesh.Rotation.X, mesh.Rotation.Y, mesh.Rotation.Z) * TGCMatrix.Translation(mesh.Position); }
    }
}
