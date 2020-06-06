using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    public abstract class Colisionable : IRenderizable
    {
        internal Nave naveDelJugador;
        internal TgcMesh mainMesh;

        public Colisionable(Nave naveDelJugador)
        {
            this.naveDelJugador = naveDelJugador;
        }

        public Boolean ColisionaConNave()
        {
            return naveDelJugador.ColisionaConColisionable(this);
        }

        internal virtual void ColisionarConNave()
        {
            naveDelJugador.Chocar();
        }

        public abstract void Init();

        public abstract void Render();

        public abstract void Update(float elapsedTime);

        public abstract void Dispose();

        public TgcBoundingAxisAlignBox GetBoundingBox()
        {
            return mainMesh.BoundingBox;
        }
    }
}
