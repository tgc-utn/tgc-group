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
        public TGCMatrix traslado, rotado, escalado;
        protected List<Rueda> ruedas = new List<Rueda>();
        protected Rueda delanteraIzquierda;
        protected Rueda delanteraDerecha;
        protected TGCVector3 vectorDireccion;
        private EstadoVehiculo estado;
        private float velocidadActual;
        private float velocidadActualDeSalto;
        protected float velocidadRotacion = 1f;
        protected float velocidadInicialDeSalto = 15f;
        protected float velocidadMaximaDeAvance = 30f;
        protected float aceleracionAvance = 0.3f;
        protected float aceleracionRetroceso;
        private float aceleracionGravedad = 0.5f;
        private float elapsedTime;
        protected float constanteDeRozamiento = 0.2f;
        protected float constanteFrenado = 1f;
        protected TGCVector3 escaladoInicial = new TGCVector3(0.005f, 0.005f, 0.005f);
        //se guarda el traslado inicial porque se usa como pivote
        protected TGCMatrix trasladoInicial;

        public Vehiculo(string mediaDir, TGCVector3 posicionInicial)
        {
            this.estado = new Stopped(this);
            this.vectorAdelante = new TGCVector3(0, 0, 1);
            this.CrearMesh(mediaDir + "meshCreator\\meshes\\Vehiculos\\Camioneta\\Camioneta-TgcScene.xml", posicionInicial);
            this.velocidadActual = 0f;
            this.velocidadActualDeSalto = this.velocidadInicialDeSalto;
            this.elapsedTime = 0f;
            this.deltaTiempoAvance = new Timer();
            this.deltaTiempoSalto = new Timer();
            this.aceleracionRetroceso = this.aceleracionAvance * 0.8f;
            this.vectorDireccion = this.vectorAdelante;


        }

        public EstadoVehiculo GetEstado()
        {
            return estado;
        }

        private void CrearMesh(string rutaAMesh, TGCVector3 posicionInicial)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(rutaAMesh);
            this.mesh = scene.Meshes[0];
            this.rotado = TGCMatrix.RotationY(FastMath.PI);
            this.escalado = TGCMatrix.Scaling(this.escaladoInicial);
            this.traslado = TGCMatrix.Translation(posicionInicial.X, posicionInicial.Y, posicionInicial.Z);
            this.trasladoInicial = traslado;

        }

        public void SetVectorAdelante(TGCVector3 vector)
        {
            this.vectorAdelante = vector;
        }

        public float GetVelocidadDeRotacion()
        {
            return this.velocidadRotacion;
        }

        //sirve para imprimirlo por pantalla
        public float GetVelocidadActualDeSalto()
        {
            return this.velocidadActualDeSalto;
        }

        public float VelocidadFisica()
        {
            return System.Math.Min(this.velocidadMaximaDeAvance, this.velocidadActual + this.aceleracionAvance * this.deltaTiempoAvance.tiempoTranscurrido());
        }

        public float VelocidadFisicaRetroceso()
        {
            return System.Math.Max(-this.velocidadMaximaDeAvance, this.velocidadActual + (-this.aceleracionRetroceso) * this.deltaTiempoAvance.tiempoTranscurrido());
        }

        public TGCVector3 GetVectorAdelante()
        {
            return this.vectorAdelante;
        }

        //devuelve la posicion del auto en el mapa, sirve para la camara
        public TGCVector3 GetPosicionCero()
        {
            return this.mesh.Position;
        }

        public void SetElapsedTime(float time)
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

        public float GetVelocidadActual()
        {
            return this.velocidadActual;
        }

        public void Render()
        {
            this.mesh.Render();
            delanteraIzquierda.Render();
            //delanteraDerecha.Render();
            foreach (var rueda in this.ruedas)
            {
                rueda.Render();
            }
        }

        public void Dispose()
        {
            this.mesh.Dispose();
        }

        public Timer GetDeltaTiempoAvance()
        {
            return this.deltaTiempoAvance;
        }

        public float GetElapsedTime()
        {
            return this.elapsedTime;
        }

        public void SetVelocidadActual(float nuevaVelocidad)
        {
            this.velocidadActual = nuevaVelocidad;
        }

        public void SetEstado(EstadoVehiculo estado)
        {
            this.estado = estado;
        }

        public void SetVelocidadActualDeSalto(float velocidad)
        {
            this.velocidadActualDeSalto = velocidad;
        }

        public float GetAceleracionGravedad()
        {
            return this.aceleracionGravedad;
        }

        public Timer GetDeltaTiempoSalto()
        {
            return this.deltaTiempoSalto;
        }

        public float GetVelocidadMaximaDeSalto()
        {
            return this.velocidadInicialDeSalto;
        }

        public float GetConstanteRozamiento()
        {
            return this.constanteDeRozamiento;
        }

        public float GetConstanteFrenado()
        {
            return this.constanteFrenado;
        }

        public void Move(TGCVector3 desplazamiento)
        {

            this.traslado = this.traslado * TGCMatrix.Translation(desplazamiento.X, desplazamiento.Y, desplazamiento.Z);
            this.delanteraIzquierda.RotateX(this.GetVelocidadActual());
        }

        public TGCVector3 GetPosicion()
        {
            return TGCVector3.transform(new TGCVector3(0, 0, 0), this.rotado * this.traslado);
        }

        public TGCVector3 GetVectorCostado()
        {
            return TGCVector3.Cross(this.vectorAdelante, new TGCVector3(0, 1, 0));
        }


        public List<Rueda> GetRuedas()
        {
            return this.ruedas;

        }

        virtual public void Transform()
        {
            var transformacion = GetTransformacion();
            this.mesh.Transform = transformacion;
            this.mesh.BoundingBox.transform(transformacion);
            this.delanteraIzquierda.Transform(this.GetTransformacion());
            
        }

        public TGCMatrix GetTransformacion()
        {
            return this.escalado * this.rotado * this.traslado;
        }

        public void Rotate(float rotacion)
        {
            this.rotado = TGCMatrix.RotationY(rotacion) * this.rotado;
        }

        public void RotarDelanteras(float rotacion)
        {
        }
    }
}
