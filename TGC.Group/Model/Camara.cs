using TGC.Core.Mathematica;

namespace TGC.Examples.Camara
{
    class Camara : TgcThirdPersonCamera
    {

        #region Constructores 
        //Tal vez no es necesario tener los 3 constructores pero por si acaso los pongo por ahora.
        public Camara(): base()
        {
        }

        public Camara(TGCVector3 target, float offsetHeight, float offsetForward) : base(target,offsetHeight,offsetForward)
        {
        }

        public Camara(TGCVector3 target, TGCVector3 targetDisplacement, float offsetHeight, float offsetForward) : base(target, targetDisplacement, offsetHeight, offsetForward)
        {
        }
        #endregion 


        public void Update(float ElapsedTime)
        {
            var cameraMovement = new TGCVector3(0, 0, 1);
            cameraMovement *= 10f * ElapsedTime;
            Target += cameraMovement;
        }
    }
}
