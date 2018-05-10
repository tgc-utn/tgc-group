using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Vehiculos.Estados
{
    class Backward : EstadoVehiculo
    {

        public Backward(Vehiculo auto) : base(auto)
        {

        }
        public override void Back()
        {
            base.Back();
            Move(auto.GetVectorAdelante() * auto.GetVelocidadActual() * auto.GetElapsedTime());
        }

        public override void Advance()
        {
            brake(auto.GetConstanteFrenado());
        }

        public override void SpeedUpdate()
        {
            brake(auto.GetConstanteRozamiento());
        }

        private void brake(float constante)
        {
            auto.GetDeltaTiempoAvance().resetear();
            auto.SetVelocidadActual(auto.GetVelocidadActual() + constante);
            if (auto.GetVelocidadActual() > 0)
            {
                auto.SetVelocidadActual(0);
                auto.GetDeltaTiempoAvance().resetear();
                auto.SetEstado(new Stopped(this.auto));
                return;
            }
            this.Move(auto.GetVectorAdelante() * auto.GetVelocidadActual() * auto.GetElapsedTime());
        }
    }
}
