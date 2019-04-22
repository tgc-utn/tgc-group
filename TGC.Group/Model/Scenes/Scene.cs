using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Input;
using TGC.Core.Camara;

namespace TGC.Group.Model.Scenes
{
    abstract class Scene
    {
        private TgcCamera _camera = null;
        public TgcCamera Camera
        {
            set { _camera = value; }
            get { return _camera ?? new TgcCamera(); }
        }
        protected TgcD3dInput Input { get; set; }

        protected Scene(TgcD3dInput Input)
        {
            this.Input = Input;
        }

        abstract public void Update();
        abstract public void Render();
        virtual public void Dispose() {}
    }
}
