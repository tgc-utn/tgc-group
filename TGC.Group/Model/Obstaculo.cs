using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    public class Obstaculo : Destruible
    {
        private string direccionDelScene;

        public Obstaculo(string direccionDelScene, Nave naveDelJugador) : base(naveDelJugador)
        {
            this.direccionDelScene = direccionDelScene;
        }

        public override void Init()
        {
        }

        public override void Render()
        {
            mainMesh.Render();
        }

        public override TgcBoundingAxisAlignBox GetBoundingBox()
        {
            return mainMesh.BoundingBox;
        }

    }
}
