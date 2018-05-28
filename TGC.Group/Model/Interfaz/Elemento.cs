using System.Drawing;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.Text;

namespace TGC.Group.Model.Interfaz {
    public abstract class Elemento : IRenderObject {
        protected TgcText2D texto;
        protected Size size;
        protected Font DEFAULT_FONT = new Font("Arial", 30, FontStyle.Regular, GraphicsUnit.Pixel);

        // las posiciones son un valor entre 0 y 1, indican en que parte de la pantalla quiero que esté.
        // x ej, en el medio serían 0.5f y 0.5f
        public Elemento(string contenido, float xpos, float ypos) {
            var viewport = D3DDevice.Instance.Device.Viewport;

            texto = new TgcText2D {
                Text = contenido,
                Color = Color.White
            };

            // por defecto un tgctext2d tiene size = viewport
            // textrenderer me da mas o menos 1.5 veces mas chico
            // es un hack, pero fue
            size = TextRenderer.MeasureText(texto.Text, DEFAULT_FONT);
            size.Width = (int) (size.Width * 1.5);
            texto.Size = size;

            texto.changeFont(DEFAULT_FONT);

            texto.Position = new Point(
                (int)((xpos * viewport.Width)),
                (int)((ypos * viewport.Height))
            );

        }

        public bool AlphaBlendEnable { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public abstract void Update(float deltaTime, TgcD3dInput input);
        public abstract void Render();
        public abstract void Dispose();

        public Rectangle getRect() {
            return new Rectangle {
                X = texto.Position.X,
                Y = texto.Position.Y,
                Width = size.Width,
                Height = size.Height
            };
        }

    }
}
