using Microsoft.DirectX.DirectInput;
using TGC.Core.Camara;
using TGC.Core.Collision;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Examples.Collision.SphereCollision;
using TGC.Group.Model.Niveles;

namespace TGC.Group.Model.Scenes {
    class GameEscena : Escena {
        private Personaje personaje;
        private Nivel nivel;
        private TGCVector3 cameraOffset;
        private SphereCollisionManager collisionManager;

        private TGCVector3 VEC_GRAVEDAD = new TGCVector3(0, -0.25f, 0);

        public void init(string mediaDir) {
            cameraOffset = new TGCVector3(0, 200, 400);
            personaje = new Personaje(mediaDir);

            // TEMP
            setNivel(new NivelDemo(mediaDir));

            collisionManager = new SphereCollisionManager();
            collisionManager.GravityEnabled = true;
            collisionManager.GravityForce = VEC_GRAVEDAD;
            collisionManager.SlideFactor = 1.3f;
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

            nivel.update(deltaTime);

            // checkeo sobre que estoy parado
            // TODO: hacer funcion aparte
            foreach (var box in nivel.getPisos()) {
                if (TgcCollisionUtils.testSphereAABB(personaje.getPies(), box)) {
                    if (nivel.esPisoDesplazante(box)) {
                        personaje.addVelocity(nivel.getPlataformaDesplazante(box).getVelocity());
                    } else if (nivel.esPisoRotante(box)) {
                        var plataformaRotante = nivel.getPlataformaRotante(box);
                        personaje.addVelocity(plataformaRotante.getVelAsVector(personaje.getPosition()) * deltaTime);
                        personaje.setRotation(plataformaRotante.getAngle());
                    } else if (nivel.esPisoAscensor(box)) {
                        personaje.addVelocity(nivel.getPlataformaAscensor(box).getVel());
                    }

                    personaje.aterrizar();
                    personaje.setPatinando(nivel.esPisoResbaladizo(box));
                }
            }

            checkearMuerte();

            if (personaje.getPosition().Y < -1500) personaje.volverAlOrigen();

            // tecla de reset
            if (input.keyPressed(Key.F9)) personaje.volverAlOrigen();

            var movement = collisionManager.moveCharacter(
                personaje.getBoundingSphere(),
                personaje.getVelocity(),
                nivel.getBoundingBoxes()
            );

            // muevo al personaje
            personaje.move(movement);


            camara.SetCamera(personaje.getPosition() + cameraOffset, personaje.getPosition());
        }

        public void render(float deltaTime) {
            nivel.render();
            personaje.render(deltaTime);
        }

        public void dispose() {
            personaje.dispose();
            nivel.dispose();
        }

        private void checkearEmpujeCajas() {
            foreach (var caja in nivel.getCajas()) {
                if (TgcCollisionUtils.testSphereAABB(personaje.getBoundingSphere(), caja.getCuerpo())) {

                    // obtengo dirección para mover la caja
                    var distanciaPersonajeCaja = caja.getCuerpo().calculateBoxCenter() - personaje.getBoundingSphere().Center;
                    var cajaMovementDeseado = TGCVector3.Empty;

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

        private void checkearMuerte()
        {
            foreach (var box in nivel.getDeathPlanes())
            {
                if (TgcCollisionUtils.testSphereAABB(personaje.getBoundingSphere(), box))
                {
                    personaje.volverAlOrigen();
                }
            }
        }

    }
}
