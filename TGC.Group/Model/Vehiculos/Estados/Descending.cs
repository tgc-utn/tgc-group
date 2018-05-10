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

        public override void Advance()
        {
            if (auto.GetVelocidadActual() < 0)
            {
                auto.SetVelocidadActual(auto.GetVelocidadActual() + auto.GetConstanteFrenado() * 2);
                if (auto.GetVelocidadActual() > 0)
                {
                    auto.SetVelocidadActual(0);
                    auto.GetDeltaTiempoAvance().resetear();
                }
                return;
            }
            base.Advance();

        }

        public override void Back()
        {
            if (auto.GetVelocidadActual() > 0)
            {
                auto.SetVelocidadActual(auto.GetVelocidadActual() - auto.GetConstanteFrenado() * 2);
                if (auto.GetVelocidadActual() < 0)
                {
                    auto.SetVelocidadActual(0);
                    auto.GetDeltaTiempoAvance().resetear();
                }
                return;
            }
            base.Back();
        }

        public override void Jump()
        {
            return;
        }

        public override void JumpUpdate()
        {
            auto.SetVelocidadActualDeSalto(this.VelocidadFisicaDeSalto());
            float desplazamientoEnY = auto.GetVelocidadActualDeSalto() * auto.GetElapsedTime();
            desplazamientoEnY = (TGCVector3.transform(auto.GetPosicionCero(), auto.transformacion).Y + desplazamientoEnY < 0) ? -TGCVector3.transform(auto.GetPosicionCero(), auto.transformacion).Y : desplazamientoEnY;
            TGCVector3 nuevoDesplazamiento = new TGCVector3(0, desplazamientoEnY, 0);
            //this.move(nuevoDesplazamiento);
            //this.move(auto.getVectorAdelante() * this.initialSpeed * auto.getElapsedTime());
            this.Move(nuevoDesplazamiento + auto.GetVectorAdelante() * this.initialSpeed * auto.GetElapsedTime());
            if(TGCVector3.transform(auto.GetPosicionCero(), auto.transformacion).Y == 0)
            {
                auto.GetDeltaTiempoSalto().resetear();
                auto.SetVelocidadActualDeSalto(auto.GetVelocidadMaximaDeSalto());
                if(auto.GetVelocidadActual() > 0)
                {
                    auto.SetEstado(new Forward(this.auto));
                }
                else if(auto.GetVelocidadActual() < 0)
                {
                    auto.SetEstado(new Backward(this.auto));
                }
                else
                {
                    auto.SetEstado(new Stopped(this.auto));
                }
            }
        }

        public override void Right(CamaraEnTerceraPersona camara)
        {
            //TODO mover ruedas;
        }

        public override void Left(CamaraEnTerceraPersona camara)
        {
            //TODO mover ruedas;
        }
    }
}
