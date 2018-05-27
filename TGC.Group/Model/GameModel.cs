using TGC.Core.Example;
using TGC.Group.Model.Scenes;

namespace TGC.Group.Model {
    public class GameModel : TgcExample {
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir) {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public override void Init() {
            Escena escenaInicial = new GameEscena();
            escenaInicial.init(MediaDir);
            EscenaManager.getInstance().addScene(escenaInicial);
        }

        public override void Update() {
            PreUpdate();
            EscenaManager.getInstance().update(ElapsedTime, Input, Camara);
            PostUpdate();
        }

        public override void Render() {
            PreRender();
            EscenaManager.getInstance().render(ElapsedTime);
            PostRender();
        }

        public override void Dispose() {
            EscenaManager.getInstance().dispose();
        }
    }
}