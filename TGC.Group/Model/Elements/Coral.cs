using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Items;

namespace TGC.Group.Model.Elements
{
    class Coral : Element
    {
        public override IItem item { get; } = new Gold();

        public Coral(TgcMesh model, RigidBody rigidBody) : base(model, rigidBody)
        {
        }
    }
}
