using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Elements.RigidBodyFactories;
using TGC.Group.Model.Resources.Meshes;

namespace TGC.Group.Model.Elements.ElementFactories
{
    class FishFactory : ElementFactory
    {
        private static FishFactory instance = null;
        public static FishFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FishFactory();
                }
                return instance;
            }
        }

        private FishFactory() : base(FishMeshes.All(), new CapsuleFactory())
        {
        }

        protected new Element Generate(TgcMesh mesh, RigidBody rigidBody)
        {
            return new Fish(mesh, rigidBody);
        }

    }
}
