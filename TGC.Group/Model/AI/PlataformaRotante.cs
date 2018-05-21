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



        //Manejo de colision Esfera OBB
        //TODO: Reificar
        override public bool colisionaConPersonaje(TgcBoundingSphere esferaPersonaje)
        {
            return TgcCollisionUtils.testSphereOBB(esferaPersonaje, OBB);
        }

        private TGCVector3 EPSILON = new TGCVector3(0f, 0.0000005f, 0f);

        public TGCVector3 colisionConRotante(TgcBoundingSphere esfera, TGCVector3 movementVector)
        {
            
            if (distanciaEsferaOBB(esfera, OBB) <= esfera.Radius)
            {
                if (movementVector.LengthSq() > EPSILON.Length())
                {
                    movementVector += EPSILON;
                    esfera.moveCenter(movementVector);
                }
            }
            return movementVector;
        }
        
        private float distanciaEsferaOBB(TgcBoundingSphere esfera, TgcBoundingOrientedBox obb)
        {
            var closest = vMenorDistancia(esfera, obb);
            float sqDist = TGCVector3.Dot(closest - esfera.Center, closest - esfera.Center);
            return sqDist;

        }

        private TGCVector3 vMenorDistancia(TgcBoundingSphere esfera, TgcBoundingOrientedBox obb)
        {
            var v = esfera.Center - obb.Center;
            var q = esfera.Center;
            foreach(TGCVector3 eje in obb.Orientation)
            {
                //Distancia al eje de la caja
                float dist = TGCVector3.Dot(v, eje), excess = 0f;

                if (dist > eje.Length()) dist = eje.Length();
                else if (dist < -eje.Length()) dist = -eje.Length();

                TGCVector3.Add(q,TGCVector3.Multiply(eje,excess));

            }
            return q;
        }

    }
}
