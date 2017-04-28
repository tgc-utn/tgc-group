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
        private List<TgcBoundingAxisAlignBox> colliders;
        private Vector3                    preRotation;
        private float                      deltaSideRotation;
        private TgcArrow                   arrow;
        protected TgcSkeletalMesh          hand;

        private float yRotation;

        public EntityPlayer(TgcD3dInput inputManager, TgcSkeletalLoader loader, string mediaPath)
        {
            TgcBoundingAxisAlignBox alignedBB = new TgcBoundingAxisAlignBox(new Vector3(-40f, 0f, -30f), new Vector3(40f, 200f, 30f));                                                
            this.inputManager                 = inputManager;
            this.boundingBox                  = TgcBoundingOrientedBox.computeFromAABB(alignedBB);
            this.rotation                     = new Vector3(0f, 0f, 0f);
            this.preRotation                  = new Vector3(0f, 0f, 0f);
            this.lookingRay                   = new TgcRay(this.boundingBox.Center, this.LookAt);
            this.arrow                        = new TgcArrow();            
            this.playerLookCamera             = new TgcCamera();
            this.hand                         = loader.loadMeshAndAnimationsFromFile(mediaPath + "/Hand-TgcSkeletalMesh.xml", new string[] { mediaPath + "/HandAnimation-TgcSkeletalAnim.xml" });
            this.hand.playAnimation("Animation");
            this.hand.stopAnimation();
            this.hand.rotateX(-(float)Math.PI / 2);
            this.hand.Scale                   = new Vector3(1 / 6f, 1 / 6f, 1 / 6f);
            
            
            this.arrow.Thickness = 1;
            this.arrow.HeadSize = new Vector2(10, 20);
            this.arrow.HeadColor = System.Drawing.Color.Blue;
            this.arrow.BodyColor = System.Drawing.Color.Red;
            MessageBox.Show(this.boundingBox.Orientation[0].ToString() + "\n" + this.boundingBox.Orientation[1].ToString());

        }
        

        public TgcBoundingOrientedBox BoundingBox
        {
            get {return this.boundingBox;}
        }

        public Vector3 HeadPosition
        {
            get { return Vector3.Add(this.boundingBox.Center, new Vector3(0f, 90f, 0f)); }
        }

        protected Vector3 HandPosition
        {
            get { return this.boundingBox.Orientation[1] * 20f + this.boundingBox.Orientation[0] * 50f + this.boundingBox.Orientation[2] + this.boundingBox.Center; }
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
            this.rotateAndMove();

            this.hand.Rotation = new Vector3(this.hand.Rotation.X, this.yRotation - FastMath.PI_HALF, this.hand.Rotation.Z);
            this.hand.Position = this.HandPosition - this.LookAt * 60f;

            this.hand.getBoneByName("Arm").MatFinal.RotateY(FastMath.PI_HALF);
            this.hand.UpdateMeshTransform();
            this.hand.updateAnimation(elapsedTime);


            /** DELETE, JUST DEBUG WITH THIS */
            this.arrow.PStart = this.boundingBox.Center;
            this.arrow.PEnd = this.boundingBox.Center + this.velocity;
            this.arrow.updateValues();
            
        }

        protected void calculateLookAt()
        {
            this.preRotation    = new Vector3(this.rotation.X, this.rotation.Y, this.rotation.Z);            
            this.deltaSideRotation = this.inputManager.XposRelative * 0.01f;
            if(this.deltaSideRotation < FastMath.QUARTER_PI / 2)
            {
                this.sideRotation += this.deltaSideRotation;
            }
            this.nodRotation   -= this.inputManager.YposRelative * 0.01f;
            this.rotationMatrix = Matrix.RotationX(nodRotation) * Matrix.RotationY(sideRotation);
            this.rotation       = Vector3.TransformNormal(new Vector3(0f, 0f, -1f), rotationMatrix);            
        }

        protected void rotateAndMove()
        {
            this.yRotation = FastMath.Atan2(this.rotation.X, this.rotation.Z);
            this.playerLookCamera.SetCamera(this.HeadPosition, this.rotation + this.HeadPosition);
            this.boundingBox.setRotation(new Vector3(0f, this.yRotation, 0f));
            this.boundingBox.move(this.velocity);

            int colliderCount = this.colliders.Count;
            for(int index = 0; index < colliderCount; index++)
            {
                if (TgcCollisionUtils.testObbAABB(this.boundingBox, this.colliders[index]))
                {                    
                    this.playerLookCamera.SetCamera(this.HeadPosition, this.preRotation + this.HeadPosition);
                    this.boundingBox.setRotation(new Vector3(0f, (float)Math.Atan2(this.preRotation.X, this.preRotation.Z), 0f));
                    this.boundingBox.move(-this.velocity);
                    this.rotation = this.preRotation;
                }
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
            this.hand.render();
            this.hand.BoundingBox.render();            
            this.boundingBox.render();
        }

        public new void dispose()
        {
            this.hand.dispose();
            this.boundingBox.dispose();
            
        }
        
    }
}
