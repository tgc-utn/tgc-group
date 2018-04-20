using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.DirectInput;
using TGC.Group.VectorMovement;


namespace TGC.Group.Model
{
    class Personaje
    {

        private Calculos calculo = new Calculos();

        private DirectionVector angleDir = new DirectionVector(90f);
        private DirectionVector upDir = new UpDirection();
        private DirectionVector downDir = new DownDirection();
        private DirectionVector rightDir = new RightDirection();
        private DirectionVector leftDir = new LeftDirection();
        private DirectionVector downRightDir = new DownRightDirection();
        private DirectionVector downLeftDir = new DownLeftDirection();
        private DirectionVector upRightDir = new UpRightDirection();
        private DirectionVector upLeftDir = new UpLeftDirection();
        private float anguloMovido;


        public float RotationAngle(Key input)
        {
            //Input W
            if(input == Key.W && angleDir.angulo != upDir.angulo)
            {

                anguloMovido = calculo.MovementAngle(angleDir, upDir);
                angleDir.angulo = upDir.angulo;
                angleDir.anguloRad = upDir.anguloRad;
                return anguloMovido;
            }

            //Input S
            if (input == Key.S && angleDir.angulo != downDir.angulo)
            {

                anguloMovido = calculo.MovementAngle(angleDir, downDir);
                angleDir.angulo = downDir.angulo;
                angleDir.anguloRad = downDir.anguloRad;
                return anguloMovido;
            }
            //Input A
            if (input == Key.A && angleDir.angulo != leftDir.angulo)
            {

                anguloMovido =  calculo.MovementAngle(angleDir, leftDir);
                angleDir.angulo = leftDir.angulo;
                angleDir.anguloRad = leftDir.anguloRad;
                return anguloMovido;
            }
            //Input D
            if (input == Key.D && angleDir.angulo != rightDir.angulo)
            {
                anguloMovido = calculo.MovementAngle(angleDir, rightDir);
                angleDir.angulo = rightDir.angulo;
                angleDir.anguloRad = rightDir.anguloRad;
                return anguloMovido;
                
            }
            return 0f;
        }
      




    }
}
