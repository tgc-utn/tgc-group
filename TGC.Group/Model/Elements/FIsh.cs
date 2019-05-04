using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Elements
{
    class Fish : Element
    {
        public Fish(TgcMesh model, RigidBody rigidBody) : base(model, rigidBody)
        {
        }
    }
}
