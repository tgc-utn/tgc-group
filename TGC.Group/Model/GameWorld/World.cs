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
using Microsoft.DirectX.DirectInput;
using TGC.Core.Shaders;
using TGC.Core.Text;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.GameWorld
{
    public class World
    {
        protected List<IEntity>          entities;
        protected List<EntityUpdatable>  updatableEntities;
        protected EntityPlayer           player;
        protected WorldMap               worldMap;
        protected TgcD3dInput            inputManager;     
        protected bool                   outsideCamera;
        protected TgcCamera              camera;
        protected Microsoft.DirectX.Direct3D.Effect currentShader;

        public World(string mediaPath, TgcD3dInput inputManager)
        {
            TgcSkeletalLoader loader = new TgcSkeletalLoader();
            this.inputManager        = inputManager;
            this.camera = new FirstPersonCamera(inputManager);

            TgcSceneLoader sceneLoader = new TgcSceneLoader();

            List<EntityPlayerItem> items = new List<EntityPlayerItem>();
            items.Add(new EntityPlayerItemCandle(sceneLoader.loadSceneFromFile(mediaPath + "/Candle-TgcScene.xml").Meshes));
            items.Add(new EntityPlayerItemFlashLight(sceneLoader.loadSceneFromFile(mediaPath + "/Flashlight-TgcScene.xml").Meshes));
            items.Add(new EntityPlayerItemLamp(sceneLoader.loadSceneFromFile(mediaPath + "/Lamp-TgcScene.xml").Meshes));

            EntityPlayer player      = new EntityPlayer(this.inputManager, loader, mediaPath, items);
            EntityMonster monster    = new EntityMonster(loader, mediaPath);
            this.player = player;

            //this.currentShader        = TgcShaders.Instance.TgcMeshSpotLightShader;
            this.outsideCamera        = false;            
            this.entities             = new List<IEntity>();
            this.updatableEntities    = new List<EntityUpdatable>();
            this.worldMap             = new WorldMap(mediaPath);

            List<TgcMesh> itemMeshes = this.worldMap.Items;                        
            List<EntityItem> itemEntities = itemMeshes.ConvertAll(i => { return new EntityItem(i); });
                        
            this.entities.AddRange(itemEntities);
            this.updatableEntities.AddRange(itemEntities);
            
            player.Colliders = this.worldMap.Collidables;
            monster.WalkingNodes = this.worldMap.EnemyIA;
			monster.IntersectionPoints = this.worldMap.IntersectionPoints;
			monster.playerColider(player.BoundingBox);
			monster.WalkingNodes = this.worldMap.EnemyIA;

			this.entities.Add(player);
            this.entities.Add(monster);
            this.updatableEntities.Add(player);
            this.updatableEntities.Add(monster);
        }

        public TgcCamera Camera
        {
            get { return (this.outsideCamera) ? this.camera : this.player.Camera; }
        }

        private bool freeCamera = false;


        public void update(float elapsedTime)
        {
            foreach (EntityUpdatable currentEntity in this.updatableEntities)
            {
                currentEntity.update(elapsedTime);
            }

            if (inputManager.keyPressed(Microsoft.DirectX.DirectInput.Key.C))
            {
                this.freeCamera = !this.freeCamera;
                this.outsideCamera = freeCamera;
                this.worldMap.ShouldShowRoof = !freeCamera;
                this.worldMap.ShouldShowBoundingVolumes = freeCamera;
            }
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
        /*
        protected void createShaders()
        {
            
            //Aplicar a cada mesh el shader actual
            foreach (var mesh in scene.Meshes)
            {
                mesh.Effect = currentShader;
                //El Technique depende del tipo RenderType del mesh
                mesh.Technique = TgcShaders.Instance.getTgcMeshTechnique(mesh.RenderType);
            }
        }*/
    }
}
