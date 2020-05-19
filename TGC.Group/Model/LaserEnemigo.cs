using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class LaserEnemigo : Colisionable
    {
        private Laser laser;
        public LaserEnemigo(string mediaDir, TGCVector3 posicionInicial, TGCVector3 direccionDisparo, Nave naveDelJugador): base(naveDelJugador)
        {
            string direccionDeScene = mediaDir + "Xwing\\laser-TgcScene.xml";
            laser = new Laser(direccionDeScene,posicionInicial,direccionDisparo);
        }

        internal override void ColisionarConNave()
        {
            if (EstaColisionandoConNave())
            {
                naveDelJugador.ChocarConLaser();
            }
            
        }

        public override void Init()
        {
            laser.Init();
        }

        public override void Dispose()
        {
            laser.Dispose();
        }

        public override void Update(float elapsedTime)
        {
            laser.Update(elapsedTime);
            base.Update(elapsedTime);
        }

        public override void Render()
        {
            
            if (EstaColisionandoConNave())
            {
                GetBoundingBox().setRenderColor(Color.Pink);
            }

            laser.Render();

        }

        public override TgcBoundingAxisAlignBox GetBoundingBox()
        {
            return laser.GetModeloLaser().BoundingBox;
        }

    }
}
