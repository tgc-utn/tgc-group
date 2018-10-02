using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    public abstract class Escenario
    {

        public TgcMesh planoPiso;
        public TgcMesh planoIzq;
        public TgcMesh planoDer;
        public TgcMesh planoFront;
        public TgcMesh planoBack;

        public abstract void Render();
    }
}