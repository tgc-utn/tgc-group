using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;

namespace TGC.Group.Model
{
    class Pasto : ObjetoJuego
    {
        private float altura;
        private List<TgcMesh> capas;
        private float time;

        public Pasto(TgcMesh mesh, Effect effect, int capasCount, float altura) : base(mesh, default, default, default)
        {
            this.altura = altura;
            capas = new List<TgcMesh>();
            for (int i = 0; i < capasCount; i++)
            {
                TgcMesh pasto = mesh.clone("Pasto");
                pasto = mesh.createMeshInstance("Pasto" + i);
                pasto.Position += TGCVector3.Up * (i * altura / capasCount);
                pasto.Transform = TGCMatrix.Scaling(pasto.Scale) * TGCMatrix.RotationYawPitchRoll(pasto.Rotation.Y, pasto.Rotation.X, pasto.Rotation.Z) * TGCMatrix.Translation(pasto.Position);
                pasto.Effect = effect;
                pasto.Technique = "Pasto";
                capas.Add(pasto);
            }
        }

        public override void Update(float elapsedTime)
        {
            time += elapsedTime;
        }

        public override void Render()
        {
            foreach(var capa in capas)
            {
                capa.Technique = "Pasto";
                capa.Effect.SetValue("nivel", capa.Position.Y / altura);
                capa.Effect.SetValue("time", time);
                capa.Render();
            }
        }
    }
}
