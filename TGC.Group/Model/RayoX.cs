using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    public class RayoX : Rayo
    {
        private int distanciaMinima;

        public RayoX(TGCVector3 centroCara, TGCVector3 direccion)
            : base (centroCara, direccion)
        {
         
        }

        protected override int DistanciaMinima() {
            return 2;
        }

        protected override float Intervalo()
        {
            return this.puntoInterseccion.X - this.origen.X;
        }
    }
}