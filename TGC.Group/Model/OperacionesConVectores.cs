using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class OperacionesConVectores
    {
        public static TGCVector3 minimaAlturaEntreVectores(TGCVector3 vector1, TGCVector3 vector2)
        {
            return vector1.Y < vector2.Y ? vector1 : vector2;
        }

        public static TGCVector3 maximaAlturaEntreVectores(TGCVector3 vector1, TGCVector3 vector2)
        {
            return vector1.Y > vector2.Y ? vector1 : vector2;
        }

        public static TGCVector3 sumaDeVectores(TGCVector3 vector1, TGCVector3 vector2)
        {
            return new TGCVector3(vector1.X + vector2.X, vector1.Y + vector2.Y, vector1.Z + vector2.Z);
        }


    }
}
