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
        public override void back()
        {
            base.back();
            move(auto.getVectorAdelante() * auto.getVelocidadActual() * auto.getElapsedTime());
        }

        public override void advance()
        {
            brake(auto.getConstanteFrenado());
        }

        public override void speedUpdate()
        {
            brake(auto.getConstanteRozamiento());
        }

        private void brake(float constante)
        {
            auto.getDeltaTiempoAvance().resetear();
            auto.setVelocidadActual(auto.getVelocidadActual() + constante);
            if (auto.getVelocidadActual() > 0)
            {
                auto.setVelocidadActual(0);
                auto.getDeltaTiempoAvance().resetear();
                auto.setEstado(new Stopped(this.auto));
                return;
            }
            this.move(auto.getVectorAdelante() * auto.getVelocidadActual() * auto.getElapsedTime());
        }
    }
}
