using System;
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

        public TGCVector2 Size { get { return getSize(); } }

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

        public virtual void Init(){}

        protected virtual TGCVector2 getSize()
        {
            throw new NotImplementedException();
        }

        protected TGCVector2 trasladar()
        {
            TGCVector2 size = getSize();
            TGCVector2 pos = new TGCVector2();
            switch (anclajeHorizontal)
            {
                case AnclajeHorizontal.LEFT:
                    pos.X = D3DDevice.Instance.Width * desplazamiento.X;
                    break;
                case AnclajeHorizontal.CENTER:
                    pos.X = D3DDevice.Instance.Width / 2 - size.X / 2;
                    break;
                case AnclajeHorizontal.RIGHT:
                    pos.X = D3DDevice.Instance.Width - D3DDevice.Instance.Width * desplazamiento.X - size.X;
                    break;
            }
            switch (anclajeVertical)
            {
                case AnclajeVertical.TOP:
                    pos.Y = D3DDevice.Instance.Height * desplazamiento.Y;
                    break;
                case AnclajeVertical.CENTER:
                    pos.Y = D3DDevice.Instance.Height / 2 - size.Y;
                    break;
                case AnclajeVertical.BOTTOM:
                    pos.Y = D3DDevice.Instance.Height - D3DDevice.Instance.Height * desplazamiento.Y - size.Y;
                    break;
            }
            return pos;
        }

        public virtual void Render(){}

        public virtual void Dispose()
        {

        }
    }
}
