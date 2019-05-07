using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using BulletSharp;
using Microsoft.DirectX.Direct3D;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Items;

namespace TGC.Group.Model.Elements
{
    class Fish : Element
    {
        public Fish(TgcMesh model, RigidBody rigidBody) : base(model, rigidBody)
        {
        }

        public override IItem item { get; } = new Items.Fish();
    }
}
