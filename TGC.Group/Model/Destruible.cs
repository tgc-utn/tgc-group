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
            mainMesh.Dispose();
        }

        private void Destruirse()
        {
            GameManager.Instance.QuitarRenderizable(this);
            GameManager.Instance.LaserQueColisiona(GetBoundingBox()).ImpactoUnDestruible();
        }

        private Boolean LePegaUnLaser()
        {
           return GameManager.Instance.HayUnLaserEnBoundingBox(GetBoundingBox());
        }

        public new abstract TgcBoundingAxisAlignBox GetBoundingBox();
    }
}
