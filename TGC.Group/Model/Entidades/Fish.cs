using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Entidades
{
    class Fish : Entity
    {

        //Config
        const float speed = 7.5f;
        const float distanceToMove = 70f;

        //Internal vars
        TGCVector3 goalPos = TGCVector3.Empty;
        TGCQuaternion rotation = TGCQuaternion.Identity;

        public Fish(TgcMesh mesh) : base(mesh, new TGCVector3(-1,0,0)) { }

        protected override void InitEntity()
        {
            mesh.Scale = new TGCVector3(0.3f, 0.3f, 0.3f);
        }

        protected override void UpdateEntity(float ElapsedTime)
        {
            if (ArrivedGoalPos()) 
                GetNewGoalPos();

            Move(goalPos, speed, ElapsedTime);
        }

        protected override void RenderEntity() { }

        protected override void DisposeEntity() { }


        //Internal functions

        private void GetNewGoalPos()
        {
            Random r = new Random();
            var x = (float) r.NextDouble();
            var y = (float) r.NextDouble();
            var z = (float) r.NextDouble();
            goalPos = new TGCVector3(x, y, z) * distanceToMove;
        }

        private bool ArrivedGoalPos() { return Math.Abs( (goalPos - mesh.Position).Length() ) < 0.1f; }
    }
}
