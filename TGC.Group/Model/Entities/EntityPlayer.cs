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
        private new TgcBoundingCylinderFixedY boundingBox;
        private static float                  velocityNormal = 400f;
        private new Vector3                   rotation;
        private Vector3                       velocity;
        private TgcD3dInput                   inputManager;
        private TgcCamera                     playerLookCamera;
        private Matrix                        rotationMatrix;
        private float                         sideRotation;
        private float                         nodRotation;
        private TgcRay                        velocityRay;
        private List<TgcBoundingAxisAlignBox> colliders;
        private TgcArrow                      arrow;
        private TgcArrow                      distanceArrow;        
        protected TgcSkeletalMesh             hand;
        protected TgcText2D text;

            

        public EntityPlayer(TgcD3dInput inputManager, TgcSkeletalLoader loader, string mediaPath)
        {
            this.text = new TgcText2D();
            this.text.Color = System.Drawing.Color.Red;
            this.text.Position = new System.Drawing.Point(500, 500);
            
            this.inputManager                 = inputManager;
            this.boundingBox                  = new TgcBoundingCylinderFixedY(new Vector3(0f, 100f, 0f), 28f, 100f);
            this.rotation                     = new Vector3(0f, 0f, 0f);
            this.playerLookCamera             = new TgcCamera();
            this.velocityRay                  = new TgcRay(this.boundingBox.Center, this.LookAt);
            this.arrow                        = new TgcArrow();            
            
            this.hand                         = loader.loadMeshAndAnimationsFromFile(mediaPath + "/Hand-TgcSkeletalMesh.xml", new string[] { mediaPath + "/HandAnimation-TgcSkeletalAnim.xml", mediaPath + "/HandzAnimation-TgcSkeletalAnim.xml" });
            this.hand.playAnimation("Animation2");
            this.hand.stopAnimation();
            this.hand.rotateX(-(float)Math.PI / 2);
            this.hand.Scale                   = new Vector3(1 / 6f, 1 / 6f, 1 / 6f);
            
            
            this.arrow.Thickness = 1;
            
            this.arrow.HeadSize = new Vector2(5, 6);
            this.arrow.HeadColor = System.Drawing.Color.Blue;
            this.arrow.BodyColor = System.Drawing.Color.Red;

            this.distanceArrow = new TgcArrow();
            this.distanceArrow.Thickness = 1;
            this.distanceArrow.HeadSize = new Vector2(7, 7);
            this.distanceArrow.HeadColor = System.Drawing.Color.Green;
            this.distanceArrow.BodyColor = System.Drawing.Color.LightGreen;
        }
        

        public TgcBoundingCylinderFixedY BoundingBox
        {
            get {return this.boundingBox;}
        }

        public Vector3 HeadPosition
        {
            get { return Vector3.Add(this.boundingBox.Center, new Vector3(0f, 85f, 0f)); }
        }

        protected Vector3 HandPosition
        {
            get { return this.boundingBox.Center; }
        }

        public Vector3 LookAt
        {
            get {return new Vector3(this.rotation.X, 0f, this.rotation.Z);}
        }

        public Vector3 Side
        {
            get { return new Vector3(this.LookAt.Z, 0f, -this.LookAt.X); }
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
            this.rotateCamera();
            this.move();

            this.hand.Rotation = new Vector3(this.hand.Rotation.X, this.Camera.LookAt.Y, this.hand.Rotation.Z);
            this.hand.Position = this.HandPosition - this.LookAt * 60f;

            this.hand.getBoneByName("Arm").MatFinal.RotateY(FastMath.PI_HALF);
            this.hand.UpdateMeshTransform();
            this.hand.updateAnimation(elapsedTime);


            /** DELETE, JUST DEBUG WITH THIS */
           

            
            
        }

        protected void calculateLookAt()
        {            
            this.sideRotation += this.inputManager.XposRelative * 0.01f;            
            this.nodRotation   -= this.inputManager.YposRelative * 0.01f;
            this.rotationMatrix = Matrix.RotationX(nodRotation) * Matrix.RotationY(sideRotation);
            this.rotation       = Vector3.TransformNormal(new Vector3(0f, 0f, -1f), rotationMatrix);            
        }

        protected void rotateCamera()
        {
            this.playerLookCamera.SetCamera(this.HeadPosition, this.rotation + this.HeadPosition);
        }

        protected void move()
        {          
            if (this.velocity.Length() > 0)
            {
                this.velocityRay.Origin = this.boundingBox.Center;
                this.velocityRay.Direction = this.velocity;
                Vector3 distanceToClosestCollider = new Vector3();
                TgcBoundingAxisAlignBox closestCollider = this.getClosestColliderInDirection(this.velocityRay, this.colliders, out distanceToClosestCollider);
                closestCollider.setRenderColor(System.Drawing.Color.Azure);
                if((Vector3.Normalize(this.velocity) * this.boundingBox.Radius + this.velocity).Length() < distanceToClosestCollider.Length())
                {
                    this.boundingBox.Center += this.velocity;
                    /*this.boundingBox.Center += this.velocity;
                    int colliderCount = this.colliders.Count;
                    for (int index = 0; index < colliderCount; index++)
                    {
                        if (TgcCollisionUtils.testAABBCylinder(this.colliders[index], this.boundingBox))
                        {
                            this.boundingBox.Center -= this.velocity;
                            return;
                        }
                    }*/
                }           
            }
        }

        protected TgcBoundingAxisAlignBox getClosestColliderInDirection(TgcRay ray, List<TgcBoundingAxisAlignBox> colliders, out Vector3 distanceToClosestCollider)
        {            
            int colliderCount = colliders.Count;
            Vector3 meetingPoint     = new Vector3();
            Vector3 lastMeetingPoint = new Vector3();
            float closestDistance = 10000000f;
            TgcBoundingAxisAlignBox closestCollider = new TgcBoundingAxisAlignBox();
            for(int index = 0; index < colliderCount; index++)
            {
                this.colliders[index].setRenderColor(System.Drawing.Color.Yellow);
                if(TgcCollisionUtils.intersectRayAABB(ray, colliders[index], out meetingPoint))
                {                    
                    float distanceNormalized = Vector3.Subtract(meetingPoint, ray.Origin).Length();
                    if (distanceNormalized < closestDistance)
                    {
                        lastMeetingPoint = new Vector3(meetingPoint.X, meetingPoint.Y, meetingPoint.Z);
                        closestDistance = distanceNormalized;
                        closestCollider = colliders[index];
                    }                    
                }
            }            
            distanceToClosestCollider = Vector3.Subtract(lastMeetingPoint, ray.Origin);
            return closestCollider;
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
            this.hand.render();
            this.hand.BoundingBox.render();            
            this.boundingBox.render();
            this.distanceArrow.render();

            this.text.render();
        }

        public new void dispose()
        {
            this.hand.dispose();
            this.boundingBox.dispose();
                
        }
        
    }
}
