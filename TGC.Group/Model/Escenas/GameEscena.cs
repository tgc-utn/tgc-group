using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Linq;
using TGC.Core.BoundingVolumes;
using TGC.Core.Camara;
using TGC.Core.Collision;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Examples.Collision.SphereCollision;
using TGC.Group.Model.Niveles;

namespace TGC.Group.Model.Scenes {
    class GameEscena : Escena {
        private Personaje personaje;
        private Nivel nivel;
        private TGCVector3 cameraOffset;

        private TGCVector3 VEC_GRAVEDAD = new TGCVector3(0, -0.25f, 0);

        public void init(string mediaDir) {
            cameraOffset = new TGCVector3(0, 200, 400);
            personaje = new Personaje(mediaDir);

            // TEMP
            setNivel(new NivelDemo(mediaDir));

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

            personaje.move(personaje.getVelocity(), deltaTime, nivel.getBoundingBoxes(), VEC_GRAVEDAD);


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

            // Checkear si toque la levelFinishBox
            if (TgcCollisionUtils.testSphereAABB(personaje.getBoundingSphere(), nivel.getLFBox())) {
                setNivel(nivel.siguienteNivel);
            }

            checkearMuerte();

            if (personaje.getPosition().Y < -1500) personaje.volverAlOrigen();

            // tecla de reset
            if (input.keyPressed(Key.F9)) personaje.volverAlOrigen();

            camara.SetCamera(personaje.getPosition() + cameraOffset, personaje.getPosition());
        }

        public void render(float deltaTime) {
            nivel.render();
            personaje.render(deltaTime);

            TgcText2D t = new TgcText2D();
            t.Text = personaje.getPosition().ToString();
            t.render();
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

        private void checkearMuerte() {
            foreach (var box in nivel.getDeathPlanes()) {
                if (TgcCollisionUtils.testSphereAABB(personaje.getBoundingSphere(), box)) {
                    personaje.volverAlOrigen();
                }
            }
        }

    }
} 