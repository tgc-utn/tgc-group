using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Turbo : ObjetoJuego
    {
        private const int TIEMPO_INACTIVO = 30; // Tiempo en segundos que el turbo esta inactivo (o sea, no se puede recoger)
        private int poder; // La cantidad de turbo que le da al auto
        public bool Activo { get; private set; } = true; // Seguramente es mejorable todo esto

        public Turbo(TgcMesh mesh, TGCVector3 translation, int poder = 25, TGCVector3 rotation = new TGCVector3(), float angle = 0) : base(mesh, translation, rotation, angle)
        {
            this.poder = poder;
        }

        public void Update(float ElapsedTime)
        {
            // TODO: Si esta inactivo, cuando pase TIEMPO_INACTIVO se debe volver a activar
        }

        public void Render()
        {
            if (Activo) // Si no esta activo, no renderizamos. TODO: Estaria bueno tener un shader que lo oscurezca o algo asi
            {
                mesh.Transform = TGCMatrix.Translation(new TGCVector3(translation));
                mesh.Render();
                mesh.BoundingBox.transform(mesh.Transform);
                mesh.BoundingBox.Render();
            }
        }

        public int Usar()
        {
            if (!Activo)
                return 0;

            Activo = false;
            return poder;
        }
    }
}
