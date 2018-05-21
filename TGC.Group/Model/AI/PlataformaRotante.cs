using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Collision;
using TGC.Core.BoundingVolumes;
using TGC.Core.Geometry;

namespace TGC.Group.Model.AI
{
    class PlataformaRotante : Plataforma
    {
        private TgcMesh plataformaMesh;
        private Escenario escenario;
        public float anguloRotacion;

        private TGCVector3 posicionInicialBB;
        private TGCMatrix mTraslacionAlOrigen;
        private TGCMatrix mTraslacionPosInicial;
        public TgcBoundingOrientedBox OBB;
        private TGCVector3 vRotacionOBB;

        public PlataformaRotante(TgcMesh plataformaMesh, Escenario escenario, int coeficienteRotacion) : base(plataformaMesh, escenario)
        {
            this.plataformaMesh = plataformaMesh;
            this.escenario = escenario;

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

            plataformaMesh.BoundingBox.Dispose();

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

        override public bool colisionaConPersonaje(TgcBoundingSphere esferaPersonaje)
        {
            return TgcCollisionUtils.testSphereOBB(esferaPersonaje, OBB);
        }

        private TGCVector3 EPSILON = new TGCVector3(0f, 0.005f, 0f);

        public TGCVector3 colisionConRotante(TgcBoundingSphere esfera, TGCVector3 movementVector)
        {
            if(TgcCollisionUtils.testSphereOBB(esfera, OBB))//distancia(esfera, this.obb) <= esfera.Radius) --> update
            {
                movementVector += EPSILON;
                esfera.moveCenter(movementVector);
            }
            return movementVector;
        }
        /***********************************PROXIMAMENTE************************************************/
        private float distancia(TgcBoundingSphere esfera, TgcBoundingOrientedBox obb)
        {
            var v = esfera.Center - obb.Center;
            float sqDist = 0f;
            foreach(TGCVector3 eje in obb.Orientation)
            {
                float d = TGCVector3.Dot(v, eje), excess = 0f;
                if (d < -eje.Length()) excess = d + eje.LengthSq();
                else if (d > eje.Length()) excess = d - eje.LengthSq();
                sqDist += FastMath.Pow2(excess);

            }


            return sqDist;
        }

    }
}
