using TGC.Core.Camara;
using TGC.Core.Input;

namespace TGC.Group.Model {
    interface Escena {
        void init(string mediaDir);
        void update(float deltaTime, TgcD3dInput input, TgcCamera camara);
        void render(float deltaTime);
        void dispose();
    }
}
