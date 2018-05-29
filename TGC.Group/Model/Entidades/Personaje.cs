﻿using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Linq;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SkeletalAnimation;
using TGC.Core.Text;
using TGC.Group.Model.Niveles;

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
        private TGCVector3 dir;
        private TGCVector3 vel;
        private const float WALK_SPEED = 500f;
        private const float MULT_CORRER = 1.5f;
        private const float MULT_CAMINAR = 0.5f;
        private const float VEL_TERMINAL = -10;
        private const float MODIFICADOR_HIELO = 0.75f;
        private TgcRay rayoVelocidad = new TgcRay();

        private TGCVector3 distance;

        // saltos
        private int saltosRestantes = 0;
        private const int SALTOS_TOTALES = 2;
        private const float JUMP_SPEED = 10f;

        public Personaje(string MediaDir) {
            dir = TGCVector3.Empty;
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

            pies = new TgcBoundingSphere(posicionPies, 20);
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

            TgcText2D t = new TgcText2D();
            t.Text = distance.ToString();
            t.render();


            // seria un post-update
            resetUpdateVariables();
            if (dir.Length() == 0) moving = false;
        }

        public void dispose() {
            mesh.Dispose();
        }

        private void checkInputs(TgcD3dInput Input) {
            float FRAME_WALK_SPEED = WALK_SPEED;
            dir = TGCVector3.Empty;

            if (Input.keyDown(Key.W) || Input.keyDown(Key.UpArrow)) {
                dir.Z = -FRAME_WALK_SPEED;
                moving = true;
                meshAngle = 0;
            }

            if (Input.keyDown(Key.S) || Input.keyDown(Key.DownArrow)) {
                dir.Z = FRAME_WALK_SPEED;
                moving = true;
                meshAngle = 2;
            }

            if (Input.keyDown(Key.D) || Input.keyDown(Key.RightArrow)) {
                dir.X = -FRAME_WALK_SPEED;
                moving = true;
                meshAngle = 1;
            }

            if (Input.keyDown(Key.A) || Input.keyDown(Key.LeftArrow)) {
                dir.X = FRAME_WALK_SPEED;
                moving = true;
                meshAngle = 3;
            }
            
            if (Input.keyPressed(Key.Space) && saltosRestantes > 0) {
                dir.Y = JUMP_SPEED;
                saltosRestantes--;
            }

            if (Input.keyDown(Key.LeftShift)) {
                dir.X = dir.X * MULT_CORRER;
                dir.Z = dir.Z * MULT_CORRER;
            } else if (Input.keyDown(Key.LeftAlt)) {
                dir.X = dir.X * MULT_CAMINAR;
                dir.Z = dir.Z * MULT_CAMINAR;
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
        
        public void move(float deltaTime, Nivel nivel, TGCVector3 gravedad) {
            TGCVector3 velAnterior = vel;
            vel = TGCVector3.Empty;
            vel.Y = velAnterior.Y;

            // movimiento por teclado
            vel += dir * deltaTime;

            if (dir.Y != 0) vel.Y = dir.Y;

            // movimiento por entorno
            TgcBoundingAxisAlignBox piso = nivel.getBoundingBoxes().Find(b => TgcCollisionUtils.testSphereAABB(pies, b));

            if (piso == null) {
                // si estoy en el aire
                if (vel.Y > VEL_TERMINAL) {
                    vel += gravedad * deltaTime;
                }
            } else {
                // si estoy en algun piso
                dir.Y = 0;
                aterrizar();
                modificarMovimientoSegunPiso(piso, nivel, deltaTime);
            }

            TGCVector3 horizontal = new TGCVector3 {
                X = vel.X,
                Y = 0,
                Z = vel.Z
            };

            TGCVector3 vertical = new TGCVector3 {
                X = 0,
                Y = vel.Y,
                Z = 0
            };

            movimientoHorizontal(horizontal, nivel.getBoundingBoxes(), deltaTime, 0);
            movimientoVertical(vertical, nivel.getBoundingBoxes(), deltaTime, 0);

            resetearMovimientoSegunPiso(piso, nivel);
        }

        private void modificarMovimientoSegunPiso(TgcBoundingAxisAlignBox piso, Nivel nivel, float deltaTime) {
            if (nivel.esPisoDesplazante(piso)) {
                vel += nivel.getPlataformaDesplazante(piso).getVelocity() * deltaTime;
            } else if (nivel.esPisoAscensor(piso)) {
                vel += nivel.getPlataformaAscensor(piso).getVel() * deltaTime;
            } else if (nivel.esPisoRotante(piso)) {
                setRotation(nivel.getPlataformaRotante(piso).getAngle());
            }
        }

        private void resetearMovimientoSegunPiso(TgcBoundingAxisAlignBox piso, Nivel nivel) {
            if (nivel.esPisoResbaladizo(piso)) {
                vel.X = vel.X * MODIFICADOR_HIELO;
                vel.Z = vel.Z * MODIFICADOR_HIELO;
            } else {
                vel.X = 0;
                vel.Z = 0;
            }
        }

        private void movimientoHorizontal(TGCVector3 movement, List<TgcBoundingAxisAlignBox> colliders, float deltaTime, int count) {
            if (count > 5) return;

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
            } else {
                // llamo a la función una vez mas para obtener la intersección en aux
                TgcCollisionUtils.intersectRayAABB(rayoVelocidad, paredCercana, out aux);

                TGCVector3 radius = 
                    TGCVector3.Normalize(movement) * this.getBoundingSphere().Radius;

                TGCVector3 distance =
                    TgcCollisionUtils.closestPointAABB(this.getBoundingSphere().Center, paredCercana) - this.getBoundingSphere().Center;

                if ((distance - radius).Length() < 1) return;

                if ((radius + movement).Length() < distance.Length()) {
                    this.translate(movement);
                } else {
                    var foo = TGCVector3.Normalize(movement) * distance.Length();
                    movement = TGCVector3.Normalize(foo - distance) * WALK_SPEED * deltaTime;

                    this.movimientoHorizontal(movement, colliders, deltaTime, count + 1);
                }
            }
        }

        private void movimientoVertical(TGCVector3 movement, List<TgcBoundingAxisAlignBox> colliders, float deltaTime, int count) {
            if (count > 5) return;

            rayoVelocidad.Origin = this.getPies().Center;
            rayoVelocidad.Direction = movement;

            TGCVector3 aux = new TGCVector3();

            TgcBoundingAxisAlignBox pisoCercano = colliders
                .Where(b => TgcCollisionUtils.intersectRayAABB(rayoVelocidad, b, out aux))
                .OrderBy(b => {
                    TgcCollisionUtils.intersectRayAABB(rayoVelocidad, b, out aux);
                    return aux.Length();
                })
                .DefaultIfEmpty(null)
                .First();

            // no tengo piso abajo (caso raro) 
            // o salté y no hay ningún techo (caso comun)
            if (pisoCercano == null) {
                this.translate(movement);
            } else {
                TgcCollisionUtils.intersectRayAABB(rayoVelocidad, pisoCercano, out aux);

                TGCVector3 radius = 
                    TGCVector3.Normalize(movement) * this.getBoundingSphere().Radius;

                TGCVector3 distance =
                    TgcCollisionUtils.closestPointAABB(this.getBoundingSphere().Center, pisoCercano) - this.getBoundingSphere().Center;

                if ((distance - radius).Length() < 1) return;
                
                if ((radius + movement).Length() < distance.Length()) {
                    this.translate(movement);
                } else {
                    var foo = TGCVector3.Normalize(movement) * distance.Length();
                    movement = TGCVector3.Normalize(foo - distance) * WALK_SPEED * deltaTime;

                    this.movimientoVertical(movement, colliders, deltaTime, count + 1);
                }
            }
        }

        private void translate(TGCVector3 movement) {
            mesh.Move(movement);
            pies.moveCenter(movement);
            boundingSphere.moveCenter(movement);
        }

        private void resetUpdateVariables() {
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

        public void setRotation(float angle) {
            meshAngle = angle;
        }

        public void volverAlOrigen() {
            mesh.Position = POS_ORIGEN;
            boundingSphere.setValues(mesh.BoundingBox.calculateBoxCenter(), mesh.BoundingBox.calculateBoxRadius());

            var posicionPies = mesh.BoundingBox.calculateBoxCenter();
            posicionPies.Y = mesh.BoundingBox.PMin.Y;
            pies.setValues(posicionPies, 20);

            dir = TGCVector3.Empty;
            vel = TGCVector3.Empty;
        }
    }
}
