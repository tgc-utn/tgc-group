using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Vehiculos.Estados
{
    abstract class EstadoVehiculo
    {
        protected Vehiculo auto;

        public EstadoVehiculo(Vehiculo auto)
        {
            this.auto = auto;
        }

        virtual public void advance()
        {
            auto.getDeltaTiempoAvance().acumularTiempo(auto.getElapsedTime());
            auto.setVelocidadActual(auto.velocidadFisica());
            return;
        }

        virtual public void back()
        {
            auto.getDeltaTiempoAvance().acumularTiempo(auto.getElapsedTime());
            auto.setVelocidadActual(auto.velocidadFisicaRetroceso());
            return;
        }

        virtual public void left(CamaraEnTerceraPersona camara)
        {
            return;
        }

        virtual public void right(CamaraEnTerceraPersona camara)
        {
            return;
        }

        virtual public void jump()
        {
            auto.getDeltaTiempoSalto().acumularTiempo(auto.getElapsedTime());
            this.auto.setEstado(new Jumping(this.auto));
            return;
        }

        virtual public void speedUpdate()
        {
            return;
        }

        virtual public void jumpUpdate()
        {
            return;
        }

        protected void move()
        {
            auto.move(auto.getVectorAdelante() * auto.getVelocidadActual() * auto.getElapsedTime());
        }

        protected float velocidadFisicaDeSalto()
        {
            return auto.getVelocidadActualDeSalto() + (-auto.getAceleracionGravedad()) * auto.getDeltaTiempoSalto().tiempoTranscurrido();
        }
    }
}
