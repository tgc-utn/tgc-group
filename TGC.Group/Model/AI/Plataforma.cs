using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Collision;
using TGC.Core.SkeletalAnimation;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model.AI
{
    class Plataforma 
    {
        public TgcMesh plataformaMesh;
        private Escenario escenario;
        public TGCVector3 vectorMovimiento = new TGCVector3(0,0,0);

        public Plataforma(TgcMesh plataformaMesh, Escenario escenario)
        {
            this.plataformaMesh = plataformaMesh;
            this.escenario = escenario;
            
        }

        public virtual bool colisionaConPersonaje(TgcBoundingSphere esferaPersonaje)
        {
            
            return TgcCollisionUtils.testSphereAABB(esferaPersonaje, plataformaMesh.BoundingBox);
        }

        public virtual TGCVector3 VectorMovimiento()
        {
            return vectorMovimiento;
        }

        public virtual void Update(float tiempo)
        {
            
        }


    }
}
