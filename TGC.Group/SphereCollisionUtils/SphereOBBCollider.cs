using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Mathematica;

namespace TGC.Group.SphereCollisionUtils
{
    class SphereOBBCollider
    {
        private float EPSILON = 0.4f;


        public bool colisionaEsferaOBB(TgcBoundingSphere esfera, TgcBoundingOrientedBox obb) => TgcCollisionUtils.testSphereOBB(esfera, obb);

        public TGCVector3 manageColisionEsferaOBB(TgcBoundingSphere esfera, TGCVector3 movementVector, TgcBoundingOrientedBox OBB)
        {
            //Si esta parador Arriba de la caja
            if (colisionaEsferaOBB(esfera, OBB) && esfera.Center.Y > OBB.Center.Y + OBB.Extents.Y)
            {
                esfera.moveCenter(movementVector);
                return movementVector;
            }//Si choca por debajo a la plataforma
            else if (colisionaEsferaOBB(esfera, OBB) && esfera.Center.Y < OBB.Center.Y - OBB.Extents.Y)
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
