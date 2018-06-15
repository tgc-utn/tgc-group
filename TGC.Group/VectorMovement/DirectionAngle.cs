using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Group.Model;
using TGC.Core.Mathematica;
namespace TGC.Group.VectorMovement
{
    class DirectionAngle
    {
        public float angulo { get; set; }
        public float anguloRad { get; set; }
        private Calculos calculo = new Calculos();
        public DirectionAngle(float unAngulo)
        {
            angulo = unAngulo;
            anguloRad = calculo.AnguloARadianes(unAngulo, 1f);
        }

        public void setAngulo(float radAngle)
        {
            anguloRad = radAngle;
            angulo = FastMath.ToDeg(radAngle);
        }


    }
}
