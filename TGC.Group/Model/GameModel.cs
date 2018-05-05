using Microsoft.DirectX.DirectInput;
using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model {
    public class GameModel : TgcExample {
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir) {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        // Escena de la ciudad.
        private TgcScene scene;

        private Personaje personaje;
        private TGCVector3 cameraOffset;


        public override void Init() {
            scene = new TgcSceneLoader().loadSceneFromFile(MediaDir + "Jungla\\Jungla -TgcScene.xml");
            cameraOffset = new TGCVector3(0, 200, 400);
            personaje = new Personaje(MediaDir);
        }


        public override void Update() {
            PreUpdate();

            Camara.SetCamera(personaje.getPosition() + cameraOffset, personaje.getPosition());
            personaje.update(ElapsedTime, Input);

            PostUpdate();
        }

        public override void Render() {
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
            PreRender();

            scene.RenderAll();
            personaje.render(ElapsedTime);

            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        public override void Dispose() {
            personaje.dispose();
            scene.DisposeAll();
        }
    }
}