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
        TgcMesh mesh;

        //Config
        const float speed = 7.5f;
        const float distanceToMove = 70f;

        //Internal vars
        TGCVector3 goalPos = TGCVector3.Empty;
        TGCQuaternion rotation = TGCQuaternion.Identity;

        protected override void InitEntity(string MediaDir)
        {
            var loader = new TgcSceneLoader();
            var scene = loader.loadSceneFromFile(MediaDir + "yellow_fish-TgcScene.xml");
            mesh = scene.Meshes[0];
            mesh.Scale = new TGCVector3(0.3f, 0.3f, 0.3f);
        }

        protected override void UpdateEntity(float ElapsedTime)
        {
            if (ArrivedGoalPos()) 
                GetNewGoalPos();

            TGCVector3 dir = TGCVector3.Normalize(goalPos - mesh.Position);
            LookAt(dir);

            TGCVector3 movement = dir * speed * ElapsedTime;
            Move(movement);

            UpdateTransform();
        }

        protected override void RenderEntity()
        {
            mesh.Render();
            //mesh.BoundingBox.Render();
        }

        protected override void DisposeEntity() { mesh.Dispose(); }


        protected override TgcMesh GetEntityMesh() { return mesh; }

        //Transformation functions
        private void UpdateTransform()
        {
            mesh.Transform = TGCMatrix.Scaling(mesh.Scale) * TGCMatrix.RotationTGCQuaternion(rotation) * TGCMatrix.Translation(mesh.Position);
        }

        //Internal functions
        private void Move(TGCVector3 movement) { mesh.Position += movement; }

        private void LookAt(TGCVector3 lookDir)
        {
            var defaultLookDir = new TGCVector3(-1, 0, 0);
            float angle = FastMath.Acos( TGCVector3.Dot(defaultLookDir, lookDir) );
            TGCVector3 rotVector = TGCVector3.Cross(defaultLookDir, lookDir);
            rotation = TGCQuaternion.RotationAxis(rotVector, angle);            
        }

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
