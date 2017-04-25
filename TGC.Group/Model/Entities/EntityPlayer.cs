using System;
using Microsoft.DirectX;

using Microsoft.DirectX.DirectInput;
using TGC.Core.BoundingVolumes;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.SkeletalAnimation;
using System.Windows.Forms;
using TGC.Core.Text;
using TGC.Core.Geometry;
using TGC.Core.Utils;
using TGC.Core.Direct3D;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Camara;
using System.Collections.Generic;
using TGC.Core.Collision;

namespace TGC.Group.Model.Entities
{
    public class EntityPlayer : EntityUpdatable
    {
        private new TgcBoundingOrientedBox boundingBox;
        private static float               velocityNormal = 400f;
        private new Vector3                rotation;
        private Vector3                    velocity;
        private TgcD3dInput                inputManager;
        private TgcCamera                  playerLookCamera;

        private Matrix                     rotationMatrix;
        private float                      sideRotation;
        private float                      nodRotation;
        private TgcRay                     lookingRay;
        private TgcArrow arrow;
        private TgcText2D t;
        private List<TgcBoundingAxisAlignBox> colliders;

        public EntityPlayer(TgcD3dInput inputManager)
        {
            this.t = new TgcText2D();

            t.Position = new System.Drawing.Point(500, 500);
            t.Color = System.Drawing.Color.Red;
            TgcBoundingAxisAlignBox alignedBB = new TgcBoundingAxisAlignBox(new Vector3(-40f, 0f, -30f), new Vector3(40f, 200f, 30f));                                                
            this.inputManager                 = inputManager;
            this.boundingBox                  = TgcBoundingOrientedBox.computeFromAABB(alignedBB);
            this.rotation                     = new Vector3(0f, 0f, 0f);
            this.lookingRay                   = new TgcRay(this.boundingBox.Center, this.LookAt);
            this.arrow                        = new TgcArrow();            
            this.playerLookCamera             = new TgcCamera();
            this.arrow.Thickness = 1;
            this.arrow.HeadSize = new Vector2(10, 20);
            this.arrow.HeadColor = System.Drawing.Color.Blue;
            this.arrow.BodyColor = System.Drawing.Color.Red;

        }
        

        public TgcBoundingOrientedBox BoundingBox
        {
            get {return this.boundingBox;}
        }

        public Vector3 HeadPosition
        {
            get { return Vector3.Add(this.boundingBox.Center, new Vector3(0f, 90f, 0f)); }
        }

        public Vector3 LookAt
        {
            get {return this.boundingBox.Orientation[2];}
        }

        public Vector3 Side
        {
            get { return this.boundingBox.Orientation[0]; }
        }
        
        public TgcCamera Camera
        {
            get { return this.playerLookCamera; }
        }

        public List<TgcBoundingAxisAlignBox> Colliders
        {
            set { this.colliders = value; }
        }
        
        public override void update(float elapsedTime)
        {            
            this.calculateLookAt();
            this.calculateVelocities(elapsedTime);            
            
            this.manageCollisions();

            this.playerLookCamera.SetCamera(this.HeadPosition, this.rotation + this.HeadPosition);
            
            /** DELETE, JUST DEBUG WITH THIS */
            this.arrow.PStart = this.boundingBox.Center;
            this.arrow.PEnd = this.boundingBox.Center + this.velocity;
            this.arrow.updateValues();
            t.Text = this.LookAt.ToString();
        }

        protected void calculateLookAt()
        {
            this.sideRotation  -= -this.inputManager.XposRelative * 0.01f;
            this.nodRotation   -= this.inputManager.YposRelative * 0.01f;
            this.rotationMatrix = Matrix.RotationX(nodRotation) * Matrix.RotationY(sideRotation);
            this.rotation = Vector3.TransformNormal(new Vector3(0f, 0f, -1f), rotationMatrix);            
        }

        protected void manageCollisions()
        {
            this.lookingRay.Origin    = this.boundingBox.Center;
            this.lookingRay.Direction = this.velocity;

            int colliderCount = this.colliders.Count;
            Vector3 distance = new Vector3(0f, 0f, 0f);
            float   closestDistance = 1000000f;
            int     closestColliderIndex = -1;
            for(int index = 0; index < colliderCount; index++)
            {
                if(TgcCollisionUtils.intersectRayAABB(this.lookingRay, this.colliders[index], out distance))
                {
                    if(Vector3.Subtract(distance, this.boundingBox.Center).Length() <= closestDistance)
                    {
                        closestDistance      = distance.Length();
                        closestColliderIndex = index;
                    }
                }                
            }
            
                
            if(closestColliderIndex != -1)
            {

                this.boundingBox.move(this.velocity);
                this.boundingBox.setRotation(new Vector3(0f, (float)Math.Atan2(this.rotation.X, this.rotation.Z), 0f));
                if (TgcCollisionUtils.testObbAABB(this.boundingBox, this.colliders[closestColliderIndex]))
                {
                    this.boundingBox.move(-this.velocity);
                    
                }
                // TODO handle collition
            }
        }

        protected void calculateVelocities(float elapsedTime)
        {
            Vector2 twoDimensionalVelocity = new Vector2(0f, 0f);

            if(this.inputManager.keyDown(Key.W))
            {
                twoDimensionalVelocity.X = this.getTimeBasedVelocity(elapsedTime);
            }
            else if(this.inputManager.keyDown(Key.S))
            {
                twoDimensionalVelocity.X = -this.getTimeBasedVelocity(elapsedTime);
            }

            if(this.inputManager.keyDown(Key.D))
            {
                twoDimensionalVelocity.Y = this.getTimeBasedVelocity(elapsedTime);
            }
            else if(this.inputManager.keyDown(Key.A))
            {
                twoDimensionalVelocity.Y = -this.getTimeBasedVelocity(elapsedTime);                
            }
            this.velocity = this.LookAt * twoDimensionalVelocity.X + this.Side * twoDimensionalVelocity.Y;
        }

        protected float getTimeBasedVelocity(float elapsedTime)
        {
            return velocityNormal * elapsedTime;
        }

        public override void render()
        {
            this.arrow.render();
            
            this.t.render();
            this.boundingBox.render();
        }

        public new void dispose()
        {
            this.boundingBox.dispose();
            
        }
        
    }
}
