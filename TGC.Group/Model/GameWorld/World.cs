using System.Collections.Generic;
using TGC.Group.Model.Entities;
using TGC.Group.Model.Camera;
using TGC.Core.Input;
using TGC.Core.Camara;
using TGC.Core.SkeletalAnimation;
using Microsoft.DirectX;

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

        public World(string mediaPath, TgcD3dInput inputManager)
        {
            this.entities             = new List<IEntity>();
            this.updatableEntities    = new List<EntityUpdatable>();            
            this.camera               = new FirstPersonCamera(inputManager);
            this.camera.RotationSpeed = 0.01f;
            TgcSkeletalLoader loader  = new TgcSkeletalLoader();
            enemy                     = loader.loadMeshAndAnimationsFromFile(mediaPath + "/Monster-TgcSkeletalMesh.xml", new string[]{mediaPath + "/Run-TgcSkeletalAnim.xml"});
            enemy.playAnimation("Run");
            enemy.Position            = new Vector3(0f, 0f,0f);
            enemy.Scale = new Vector3(2.5f, 2.5f, 2.5f);
            
            //this.player             = (EntityPlayer)lo.loadMeshAndAnimationsFromFile(mediaPath + "/SkeletalAnimations/Robot/Robot-TgcSkeletalMesh.xml", new string[] { });
            this.worldMap             = new WorldMap(mediaPath);

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
        }

        public void dispose()
        {            
            foreach(IEntity currentEntity in this.entities)
            {
                currentEntity.dispose();
            }
            this.enemy.dispose();
            this.worldMap.dispose();
        }
    }
}
