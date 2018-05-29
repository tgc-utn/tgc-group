using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Linq;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SkeletalAnimation;

namespace TGC.Group.Model {
    class Personaje {
        private TgcBoundingSphere boundingSphere;
        private TgcBoundingSphere pies;

        private TGCVector3 POS_ORIGEN = new TGCVector3(0, 7.5f, 9000);

        // animaciones
        private TgcSkeletalMesh mesh;
        private float meshAngle;
        private float meshAngleAnterior;
        private bool moving = false;

        // movimiento
        private TGCVector3 vel;
        private const float WALK_SPEED = 500f;
        private const float MULT_CORRER = 1.5f;
        private const float MULT_CAMINAR = 0.5f;
        private bool patinando = false;
        private TgcRay rayoVelocidad = new TgcRay();

        // saltos
        private int saltosRestantes = 0;
        private const int SALTOS_TOTALES = 2;
        // private const float JUMP_SPEED = 1000f; PC Cristian
        private const float JUMP_SPEED = 10000f; // PC Pepe

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
            mesh.Position = POS_ORIGEN;
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
            checkInputs(Input, deltaTime);
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

        private void checkInputs(TgcD3dInput Input, float deltaTime) {
            float FRAME_WALK_SPEED = WALK_SPEED * deltaTime;
            float FRAME_JUMP_SPEED = JUMP_SPEED * deltaTime;

            if (Input.keyDown(Key.W) || Input.keyDown(Key.UpArrow)) {
                vel.Z = -FRAME_WALK_SPEED;
                moving = true;
                meshAngle = 0;
            }

            if (Input.keyDown(Key.S) || Input.keyDown(Key.DownArrow)) {
                vel.Z = FRAME_WALK_SPEED;
                moving = true;
                meshAngle = 2;
            }

            if (Input.keyDown(Key.D) || Input.keyDown(Key.RightArrow)) {
                vel.X = -FRAME_WALK_SPEED;
                moving = true;
                meshAngle = 1;
            }

            if (Input.keyDown(Key.A) || Input.keyDown(Key.LeftArrow)) {
                vel.X = FRAME_WALK_SPEED;
                moving = true;
                meshAngle = 3;
            }
            
            if (Input.keyPressed(Key.Space) && saltosRestantes > 0) {
                vel.Y = FRAME_JUMP_SPEED;
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
        
        public void move(TGCVector3 movement, float deltaTime, List<TgcBoundingAxisAlignBox> colliders, TGCVector3 gravedad, int count) {
            if (count > 5) return;

            movimientoVertical(movement, colliders, gravedad);

            rayoVelocidad.Origin = this.getPies().Center;
            rayoVelocidad.Direction = movement;

            TGCVector3 aux = new TGCVector3();

            TgcBoundingAxisAlignBox paredCercana = colliders
                // tomo las aabb que colisionan
                .Where(b => TgcCollisionUtils.intersectRayAABB(rayoVelocidad, b, out aux))
                // ordeno por distancia
                .OrderBy(b => {
                    TgcCollisionUtils.intersectRayAABB(rayoVelocidad, b, out aux);
                    return aux.Length();
                })
                // tomo la primera
                .DefaultIfEmpty(null)
                .First();

            if (paredCercana == null) {
                this.translate(movement);
            } else if (movement.Length() > 1) {
                // llamo a la función una vez mas para obtener la intersección en aux
                TgcCollisionUtils.intersectRayAABB(rayoVelocidad, paredCercana, out aux);

                TGCVector3 radius = 
                    TGCVector3.Normalize(movement) * this.getBoundingSphere().Radius;

                TGCVector3 distance =
                    TgcCollisionUtils.closestPointAABB(this.getBoundingSphere().Center, paredCercana) - this.getBoundingSphere().Center;
                
                if ((radius + movement).Length() < distance.Length()) {
                    this.translate(movement);
                } else {
                    var foo = TGCVector3.Normalize(movement) * distance.Length();
                    movement = TGCVector3.Normalize(foo - distance) * WALK_SPEED * deltaTime;

                    this.move(movement, deltaTime, colliders, gravedad, count + 1);
                }
            }
        }

        public void move(TGCVector3 movement, float deltaTime, List<TgcBoundingAxisAlignBox> colliders, TGCVector3 gravedad) {
            move(movement, deltaTime, colliders, gravedad, 0);
        }

        private void movimientoVertical(TGCVector3 movement, List<TgcBoundingAxisAlignBox> colliders TGCVector3 gravedad) {

        }

        private void translate(TGCVector3 movement) {
            mesh.Move(movement);
            pies.moveCenter(movement);
            boundingSphere.moveCenter(movement);
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

        public void volverAlOrigen() {
            mesh.Position = POS_ORIGEN;
            boundingSphere.setValues(mesh.BoundingBox.calculateBoxCenter(), mesh.BoundingBox.calculateBoxRadius());

            var posicionPies = mesh.BoundingBox.calculateBoxCenter();
            posicionPies.Y = mesh.BoundingBox.PMin.Y;
            pies.setValues(posicionPies, 10);

            vel = TGCVector3.Empty;
        }
    }
}
