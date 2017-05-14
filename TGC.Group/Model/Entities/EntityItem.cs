using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Entities
{
    public class EntityItem : EntityUpdatable
    {
        private const float Y_VELOCITY = 10f;
        private const float DISTANCE   = 60f;

        private List<TgcMesh> meshes;
        
        private float initialY;
        private bool isGoingUp;
        

        public EntityItem(TgcScene scene)            
        {
            this.meshes   = scene.Meshes;
            this.initialY = this.meshes[0].Position.Z;
            this.isGoingUp  = true;
        }

        public new Vector3 Position
        {
            get { return this.meshes[0].Position; }
            set
            {
                int meshCount = this.meshes.Count;
                for(int index = 0; index < meshCount; index++)
                {
                    Vector3 distance = this.meshes[index].Position - this.meshes[0].Position;
                    this.meshes[index].Position = value + distance;
                }
                this.initialY = this.meshes[0].Position.Z;
            }
        }

        public override void update(float elapsedTime)
        {
            if(this.isGoingUp)
            {
                if(this.Position.Z - initialY > DISTANCE)
                {
                    this.isGoingUp = false;
                }
                else
                {
                    this.Position = new Vector3(this.Position.X, this.Position.Y + Y_VELOCITY, this.Position.Z);
                }                
            }
            else
            {
                if (this.Position.Z - initialY < -DISTANCE)
                {
                    this.isGoingUp = true;
                }
                else
                {
                    this.Position = new Vector3(this.Position.X, this.Position.Y - Y_VELOCITY, this.Position.Z);
                }
            }
        }

    }
}
