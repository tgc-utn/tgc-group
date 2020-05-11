using BulletSharp;
using Microsoft.DirectX.DirectInput;
using TGC.Core.BulletPhysics;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Jugador
    {
        //Objetos de meshes
        public TgcMesh    Mesh { get; }
        public TGCVector3 Position { get; set; }
        public TGCVector3 Rotation { get; set; }

        //Objetos de fisica
        public RigidBody CocheBody { get; }
        private BulletSharp.Math.Quaternion rotation;

        //Objetos de juego
        private Microsoft.DirectX.DirectInput.Key inputAvanzar;

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
        }

        public void HandleInput(TgcD3dInput input)
        {
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
            Mesh.Render();

            Mesh.BoundingBox.transform(Mesh.Transform);
            Mesh.BoundingBox.Render();
        }


    }
}
