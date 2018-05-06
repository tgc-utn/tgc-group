using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Group.Model.Vehiculos.Estados;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Vehiculos.Estados
{
    class Stopped : EstadoVehiculo
    {
        public Stopped(Vehiculo auto) : base(auto)
        {

        }

        override public void advance()
        {
            base.advance();
            this.move(auto.getVectorAdelante() * auto.getVelocidadActual() * auto.getElapsedTime());
            auto.setEstado(new Forward(this.auto));
        }

        override public void back()
        {
            base.back();
            this.move(auto.getVectorAdelante() * auto.getVelocidadActual() * auto.getElapsedTime());
            auto.setEstado(new Backward(this.auto));
        }

        override public void left(CamaraEnTerceraPersona camara)
        {
            float rotacionReal = -auto.getVelocidadDeRotacion() * auto.getElapsedTime();
            auto.RotarDelanteras(rotacionReal);
        }

        override public void right(CamaraEnTerceraPersona camara)
        {
            float rotacionReal = auto.getVelocidadDeRotacion() * auto.getElapsedTime();
            auto.RotarDelanteras(rotacionReal);
        }

    }
}
