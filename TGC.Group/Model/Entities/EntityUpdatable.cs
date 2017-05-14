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
using TGC.Core.SkeletalAnimation;

namespace TGC.Group.Model.Entities
{
    public abstract class EntityUpdatable : TgcSkeletalMesh, IEntity, IUpdateObject
    {        
        public virtual new void render()
        {
            base.render();
            this.UpdateMeshTransform();
        }
        

        public abstract void update(float elapsedTime);
    }
}
