using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TGC.Core.SceneLoader;
using TGC.Core.Utils;

namespace TGC.Group.Model.Entities
{
    public abstract class EntityPlayerItem : IUpdateObject
    {        
        protected static bool  firstUpdate = true;

        protected List<TgcMesh> meshes;
        protected int updatesToSkip = 30;
        protected float duration;
        protected delegate void UpdateDelegate(float elapsedTime);
        protected UpdateDelegate updateFunction;
        


        public EntityPlayerItem(List<TgcMesh> meshes)
        {
            this.meshes = meshes;
            this.meshes.ForEach(m => { m.rotateX(-FastMath.PI_HALF); });
            this.meshes.ForEach(m => { m.move(0f, 50f, 0f); });
            this.duration = this.maxDuration();
            this.updateFunction = new UpdateDelegate(skipFirstNUpdates);
        }

        public virtual float maxDuration() { return 500f; }

        public void skipFirstNUpdates(float elapsedTime)
        {
            if(this.updatesToSkip > 0)
            {
                this.updatesToSkip--;
                return;
            }
            this.updateFunction = new UpdateDelegate(updateItemDuration);            
        }

        public virtual void updateItemDuration(float elapsedTime)
        {
            this.duration -= elapsedTime * 40;
            if (this.duration < 0)
            {
                this.duration = 0;                
            }
        }

        public bool isLit
        {
            get { return this.duration > 0; }
        }

        public void update(float elapsedTime)
        {
            this.updateFunction(elapsedTime);
        }

        public void render()
        {
            foreach(TgcMesh currentMesh in this.meshes)
            {
                currentMesh.render();
            }
        }

        public void dispose()
        {
            foreach (TgcMesh currentMesh in this.meshes)
            {
                currentMesh.dispose();
            }
        }
    }
}
