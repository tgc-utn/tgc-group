using System.Collections.Generic;
using TGC.Core.Camara;
using TGC.Core.Input;

namespace TGC.Group.Model.Scenes {
    class EscenaManager {
        private Stack<Escena> scenes;
        private static EscenaManager instance;

        // singleton
        private EscenaManager() {
            scenes = new Stack<Escena>();
        }

        public static EscenaManager getInstance() {
            if (instance == null) {
                instance = new EscenaManager();
            }

            return instance;
        }

        public void update(float deltaTime, TgcD3dInput input, TgcCamera camara) {
            scenes.Peek().update(deltaTime, input, camara);
        }

        public void render(float deltaTime) {
            scenes.Peek().render(deltaTime);
        }

        public void dispose() {
            // vuelo todas las escenas
            while (scenes.Count > 0) {
                scenes.Pop().dispose();
            }
        }

        public void addScene(Escena scene) {
            scenes.Push(scene);
        }

        public void goBack() {
            scenes.Pop();
        }
    }
}
