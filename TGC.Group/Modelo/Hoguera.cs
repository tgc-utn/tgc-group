using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Particle;
using TGC.Core.Mathematica;
using TGC.Core.Direct3D;

namespace TGC.Group.Modelo
{
    public class Hoguera
    {
        public TgcMesh MeshHoguera { get; set; }
        public bool Encendida { get; set; }
        public int ManzanasNecesarias { get; set; }
        private List<ParticleEmitter> emitters;

        public static string texturesPath;
        private string smokeTex = "humoparticula.png";
        private string fireTex = "fuegoparticula.png";
        private int selectedParticleCount = 15;

        public Hoguera(TgcMesh mesh, int necesarios)
        {
            MeshHoguera = mesh;
            Encendida = false;
            ManzanasNecesarias = necesarios;
            emitters = new List<ParticleEmitter>();

            var e1 = new ParticleEmitter(texturesPath + smokeTex, selectedParticleCount);
            e1.Position = MeshHoguera.BoundingBox.Position;
            e1.MinSizeParticle = 3f;
            e1.MaxSizeParticle = 3;
            e1.ParticleTimeToLive = 1.75f;
            e1.CreationFrecuency = 0.9f;
            e1.Dispersion = 50;
            e1.Speed = new TGCVector3(65, 20, 15);
            emitters.Add(e1);

            var e2 = new ParticleEmitter(texturesPath + smokeTex, selectedParticleCount);
            e2.Position = MeshHoguera.BoundingBox.Position;
            e2.MinSizeParticle = 2.5f;
            e2.MaxSizeParticle = 2.5f;
            e2.ParticleTimeToLive = 2.5f;
            e2.CreationFrecuency = 0.4f;
            e2.Dispersion = 30;
            e2.Speed = new TGCVector3(45, 30, 15);
            emitters.Add(e2);

            var e3 = new ParticleEmitter(texturesPath + smokeTex, selectedParticleCount);
            e3.Position = MeshHoguera.BoundingBox.Position;
            e3.MinSizeParticle = 3;
            e3.MaxSizeParticle = 3;
            e3.ParticleTimeToLive = 2f;
            e3.CreationFrecuency = 0.1f;
            e3.Dispersion = 10;
            e3.Speed = new TGCVector3(65, 60, 15);
            emitters.Add(e1);
        }

        public void render(float ElapsedTime)
        {
            MeshHoguera.Render();
            renderParticles(ElapsedTime);
        }

        public void renderParticles(float ElapsedTime)
        {
            D3DDevice.Instance.ParticlesEnabled = true;
            D3DDevice.Instance.EnableParticles();
            foreach (ParticleEmitter e in emitters)
            {
                e.render(ElapsedTime);
            }
        }

        public void encender(int manzanas)
        {
            if (!Encendida && manzanas >= ManzanasNecesarias)
            {
                emitters = new List<ParticleEmitter>();
                Encendida = true;
                var e4 = new ParticleEmitter(texturesPath + smokeTex, selectedParticleCount);
                e4.Position = MeshHoguera.BoundingBox.Position;
                e4.MinSizeParticle = 1f;
                e4.MaxSizeParticle = 3;
                e4.ParticleTimeToLive = 3f;
                e4.CreationFrecuency = 1.5f;
                e4.Dispersion = 80;
                e4.Speed = new TGCVector3(65, 40, 15);
                emitters.Add(e4);

                var e1 = new ParticleEmitter(texturesPath + fireTex, selectedParticleCount);
                e1.Position = MeshHoguera.BoundingBox.Position;
                e1.MinSizeParticle = 3f;
                e1.MaxSizeParticle = 3;
                e1.ParticleTimeToLive = 1.75f;
                e1.CreationFrecuency = 0.9f;
                e1.Dispersion = 50;
                e1.Speed = new TGCVector3(65, 20, 15);
                emitters.Add(e1);

                var e2 = new ParticleEmitter(texturesPath + fireTex, selectedParticleCount);
                e2.Position = MeshHoguera.BoundingBox.Position;
                e2.MinSizeParticle = 2.5f;
                e2.MaxSizeParticle = 2.5f;
                e2.ParticleTimeToLive = 2.5f;
                e2.CreationFrecuency = 0.4f;
                e2.Dispersion = 30;
                e2.Speed = new TGCVector3(45, 30, 15);
                emitters.Add(e2);

                var e3 = new ParticleEmitter(texturesPath + fireTex, selectedParticleCount);
                e3.Position = MeshHoguera.BoundingBox.Position;
                e3.MinSizeParticle = 3;
                e3.MaxSizeParticle = 3;
                e3.ParticleTimeToLive = 2f;
                e3.CreationFrecuency = 0.1f;
                e3.Dispersion = 10;
                e3.Speed = new TGCVector3(65, 60, 15);
                emitters.Add(e1);


            }
        }
    }
}
