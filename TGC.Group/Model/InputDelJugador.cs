using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Input;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    public class InputDelJugador
    {
        private readonly TgcD3dInput input;

        public InputDelJugador(TgcD3dInput input)
        {
            this.input = input;
        }

        //TODO: Cambiar, la rotacion esta acoplada al movimiento, entonces no es necesario 2 metodos.
        public TGCVector3 RotacionDelInput()
        {
            TGCVector3 rotacionDelInput = new TGCVector3();

            if (input.keyDown(Key.Up) || input.keyDown(Key.W))
            {
                rotacionDelInput.X = 1;
            }
            else if (input.keyDown(Key.Down) || input.keyDown(Key.S))
            {
                rotacionDelInput.X = -1;
            }

            if (input.keyDown(Key.Left) || input.keyDown(Key.A))
            {
                rotacionDelInput.Z = 1;
            }
            else if (input.keyDown(Key.Right) || input.keyDown(Key.D))
            {
                rotacionDelInput.Z = -1;
            }

            return rotacionDelInput;
        }

        public Boolean HayInputDeRotacion()
        {
            return !RotacionDelInput().Equals(new TGCVector3(0, 0, 0));
        }

        public TGCVector3 DireccionDelInput()
        {
            TGCVector3 direccionDelInput = new TGCVector3(0, 0, 0);

            if (input.keyDown(Key.Up) || input.keyDown(Key.W))
            {
                direccionDelInput.Y = 1;
            }
            else if (input.keyDown(Key.Down) || input.keyDown(Key.S))
            {
                direccionDelInput.Y = -1;
            }

            if (input.keyDown(Key.Left) || input.keyDown(Key.A))
            {
                direccionDelInput.X = -1;
            }
            else if (input.keyDown(Key.Right) || input.keyDown(Key.D))
            {
                direccionDelInput.X = 1;
            }

            return direccionDelInput;

        }

        public float SentidoDeAceleracionDelInput()
        {
            if (input.keyDown(Key.LeftShift))
            {
                return 1f;
            }
            else if (input.keyDown(Key.LeftControl))
            {
                return -1f;
            }
            else
            {
                return 0f;
            }
        }

        public Boolean HayInputDeAceleracion()
        {
            return !SentidoDeAceleracionDelInput().Equals(0f);
        }

        public Boolean HayInputDePonerseVertical()
        {
            return input.keyDown(Key.E);
        }

        public Boolean HayInputDeRoll()
        {
            return input.keyDown(Key.Q);
        }
        public Boolean HayInputDeDisparo()
        {
            return input.keyDown(Key.Space);
        }
    }
}
