using System;
using System.Collections.Generic;
using TGC.Core.SkeletalAnimation;
using Microsoft.DirectX;
using TGC.Core.Utils;

namespace TGC.Group.Model.GameWorld
{
    public class Monster
    {

        protected TgcSkeletalMesh enemy;
        private const float MOVEMENT_SPEED = 200f;
        private Vector3 move = new Vector3(0, 0, -1);
        private Boolean rotating = false;
        private float rotationAngle = 0;

        public Monster(String mediaPath)
        {
            TgcSkeletalLoader loader = new TgcSkeletalLoader();
            enemy = loader.loadMeshAndAnimationsFromFile(mediaPath + "/Monster-TgcSkeletalMesh.xml", new string[] { mediaPath + "/Run-TgcSkeletalAnim.xml" });
            enemy.playAnimation("Run");
            enemy.Position = new Vector3(330f, 0f, 330f);
            enemy.Scale = new Vector3(2.5f, 2.5f, 2.5f);
        }


        public void update(List<AINode> directions, float elapsedTime)
        {
            for (int i = 0; i < directions.Count && !rotating; i++)
            {
                if (directions[i].isInThePoint(this.enemy.Position))
                {
                    var originalMove = this.move;
                    Random rdn = new Random();
                    this.move = directions[i].Direction;
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
                }
            }
            if (rotating)
            {
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

        public void render()
        {
            this.enemy.render();
        }

        public void dispose()
        {
            this.enemy.dispose();
        }

    }
}
