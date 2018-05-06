using System;
using System.Drawing;
using TGC.Core.BoundingVolumes;
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
        private TGCVector3 cajaMovement;
        private bool empujando;


        public override void Init() {
            cameraOffset = new TGCVector3(0, 200, 400);
            personaje = new Personaje(MediaDir);
            nivel = new Nivel(MediaDir);

            collisionManager = new SphereCollisionManager();
            collisionManager.GravityEnabled = true;
            collisionManager.GravityForce = new TGCVector3(0, -0.3f, 0);
            collisionManager.SlideFactor = 1f;
        }

        public override void Update() {
            PreUpdate();

            // calculo nueva velocidad
            personaje.update(ElapsedTime, Input);
            
            // reviso si debo empujar alguna caja
            foreach (var caja in nivel.getCajas()) {
                if (TgcCollisionUtils.testSphereAABB(personaje.getBoundingSphere(), caja.getCuerpo())) {
                    var boundingSphereSimilar = new TgcBoundingSphere(caja.getCuerpo().Position, caja.getCuerpo().calculateBoxRadius());

                    // saco la dirección del movimiento restando las dos posiciones
                    var cajaMovementDeseado = TGCVector3.Normalize(caja.getCuerpo().calculateBoxCenter() - personaje.getBoundingSphere().Center) * 5;
                    cajaMovementDeseado.Y = 0;

                    // no se porque devuelve siempre TGCVector3.empty
                    cajaMovement = collisionManager.moveCharacter(
                        boundingSphereSimilar,
                        cajaMovementDeseado,
                        nivel.getBoundingBoxes()
                    );

                    Console.WriteLine(cajaMovementDeseado);
                    Console.WriteLine("-----");
                    Console.WriteLine(cajaMovement);

                    //caja.move(cajaMovement); // <--- debería ser así
                    caja.move(cajaMovementDeseado); //TEMPORAL
                    empujando = true;
                } else {
                    empujando = false;
                }
            }

            // obtengo movimiento real en base a la velocidad que tengo del personaje 
            // y las colisiones que puedan ocurrir
            movement = collisionManager.moveCharacter(
                personaje.getBoundingSphere(),
                personaje.getVelocity(),
                nivel.getBoundingBoxes()
            );

            // muevo al personaje
            personaje.move(movement);

            // checkeo si estoy sobre hielo
            foreach (var box in nivel.getBoundingBoxes()) {
                if (TgcCollisionUtils.testSphereAABB(personaje.getPies(), box)) {
                    personaje.aterrizar();
                    personaje.setPatinando(nivel.esPisoResbaladizo(box));
                }
            }

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
            DrawText.drawText(string.Format("empujando: ({0})", empujando), 0, 40, Color.White);

            nivel.render();
            personaje.render(ElapsedTime);

            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        public override void Dispose() {
            personaje.dispose();
            nivel.dispose();
        }
    }
}