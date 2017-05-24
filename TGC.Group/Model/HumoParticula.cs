using System.Collections.Generic;
using Microsoft.DirectX;
using TGC.Core.Particle;
using TGC.Core.Utils;

namespace TGC.Group.Model
{
    class HumoParticula
    {
        private ParticleEmitter emitter;
        private const int CANTIDAD_PARTICULAS = 10;
        private const float TAMANO_PARTICULA = 0.75f;
        private const float TTL_PARTICULA = 0.8f;
        private const float FQ_PARTICULA = 0.25f;
        private const int DISPERSION_PARTICULA = 50;
        private const float VELOCIDAD_PARTICULA = 40f;
        private float ElapsedTime = 0;

        public HumoParticula (string MediaDir)
        {
            emitter = new ParticleEmitter (MediaDir + "Textures\\Particles\\pisada.png", CANTIDAD_PARTICULAS);
            emitter.MinSizeParticle = TAMANO_PARTICULA;
            emitter.MaxSizeParticle = TAMANO_PARTICULA + 1;
            emitter.ParticleTimeToLive = TTL_PARTICULA;
            emitter.CreationFrecuency = FQ_PARTICULA;
            emitter.Dispersion = DISPERSION_PARTICULA;
            emitter.Speed = new Vector3(VELOCIDAD_PARTICULA, 0, VELOCIDAD_PARTICULA);
        }

        public void Update(float ElapsedTime, Vector3 MeshPosition, float Rotation)
        {
            float rohumo, alfa_humo;
            float posicion_xhumo;
            float posicion_yhumo;

            rohumo = FastMath.Sqrt(-this.emitter.Position.X * -this.emitter.Position.X + this.emitter.Position.Z * this.emitter.Position.Z);

            alfa_humo = FastMath.Asin(this.emitter.Position.X / rohumo);
            posicion_xhumo = FastMath.Sin(alfa_humo + Rotation) * rohumo;
            posicion_yhumo = FastMath.Cos(alfa_humo + Rotation) * rohumo;

            this.ElapsedTime = ElapsedTime;
            this.emitter.Position = MeshPosition + new Vector3 (posicion_xhumo, 0, posicion_yhumo);
        }

        public void Render()
        {
            emitter.render(this.ElapsedTime);
        }

        public void Dispose ()
        {
            emitter.dispose();
        }
    }
}
