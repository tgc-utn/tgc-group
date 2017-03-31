using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Geometry;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using Microsoft.DirectX;
using System.Windows.Forms;

namespace TGC.Group.Model.World
{
    public class World
    {
        protected List<IEntity>          entities;
        protected List<EntityUpdatable>  updatableEntities;
        protected EntityPlayer           player;
        protected WorldMap               worldMap;

        public World()
        {
            this.entities          = new List<IEntity>();
            this.updatableEntities = new List<IEntity>();
            this.entities          = new List<IEntity>();
            Camara                 = new FirstPersonCamera();
        }

        public void update()
        {
            foreach(EntityUpdatable currentEntity in this.updatableEntities)
            {
                currentEntity.update();
            }
            Camara.update(this.player);
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
