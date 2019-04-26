using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Input;
using TGC.Core.Camara;
using System.Drawing;
using TGC.Core.Direct3D;
using Microsoft.DirectX.Direct3D;

namespace TGC.Group.Model.Scenes
{
    abstract class Scene
    {
        protected Color backgroundColor = Color.Black;
        protected bool _uses3DCamera = true;
        public bool Uses3DCamera { get { return _uses3DCamera; } }
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
        protected void ClearScreen()
        {
            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, backgroundColor, 1.0f, 0);
        }
    }
}
