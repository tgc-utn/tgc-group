using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class AnimacionGol
    {
        public Boolean Activo { get; set; }
        public float time = 0;
        private const float DURACION_ANIMACION = 5;
        public AnimacionGol()
        {
            Activo = false;
        }

        public void Update(float ElapsedTime)
        {
            time += ElapsedTime;
            if (time >= DURACION_ANIMACION)
            {
                time = 0;
                Activo = false;
            }
        }

        public void AnimarGol(List<Jugador> objetos, TGCVector3 centro)
        {
            if (!Activo)
            {
                Activo = true;
                foreach (var objeto in objetos)
                {
                    var direccion = objeto.Translation - centro;
                    var fuerza = TGCVector3.Normalize(direccion) * (1000000f / direccion.Length());
                    objeto.Cuerpo.ApplyCentralForce(fuerza.ToBulletVector3());
                }
            }

        }

    }
}
