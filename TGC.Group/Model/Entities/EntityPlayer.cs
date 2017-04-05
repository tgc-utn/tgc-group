using Microsoft.DirectX;

using Microsoft.DirectX.DirectInput;
using TGC.Core.BoundingVolumes;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.SkeletalAnimation;

namespace TGC.Group.Model.Entities
{
    public class EntityPlayer : EntityUpdatable
    {
        private TgcBoundingOrientedBox boundingBox;
        private float                  velocity = 15f;
        private Vector3                lookAt;
        private Vector2                velocities;

        public EntityPlayer()
        {            
            this.boundingBox  = TgcBoundingOrientedBox.computeFromPoints(new Vector3[]{
            new Vector3(0, 0, 0), new Vector3(0, 10, 0), new Vector3(10, 0, 0), new Vector3(10, 10, 0),
            new Vector3(0, 0, 60), new Vector3(0, 10, 60), new Vector3(10, 0, 60), new Vector3(10, 10, 60)});
            this.lookAt  = this.boundingBox.Orientation[0];
        }

        public TgcD3dInput InputManager
        {
            get;
            set;
        }

        public TgcBoundingOrientedBox BoundingBox
        {
            get {return this.boundingBox;}
        }

        public Vector3 HeadPosition
        {
            get { Vector3 vec3 = this.boundingBox.Center; vec3.Add(new Vector3(0, 0, 10)); return vec3; }
        }

        public Vector3 LookAt
        {
            get {return this.lookAt;}
        }

        public override void update()
        {
            this.calculateLookAt();
            this.calculateVelocities();


            //this.skeletalMesh.move(this.lookAt * this.velocity * ElapsedTime)
        }

        protected void calculateLookAt()
        {
            // TODO change
            this.lookAt = new Vector3(0, 20, 0);
        }

        protected void calculateVelocities()
        {
            if(InputManager.keyDown(Key.S))
            {
                this.velocities.X = -this.velocity;
            }

            if(InputManager.keyDown(Key.W))
            {
                this.velocities.X = this.velocity;
            }

            if (InputManager.keyDown(Key.D))
            {
                this.velocities.Y = this.velocity;
            }

            if (InputManager.keyDown(Key.A))
            {
                this.velocities.Y = -this.velocity;
            }
        }

        public override void render()
        {
            this.UpdateMeshTransform();
            this.render();
        }

        public void dispose()
        {
            this.dispose();
        }
        

    }
}
