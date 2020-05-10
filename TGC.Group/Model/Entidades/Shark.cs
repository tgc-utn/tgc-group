using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Collision;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model.Entidades
{
    class Shark : Entity
    {
        static TGCVector3 meshLookDir = new TGCVector3(-1, 0, 0);
        Player player;

        //Config
        const float DAMAGE = 30f;
        const float speed = 10f;
        const float distanceToEscape = 300f;

        //Internal vars
        TGCVector3 goalPos = TGCVector3.Empty;

        bool canDealDamage = true;

        public Shark(TgcMesh mesh, Player player) : base(mesh, meshLookDir) 
        {
            this.player = player;
        }

        //Entity functions
        protected override void InitEntity()
        {
            mesh.Scale = new TGCVector3(0.1f, 0.1f, 0.1f);
        }

        protected override void UpdateEntity(float ElapsedTime)
        {
            if (ArrivedGoalPos())
                SetEscapeGoalPos();

            if (canDealDamage)
                Attack();
            

            Move(goalPos, speed, ElapsedTime);
        }

        protected override void RenderEntity() { 
            mesh.BoundingBox.Render();
        }

        protected override void DisposeEntity() { }

        //Gamemodel functions
        public void Spawn()
        {
            //Reset vars
            canDealDamage = true;
            SetPlayerGoalPos();

            //Position shark
            Random r = new Random();
            var x = (float)r.NextDouble();
            var z = (float)r.NextDouble();
            var sign = r.Next(-1, 1) >= 0 ? 1 : -1;
            mesh.Position = player.Position() + sign * new TGCVector3(x, 0, z) * 100f;
        }

        //Internal functions

        private void SetPlayerGoalPos() { goalPos = player.Position(); }

        private void SetEscapeGoalPos()
        {
            Random r = new Random();
            var x = (float)r.NextDouble();
            var z = (float)r.NextDouble();
            goalPos = new TGCVector3(x, 0, z) * distanceToEscape;
      }

        private bool ArrivedGoalPos() { return Math.Abs((goalPos - mesh.Position).Length()) < 0.1f; }

        private void Attack()
        {
            //Chequear colisiones
            var hitPlayer = TgcCollisionUtils.testAABBAABB(mesh.BoundingBox, player.BoundingBox());
            if (hitPlayer)
            {
                player.GetDamage(DAMAGE);
                canDealDamage = false;
            }
        }
    }
}
