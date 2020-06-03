using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class LaserDeJugador : Laser
    {
        public LaserDeJugador(string direccionDeScene, TGCVector3 posicionInicial, TGCVector3 direccion) : base(direccionDeScene,posicionInicial,direccion)
        {
            this.velocidad = 10f;
        }
        public override void Update(float elapsedTime)
        {
            
            TGCQuaternion rotation = TGCQuaternion.RotationAxis(new TGCVector3(1.0f, 0.0f, 0.0f), Geometry.DegreeToRadian(90f));
            TGCVector3 direccionDisparo = direccion;
            direccionDisparo.Normalize();
            TGCVector3 movement = direccionDisparo * 80f * elapsedTime*velocidad;
            mainMesh.Position += movement;
            TGCMatrix matrizTransformacion = baseScaleRotation * TGCMatrix.RotationTGCQuaternion(rotation)
                * TGCMatrix.Translation(mainMesh.Position);
            mainMesh.Transform = matrizTransformacion;
            //mainMesh.updateBoundingBox();
            mainMesh.BoundingBox.transform(matrizTransformacion);
            //base.Update(elapsedTime);

            if (SuperoCiertoTiempoDeVida(1))
            {
                GameManager.Instance.QuitarRenderizable(this);
            }
        }

    }
}
