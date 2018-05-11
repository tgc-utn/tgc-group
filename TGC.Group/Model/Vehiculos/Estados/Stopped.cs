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

        override public void Advance()
        {
            base.Advance();
            this.Move(auto.GetVectorAdelante() * auto.GetVelocidadActual() * auto.GetElapsedTime());
            auto.SetEstado(new Forward(this.auto));
        }

        override public void Back()
        {
            base.Back();
            this.Move(auto.GetVectorAdelante() * auto.GetVelocidadActual() * auto.GetElapsedTime());
            auto.SetEstado(new Backward(this.auto));
        }

        override public void Left(CamaraEnTerceraPersona camara)
        {
            float rotacionReal = -auto.GetVelocidadDeRotacion() * auto.GetElapsedTime();
        }

        override public void Right(CamaraEnTerceraPersona camara)
        {
            float rotacionReal = auto.GetVelocidadDeRotacion() * auto.GetElapsedTime();
        }

    }
}
