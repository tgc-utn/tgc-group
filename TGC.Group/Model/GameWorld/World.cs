using System.Collections.Generic;
using TGC.Group.Model.Entities;
using TGC.Group.Model.Camera;
using TGC.Core.Input;
using TGC.Core.Camara;
using TGC.Core.SkeletalAnimation;
using Microsoft.DirectX;
using System;

namespace TGC.Group.Model.GameWorld
{
    public class World
    {
        protected List<IEntity>          entities;
        protected List<EntityUpdatable>  updatableEntities;
        protected EntityPlayer           player;
        protected WorldMap               worldMap;
        protected FirstPersonCamera      camera;
        protected TgcD3dInput            inputManager;
        protected TgcSkeletalMesh        enemy;

        protected TgcSkeletalMesh hand;

        public World(string mediaPath, TgcD3dInput inputManager)
        {
            this.inputManager         = inputManager;
            this.entities             = new List<IEntity>();
            this.updatableEntities    = new List<EntityUpdatable>();            
            this.camera               = new FirstPersonCamera(this.inputManager);
            this.camera.RotationSpeed = 0.01f;
            TgcSkeletalLoader loader  = new TgcSkeletalLoader();

            enemy                     = loader.loadMeshAndAnimationsFromFile(mediaPath + "/Monster-TgcSkeletalMesh.xml", new string[]{mediaPath + "/Run-TgcSkeletalAnim.xml"});
            enemy.playAnimation("Run");
            enemy.Position            = new Vector3(0f, 0f,0f);
            enemy.Scale = new Vector3(3f, 3f, 3f);

            //this.player             = (EntityPlayer)loader.loadMeshAndAnimationsFromFile(mediaPath + "/SkeletalAnimations/Robot/Robot-TgcSkeletalMesh.xml", new string[] { });
            this.worldMap             = new WorldMap(mediaPath);
            this.hand = loader.loadMeshAndAnimationsFromFile(mediaPath + "/Hand-TgcSkeletalMesh.xml", new string[] { mediaPath + "/HandAnimation-TgcSkeletalAnim.xml" });
            hand.playAnimation("Animation");
            
            hand.Position = new Vector3(0f, 500f, 0f);
            hand.Scale = new Vector3(1 / 6f, 1 / 6f, 1 / 6f);
            hand.rotateX(-(float)Math.PI / 2);



        }

        public TgcCamera Camera
        {
            get { return this.camera; }
        }

        public void update(float elapsedTime)
        {
            foreach(EntityUpdatable currentEntity in this.updatableEntities)
            {
                currentEntity.update();
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
            this.camera.UpdateCamera(elapsedTime);
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
        }

        public void dispose()
        {            
            foreach(IEntity currentEntity in this.entities)
            {
                currentEntity.dispose();
            }
            this.hand.dispose();
            this.enemy.dispose();
            this.worldMap.dispose();
        }
    }
}
