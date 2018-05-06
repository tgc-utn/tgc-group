using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Vehiculos.Estados
{
    class Descending : EstadoVehiculo
    {
        private float initialSpeed;

        public Descending(Vehiculo auto, float initialSpeed) : base(auto)
        {
            this.initialSpeed = initialSpeed;
        }

        public override void advance()
        {
            if (auto.getVelocidadActual() < 0)
            {
                auto.setVelocidadActual(auto.getVelocidadActual() + auto.getConstanteFrenado() * 2);
                if (auto.getVelocidadActual() > 0)
                {
                    auto.setVelocidadActual(0);
                    auto.getDeltaTiempoAvance().resetear();
                }
                return;
            }
            base.advance();

        }

        public override void back()
        {
            if (auto.getVelocidadActual() > 0)
            {
                auto.setVelocidadActual(auto.getVelocidadActual() - auto.getConstanteFrenado() * 2);
                if (auto.getVelocidadActual() < 0)
                {
                    auto.setVelocidadActual(0);
                    auto.getDeltaTiempoAvance().resetear();
                }
                return;
            }
            base.back();
        }

        public override void jump()
        {
            return;
        }

        public override void jumpUpdate()
        {
            auto.setVelocidadActualDeSalto(this.velocidadFisicaDeSalto());
            float desplazamientoEnY = auto.getVelocidadActualDeSalto() * auto.getElapsedTime();
            desplazamientoEnY = (TGCVector3.transform(auto.posicion(), auto.transformacion).Y + desplazamientoEnY < 0) ? -TGCVector3.transform(auto.posicion(), auto.transformacion).Y : desplazamientoEnY;
            TGCVector3 nuevoDesplazamiento = new TGCVector3(0, desplazamientoEnY, 0);
            //this.move(nuevoDesplazamiento);
            //this.move(auto.getVectorAdelante() * this.initialSpeed * auto.getElapsedTime());
            this.move(nuevoDesplazamiento + auto.getVectorAdelante() * this.initialSpeed * auto.getElapsedTime());
            if(TGCVector3.transform(auto.posicion(), auto.transformacion).Y == 0)
            {
                auto.getDeltaTiempoSalto().resetear();
                auto.setVelocidadActualDeSalto(auto.getVelocidadMaximaDeSalto());
                if(auto.getVelocidadActual() > 0)
                {
                    auto.setEstado(new Forward(this.auto));
                }
                else if(auto.getVelocidadActual() < 0)
                {
                    auto.setEstado(new Backward(this.auto));
                }
                else
                {
                    auto.setEstado(new Stopped(this.auto));
                }
            }
        }

        public override void right(CamaraEnTerceraPersona camara)
        {
            //TODO mover ruedas;
        }

        public override void left(CamaraEnTerceraPersona camara)
        {
            //TODO mover ruedas;
        }
    }
}
