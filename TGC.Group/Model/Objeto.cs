using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using TGC.Core.Collision;
using System.Drawing;

namespace TGC.Group.Model
{
    class Objeto
    {
        private List<TgcMesh> elementos;

        public Objeto(List<TgcMesh> elementos, TGCMatrix transformacion)
        {
            this.elementos = elementos;
            foreach (TgcMesh elemento in this.elementos)
            {
                elemento.AutoTransform = false;
                elemento.Transform = transformacion;
                elemento.BoundingBox.transform(transformacion);
            }
        }

        public void Render()
        {
            foreach (TgcMesh elemento in this.elementos)
            {
                elemento.Render();
            }
        }

        public void RenderBoundingBox()
        {
            foreach (TgcMesh elemento in this.elementos)
            {
                elemento.BoundingBox.Render();
            }
        }

        public TgcMesh TestColision(TgcMesh mesh)
        {

            foreach (TgcMesh elemento in this.elementos)
            {
                if (TgcCollisionUtils.testAABBAABB(mesh.BoundingBox, elemento.BoundingBox))
                {
                    return elemento;
                }
            }

            return null;
            
        }

        public void Dispose()
        {
            foreach (TgcMesh elemento in this.elementos)
            {
                elemento.Dispose();
            }
        }

        public void SetColorBoundingBox(Color color)
        {
            foreach (TgcMesh elemento in this.elementos)
            {
                elemento.BoundingBox.setRenderColor(color);
            }
        }
    }
}
