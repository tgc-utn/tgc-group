using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Geometry;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using Microsoft.DirectX;
using System.Windows.Forms;

namespace TGC.Group.Model.Entities
{
    public abstract class Entity
    {
        private TgcMesh mesh;
        private bool    renderBoundingBox;
                        

        public Entity(TgcMesh mesh)
        {
            this.mesh              = mesh;
            this.renderBoundingBox = false;
            this.mesh.BoundingBox.setRenderColor(System.Drawing.Color.Green);
        }
        
        public bool shouldRenderBoundingBox
        {
            set { this.renderBoundingBox = value; }
        }

        public System.Drawing.Color boundingBoxRenderColor
        {
            set { this.mesh.BoundingBox.setRenderColor(value); }
        }

        public abstract void update();

        public Vector3 Position
        {
            get { return this.mesh.Position;  }
            set { this.mesh.Position = value; }
        }

        public void rotateX(float angle)
        {
            this.mesh.rotateX(angle);
        }

        public void rotateY(float angle)
        {
            this.mesh.rotateY(angle);
        }

        public void rotateZ(float angle)
        {
            this.mesh.rotateZ(angle);
        }
        
        public void render()
        {            
            this.mesh.UpdateMeshTransform();
            this.mesh.render();            
            if(this.renderBoundingBox)
            {                
                this.mesh.BoundingBox.render();
            }            
        }

        public void dispose()
        {
            this.mesh.dispose();
        }

    }
}
