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

namespace TGC.Group.Model
{
    abstract class Vehiculo
    {
        private float velocidadActual;
        private float velocidadActualDeSalto;
        private TgcMesh mesh;
        private TGCVector3 vectorAdelante;
        protected float velocidadRotacion = 1f;
        protected float velocidadMaximaDeSalto = 60f;
        protected float velocidadMaximaDeAvance = 300f;
        private float elapsedTime;
        private float deltaTiempoAvance;
        private float deltaTiempoSalto;
        protected float aceleracionAvance = 0.5f;
        protected float aceleracionRetroceso = 0.3f;
        private float aceleracionGravedad = 0.5f;

        public Vehiculo(string rutaAMesh)
        {
            this.vectorAdelante = new TGCVector3(0, 0, 1);
            this.crearMesh(rutaAMesh);
            this.velocidadActual = 0f;
            this.velocidadActualDeSalto = 60f;
            this.elapsedTime = 0f;
            this.deltaTiempoAvance = 0f;
            this.deltaTiempoSalto = 0f;
        }

        private void crearMesh(string rutaAMesh)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(rutaAMesh);
            this.mesh = scene.Meshes[0];
            mesh.RotateY(FastMath.PI);
            mesh.Scale = new TGCVector3(0.1f, 0.1f, 0.1f);
        }

        public void avanzar()
        {
            this.avanzarTiempo();
            this.velocidadActual = this.velocidadFisica();
            TGCVector3 cuenta = this.vectorAdelante * this.velocidadActual * this.elapsedTime;
            mesh.Move(cuenta);      
        }

        //sirve para imprimirlo por pantalla
        public float getVelocidadActualDeSalto()
        {
            return this.velocidadActualDeSalto;
        }

        private float velocidadFisica()
        {
            return System.Math.Min(this.velocidadMaximaDeAvance, this.velocidadActual + this.aceleracionAvance * this.deltaTiempoAvance);
        }

        private float velocidadFisicaRetroceso()
        {
            return System.Math.Max(-this.velocidadMaximaDeAvance, this.velocidadActual + (-this.aceleracionRetroceso) * this.deltaTiempoAvance);
        }

        public void retroceder()
        {
            this.avanzarTiempo();
            //lo mismo que en el anterior nada más que cambio el sentido del vector adelante
            this.velocidadActual = this.velocidadFisicaRetroceso();
            mesh.Move(this.vectorAdelante * this.velocidadActual * this.elapsedTime);
        }

        public void actualizarVelocidad()
        {
            float constanteDesaceleracion = 0.5f;
            this.resetearDeltaTiempo();
            if (this.velocidadActual > 0)
            {
                this.velocidadActual -= constanteDesaceleracion;
                if(this.velocidadActual < 0)
                {
                    this.velocidadActual = 0;
                    this.resetearDeltaTiempo();
                }
                mesh.Move(this.vectorAdelante * this.velocidadActual * this.elapsedTime);
            }
            else if(this.velocidadActual < 0)
            {  
                this.velocidadActual += constanteDesaceleracion;
                if (this.velocidadActual > 0)
                {
                    this.velocidadActual = 0;
                    this.resetearDeltaTiempo();
                }
                mesh.Move(this.vectorAdelante * this.velocidadActual * this.elapsedTime);
            }
        }

        private void  resetearDeltaTiempo()
        {
            this.deltaTiempoAvance = 0;
        }

        public void doblarALaDerecha(CamaraEnTerceraPersona camara)
        {
            if (this.velocidadActual == 0) return;
            float rotacionReal = this.velocidadRotacion * this.elapsedTime;
            rotacionReal = (this.velocidadActual > 0) ? rotacionReal : -rotacionReal;
            this.mesh.RotateY(rotacionReal);
            TGCMatrix matrizDeRotacion = new TGCMatrix();
            matrizDeRotacion.RotateY(rotacionReal);
            vectorAdelante.TransformCoordinate(matrizDeRotacion);
            camara.rotateY(rotacionReal);

        }

        //lo mismo que arriba
        public void doblarALaIzquierda(CamaraEnTerceraPersona camara)
        {
            if (this.velocidadActual == 0) return;
            float rotacionReal = -this.velocidadRotacion * this.elapsedTime;
            rotacionReal = (this.velocidadActual < 0) ? -rotacionReal : rotacionReal;
            this.mesh.RotateY(rotacionReal);
            TGCMatrix matrizDeRotacion = new TGCMatrix();
            matrizDeRotacion.RotateY(rotacionReal);
            vectorAdelante.TransformCoordinate(matrizDeRotacion);
            camara.rotateY(rotacionReal);

        }

        //devuelve la posicion del auto en el mapa, sirve para la camara
        public TGCVector3 posicion()
        {
            return mesh.Position;
        }

        private bool estaEnPlenoSalto()
        {
            return mesh.Position.Y != 0;
        }

        public void saltar()
        {
            if(!this.estaEnPlenoSalto())
            {
                this.avanzarTiempoSalto();
                TGCVector3 nuevaPosicion = new TGCVector3(0, 1, 0);
                mesh.Move(nuevaPosicion);
                
            }
        }

        public float velocidadFisicaDeSalto()
        {
            return this.velocidadActualDeSalto + (-this.aceleracionGravedad) * this.deltaTiempoSalto;
        }

        private void resetearDeltaTiempoSalto()
        {
            this.deltaTiempoSalto = 0f;
        }

        public void actualizarSalto()
        {
            if(this.deltaTiempoSalto != 0)
            {
                this.velocidadActualDeSalto = this.velocidadFisicaDeSalto();
                float desplazamientoEnY = this.velocidadActualDeSalto * elapsedTime;
                desplazamientoEnY = (mesh.Position.Y + desplazamientoEnY < 0)? -mesh.Position.Y : desplazamientoEnY;
                TGCVector3 nuevoDesplazamiento = new TGCVector3(0, desplazamientoEnY, 0);
                if (nuevoDesplazamiento.Y == 0f)
                {
                    this.resetearDeltaTiempoSalto();
                    this.velocidadActualDeSalto = this.velocidadMaximaDeSalto;
                }
                mesh.Move(nuevoDesplazamiento);
            }

        }

        private void avanzarTiempo()
        {
            this.deltaTiempoAvance += this.elapsedTime;
        }

        private void avanzarTiempoSalto()
        {
            this.deltaTiempoSalto += this.elapsedTime;
        }

        public void setElapsedTime(float time)
        {
            this.elapsedTime = time;
            if(this.deltaTiempoAvance != 0)
            {
                this.avanzarTiempo();
            }
            if (this.deltaTiempoSalto != 0)
            {
                this.avanzarTiempoSalto();
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

    }
}
