using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TGC.Core.BoundingVolumes;
using TGC.Core.SceneLoader;
using TGC.Core.Text;
using TGC.Core.Utils;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model.Entities
{
    public class EntityItem : EntityUpdatable
    {
        private const  float Y_VELOCITY = 0.05f;
        private const  float DISTANCE   = 15f;
        private static float Y_ROTATION = FastMath.QUARTER_PI * 0.004f;

        private TgcMesh mesh;

        protected TgcBoundingCylinderFixedY boundingBox;

        public EntityItem(TgcMesh mesh)            
        {
            this.mesh                  = mesh;                 
            this.move(0f, DISTANCE , 0f);
            this.mesh.updateBoundingBox();
            this.mesh.UpdateMeshTransform();
            this.mesh.BoundingBox      = TGCUtils.updateMeshBoundingBox(this.mesh);
            this.mesh.AlphaBlendEnable = true;
            this.boundingBox           = new TgcBoundingCylinderFixedY(this.mesh.BoundingBox.calculateBoxCenter(), 20f, 10f);
        }

        public Vector3 Position
        {
            get { return this.mesh.Position; }            
        }

        public void move(float x, float y, float z)
        {
            this.mesh.move(x, y, z);
        }

        public void move(Vector3 translation)
        {
            this.mesh.move(translation);
        }

        public TgcMesh Mesh
        {
            get
            {
                return this.mesh;
            }
        }
        
        public TgcBoundingCylinderFixedY BoundingBox
        {
            get { return this.boundingBox; }
        }

        public override void update(float elapsedTime)
        {
            this.mesh.rotateY(Y_ROTATION);
            this.mesh.move(0f, FastMath.Sin(this.mesh.Rotation.Y) * Y_VELOCITY, 0f);
            this.mesh.UpdateMeshTransform();
            float rotY = this.mesh.Rotation.Y;
            this.mesh.Rotation = new Vector3(this.mesh.Rotation.X, 0f, this.mesh.Rotation.Z);
            this.mesh.BoundingBox = TGCUtils.updateMeshBoundingBox(this.mesh);
            this.mesh.Rotation = new Vector3(this.mesh.Rotation.X, rotY, this.mesh.Rotation.Z);            
        }

        public override void render()
        {
            this.mesh.render();
        }

        

        public override void dispose()
        {
            this.mesh.dispose();
        }
    }
}
