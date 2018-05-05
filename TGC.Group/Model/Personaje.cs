using Microsoft.DirectX.DirectInput;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SkeletalAnimation;

namespace TGC.Group.Model {
    class Personaje {
        private TGCVector3 vel;
        private TgcSkeletalMesh mesh;
        private bool moving = false;
        private const float WALK_SPEED = 5f;

        public Personaje(string MediaDir) {
            vel = TGCVector3.Empty;
            var skeletalLoader = new TgcSkeletalLoader();

            mesh = skeletalLoader.loadMeshAndAnimationsFromFile(
                MediaDir + "Robot\\Robot-TgcSkeletalMesh.xml",
                MediaDir + "Robot\\",
                new[] {
                     MediaDir + "Robot\\Caminando-TgcSkeletalAnim.xml",
                     MediaDir + "Robot\\Parado-TgcSkeletalAnim.xml"
                }
            );

            mesh.buildSkletonMesh();
            mesh.playAnimation("Parado", true);
            mesh.Scale = new TGCVector3(0.5f, 0.5f, 0.5f);
            mesh.Position = new TGCVector3(300, -85, 0);
        }

        public void update(float deltaTime, TgcD3dInput Input) {
            if (Input.keyDown(Key.W) || Input.keyDown(Key.UpArrow)) {
                vel.Z = -WALK_SPEED;
                moving = true;
            }

            if (Input.keyDown(Key.S) || Input.keyDown(Key.DownArrow)) {
                vel.Z = WALK_SPEED;
                moving = true;
            }

            if (Input.keyDown(Key.D) || Input.keyDown(Key.RightArrow)) {
                vel.X = -WALK_SPEED;
                moving = true;
            }

            if (Input.keyDown(Key.A) || Input.keyDown(Key.LeftArrow)) {
                vel.X = WALK_SPEED;
                moving = true;
            }

            if (Input.keyDown(Key.LeftShift)) {
                vel.X = vel.X * 2;
                vel.Z = vel.Z * 2;
            } else if (Input.keyDown(Key.LeftAlt)) {
                vel.X = vel.X / 2;
                vel.Z = vel.Z / 2;
            }

            if (moving) {
                mesh.playAnimation("Caminando", true);
            } else {
                mesh.playAnimation("Parado", true);
            }

            //Aplicar movimiento hacia adelante o atras segun la orientacion actual del Mesh
            var lastPos = mesh.Position;

            mesh.Move(vel);
            vel.X = 0;
            vel.Z = 0;
            moving = false;
        }

        public void render(float deltaTime) {
            mesh.UpdateMeshTransform();
            mesh.animateAndRender(deltaTime);
        }

        public void dispose() {
            mesh.Dispose();
        }

        public TGCVector3 getPosition() {
            return mesh.Position;
        }


    }
}
