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

namespace TGC.Group.Model.GameWorld
{
    public class World
    {
        protected List<IEntity>          entities;
        protected List<EntityUpdatable>  updatableEntities;
        protected EntityPlayer           player;
        protected WorldMap               worldMap;
        protected TgcD3dInput            inputManager;
     //   protected TgcSkeletalMesh        enemy;        
        protected bool                   outsideCamera;
        protected TgcCamera              camera;
        protected Monster monster;
        protected Microsoft.DirectX.Direct3D.Effect currentShader;

        public World(string mediaPath, TgcD3dInput inputManager)
        {
            TgcSkeletalLoader loader = new TgcSkeletalLoader();            
            this.currentShader        = TgcShaders.Instance.TgcMeshSpotLightShader;
            this.outsideCamera        = false;
            this.inputManager         = inputManager;
            this.entities             = new List<IEntity>();
            this.updatableEntities    = new List<EntityUpdatable>();
            this.worldMap             = new WorldMap(mediaPath);
            this.player               = new EntityPlayer(this.inputManager, loader, mediaPath);            
            this.camera               = new TgcCamera();
            this.camera.SetCamera(new Vector3(0f, 1000f, 0f), new Vector3(10f, 0f, 10f));
            


            
            var enemy                     = loader.loadMeshAndAnimationsFromFile(mediaPath + "/Monster-TgcSkeletalMesh.xml", new string[]{mediaPath + "/Run-TgcSkeletalAnim.xml"});
            enemy.playAnimation("Run");
            enemy.Position            = new Vector3(0f, 0f,0f);
            enemy.Scale = new Vector3(3f, 3f, 3f);

            this.monster = new GameWorld.Monster(mediaPath);
            


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
                this.worldMap.ShouldShowRoof = false;
                this.worldMap.ShouldShowBoundingVolumes = true;
            }
            else
            {
                this.outsideCamera = false;
                this.worldMap.ShouldShowRoof = true;
                this.worldMap.ShouldShowBoundingVolumes = false;

            }
            
            // this.enemy.UpdateMeshTransform();
            // this.enemy.updateAnimation(elapsedTime);
            this.monster.update(this.worldMap.EnemyIA, elapsedTime);
            this.player.Colliders = this.worldMap.Walls;        
            this.player.update(elapsedTime);

        }

        public void render()
        {
            foreach(IEntity currentEntity in this.entities)
            {
                currentEntity.render();
            }
            
            this.monster.render();
            this.worldMap.render();            
            this.player.render();
        }

        public void dispose()
        {            
            foreach(IEntity currentEntity in this.entities)
            {
                currentEntity.dispose();
            }
            this.player.dispose();
            this.monster.dispose();
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
