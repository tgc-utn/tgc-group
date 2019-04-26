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
    }
}