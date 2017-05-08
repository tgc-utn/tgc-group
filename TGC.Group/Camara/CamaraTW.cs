using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Camara;
using TGC.Core.Collision;
using TGC.Core.SceneLoader;
using TGC.Core.Utils;
using TGC.Group.Model;

namespace TGC.Camara
{
    class CamaraTW
    {
        //Camara
        private TgcThirdPersonCamera CamaraInterna;

        //Seteo de camara
        private float OffSetHeight;
        private float OffSetForward;
        private Vector3 TargetDisplacement;

        public CamaraTW(Vector3 PosicionAuto)
        {
            this.CamaraInterna = new TgcThirdPersonCamera (PosicionAuto, 120, 280);
            this.OffSetHeight = CamaraInterna.OffsetHeight;
            this.OffSetForward = CamaraInterna.OffsetForward;
            this.TargetDisplacement = CamaraInterna.TargetDisplacement;

            return;
        }

        public void AjustarPosicionDeCamara(Vector3 MeshPosition, float RotationAngle)
        {
            Vector3 position, target, q;
            float distSq = 0, minDistSq = 0, newOffsetForward = 0;

            //Actualizo la posición
            this.CamaraInterna.rotateY(RotationAngle);
            this.CamaraInterna.Target = MeshPosition;
            this.CamaraInterna.OffsetHeight = this.OffSetHeight - 10;
            this.CamaraInterna.OffsetForward = this.OffSetForward;
            this.CamaraInterna.TargetDisplacement = this.TargetDisplacement;

            //Pedirle a la camara cual va a ser su proxima posicion
            this.CamaraInterna.CalculatePositionTarget(out position, out target);

            //Detectar colisiones entre el segmento de recta camara-personaje y todos los objetos del escenario
            minDistSq = FastMath.Pow2(this.OffSetForward);

            foreach (TgcMesh unMesh in GameModel.MeshPrincipal)
            {
                //Hay colision del segmento camara-personaje y el objeto
                if (TgcCollisionUtils.intersectSegmentAABB(target, position, unMesh.BoundingBox, out q))
                {
                    //Si hay colision, guardar la que tenga menor distancia
                    distSq = Vector3.Subtract(q, target).LengthSq();

                    //Hay dos casos singulares, puede que tengamos mas de una colision hay que quedarse con el menor offset.
                    //Si no dividimos la distancia por 2 se acerca mucho al target.
                    minDistSq = FastMath.Min(distSq / 2, minDistSq);
                }
            }

            //Acercar la camara hasta la minima distancia de colision encontrada (pero ponemos un umbral maximo de cercania)
            newOffsetForward = FastMath.Sqrt(minDistSq);

            if (FastMath.Abs(newOffsetForward) < 10)
            {
                newOffsetForward = 10;
            }

            this.CamaraInterna.OffsetForward = newOffsetForward;

            //Asignar la ViewMatrix haciendo un LookAt desde la posicion final anterior al centro de la camara
            this.CamaraInterna.CalculatePositionTarget(out position, out target);
            this.CamaraInterna.SetCamera(position, target);
        }

        public TgcCamera GetCamera()
        {
            return this.CamaraInterna;
        }

        public void Update(Vector3 MeshPosition, float RotationAngle)
        {
            this.AjustarPosicionDeCamara(MeshPosition, RotationAngle);
        }

        public void Render()
        { }

        public void Dispose()
        { }
    }
}
