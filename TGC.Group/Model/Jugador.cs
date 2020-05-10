using BulletSharp;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BulletPhysics;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Example;
using TGC.Core.Input;

namespace TGC.Group.Model
{
    class Jugador
    {
        public TgcMesh Mesh { get; }
        public TGCVector3 Position { get; set; }
        public TGCVector3 Rotation { get; set; }

        public RigidBody CocheBody { get; }

        private Microsoft.DirectX.DirectInput.Key inputAvanzar;

        private BulletSharp.Math.Quaternion rotation;

        private bool avanzando;

        public Jugador(TgcMesh mesh, TGCVector3 position, TGCVector3 rotation)
        {
            inputAvanzar = Key.UpArrow;

            Mesh = mesh;
            Position = position;
            Rotation = rotation;
            Position += new TGCVector3(0, 10, 0);

            this.rotation = new BulletSharp.Math.Quaternion();

            CocheBody = BulletRigidBodyFactory.Instance.CreateBox(Mesh.BoundingBox.calculateSize(), 1f, Position, 0f, 0f, 0f, 1f, true);
        }

        public void Update(float ElapsedTime)
        {

            BulletSharp.Math.Vector3 translation = new BulletSharp.Math.Vector3();
            BulletSharp.Math.Vector3 scale = new BulletSharp.Math.Vector3();
            
            CocheBody.InterpolationWorldTransform.Decompose( out scale, out rotation, out translation);
            Position = new TGCVector3(translation);

            /*if(TGCExample.Input)
            CocheBody.Velo*/
        }

        public void HandleInput(TgcD3dInput input)
        {
            float maxVelocity = 100;
            if (input.keyDown(inputAvanzar) )
            {
                BulletSharp.Math.Vector3 velocity = new BulletSharp.Math.Vector3(0, 0, -5);
                velocity = rotation.Rotate(velocity);
                CocheBody.ApplyImpulse(velocity, new BulletSharp.Math.Vector3());
                //CocheBody.LinearVelocity = velocity;
            }
            
            if(input.keyUp(inputAvanzar))
            {
                CocheBody.LinearVelocity = new BulletSharp.Math.Vector3(0, 0, 0);
            }
        }

        public void Render()
        {
            Mesh.Transform = new TGCMatrix(CocheBody.InterpolationWorldTransform);
            Mesh.BoundingBox.transform(Mesh.Transform);
            Mesh.Render();
            Mesh.BoundingBox.Render();
        }


    }
}
