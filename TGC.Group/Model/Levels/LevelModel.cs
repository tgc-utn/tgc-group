using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Levels
{
    public abstract class LevelModel
    {
        public virtual void Init() { }
        public virtual void Update(float elapsedTime) { }
        public virtual void Render(float elapsedTime) { }
        public virtual void Dispose() { }
    }
}
