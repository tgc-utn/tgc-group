using System.Collections.Generic;
using TGC.Group.Model.Entities;
using TGC.Group.Model.Camera;
using TGC.Core.Input;
using TGC.Core.Camara;
using TGC.Core.SkeletalAnimation;

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

        public World(string mediaPath, TgcD3dInput inputManager)
        {
            this.entities             = new List<IEntity>();
            this.updatableEntities    = new List<EntityUpdatable>();            
            this.camera               = new FirstPersonCamera(inputManager);
            this.camera.RotationSpeed = 0.01f;
            TgcSkeletalLoader lo      = new TgcSkeletalLoader();
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
            this.camera.UpdateCamera(elapsedTime);
        }

        public void render()
        {
            foreach(IEntity currentEntity in this.entities)
            {
                currentEntity.render();
            }
            this.worldMap.render();
        }

        public void dispose()
        {            
            foreach(IEntity currentEntity in this.entities)
            {
                currentEntity.dispose();
            }
            this.worldMap.dispose();
        }
    }
}
