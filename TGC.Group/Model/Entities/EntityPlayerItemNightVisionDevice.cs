using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Entities
{
    class EntityPlayerItemNightVisionDevice : EntityPlayerItem
    {
        public EntityPlayerItemNightVisionDevice(List<TgcMesh> meshes) : base(meshes)
        {

        }

        public override void updateItemDuration(float elapsedTime){}
    }
}
