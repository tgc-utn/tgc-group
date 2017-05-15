using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SkeletalAnimation;
using TGC.Core.Utils;
using TGC.Group.Model.GameWorld;

namespace TGC.Group.Model.Entities
{
    public class EntityMonster : EntityUpdatable
    {

        protected TgcSkeletalMesh enemy;
        private const float MOVEMENT_SPEED = 200f;
        private Vector3 move = new Vector3(0, 0, -1);
        private Boolean rotating = false;
        private float rotationAngle = 0;
        private List<AINode> walkingNodes;

        public EntityMonster(TgcSkeletalLoader loader, String mediaPath)
        {
            this.walkingNodes = new List<AINode>();            
            enemy = loader.loadMeshAndAnimationsFromFile(mediaPath + "/Monster-TgcSkeletalMesh.xml", new string[]
                {
                    mediaPath + "/Run-TgcSkeletalAnim.xml",
                    mediaPath + "/Walk-TgcSkeletalAnim.xml",
                });
            enemy.playAnimation("Run");
            enemy.Position = new Vector3(330f, 0f, 330f);
            enemy.Scale = new Vector3(2.5f, 2.5f, 2.5f);
        }

        public List<AINode> WalkingNodes
        {
            get { return this.walkingNodes; }
            set { this.walkingNodes = value; }
        }


        public override void update(float elapsedTime)
        {
           
            if(!rotating)
            {
                for (int i = 0; i < this.walkingNodes.Count; i++)
                {
                    if (this.walkingNodes[i].isInThePoint(this.enemy.Position))
                    {
                        var originalMove = this.move;
                        Random rdn = new Random();
                        this.move = this.walkingNodes[i].Direction;
                        enemy.playAnimation("Run");// tendria que poner una animacion que lo deje parado dejarlo parado
                        var newMove = this.move;
                        if ((originalMove.Z == -1 && newMove.X == 1) ||
                            (originalMove.X == -1 && newMove.Z == -1) ||
                            (originalMove.X == 1 && newMove.Z == 1) ||
                            (originalMove.Z == 1 && newMove.X == -1))
                            this.rotationAngle = FastMath.Acos(Vector3.Dot(Vector3.Normalize(newMove), Vector3.Normalize(originalMove))) + FastMath.PI;
                        else
                            this.rotationAngle = FastMath.Acos(Vector3.Dot(Vector3.Normalize(newMove), Vector3.Normalize(originalMove)));

                        rotating = true;
                        break;
                    }
                }
            }
            if(rotating)
            {
                enemy.playAnimation("Walk");
                var rotAngle = 1f / 180f;
                this.enemy.rotateY(rotAngle);
                this.rotationAngle = this.rotationAngle - rotAngle;

                if (this.rotationAngle < 0)
                { rotating = false; enemy.playAnimation("Run"); }
            }

            this.enemy.UpdateMeshTransform();
            this.enemy.updateAnimation(elapsedTime);
            if (!rotating)
            {
                enemy.move(this.move * 1f);
                this.rotationAngle = 0;
            }
        }

        public override void render()
        {
            this.enemy.render();
        }

        public override void dispose()
        {
            this.enemy.dispose();
        }


    }
}
