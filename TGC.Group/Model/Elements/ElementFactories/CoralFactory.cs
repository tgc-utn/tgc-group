using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Elements.RigidBodyFactories;
using TGC.Group.Model.Resources.Meshes;

namespace TGC.Group.Model.Elements
{
    class CoralFactory: ElementFactory 
    {

        private static CoralFactory instance = null;

        public static CoralFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CoralFactory();
                }
                return instance;
            }
        }
        private CoralFactory() : base(CoralMeshes.All(), new BoxFactory()){}
        protected override Element CreateSpecificElement(TgcMesh mesh, RigidBody rigidBody)
        {
            return new Coral(mesh, rigidBody);
        }
    }
}
