using System.Collections.Generic;
using TGC.Core.Camara;
using TGC.Core.Input;

namespace TGC.Group.Model.Scenes {
    class EscenaManager {
        private Stack<Escena> scenes;
        private static EscenaManager instance;
        private string mediaDir;

        private Escena actual;
        private Escena proxima;

        // singleton
        private EscenaManager() {
            scenes = new Stack<Escena>();
        }

        public void setMediaDir(string mediaDir) {
            this.mediaDir = mediaDir;
        }

        public static EscenaManager getInstance() {
            if (instance == null) {
                instance = new EscenaManager();
            }

            return instance;
        }

        public void update(float deltaTime, TgcD3dInput input, TgcCamera camara) {
            if (proxima != null) {
                actual = proxima;
                proxima = null;
            }

            if (actual == null) return;

            actual.update(deltaTime, input, camara);
        }

        public void render(float deltaTime) {
            if (actual == null) return;

            actual.render(deltaTime);
        }

        public void dispose() {
            // vuelo todas las escenas
            while (scenes.Count > 0) {
                scenes.Pop().dispose();
            }
        }

        public void addScene(Escena scene) {
            scene.init(mediaDir);
            scenes.Push(scene);
            proxima = scene;
        }

        public void goBack() {
            scenes.Pop();
            proxima = scenes.Peek();
        }
    }
}
