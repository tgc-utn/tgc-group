﻿using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.SkeletalAnimation;
using TGC.Group.VectorMovement;



namespace TGC.Group.Modelo
{
    class Calculos
    {

       


        public float AnguloEntreVectores(TGCVector3 v1, TGCVector3 v2)
        {

            var angle = FastMath.Acos(TGCVector3.Dot(TGCVector3.Normalize(v1), TGCVector3.Normalize(v2)));
            return angle;
        }

        public float AnguloARadianes(float angulo, float timeLapse)
        {
            return ((angulo * 3.14159265f) / 180f) * timeLapse;
        }

        public bool VectoresParalelos(TGCVector3 v1, TGCVector3 v2)
        {

            var constPar = v1.X / v2.X;

            return v1.Y / v2.Y == constPar && v1.Z / v2.Z == constPar;
        }

        public float MovementAngle(DirectionAngle anguloActual, DirectionAngle nuevoAngulo)
        {
            return nuevoAngulo.anguloRad - anguloActual.anguloRad;
        }
    }
}
