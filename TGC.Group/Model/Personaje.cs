using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.DirectInput;

namespace TGC.Group.Model
{
    class Personaje
    {

        private Calculos calculo = new Calculos();
        private Dictionary<String, bool> direccionPersonaje { get; set; } = new Dictionary<string, bool>();

        public void CreateDictionary()
        {
            direccionPersonaje.Add("facingForward", true);
            direccionPersonaje.Add("facingBackwards", false);
            direccionPersonaje.Add("facingRight", false);
            direccionPersonaje.Add("facingLeft", false);
        }
        public float RotationAngle(Key input)
        {

            //Input W
            if(input == Key.W && !direccionPersonaje["facingForward"])
            {
                if (direccionPersonaje["facingBackwards"]) return calculo.AnguloARadianes(180f, 1f);
                if (direccionPersonaje["facingRight"]) return calculo.AnguloARadianes(-90f, 1f);
                if (direccionPersonaje["facingLeft"]) return calculo.AnguloARadianes(90f, 1f);
            }

            //Input S
            if(input == Key.S && !direccionPersonaje["facingBackwards"])
            {
                if (direccionPersonaje["facingForward"]) return calculo.AnguloARadianes(180f, 1f);
                if (direccionPersonaje["facingRight"]) return calculo.AnguloARadianes(90f, 1f);
                if (direccionPersonaje["facingLeft"]) return calculo.AnguloARadianes(-90f, 1f);
            }

            //Input A
            if(input == Key.A && !direccionPersonaje["facingLeft"])
            {
                if (direccionPersonaje["facingForward"]) return calculo.AnguloARadianes(-90f, 1f);
                if (direccionPersonaje["facingRight"]) return calculo.AnguloARadianes(180f, 1f);
                if (direccionPersonaje["facingBackwards"]) return calculo.AnguloARadianes(90f, 1f);
            }
            //Input D
            if(input == Key.D && !direccionPersonaje["facingRight"])
            {
                if (direccionPersonaje["facingForward"]) return calculo.AnguloARadianes(90f, 1f);
                if (direccionPersonaje["facingLeft"]) return calculo.AnguloARadianes(180f, 1f);
                if (direccionPersonaje["facingBackwards"]) return calculo.AnguloARadianes(-90f, 1f);
            }
            return 0f;
        }

        public void turnForward()
        {

            direccionPersonaje["facingForward"] = true;
            direccionPersonaje["facingBackwards"] = false;
            direccionPersonaje["facingRight"] = false;
            direccionPersonaje["facingLeft"] = false;
        }

        public void turnBackwards()
        {
            direccionPersonaje["facingForward"] = false;
            direccionPersonaje["facingBackwards"] = true;
            direccionPersonaje["facingRight"] = false;
            direccionPersonaje["facingLeft"] = false;

        }

        public void turnRight()
        {
            direccionPersonaje ["facingForward"] = false;
            direccionPersonaje ["facingBackwards"] = false;
            direccionPersonaje ["facingRight"] = true;
            direccionPersonaje ["facingLeft"] = false;

        }

        public void turnLeft()
        {
            direccionPersonaje["facingForward"] = false;
            direccionPersonaje["facingBackwards"] = false;
            direccionPersonaje["facingRight"] = false;
            direccionPersonaje["facingLeft"] = true;

        }




    }
}
