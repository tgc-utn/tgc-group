using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Direction_Vectors
{
    class DirectionVector
    {
        public float angulo;
        private Calculos calculo = new Calculos();
        public DirectionVector(float unAngulo)
        {
            angulo = calculo.AnguloARadianes(unAngulo, 1f);
        }

        public float getAngulo()
        {
            return angulo;
        }
    }
}
