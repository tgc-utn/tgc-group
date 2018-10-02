using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    public class RayoY : Rayo
    {

        public RayoY(TGCVector3 centroCara, TGCVector3 direccion)
            : base(centroCara, direccion)
        {

        }

        public override int DistanciaMinima()
        {
            return 2;
        }

        protected override float Intervalo()
        {
            return this.puntoInterseccion.Y - this.origen.Y;
        }
    }
}