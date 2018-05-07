using Microsoft.DirectX.DirectInput;
using TGC.Core.BoundingVolumes;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SkeletalAnimation;

namespace TGC.Group.Model {
    class Personaje {
        private TgcBoundingSphere boundingSphere;
        private TgcBoundingSphere pies;

        // animaciones
        private TgcSkeletalMesh mesh;
        private float meshAngle;
        private float meshAngleAnterior;
        private bool moving = false;

        // movimiento
        private TGCVector3 vel;
        private const float WALK_SPEED = 2f;
        private const float MULT_CORRER = 1.5f;
        private const float MULT_CAMINAR = 0.5f;
        private bool patinando = false;

        // saltos
        private int saltosRestantes = 0;
        private const int SALTOS_TOTALES = 2;
        private const float JUMP_SPEED = 10f;

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
            mesh.Position = new TGCVector3(0, 7.5f, 0);
            meshAngle = 0;
            meshAngleAnterior = 0;

            boundingSphere = new TgcBoundingSphere(
                mesh.BoundingBox.calculateBoxCenter(),
                mesh.BoundingBox.calculateBoxRadius()
            );

            var posicionPies = mesh.BoundingBox.calculateBoxCenter();
            posicionPies.Y = mesh.BoundingBox.PMin.Y;

            pies = new TgcBoundingSphere(posicionPies, 10);
        }

        public void update(float deltaTime, TgcD3dInput Input) {
            checkInputs(Input);
            updateAnimations();
        }

        public void render(float deltaTime) {
            mesh.UpdateMeshTransform();
            mesh.animateAndRender(deltaTime);

            // para debug
            boundingSphere.Render();
            pies.Render();

            // seria un post-update
            resetUpdateVariables();
            if (vel.Length() == 0) moving = false;
        }

        public void dispose() {
            mesh.Dispose();
        }

        private void checkInputs(TgcD3dInput Input) {
            if (Input.keyDown(Key.W) || Input.keyDown(Key.UpArrow)) {
                vel.Z = -WALK_SPEED;
                moving = true;
                meshAngle = 0;
            }

            if (Input.keyDown(Key.S) || Input.keyDown(Key.DownArrow)) {
                vel.Z = WALK_SPEED;
                moving = true;
                meshAngle = 2;
            }

            if (Input.keyDown(Key.D) || Input.keyDown(Key.RightArrow)) {
                vel.X = -WALK_SPEED;
                moving = true;
                meshAngle = 1;
            }

            if (Input.keyDown(Key.A) || Input.keyDown(Key.LeftArrow)) {
                vel.X = WALK_SPEED;
                moving = true;
                meshAngle = 3;
            }
            
            if (Input.keyPressed(Key.Space) && saltosRestantes > 0) {
                vel.Y = JUMP_SPEED;
                saltosRestantes--;
            }

            // HACK: no puede correr mientras patina
            // si pudiera correr mientras patina se hace quilombo con el sliding
            // y le queda una velocidad constante, que podría ser un feature mas que
            // un bug pero no se
            if (Input.keyDown(Key.LeftShift) && !patinando) {
                vel.X = vel.X * MULT_CORRER;
                vel.Z = vel.Z * MULT_CORRER;
            } else if (Input.keyDown(Key.LeftAlt)) {
                vel.X = vel.X * MULT_CAMINAR;
                vel.Z = vel.Z * MULT_CAMINAR;
            }
        }

        private void updateAnimations() {
            if (moving) {
                mesh.playAnimation("Caminando", true);
            } else {
                mesh.playAnimation("Parado", true);
            }

            // no puedo setear un angulo absoluto
            // entonces le resto el angulo anterior
            // y le sumo el nuevo angulo
            mesh.RotateY(-meshAngleAnterior * FastMath.PI / 2);
            mesh.RotateY(meshAngle * FastMath.PI / 2);
            meshAngleAnterior = meshAngle;
        }
        
        public void move(TGCVector3 movement) {
            // deshago el movimiento del manager
            boundingSphere.moveCenter(-movement);
            // limite de velocidad
            if (movement.X > WALK_SPEED * MULT_CORRER) movement.X = WALK_SPEED * 1.5f;
            if (movement.Z > WALK_SPEED * MULT_CORRER) movement.Z = WALK_SPEED * 1.5f;
            if (movement.X < WALK_SPEED * MULT_CORRER * -1) movement.X = WALK_SPEED * 1.5f * -1;
            if (movement.Z < WALK_SPEED * MULT_CORRER * -1) movement.Z = WALK_SPEED * 1.5f * -1;

            mesh.Move(movement);
            pies.moveCenter(movement);
            boundingSphere.moveCenter(movement);

            vel = movement;
        }

        private void resetUpdateVariables() {
            if (! patinando) {
                vel.X = 0;
                vel.Z = 0;
                moving = false;
            }
        }

        public void aterrizar() {
            saltosRestantes = SALTOS_TOTALES;
        }

        public TGCVector3 getPosition() {
            return mesh.Position;
        }

        public TgcBoundingSphere getBoundingSphere() {
            return boundingSphere;
        }

        public TgcBoundingSphere getPies() {
            return pies;
        }

        public TGCVector3 getVelocity() {
            return vel;
        }

        public void setPatinando(bool b) {
            patinando = b;
        }

        public void addVelocity(TGCVector3 velAdded) {
            vel += velAdded;
        }

        public void setRotation(float angle) {
            meshAngle = angle;
        }
    }
}
