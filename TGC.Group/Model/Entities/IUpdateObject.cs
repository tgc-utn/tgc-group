using System;
using System.Text;
using TGC.Core.Geometry;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using Microsoft.DirectX;
using System.Windows.Forms;

namespace TGC.Group.Model.Entities
{
    public interface IUpdateObject
    {
        void update(float elapsedTime);
    }
}
