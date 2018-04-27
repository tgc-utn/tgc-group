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
            auto.mesh.Move(auto.getVectorAdelante() * auto.getVelocidadActual() * auto.getElapsedTime());
        }

        protected float velocidadFisicaDeSalto()
        {
            return auto.getVelocidadActualDeSalto() + (-auto.getAceleracionGravedad()) * auto.getDeltaTiempoSalto().tiempoTranscurrido();
        }

        virtual public void right(CamaraEnTerceraPersona camara)
        {
            if (auto.getVelocidadActual() == 0) return;
            float rotacionReal = auto.getVelocidadDeRotacion() * auto.getElapsedTime();
            rotacionReal = (auto.getVelocidadActual() > 0) ? rotacionReal : -rotacionReal;
            auto.mesh.RotateY(rotacionReal);
            TGCMatrix matrizDeRotacion = new TGCMatrix();
            matrizDeRotacion.RotateY(rotacionReal);
            auto.vectorAdelante.TransformCoordinate(matrizDeRotacion);
            camara.rotateY(rotacionReal);

        }

        //lo mismo que arriba
        virtual public void left(CamaraEnTerceraPersona camara)
        {
            if (auto.getVelocidadActual() == 0) return;
            float rotacionReal = auto.getVelocidadDeRotacion() * auto.getElapsedTime();
            rotacionReal = (auto.getVelocidadActual() < 0) ? rotacionReal : -rotacionReal;
            auto.mesh.RotateY(rotacionReal);
            TGCMatrix matrizDeRotacion = new TGCMatrix();
            matrizDeRotacion.RotateY(rotacionReal);
            auto.vectorAdelante.TransformCoordinate(matrizDeRotacion);
            camara.rotateY(rotacionReal);

        }
    }
}
