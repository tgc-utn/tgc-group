using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Collision;
using TGC.Core.BoundingVolumes;
namespace TGC.Group.Model.AI
{
    class PlataformaRotante : Plataforma
    {
        private TgcMesh plataformaMesh;
        private Escenario escenario;
        public float anguloRotacion;

        private TGCVector3 initPosBB;
        private TGCMatrix MTraslacionOrigen;
        private TGCMatrix MTraslacionPos;
        public TgcBoundingOrientedBox obb;

        public PlataformaRotante(TgcMesh plataformaMesh, Escenario escenario) : base(plataformaMesh, escenario)
        {
            this.plataformaMesh = plataformaMesh;
            this.escenario = escenario;
            this.plataformaMesh.AutoTransform = false;
            this.plataformaMesh.AutoUpdateBoundingBox = true;
            anguloRotacion = FastMath.ToRad(10f);

            initPosBB = this.plataformaMesh.BoundingBox.calculateBoxCenter(); //Posicion inicial BoundingBox (Se setea en la posición correcta)
            MTraslacionPos = TGCMatrix.Translation(initPosBB);
             
            MTraslacionOrigen = TGCMatrix.Translation(new TGCVector3(-initPosBB.X, -initPosBB.Y, -initPosBB.Z));

            obb = TgcBoundingOrientedBox.computeFromAABB(this.plataformaMesh.BoundingBox);
            
            obb.setRenderColor(System.Drawing.Color.Red);
            plataformaMesh.BoundingBox.Dispose();
            //plataformaMesh.BoundingBox.move(new TGCVector3(0f,-10000f,0f));
        }

        public void RotateOBB(float tiempo)
        {
            
           // plataformaMesh.BoundingBox.Render();
            obb.setRotation(new TGCVector3(0f, anguloRotacion * tiempo, 0f));
            //obb.updateValues();
            obb.Render();
            
           // plataformaMesh.BoundingBox.Render();
            
        }
        public override void Update(float tiempo)
        {
            var Rot = MTraslacionOrigen * TGCMatrix.RotationY(anguloRotacion * tiempo) * MTraslacionPos;
            plataformaMesh.Transform = Rot;

        }

        override public bool colisionaConPersonaje(TgcBoundingSphere esferaPersonaje)
        {
            return TgcCollisionUtils.testSphereOBB(esferaPersonaje, obb);
        }

    }
}
