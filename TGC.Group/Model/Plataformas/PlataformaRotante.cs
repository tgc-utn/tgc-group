using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model.Plataformas
{
    class PlataformaRotante : Plataforma
    {
        private TgcMesh plataformaMesh;
        private Escenario escenario;
        public float anguloRotacion;
        public float coeficienteRotacion;

        private TGCVector3 posicionInicialBB;
        private TGCMatrix mTraslacionAlOrigen;
        private TGCMatrix mTraslacionPosInicial;
        public TgcBoundingOrientedBox OBB;
        public TGCVector3 vRotacionOBB;

        public PlataformaRotante(TgcMesh plataformaMesh, Escenario escenario, int coeficienteRotacion) : base(plataformaMesh, escenario)
        {
            this.plataformaMesh = plataformaMesh;
            this.escenario = escenario;
            this.coeficienteRotacion = coeficienteRotacion;

            this.plataformaMesh.AutoTransform = false;
            this.plataformaMesh.AutoUpdateBoundingBox = false;
            

            //Defino angulo de rotacion --> coeficiente puede ser -1 o 1, define direccion de rotacion
            anguloRotacion = FastMath.ToRad(15f);
            anguloRotacion *= coeficienteRotacion;


            //Defino Matrices para rotacion del mesh de la plataforma
            posicionInicialBB = this.plataformaMesh.BoundingBox.calculateBoxCenter(); 
            mTraslacionPosInicial = TGCMatrix.Translation(posicionInicialBB);
            mTraslacionAlOrigen = TGCMatrix.Translation(new TGCVector3(-posicionInicialBB.X, -posicionInicialBB.Y, -posicionInicialBB.Z));

            //Defino OrientedBoundingBox y hago el Dispose() de la AABB
            OBB = TgcBoundingOrientedBox.computeFromAABB(this.plataformaMesh.BoundingBox);
            OBB.setRenderColor(System.Drawing.Color.Empty);
            vRotacionOBB = new TGCVector3(0f, anguloRotacion, 0f);

           // plataformaMesh.BoundingBox.Dispose();

        }
        public void Render(float tiempo)
        {
            OBB.setRotation(TGCVector3.Multiply(vRotacionOBB,tiempo));
            OBB.Render();
        }
        public override void Update(float tiempo)
        {
            //Traslado Mesh al origen --> Roto el Mesh --> Vuelve a la posicion inicial
            plataformaMesh.Transform = mTraslacionAlOrigen * TGCMatrix.RotationY(anguloRotacion * tiempo) * mTraslacionPosInicial;
        }

        
    }
}
