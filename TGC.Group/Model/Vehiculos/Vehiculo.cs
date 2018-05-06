using System;
using System.Collections.Generic;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Vehiculos.Estados;
using TGC.Group.Model.Vehiculos;

namespace TGC.Group.Model
{
    abstract class Vehiculo
    {
        public TgcMesh mesh;
        private Timer deltaTiempoAvance;
        private Timer deltaTiempoSalto;
        public TGCVector3 vectorAdelante;
        public TGCMatrix transformacion;
        protected List<Rueda> ruedas = new List<Rueda>();
        private EstadoVehiculo estado;
        private float velocidadActual;
        private float velocidadActualDeSalto;
        protected float velocidadRotacion = 1f;
        protected float velocidadInicialDeSalto = 60f;
        protected float velocidadMaximaDeAvance = 300f;
        protected float aceleracionAvance = 0.3f;
        protected float aceleracionRetroceso;
        private float aceleracionGravedad = 0.5f;
        private float elapsedTime;
        protected float constanteDeRozamiento = 0.2f;
        protected float constanteFrenado = 1f;

        public Vehiculo(string mediaDir, TGCVector3 posicionInicial)
        {
            this.estado = new Stopped(this);
            this.vectorAdelante = new TGCVector3(0, 0, 1);
            this.crearMesh(mediaDir + "meshCreator\\meshes\\Vehiculos\\Camioneta\\Camioneta-TgcScene.xml", posicionInicial);
            this.velocidadActual = 0f;
            this.velocidadActualDeSalto = this.velocidadInicialDeSalto;
            this.elapsedTime = 0f;
            this.deltaTiempoAvance = new Timer();
            this.deltaTiempoSalto = new Timer();
            this.aceleracionRetroceso = this.aceleracionAvance * 0.8f;
        }

        public EstadoVehiculo getEstado()
        {
            return estado;
        }

        private void crearMesh(string rutaAMesh, TGCVector3 posicionInicial)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(rutaAMesh);
            this.mesh = scene.Meshes[0];
            var rotacion = TGCMatrix.RotationY(FastMath.PI);
            var escala = TGCMatrix.Scaling(0.05f, 0.05f, 0.05f);
            var traslado = TGCMatrix.Translation(posicionInicial.X, posicionInicial.Y, posicionInicial.Z);

            this.transformacion = escala * rotacion * traslado;
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
            return this.mesh.Position;
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
            this.mesh.Render();
            foreach (var rueda in this.ruedas)
            {
                rueda.Render();
            }
        }

        public void dispose()
        {
            this.mesh.Dispose();
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
            return this.velocidadInicialDeSalto;
        }

        public float getConstanteRozamiento()
        {
            return this.constanteDeRozamiento;
        }

        public float getConstanteFrenado()
        {
            return this.constanteFrenado;
        }

        public void Move(TGCVector3 desplazamiento)
        {

            transformacion = transformacion * TGCMatrix.Translation(desplazamiento.X, desplazamiento.Y, desplazamiento.Z);
            foreach (var rueda in this.ruedas)
            {
                rueda.Move(desplazamiento);
            };
        }


        public List<Rueda> GetRuedas()
        {
            return this.ruedas;

        }

        public void Transform()
        {
            this.mesh.Transform = this.transformacion;
            foreach (var rueda in this.ruedas)
            {
                rueda.Transform();
            }
        }

        public void Rotate(float rotacion)
        {
            transformacion = TGCMatrix.RotationY(rotacion) * transformacion;
            foreach (var rueda in ruedas)
            {
                rueda.Rotate(this.vectorAdelante, rotacion);
            }

        }
    }
}
