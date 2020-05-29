using TGC.Core.BulletPhysics;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Turbo : ObjetoJuego
    {
        private const int TIEMPO_INACTIVO = 30; // Tiempo en segundos que el turbo esta inactivo (o sea, no se puede recoger)
        private int poder; // La cantidad de turbo que le da al auto
        private float time = 0;
        public bool Activo { get; private set; } = true; // Seguramente es mejorable todo esto

        public Turbo(TgcMesh mesh, TGCVector3 translation, int poder = 25, TGCVector3 rotation = new TGCVector3(), float angle = 0) : base(mesh, translation, rotation, angle)
        {
            this.poder = poder;

            cuerpo = BulletRigidBodyFactory.Instance.CreateCylinder(mesh.BoundingBox.calculateSize() * 0.5f, translation, 0);
            cuerpo.CollisionFlags = BulletSharp.CollisionFlags.NoContactResponse | BulletSharp.CollisionFlags.StaticObject;
        }

        public override void Update(float ElapsedTime)
        {
            // TODO: Si esta inactivo, cuando pase TIEMPO_INACTIVO se debe volver a activar
            if (Activo) // Si no esta activo, no actualizamos.
                base.Update(ElapsedTime);
            else
            {
                time += ElapsedTime;
                if(time >= TIEMPO_INACTIVO)
                {
                    time = 0;
                    Activo = true;
                }
            }
        }

        public override void Render()
        {
            if (Activo) // Si no esta activo, no renderizamos. TODO: Estaria bueno tener un shader que lo oscurezca o algo asi
                base.Render();
        }

        public int Usar()
        {
            if (!Activo)
                return 0;

            Activo = false;
            return poder;
        }
    }
}
