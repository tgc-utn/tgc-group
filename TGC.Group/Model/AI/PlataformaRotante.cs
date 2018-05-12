using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Collision;

namespace TGC.Group.Model.AI
{
    class PlataformaRotante : Plataforma
    {
        private TgcMesh plataformaMesh;
        private Escenario escenario;
        public float anguloRotacion;
        //private TGCVector3 pivote;
        private TGCVector3 initPosMesh;
        private TGCVector3 initPosBB;
        public PlataformaRotante(TgcMesh plataformaMesh, Escenario escenario) : base(plataformaMesh, escenario)
        {
            this.plataformaMesh = plataformaMesh;
            this.escenario = escenario;
            this.plataformaMesh.AutoTransform = false;
            this.plataformaMesh.AutoUpdateBoundingBox = true;
            anguloRotacion = FastMath.ToRad(10f);
            initPosMesh = this.plataformaMesh.Position; /* Posición Inicial Mesh de la plataforma. 
                                                         * Por alguna razón, desde la inicialización se setea en (0,0,0)
                                                         */
            initPosBB = this.plataformaMesh.BoundingBox.calculateBoxCenter(); //Posicion inicial BoundingBox (Se setea en la posición correcta)
    }
        
        public override void Update(float tiempo)
        {
            var TMesh = TGCMatrix.RotationAxis(initPosBB,anguloRotacion * tiempo);
            //var TBB = TGCMatrix.RotationAxis(initPosMesh, anguloRotacion * tiempo);
            plataformaMesh.Transform = TMesh;
           // plataformaMesh.BoundingBox.transform(TGCMatrix.RotationAxis(initPosBB,anguloRotacion *tiempo));

        }
    }
}
