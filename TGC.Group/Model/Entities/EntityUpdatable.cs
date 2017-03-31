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

namespace TGC.Group.Model.Entities
{
    public abstract class EntityUpdatable : IEntity, IUpdateObject
    {
        public virtual void render()
        {
            this.mesh.UpdateMeshTransform();
        }
    }
}
