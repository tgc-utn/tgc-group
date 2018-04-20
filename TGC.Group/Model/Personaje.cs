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

        private DirectionAngle angleDir = new DirectionAngle(90f);
        private DirectionAngle upDir = new UpDirection();
        private DirectionAngle downDir = new DownDirection();
        private DirectionAngle rightDir = new RightDirection();
        private DirectionAngle leftDir = new LeftDirection();
        private DirectionAngle downRightDir = new DownRightDirection();
        private DirectionAngle downLeftDir = new DownLeftDirection();
        private DirectionAngle upRightDir = new UpRightDirection();
        private DirectionAngle upLeftDir = new UpLeftDirection();
        private float anguloMovido;


       

        //Para 1 tecla
        private bool validateMovement(Key i, Key valida, DirectionAngle validationAngle)
        {
            return i == valida && angleDir.angulo != validationAngle.angulo;
        }
        public float RotationAngle(Key input)
        {
            //Input W
            if (validateMovement(input, Key.W, upDir)) return setAngle(upDir);
            //Input S
            if (validateMovement(input, Key.S, downDir)) return setAngle(downDir);
            //Input A
            if (validateMovement(input, Key.A, leftDir)) return setAngle(leftDir);
            //Input D
            if (validateMovement(input, Key.D, rightDir)) return setAngle(rightDir);
            return 0f;
        }

        //Para 2 teclas
        private bool validateMovement(Key i1, Key i2, Key valida1, Key valida2, DirectionAngle validationAngle)
        {
            return (i1 == valida1 && i2 == valida2 || i1 == valida2 && i2 == valida1) && angleDir.angulo != validationAngle.angulo;
        }
        public float RotationAngle(Key input1, Key input2)
        {

            //Input W-D
            if (validateMovement(input1, input2, Key.W, Key.D, upRightDir)) return setAngle(upRightDir);
            //Input W-A
            if (validateMovement(input1, input2, Key.W, Key.A, upLeftDir)) return setAngle(upLeftDir);
            //Input S-D
            if (validateMovement(input1, input2, Key.S, Key.D, downRightDir)) return setAngle(downRightDir);
            //Input S-A
            if (validateMovement(input1, input2, Key.S, Key.A, downLeftDir)) return setAngle(downLeftDir);
            return 0f;
        }
        //Actualiza ángulo director del personaje
        private float setAngle(DirectionAngle d1)
        {
            anguloMovido = calculo.MovementAngle(angleDir, d1);
            angleDir.angulo = d1.angulo;
            angleDir.anguloRad = d1.anguloRad;
            return anguloMovido;

        }

        



    }
}
