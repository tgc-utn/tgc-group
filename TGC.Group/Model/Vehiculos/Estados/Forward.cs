using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Group.Model.Vehiculos.Estados;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Vehiculos.Estados
{
    class Forward : EstadoVehiculo
    {
        public Forward(Vehiculo auto) : base(auto)
        {

        }

        public override void Advance()
        {
            base.Advance();
            Move(auto.GetVectorAdelante() * auto.GetVelocidadActual() * auto.GetElapsedTime());
        }

        public override void Back()
        {
            Brake(auto.GetConstanteFrenado());
        }

        public override void SpeedUpdate()
        {
            Brake(auto.GetConstanteRozamiento());
        }

        private void Brake(float constante)
        {
            auto.GetDeltaTiempoAvance().resetear();
            auto.SetVelocidadActual(auto.GetVelocidadActual() - constante);
            if (auto.GetVelocidadActual() < 0)
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
