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
            this.initialSpeed = auto.GetVelocidadActual();
        }

        public override void Advance()
        {
            if(auto.GetVelocidadActual() < 0)
            {
                auto.SetVelocidadActual(auto.GetVelocidadActual() + auto.GetConstanteFrenado() * 2);
                if(auto.GetVelocidadActual() > 0)
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
            if(auto.GetVelocidadActual() > 0)
            {
                auto.SetVelocidadActual(auto.GetVelocidadActual() - auto.GetConstanteFrenado() *2);
                if(auto.GetVelocidadActual() < 0)
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
            if(auto.GetVelocidadActualDeSalto() < 0)
            {
                auto.SetEstado(new Descending(this.auto, this.initialSpeed));
                return;
            }
            float desplazamientoEnY = auto.GetVelocidadActualDeSalto() * auto.GetElapsedTime();
            TGCVector3 nuevoDesplazamiento = new TGCVector3(0, desplazamientoEnY, 0);
            //auto.mesh.Move(nuevoDesplazamiento);
            //auto.mesh.Move(auto.getVectorAdelante() * this.initialSpeed * auto.getElapsedTime());
            this.Move(nuevoDesplazamiento + auto.GetVectorAdelante() * this.initialSpeed * auto.GetElapsedTime());
        }

        public override void Left(CamaraEnTerceraPersona camara)
        {
            //TODO mover ruedas solamente;
        }

        public override void Right(CamaraEnTerceraPersona camara)
        {
            //TODO mover ruedas solamente;
        }

    }
}
