using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace TGC.Group.Model {
    class PlataformaAscensor : Plataforma {
        private float altura;
        private float alturaRecorrida;
        private float vel;

        public PlataformaAscensor(TGCVector3 pos, TGCVector3 size, TgcTexture textura, float altura, float vel) 
            : base(pos, size, textura) {
            this.altura = altura;
            this.vel = vel;
        }

        public void update(float deltaTime) {
            box.Move(TGCVector3.Up * vel);
            box.Transform = TGCMatrix.Translation(box.Position);
            alturaRecorrida += vel;

            if (alturaRecorrida > altura || alturaRecorrida < 0) {
                vel *= -1;
            }
        }

        public TGCVector3 getVel() {
            return TGCVector3.Up * vel;
        }

        public void render() {
            box.Render();
            box.BoundingBox.Render();
        }
    }
}
