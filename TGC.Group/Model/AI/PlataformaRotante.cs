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
            return TgcCollisionUtils.testSphereOBB(esferaPersonaje,OBB);//TgcCollisionUtils.testSphereOBB(esferaPersonaje,OBB);
        }

        private float EPSILON = 0.4f;
        public TGCVector3 colisionConRotante(TgcBoundingSphere esfera, TGCVector3 movementVector)
        {
            //Si esta parador Arriba de la caja
            if (colisionaConPersonaje(esfera) && esfera.Center.Y > OBB.Center.Y + OBB.Extents.Y)
            {
                esfera.moveCenter(movementVector);
                return movementVector;
            }//Si choca por debajo a la plataforma
            else if (colisionaConPersonaje(esfera) && esfera.Center.Y < OBB.Center.Y - OBB.Extents.Y)
            {
                movementVector.Y = -EPSILON;
                esfera.moveCenter(movementVector);
                return movementVector;
            }
            else //Si la choca por los costados
            {
                movementVector.Y = 25 * EPSILON;
                esfera.moveCenter(-movementVector);
                return -movementVector;
            }
        }

    }
}
