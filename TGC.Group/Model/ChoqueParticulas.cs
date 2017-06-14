using System.Collections.Generic;
using Microsoft.DirectX;
using TGC.Core.Particle;
using TGC.Core.Utils;
using System;

namespace TGC.Group.Model
{
    class ChoqueParticula
    {
        private ParticleEmitter emitter;
        private const int TIEMPO_CHOQUE = 1;
        private const int CANTIDAD_PARTICULAS = 320;
        private const float TAMANO_PARTICULA = 1f;
        private const float TTL_PARTICULA = 1f;
        private const float FQ_PARTICULA = 0.5f;
        private const int DISPERSION_PARTICULA = 120;
        private const float VELOCIDAD_PARTICULA = 60f;
        private float ElapsedTime = 0;
        private Vector2 posicionChoque = new Vector2(0, 35);
        private DateTime InicioChoque;
        private bool ChoqueAdelanteIzquierda = false;
        private bool ChoqueAdelanteDerecha = false;
        private bool ChoqueAtrasIzquierda = false;
        private bool ChoqueAtrasDerecha = false;

        public ChoqueParticula(string MediaDir)
        {
            emitter = new ParticleEmitter (MediaDir + "Textures\\Particles\\humo.png", CANTIDAD_PARTICULAS);
            emitter.MinSizeParticle = TAMANO_PARTICULA;
            emitter.MaxSizeParticle = TAMANO_PARTICULA + 1;
            emitter.ParticleTimeToLive = TTL_PARTICULA;
            emitter.CreationFrecuency = FQ_PARTICULA;
            emitter.Dispersion = DISPERSION_PARTICULA;
            emitter.Speed = new Vector3(VELOCIDAD_PARTICULA, 0, VELOCIDAD_PARTICULA);
        }

        public void SetInicioChoque(DateTime Inicio)
        {
            this.InicioChoque = Inicio;
        }

        public void SetChoqueAdelanteIzquierda ()
        {
            this.ChoqueAdelanteIzquierda = true;
        }

        public void SetChoqueAdelanteDerecha()
        {
            this.ChoqueAdelanteDerecha = true;
        }

        public void SetChoqueAtrasIzquierda()
        {
            this.ChoqueAtrasIzquierda = true;
        }

        public void SetChoqueAtrasDerecha()
        {
            this.ChoqueAtrasDerecha = true;
        }

        public void Update(float ElapsedTime, Vector3 MeshPosition, float Rotation)
        {
            float rohumo, alfa_choque;
            float posicion_xchoque;
            float posicion_ychoque;

            if (this.ChoqueAdelanteIzquierda)
            {
                posicionChoque = new Vector2(-45, 40);
            }

            if (this.ChoqueAdelanteDerecha)
            {
                posicionChoque = new Vector2(0, 40);
            }

            if (this.ChoqueAtrasIzquierda)
            {
                posicionChoque = new Vector2(30, 35);
            }

            if (this.ChoqueAtrasDerecha)
            {
                posicionChoque = new Vector2(0, 35);
            }

            rohumo = FastMath.Sqrt(this.posicionChoque.X * this.posicionChoque.X + this.posicionChoque.Y * this.posicionChoque.Y);

            alfa_choque = FastMath.Asin(this.posicionChoque.X / rohumo);

            if (this.ChoqueAdelanteIzquierda || this.ChoqueAdelanteDerecha)
            {
                alfa_choque += FastMath.PI;
            }

            posicion_xchoque = FastMath.Sin(alfa_choque + Rotation) * rohumo;
            posicion_ychoque = FastMath.Cos(alfa_choque + Rotation) * rohumo;

            this.ElapsedTime = ElapsedTime;
            this.emitter.Position = MeshPosition + new Vector3 (posicion_xchoque, 20, posicion_ychoque);
        }

        public void Render()
        {
            if (((DateTime.Now - this.InicioChoque).Seconds < TIEMPO_CHOQUE) &&
                  (this.ChoqueAdelanteDerecha || this.ChoqueAdelanteIzquierda || this.ChoqueAtrasDerecha || this.ChoqueAtrasIzquierda)
                )
                emitter.render(this.ElapsedTime);
            else
            {
                this.ChoqueAdelanteIzquierda = false;
                this.ChoqueAdelanteDerecha = false;
                this.ChoqueAtrasIzquierda = false;
                this.ChoqueAtrasDerecha = false;
            }
        }

        public void Dispose ()
        {
            emitter.dispose();
        }
    }
}
