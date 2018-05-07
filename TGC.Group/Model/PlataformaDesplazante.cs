using System;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace TGC.Group.Model {
    class PlataformaDesplazante : Plataforma {
        private TGCVector3 posInicial;
        private TGCVector3 posFinal;
        private TGCVector3 vel;
        private TGCVector3 realVel;

        public PlataformaDesplazante(TGCVector3 pos, TGCVector3 size, 
            TgcTexture textura, TGCVector3 posFinal, TGCVector3 vel) : base(pos, size, textura) {
            posInicial = pos;
            this.posFinal = posFinal;
            this.vel = vel;
        }

        public void update(float deltaTime) {
            move(vel);
            realVel = vel * deltaTime;

            if (posInicial.X < posFinal.X) {
                if (posInicial.Z < posFinal.Z) {
                    if (box.Position.X > posFinal.X || box.Position.Z > posFinal.Z ||
                        box.Position.X < posInicial.X || box.Position.Z < posInicial.Z) {
                        changeDirection();
                    }
                } else {
                    if (box.Position.X > posFinal.X || box.Position.Z < posFinal.Z ||
                        box.Position.X < posInicial.X || box.Position.Z > posInicial.Z) {
                        changeDirection();
                    }
                }
            } else {
                if (posInicial.Z < posFinal.Z) {
                    if (box.Position.X < posFinal.X || box.Position.Z > posFinal.Z ||
                        box.Position.X > posInicial.X || box.Position.Z < posInicial.Z) {
                        changeDirection();
                    }
                } else {
                    if (box.Position.X < posFinal.X || box.Position.Z < posFinal.Z ||
                        box.Position.X > posInicial.X || box.Position.Z > posInicial.Z) {
                        changeDirection();
                    }
                }
            }
        }

        private void changeDirection() {
            vel = -vel;
        }

        private void move(TGCVector3 movement) {
            box.Move(movement);
            box.Transform = TGCMatrix.Translation(box.Position);
        }

        public TGCVector3 getVelocity() {
            return vel;
        }

        new public void render() {
            box.Render();
            box.BoundingBox.Render();
        }

    }
}
