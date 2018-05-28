using Microsoft.DirectX.Direct3D;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Textures;
using TGC.Group.Model.Escenas;
using TGC.Group.Model.Scenes;

namespace TGC.Group.Model {
    public class GameModel : TgcExample {
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir) {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public override void Init() {
            Escena escenaInicial = new InicioEscena();
            EscenaManager.getInstance().setMediaDir(MediaDir);
            EscenaManager.getInstance().addScene(escenaInicial);
        }

        public override void Update() {
            PreUpdate();

            EscenaManager.getInstance().update(ElapsedTime, Input, Camara);

            PostUpdate();
        }

        public override void Render() {
            // prerender
            // TODO: ver el color/poner skybox
            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.CadetBlue, 1.0f, 0);
            D3DDevice.Instance.Device.BeginScene();
            TexturesManager.Instance.clearAll();

            EscenaManager.getInstance().render(ElapsedTime);

            RenderAxis();
            RenderFPS();
            D3DDevice.Instance.Device.EndScene();
            D3DDevice.Instance.Device.Present();
        }

        public override void Dispose() {
            EscenaManager.getInstance().dispose();
        }
    }
}