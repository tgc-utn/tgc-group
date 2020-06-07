using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;

namespace TGC.Group.Model._2D
{
    class HUD
    {
        public enum AnclajeVertical
        {
            TOP,
            CENTER,
            BOTTOM
        }
        public enum AnclajeHorizontal
        {
            LEFT,
            CENTER,
            RIGHT
        }

        protected AnclajeHorizontal anclajeHorizontal;
        protected AnclajeVertical anclajeVertical;
        protected TGCVector2 desplazamiento;
        protected TGCVector2 escala;
        protected Drawer2D drawer2D;
        protected float scalingFactor;

        public HUD(AnclajeHorizontal anclajeHorizontal, AnclajeVertical anclajeVertical, TGCVector2 desplazamiento, TGCVector2 escala, Drawer2D drawer2D)
        {
            this.anclajeHorizontal = anclajeHorizontal;
            this.anclajeVertical = anclajeVertical;
            this.desplazamiento = desplazamiento;
            this.escala = escala;
            this.drawer2D = drawer2D;
            scalingFactor = (float)D3DDevice.Instance.Width / 1920;
        }

        public virtual void Init()
        {

        }

        public void Update() {
            
        }
        public virtual void Render() {
        
        }
    }
}
