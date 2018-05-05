using System;
using Microsoft.DirectX.DirectInput;
using TGC.Core.BoundingVolumes;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SkeletalAnimation;

namespace TGC.Group.Model {
    class Personaje {
        private TGCVector3 vel;
        private TgcBoundingSphere boundingSphere;
        private TgcSkeletalMesh mesh;
        private bool moving = false;
        private const float WALK_SPEED = 5f;
        private const float JUMP_SPEED = 30f;

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
            mesh.Position = new TGCVector3(0, 1000, 0);

            boundingSphere = new TgcBoundingSphere(
                mesh.BoundingBox.calculateBoxCenter(),
                mesh.BoundingBox.calculateBoxRadius()
            );

        }

        public void update(float deltaTime, TgcD3dInput Input) {
            checkInputs(Input);
            updateAnimations();
        }

        public void render(float deltaTime) {
            mesh.UpdateMeshTransform();
            mesh.animateAndRender(deltaTime);
            boundingSphere.Render();
            resetUpdateVariables();
        }

        public void dispose() {
            mesh.Dispose();
        }

        private void checkInputs(TgcD3dInput Input) {
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

            // TODO: logica de salto
            if (Input.keyDown(Key.Space)) {
                vel.Y = JUMP_SPEED;
            }

            if (Input.keyDown(Key.LeftShift)) {
                vel.X = vel.X * 2;
                vel.Z = vel.Z * 2;
            } else if (Input.keyDown(Key.LeftAlt)) {
                vel.X = vel.X / 2;
                vel.Z = vel.Z / 2;
            }
        }

        private void updateAnimations() {
            if (moving) {
                mesh.playAnimation("Caminando", true);
            } else {
                mesh.playAnimation("Parado", true);
            }
        }
        
        public TGCVector3 getMovement() {
            return vel;
        }

        public void move(TGCVector3 movement) {
            mesh.Move(movement);
            vel = movement;
        }

        private void resetUpdateVariables() {
            vel.X = 0;
            vel.Z = 0;
            moving = false;
        }

        public TGCVector3 getPosition() {
            return mesh.Position;
        }

        internal TgcBoundingSphere getBoundingSphere() {
            return boundingSphere;
        }


    }
}
