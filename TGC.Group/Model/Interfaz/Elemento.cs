using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.SceneLoader;
using TGC.Core.Text;

namespace TGC.Group.Model.Interfaz {
    public abstract class Elemento : IRenderObject, IUpdateable {
        protected TgcText2D texto;
        protected Font DEFAULT_FONT = new Font("Arial", 60, FontStyle.Regular, GraphicsUnit.Pixel);

        // las posiciones son un valor entre 0 y 1, indican en que parte de la pantalla quiero que esté.
        // x ej, en el medio serían 0.5f y 0.5f
        public Elemento(string contenido, float xpos, float ypos) {
            var viewport = D3DDevice.Instance.Device.Viewport;

            // todo: revisar los casteos
            texto = new TgcText2D {
                Text = contenido,
                Color = Color.White
            };

            texto.Position = new Point(
                (int)(xpos * viewport.Width) - texto.Size.Width / 2,
                (int)(ypos * viewport.Height)
            );


            texto.changeFont(DEFAULT_FONT);
        }

        public bool AlphaBlendEnable { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public abstract void Update(float deltaTime);
        public abstract void Render();
        public abstract void Dispose();

    }
}
