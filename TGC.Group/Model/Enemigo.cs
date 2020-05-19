using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model
{
    public abstract class Enemigo : IRenderizable
    {

        public abstract void Init();

        public abstract void Render();

        public virtual void Update(float elapsedTime)
        {
            if (LePegaUnLaser())
            {
                Destruirse();
            }
        }

        public virtual void Dispose() 
        {
            GameManager.Instance.QuitarRenderizable(this);
        }

        private void Destruirse()
        {
            Dispose();
        }

        private Boolean LePegaUnLaser()
        {
           return GameManager.Instance.HayUnLaserDeJugadorEnBoundingBox(GetBoundingBox());
        }

        public abstract TgcBoundingAxisAlignBox GetBoundingBox();
    }
}
