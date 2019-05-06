using TGC.Core.Collision;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Utils
{
    public class Cube
    {
        public TGCVector3 PMin { get; }
        public TGCVector3 PMax { get; }

        public Cube(TGCVector3 PMin, TGCVector3 PMax)
        {
            this.PMin = PMin;
            this.PMax = PMax;
        }
        
        public bool isIntersectedBy(TgcRay r)
        {
            var tMin = (this.PMin.X - r.Origin.X) / r.Direction.X;
            var tMax = (this.PMax.X - r.Origin.X) / r.Direction.X;
            float aux;
            
            if (tMin > tMax)
            {
                aux = tMin;
                tMin = tMax;
                tMax = aux;
            };
            
            var tyMin = (this.PMin.Y - r.Origin.Y) / r.Direction.Y;
            var tyMax = (this.PMax.Y - r.Origin.Y) / r.Direction.Y;

            if (tyMin > tyMax)
            {
                aux = tyMin;
                tyMin = tyMax;
                tyMax = aux;
            };

            if ((tMin > tyMax) || (tyMin > tMax))
                return false;

            if (tyMin > tMin)
                tMin = tyMin;

            if (tyMax < tMax)
                tMax = tyMax;

            var tzMin = (this.PMin.Z - r.Origin.Z) / r.Direction.Z;
            var tzMax = (this.PMax.Z - r.Origin.Z) / r.Direction.Z;

            if (tzMin > tzMax)
            {
                aux = tzMin;
                tzMin = tzMax;
                tzMax = aux;
            };

            return !(tMin > tzMax) && !(tzMin > tMax);
        } 
    }
}