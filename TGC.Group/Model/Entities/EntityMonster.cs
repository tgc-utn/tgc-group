using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SkeletalAnimation;

namespace TGC.Group.Model.Entities
{
    public class EntityMonster : EntityUpdatable
    {

        TgcSkeletalMesh skeletalMesh;
        Vector2 currentDirection;

        public EntityMonster(TgcSkeletalMesh mesh)
        {
            this.skeletalMesh = mesh;
            this.skeletalMesh.playAnimation("Run");
            this.skeletalMesh.Position = new Vector3(0f, 0f, 0f);
            this.skeletalMesh.Scale = new Vector3(3f, 3f, 3f);
            
            
        }

        public override void update(float elapsedTime)
        {
            
        }
        

    }
}
