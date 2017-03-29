using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Geometry;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;

namespace TGC.Group.Model.Entities
{
    public class Player : EntityEntity
    {

        public override void update()
        {
            this.rotateY((float)Math.PI / 1440);
        }
        

    }
}
