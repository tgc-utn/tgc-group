using System.Collections.Generic;
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
        private TGCVector3 cameraOffset;
        private Nivel nivel;
        // lo saque de un ejemplo, no se si vale?
        private SphereCollisionManager collisionManager;
        // podria ser una variable local, la saque aca para debuggear
        private TGCVector3 movement;


        public override void Init() {
            cameraOffset = new TGCVector3(0, 200, 400);
            personaje = new Personaje(MediaDir);
            nivel = new Nivel(MediaDir);

            collisionManager = new SphereCollisionManager();
            collisionManager.GravityEnabled = true;
            collisionManager.GravityForce = new TGCVector3(0, -1, 0);
            collisionManager.SlideFactor = 1f;

        }

        public override void Update() {
            PreUpdate();

            personaje.update(ElapsedTime, Input);

            movement = collisionManager.moveCharacter(
                personaje.getBoundingSphere(),
                personaje.getMovement(),
                nivel.getBoundingBoxes()
            );

            personaje.move(movement);

            foreach (var box in nivel.getBoundingBoxes()) {
                if (TgcCollisionUtils.testSphereAABB(personaje.getPies(), box)) {
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
    }
}