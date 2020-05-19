using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model
{
    public abstract class Destruible : Colisionable
    {

        public Destruible(Nave naveDelJugador) : base(naveDelJugador)
        {

        }

        public override void Update(float elapsedTime)
        {
            if (LePegaUnLaser())
            {
                Destruirse();
            }
        }

        public override void Dispose() 
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

        public new abstract TgcBoundingAxisAlignBox GetBoundingBox();
    }
}
