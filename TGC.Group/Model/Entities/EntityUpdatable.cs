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
    public abstract class EntityUpdatable : IEntity, IUpdateObject
    {
        public virtual bool AlphaBlendEnable
        {
            get
            {
                return true;
            }

            set
            {
                return;
            }
        }
    
        public abstract void dispose();

        public abstract void render();
        

        public abstract void update(float elapsedTime);
    }
}
