using Microsoft.DirectX.DirectInput;
using System;
using System.Drawing;
using TGC.Core.Collision;
using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Examples.Collision.SphereCollision;

namespace TGC.Group.Model {
    public class GameModel : TgcExample {
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir) {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        private Personaje personaje;
        private Nivel nivel;
        private TGCVector3 cameraOffset;
        // lo saque de un ejemplo, no se si vale?
        private SphereCollisionManager collisionManager;
        // podria ser una variable local, la saque aca para debuggear
        private TGCVector3 movement;

        private TGCVector3 VEC_GRAVEDAD = new TGCVector3(0, -0.25f, 0);

        public override void Init() {
            cameraOffset = new TGCVector3(0, 200, 400);
            personaje = new Personaje(MediaDir);
            nivel = new Nivel(MediaDir);

            collisionManager = new SphereCollisionManager();
            collisionManager.GravityEnabled = true;
            collisionManager.GravityForce = VEC_GRAVEDAD;
            collisionManager.SlideFactor = 10f;
        }

        public override void Update() {
            PreUpdate();

            // muevo plataformas
            nivel.update(ElapsedTime);

            // calculo nueva velocidad
            personaje.update(ElapsedTime, Input);

            checkearEmpujeCajas();
            aplicarGravedadCajas();

            // checkeo sobre que estoy parado
            // TODO: hacer funcion aparte
            foreach (var box in nivel.getPisos()) {
                if (TgcCollisionUtils.testSphereAABB(personaje.getPies(), box)) {
                    if (nivel.esPisoDesplazante(box)) {
                        personaje.addVelocity(nivel.getPlataformaDesplazante(box).getVelocity());
                    } else if (nivel.esPisoRotante(box)) {
                        var plataformaRotante = nivel.getPlataformaRotante(box);
                        personaje.addVelocity(plataformaRotante.getVelAsVector(personaje.getPosition()) * ElapsedTime);
                        personaje.setRotation(plataformaRotante.getAngle());
                    }
                    personaje.aterrizar();
                    personaje.setPatinando(nivel.esPisoResbaladizo(box));
                }
            }

            // manejo de muerte
            // todo: poner en otra funcion
            foreach (var box in nivel.getDeathPlanes()) {
                if (TgcCollisionUtils.testSphereAABB(personaje.getBoundingSphere(), box)) {
                    personaje.volverAlOrigen();
                }
            }

            if (personaje.getPosition().Y < -1500) personaje.volverAlOrigen();

            // tecla de reset
            if (Input.keyPressed(Key.F9)) personaje.volverAlOrigen();

            movement = collisionManager.moveCharacter(
                personaje.getBoundingSphere(),
                personaje.getVelocity(),
                nivel.getBoundingBoxes()
            );

            // muevo al personaje
            personaje.move(movement);


            Camara.SetCamera(personaje.getPosition() + cameraOffset, personaje.getPosition());
            PostUpdate();
        }

        public override void Render() {
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
            PreRender();

            // datos de debug
            var p1 = personaje.getPosition();
            var p2 = personaje.getBoundingSphere().Position;
            var rm = personaje.getVelocity();
            DrawText.drawText(string.Format("vel: ({0}, {1}, {2})", movement.X, movement.Y, movement.Z), 0, 10, Color.White);
            DrawText.drawText(string.Format("mesh: ({0}, {1}, {2})", p1.X, p1.Y, p1.Z), 0, 20, Color.White);
            DrawText.drawText(string.Format("bsphere: ({0}, {1}, {2})", p2.X, p2.Y, p2.Z), 0, 30, Color.White);

            nivel.render();
            personaje.render(ElapsedTime);

            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        public override void Dispose() {
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
    }
}