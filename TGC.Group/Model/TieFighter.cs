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
        private Nave jugador;
        private float coolDownDisparo;

        //private TgcBoundingAxisAlignBox boundingBox;

        public TieFighter(string mediaDir, TGCVector3 posicionInicial,Nave jugador)
        {
            this.mediaDir = mediaDir;
            this.posicion = posicionInicial;
            this.modeloNave = new ModeloCompuesto(mediaDir + "XWing\\xwing-TgcScene.xml", posicion);
            this.jugador = jugador;
            coolDownDisparo = 0f;
        }


        public void Init()
        {

            matrizEscala = TGCMatrix.Scaling(.5f, .5f, .5f);
            matrizPosicion = TGCMatrix.Translation(posicion);
            TGCQuaternion rotacionInicial = TGCQuaternion.RotationAxis(new TGCVector3(0.0f, 1.0f, 0.0f), Geometry.DegreeToRadian(90f));
            matrizRotacion = TGCMatrix.RotationTGCQuaternion(rotacionInicial);
        }
        
        public void Update(float elapsedTime)
        {
            modeloNave.CambiarRotacion(new TGCVector3(0f, Geometry.DegreeToRadian(0f), 0f));
            if (naveEstaMuyLejos())
            {
                Acercarse(elapsedTime);
            }
            else
            {
                IrALaVelocidadDeLaNave(elapsedTime);
                Disparar(jugador.GetPosicion(), elapsedTime);
            }
            //matrizEscala * matrizRotacion *matrizPosicion;
        }
        private bool naveEstaMuyLejos()
        {
            return posicion.Z - jugador.GetPosicion().Z >= 100f;
        }
        public void Render()
        {
            modeloNave.AplicarTransformaciones();
            modeloNave.Render();
            new TgcText2D().drawText("COOLDOWN: " + coolDownDisparo.ToString(), 5, 250, Color.White);

        }
        public void Dispose()
        {
            modeloNave.Dispose();
        }
        private void Acercarse(float elapsedTime)
        {
            TGCVector3 versorDirector = new TGCVector3(0, 0, -1);
            TGCVector3 movimientoDelFrame = new TGCVector3(0, 0, 0);
            TGCVector3 movimientoAdelante = new TGCVector3(0, 0, 1);
            movimientoDelFrame += versorDirector + movimientoAdelante;
            movimientoDelFrame *= 10f * elapsedTime * 8f;
            posicion += movimientoDelFrame;
            modeloNave.CambiarPosicion(posicion);
        }
        private void IrALaVelocidadDeLaNave(float elapsedTime)
        {
            TGCVector3 versorDirector = new TGCVector3(0, 0, 1);
            TGCVector3 movimientoDelFrame = new TGCVector3(0, 0, 0);
            TGCVector3 movimientoAdelante = new TGCVector3(0, 0, 1);
            movimientoDelFrame += versorDirector + movimientoAdelante;
            movimientoDelFrame *= 10f * elapsedTime * jugador.getVelocidadBase();
            posicion += movimientoDelFrame;
            modeloNave.CambiarPosicion(posicion);
        }
        private void Disparar(TGCVector3 posicionNave,float tiempoTranscurrido)
        {
            coolDownDisparo += tiempoTranscurrido;
            posicionNave.Z += 15f; //TODO modificar este valor respecto la velocidad de la nave
            TGCVector3 direccionDisparo = posicionNave - posicion;
            if(coolDownDisparo > 4f)
            {
                GameManager.Instance.AgregarRenderizable(new LaserEnemigo(mediaDir, posicion, direccionDisparo, jugador));
                coolDownDisparo = 0f;
            }
            

        }
    }
}
