using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Text;
using System.Drawing;
using Microsoft.DirectX.DirectInput;
using System.Timers;
using System.Diagnostics;
using TGC.Group.Model;
using TGC.Group.Model.Vehiculos.Estados;
using TGC.Group.Model.Vehiculos;

namespace TGC.Group.Model
{
    abstract class Vehiculo
    {
        private float velocidadActual;
        private float velocidadActualDeSalto;
        public TgcMesh mesh;
        public TGCVector3 vectorAdelante;
        protected float velocidadRotacion = 1f;
        protected float velocidadMaximaDeSalto = 60f;
        protected float velocidadMaximaDeAvance = 300f;
        private float elapsedTime;
        private Timer deltaTiempoAvance;
        private Timer deltaTiempoSalto;
        protected float aceleracionAvance = 0.3f;
        protected float aceleracionRetroceso;
        private float aceleracionGravedad = 0.5f;
        private EstadoVehiculo estado;
        protected float constanteRozamiento = 0.2f;
        protected float constanteFrenado = 1f;
        protected List<Rueda> ruedas = new List<Rueda>();

        public Vehiculo(string rutaAMesh)
        {
            this.estado = new Stopped(this);
            this.vectorAdelante = new TGCVector3(0, 0, 1);
            this.crearMesh(rutaAMesh);
            this.velocidadActual = 0f;
            this.velocidadActualDeSalto = 60f;
            this.elapsedTime = 0f;
            this.deltaTiempoAvance = new Timer();
            this.deltaTiempoSalto = new Timer();
            this.aceleracionRetroceso = this.aceleracionAvance * 0.8f;
        }

        public EstadoVehiculo getEstado()
        {
            return estado;
        }

        private void crearMesh(string rutaAMesh)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(rutaAMesh);
            this.mesh = scene.Meshes[0];
            mesh.RotateY(FastMath.PI);
            mesh.Scale = new TGCVector3(0.1f, 0.1f, 0.1f);
        }

        public void setVectorAdelante(TGCVector3 vector)
        {
            this.vectorAdelante = vector;
        }

        public float getVelocidadDeRotacion()
        {
            return this.velocidadRotacion;
        }

        //sirve para imprimirlo por pantalla
        public float getVelocidadActualDeSalto()
        {
            return this.velocidadActualDeSalto;
        }

        public float velocidadFisica()
        {
            return System.Math.Min(this.velocidadMaximaDeAvance, this.velocidadActual + this.aceleracionAvance * this.deltaTiempoAvance.tiempoTranscurrido());
        }

        public float velocidadFisicaRetroceso()
        {
            return System.Math.Max(-this.velocidadMaximaDeAvance, this.velocidadActual + (-this.aceleracionRetroceso) * this.deltaTiempoAvance.tiempoTranscurrido());
        }

        public TGCVector3 getVectorAdelante()
        {
            return this.vectorAdelante;
        }

        //devuelve la posicion del auto en el mapa, sirve para la camara
        public TGCVector3 posicion()
        {
            return mesh.Position;
        }

        public void setElapsedTime(float time)
        {
            this.elapsedTime = time;
            if(this.deltaTiempoAvance.tiempoTranscurrido() != 0)
            {
                this.deltaTiempoAvance.acumularTiempo(this.elapsedTime);
            }
            if (this.deltaTiempoSalto.tiempoTranscurrido() != 0)
            {
                this.deltaTiempoSalto.acumularTiempo(this.elapsedTime);
            }

        }

        public float getVelocidadActual()
        {
            return this.velocidadActual;
        }

        public void Render()
        {
            mesh.Render();
        }

        public void dispose()
        {
            mesh.Dispose();
        }

        public Timer getDeltaTiempoAvance()
        {
            return this.deltaTiempoAvance;
        }

        public float getElapsedTime()
        {
            return this.elapsedTime;
        }

        public void setVelocidadActual(float nuevaVelocidad)
        {
            this.velocidadActual = nuevaVelocidad;
        }

        public void setEstado(EstadoVehiculo estado)
        {
            this.estado = estado;
        }

        public void setVelocidadActualDeSalto(float velocidad)
        {
            this.velocidadActualDeSalto = velocidad;
        }

        public float getAceleracionGravedad()
        {
            return this.aceleracionGravedad;
        }

        public Timer getDeltaTiempoSalto()
        {
            return this.deltaTiempoSalto;
        }

        public float getVelocidadMaximaDeSalto()
        {
            return this.velocidadMaximaDeSalto;
        }

        public float getConstanteRozamiento()
        {
            return this.constanteRozamiento;
        }

        public float getConstanteFrenado()
        {
            return this.constanteFrenado;
        }
    }
}
