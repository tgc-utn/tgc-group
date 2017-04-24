using System.Collections.Generic;
using TGC.Group.Model.Entities;
using TGC.Group.Model.Camera;
using TGC.Core.Input;
using TGC.Core.Camara;
using TGC.Core.SkeletalAnimation;
using Microsoft.DirectX;
using System;
using TGC.Core.Direct3D;
using Microsoft.DirectX.Direct3D;
using System.Windows.Forms;

namespace TGC.Group.Model.GameWorld
{
    public class World
    {
        protected List<IEntity>          entities;
        protected List<EntityUpdatable>  updatableEntities;
        protected EntityPlayer           player;
        protected WorldMap               worldMap;
        protected TgcD3dInput            inputManager;
        protected TgcSkeletalMesh        enemy;
        protected TgcSkeletalMesh        hand;
        protected bool                   outsideCamera;
        protected TgcCamera              camera;
        

        public World(string mediaPath, TgcD3dInput inputManager)
        {
            this.outsideCamera = false;
            this.inputManager         = inputManager;
            this.entities             = new List<IEntity>();
            this.updatableEntities    = new List<EntityUpdatable>();
            this.player               = new EntityPlayer(this.inputManager);
            this.worldMap             = new WorldMap(mediaPath);
            this.camera               = new TgcCamera();
            this.camera.SetCamera(new Vector3(0f, 1000f, 0f), new Vector3(10f, 0f, 10f));


            TgcSkeletalLoader loader  = new TgcSkeletalLoader();
            enemy                     = loader.loadMeshAndAnimationsFromFile(mediaPath + "/Monster-TgcSkeletalMesh.xml", new string[]{mediaPath + "/Run-TgcSkeletalAnim.xml"});
            enemy.playAnimation("Run");
            enemy.Position            = new Vector3(0f, 0f,0f);
            enemy.Scale = new Vector3(3f, 3f, 3f);

            
            
            this.hand = loader.loadMeshAndAnimationsFromFile(mediaPath + "/Hand-TgcSkeletalMesh.xml", new string[] { mediaPath + "/HandAnimation-TgcSkeletalAnim.xml" });
            hand.playAnimation("Animation");
            
            hand.Position = new Vector3(0f, 500f, 0f);
            hand.Scale = new Vector3(1 / 6f, 1 / 6f, 1 / 6f);
            hand.rotateX(-(float)Math.PI / 2);



        }

        public TgcCamera Camera
        {
            get { return (this.outsideCamera) ? this.camera : this.player.Camera; }
        }

        public void update(float elapsedTime)
        {
            foreach(EntityUpdatable currentEntity in this.updatableEntities)
            {
                currentEntity.update(elapsedTime);
            }
            
            if(inputManager.keyDown(Microsoft.DirectX.DirectInput.Key.C))
            {
                this.outsideCamera = true;                
            }
            else
            {
                this.outsideCamera = false;
            }

            if(inputManager.keyDown(Microsoft.DirectX.DirectInput.Key.J))
            {
                this.worldMap.ShouldShowRoof = false;
            }
            else
            {
                this.worldMap.ShouldShowRoof = true;
            }
            if(inputManager.keyDown(Microsoft.DirectX.DirectInput.Key.K))
            {
                this.worldMap.ShouldShowBoundingVolumes = true;
            }
            else
            {
                this.worldMap.ShouldShowBoundingVolumes = false;
            }
            this.hand.UpdateMeshTransform();
            this.hand.updateAnimation(elapsedTime);
            this.enemy.UpdateMeshTransform();
            this.enemy.updateAnimation(elapsedTime);            
            this.player.update(elapsedTime);

        }

        public void render()
        {
            foreach(IEntity currentEntity in this.entities)
            {
                currentEntity.render();
            }
            
            this.enemy.render();
            this.worldMap.render();
            this.hand.render();
            this.player.render();
        }

        public void dispose()
        {            
            foreach(IEntity currentEntity in this.entities)
            {
                currentEntity.dispose();
            }
            this.player.dispose();
            this.hand.dispose();
            this.enemy.dispose();
            this.worldMap.dispose();
        }
    }
}
