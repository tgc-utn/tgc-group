using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    public class RayoZ : Rayo
    {

        public RayoZ(TGCVector3 centroCara, TGCVector3 direccion)
            : base(centroCara, direccion)
        {

        }

        public override int DistanciaMinima()
        {
            return 4;
        }

        protected override float Intervalo()
        {
            return this.puntoInterseccion.Z - this.origen.Z;
        }
    }
}