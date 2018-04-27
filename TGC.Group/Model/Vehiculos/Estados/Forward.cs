﻿using System;
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

        public override void advance()
        {
            base.advance();
            move();
        }

        public override void back()
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
            auto.setVelocidadActual(auto.getVelocidadActual() - constante);
            if (auto.getVelocidadActual() < 0)
            {
                auto.setVelocidadActual(0);
                auto.getDeltaTiempoAvance().resetear();
                auto.setEstado(new Stopped(this.auto));
                return;
            }
            auto.mesh.Move(auto.getVectorAdelante() * auto.getVelocidadActual() * auto.getElapsedTime());
        }
    }
}
