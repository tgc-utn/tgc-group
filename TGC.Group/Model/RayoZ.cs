using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    public class RayoZ : Rayo
    {

        public RayoZ(TGCVector3 centroCara, TGCVector3 direccion)
            : base(centroCara, direccion)
        {

        }

        protected override int DistanciaMinima()
        {
            return 10;
        }

        protected override float Intervalo()
        {
            return this.puntoInterseccion.Z - this.origen.Z;
        }
    }
}