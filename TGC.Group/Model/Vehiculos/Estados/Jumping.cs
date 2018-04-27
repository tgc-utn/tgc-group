using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Vehiculos.Estados
{
    class Jumping : EstadoVehiculo
    {
        private float initialSpeed;

        public Jumping(Vehiculo auto) : base(auto)
        {
            this.initialSpeed = auto.getVelocidadActual();
        }

        public override void advance()
        {
            if(auto.getVelocidadActual() < 0)
            {
                auto.setVelocidadActual(auto.getVelocidadActual() + auto.getConstanteFrenado() * 2);
                if(auto.getVelocidadActual() > 0)
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
            if(auto.getVelocidadActual() > 0)
            {
                auto.setVelocidadActual(auto.getVelocidadActual() - auto.getConstanteFrenado() *2);
                if(auto.getVelocidadActual() < 0)
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
            if(auto.getVelocidadActualDeSalto() < 0)
            {
                auto.setEstado(new Descending(this.auto, this.initialSpeed));
                return;
            }
            float desplazamientoEnY = auto.getVelocidadActualDeSalto() * auto.getElapsedTime();
            TGCVector3 nuevoDesplazamiento = new TGCVector3(0, desplazamientoEnY, 0);
            auto.mesh.Move(nuevoDesplazamiento);
            auto.move(auto.getVectorAdelante() * this.initialSpeed * auto.getElapsedTime());
        }
    }
}
