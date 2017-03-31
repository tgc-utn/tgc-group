using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Geometry;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;

namespace TGC.Group.Model.Entities
{
    public class EntityPlayer : EntityUpdatable
    {
        private TgcSkeletalMesh        skeletalMesh;
        private TgcBoundingOrientedBox boundingBox;
        private float                  velocity = 15f;
        private Vector3                lookAt;
        private Vector2                velocities;

        public Player(TgcSkeletalMesh skeletalMesh)
        {
            this.skeletalMesh = skeletalMesh;
            this.boundingBox  = TgcBoundingOrientedBox.computeFromAABB({
            {0, 0, 0}, {0, 10, 0}, {10, 0, 0}, {10, 10, 0},
            {0, 0, 60}, {0, 10, 60}, {10, 0, 60}, {10, 10, 60}});
            this.orientation  = this.boundingBox.Orientation;
        }

        public TgcBoundingOrientedBox BoundingBox
        {
            get {return this.boundingBox;}
        }

        public Vector3 HeadPosition
        {
            get {return this.Center.Add(new Vector3(0, 0, 10));}
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
            this.lookAt = {0,20,0};
        }

        protected void calculateVelocities()
        {
            if(Input.keyDown(Key.S))
            {
                this.velocities.X = -this.velocity;
            }

            if(Input.keyDown(Key.W))
            {
                this.velocities.X = this.velocity;
            }

            if (Input.keyDown(Key.D))
            {
                this.velocities.Y = this.velocity;
            }

            if (Input.keyDown(Key.A))
            {
                this.velocities.Y = -this.velocity;
            }
        }

        public override void render()
        {
            base.update();
            this.skeletalMesh.render();
        }

        public void dispose()
        {
            this.skeletalMesh.dispose();
        }
        

    }
}
