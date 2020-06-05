using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class ObstaculoMapa : Colisionable
    {
        //private TgcBoundingAxisAlignBox boundingBox;
        public ObstaculoMapa(Nave naveDelJugador, TgcMesh mesh) : base(naveDelJugador)
        {
            this.mainMesh = mesh;
        }

        public override void Init()
        {

        }

        public override void Render()
        {
            //mainMesh.Render();
            mainMesh.BoundingBox.Render();
        }

        public override void Dispose()
        {
            //Hace dispose en el bloque(scene)
            //mainMesh.Dispose();
        }
        /*
        public override TgcBoundingAxisAlignBox GetBoundingBox()
        {
            return this.boundingBox;
        }
        */
    }
}
