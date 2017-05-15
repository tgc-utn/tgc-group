using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Entities
{
    class EntityPlayerItemLamp : EntityPlayerItem
    {
        public EntityPlayerItemLamp(List<TgcMesh> meshes) : base(meshes)
        {

        }

        public override float maxDuration()
        {
            return 600f;
        }
    }
}
