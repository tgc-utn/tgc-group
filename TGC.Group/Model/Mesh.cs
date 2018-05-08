using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class Mesh
    {
        private List<TgcMesh> elementosDelMesh;

        public Mesh(List<TgcMesh> elementosDelMesh)
        {
            this.elementosDelMesh = elementosDelMesh;
        }

        public void Render()
        {
            foreach (TgcMesh elemento in this.elementosDelMesh)
            {
                elemento.Render();
            }
        }

        public List<TgcMesh> getElementos()
        {
            return this.elementosDelMesh;
        }

        public void RenderBoundingBox()
        {
            foreach (TgcMesh elemento in elementosDelMesh)
            {
                elemento.BoundingBox.Render();
            }
        }

        public TGCVector3 GetPosicion()
        {
            return this.elementosDelMesh[0].Position;
        }

        public void Dispose()
        {
            foreach (TgcMesh elemento in elementosDelMesh)
            {
                elemento.Dispose();
            }
        }
    }
}
