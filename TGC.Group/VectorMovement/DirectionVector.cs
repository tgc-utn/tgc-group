using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Group.Model;

namespace TGC.Group.VectorMovement
{
    class DirectionVector
    {
        public float angulo { get; set; }
        public float anguloRad { get; set; }
        private Calculos calculo = new Calculos();
        public DirectionVector(float unAngulo)
        {
            angulo = unAngulo;
            anguloRad = calculo.AnguloARadianes(unAngulo, 1f);
        }

    }
}
