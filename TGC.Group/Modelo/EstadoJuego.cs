using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Modelo
{
    public class EstadoJuego
    {
        
        public bool partidaPausada { get; set; } = false;
        public bool partidaPerdida { get; set; } = false;
        public bool menu { get; set; } = true;
        public bool godMode { get; set; } = false;
        

        public EstadoJuego()
        {

        }

        public void reiniciar()
        {
            partidaPausada = false;
            partidaPerdida = false;
            menu = false;
            godMode = false;
        }
    }
}
