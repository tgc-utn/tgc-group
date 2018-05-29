using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Camara;
using TGC.Core.Collision;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Textures;
using TGC.Group.Model.Niveles;

namespace TGC.Group.Model.Scenes {
    class GameEscena : Escena {
        private Personaje personaje;
        private Nivel nivel;
        private TGCVector3 cameraOffset;

        private TGCVector3 VEC_GRAVEDAD = new TGCVector3(0, -25f, 0);

        private Sprite hud;
        private TgcTexture barraStamina;
        private TgcTexture unidadStamina;
        private TgcTexture vida;

        public void init(string mediaDir) {
            cameraOffset = new TGCVector3(0, 200, 400);
            setNivel(new Nivel3(mediaDir));
            personaje = new Personaje(mediaDir);

            hud = new Sprite(D3DDevice.Instance.Device);
            barraStamina = TgcTexture.createTexture(mediaDir + "stamina.png");
            unidadStamina = TgcTexture.createTexture(mediaDir + "staminaUnidad.png");
            vida = TgcTexture.createTexture(mediaDir + "vida.png");
        }

        public void setNivel(Nivel nuevoNivel) {
            if (nivel != null) nivel.dispose();
            nivel = nuevoNivel;
        }

        public void update(float deltaTime, TgcD3dInput input, TgcCamera camara) {

            // calculo nueva velocidad
            personaje.update(deltaTime, input);

            checkearEmpujeCajas();
            aplicarGravedadCajas();

            personaje.move(deltaTime, nivel, VEC_GRAVEDAD);

            nivel.update(deltaTime);

            // Checkear si toque la levelFinishBox
            if (nivel.getLFBox() != null && TgcCollisionUtils.testSphereAABB(personaje.getBoundingSphere(), nivel.getLFBox())) {
                if (nivel.siguienteNivel == null) {
                    EscenaManager.getInstance().goBack();
                } else {
                    setNivel(nivel.siguienteNivel);
                }
            }

            checkearMuerte();

            if (personaje.getPosition().Y < -1500) personaje.morir();

            // tecla de reset
            if (input.keyPressed(Key.F9)) personaje.morir();

            camara.SetCamera(personaje.getPosition() + cameraOffset, personaje.getPosition());
        }

        public void render(float deltaTime) {
            nivel.render();
            personaje.render(deltaTime);

            hud.Begin(SpriteFlags.None);
            hud.Transform = TGCMatrix.Scaling(TGCVector3.One);
            hud.Draw(barraStamina.D3dTexture, Vector3.Empty, Vector3.Empty, 16777215);
            hud.Transform = TGCMatrix.Scaling(new TGCVector3(personaje.getStamina() / 200f * 256f, 2, 1));
            hud.Draw(unidadStamina.D3dTexture, Vector3.Empty, Vector3.Empty, 16777215);

            int posVidas = D3DDevice.Instance.Device.Viewport.Width - vida.Width;

            for (int i = 0; i < personaje.getVidas(); i++) {
                hud.Transform = TGCMatrix.Translation(new TGCVector3(posVidas, 0, 0));
                hud.Draw(vida.D3dTexture, Vector3.Empty, Vector3.Empty, 16777215);
                posVidas -= vida.Width;
            }


            hud.End();
        }

        public void dispose() {
            personaje.dispose();
            nivel.dispose();
            unidadStamina.dispose();
            barraStamina.dispose();
        }

        private void checkearEmpujeCajas() {
            foreach (var caja in nivel.getCajas()) {
                if (TgcCollisionUtils.testSphereAABB(personaje.getBoundingSphere(), caja.getCuerpo())) {

                    // obtengo dirección para mover la caja
                    var distanciaPersonajeCaja = caja.getCuerpo().calculateBoxCenter() - personaje.getBoundingSphere().Center;
                    var cajaMovementDeseado = TGCVector3.Empty;

                    // TODO: Implementar checkearColisionesCajaEstaticos y evitar que queden pegadas
                    if (FastMath.Abs(distanciaPersonajeCaja.X) > FastMath.Abs(distanciaPersonajeCaja.Z)) {
                        if (distanciaPersonajeCaja.X > 0) {
                            cajaMovementDeseado = new TGCVector3(5, 0, 0);
                        } else {
                            cajaMovementDeseado = new TGCVector3(-5, 0, 0);
                        }
                    } else {
                        if (distanciaPersonajeCaja.Z > 0) {
                            cajaMovementDeseado = new TGCVector3(0, 0, 5);
                        } else {
                            cajaMovementDeseado = new TGCVector3(0, 0, -5);
                        }
                    }

                    caja.move(cajaMovementDeseado); //TEMP
                }
            }
        }

        // Checkeo si la caja esta colisionando con una plataforma estatica o pared
        private bool checkearColisionCajaEstaticos(Caja unaCaja)
        {

            foreach(var estatico in nivel.getEstaticos())
            {
                if (TgcCollisionUtils.testAABBAABB(unaCaja.getCuerpo(), estatico))
                {
                    return true;
                }
            }
            return false;

        }

        private void aplicarGravedadCajas() {
            foreach (var caja in nivel.getCajas()) {
                var apoyada = false;
                foreach (var piso in nivel.getPisos()) {
                    if (TgcCollisionUtils.testAABBAABB(caja.getCuerpo(), piso)) {
                        apoyada = true;
                    }
                }

                if (!apoyada) {
                    caja.addVel(VEC_GRAVEDAD);
                } else {
                    caja.resetVel();
                }

                caja.applyGravity();
            }
        }

        private void checkearMuerte() {
            foreach (var box in nivel.getDeathPlanes()) {
                if (TgcCollisionUtils.testSphereAABB(personaje.getBoundingSphere(), box)) {
                    personaje.morir();
                }
            }
        }

    }
} 