using TGC.Core.Mathematica;
using TGC.Core.Text;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Collision;
using System;
using System.Collections.Generic;
using System.Linq;
using TGC.Core.BoundingVolumes;
using System.Drawing.Text;

namespace TGC.Group.Model
{
    class TieFighter : IRenderizable
    {
        private TGCVector3 posicion;
        private ModeloCompuesto modeloNave;
        private string mediaDir;
        private TGCMatrix matrizEscala;
        private TGCMatrix matrizPosicion;
        private TGCMatrix matrizRotacion;

        //private TgcBoundingAxisAlignBox boundingBox;

        public TieFighter(string mediaDir, TGCVector3 posicionInicial)
        {
            this.mediaDir = mediaDir;
            this.posicion = posicionInicial;
            this.modeloNave = new ModeloCompuesto(mediaDir + "XWing\\xwing-TgcScene.xml", posicion);
        }


        public void Init()
        {

            matrizEscala = TGCMatrix.Scaling(.5f, .5f, .5f);
            matrizPosicion = TGCMatrix.Translation(posicion);
            TGCQuaternion rotacionInicial = TGCQuaternion.RotationAxis(new TGCVector3(0.0f, 1.0f, 0.0f), Geometry.DegreeToRadian(90f));
            matrizRotacion = TGCMatrix.RotationTGCQuaternion(rotacionInicial);
        }
        
        public void Update(float ElapsedTime)
        {
            modeloNave.CambiarRotacion(new TGCVector3(0f, Geometry.DegreeToRadian(180f), 0f));
            //matrizEscala * matrizRotacion *matrizPosicion;
        }

        public void Render()
        {
            modeloNave.Render();
        }
        public void Dispose()
        {
            modeloNave.Dispose();
        }
    }
}
