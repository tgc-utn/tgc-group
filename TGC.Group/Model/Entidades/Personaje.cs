﻿using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.DirectSound;
using System;
using System.Collections.Generic;
using System.Linq;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Particle;
using TGC.Core.Shaders;
using TGC.Core.SkeletalAnimation;
using TGC.Core.Sound;
using TGC.Group.Model.Escenas;
using TGC.Group.Model.Niveles;
using TGC.Group.Model.Scenes;

namespace TGC.Group.Model {
    class Personaje {
        private TgcBoundingSphere boundingSphere;
        private TgcBoundingSphere pies;

        private TGCVector3 POS_ORIGEN = new TGCVector3(0, 100, 9500);

        private int vidas;
        private int stamina;
        private bool agotado;
        private const int MAX_STAMINA = 200;

        // animaciones
        private TgcSkeletalMesh mesh;
        private float meshAngle;
        private float meshAngleAnterior;
        private bool moving = false;
        private float pisoHeight;
        private bool enElAire = false;

        private ParticleEmitter emmiter;
        private float timerEmitter = 0;
        private const float TIMER_CORTO = 1f;

        private TgcStaticSound sound;
        private string soundDir;

        private Microsoft.DirectX.Direct3D.Effect effect;

        // movimiento
        private TGCVector3 dir;
        private TGCVector3 vel;
        private const float WALK_SPEED = 500f;
        private const float MULT_CORRER = 1.75f;
        private const float MULT_CAMINAR = 0.5f;
        private const float VEL_TERMINAL = -10;
        private const float MODIFICADOR_HIELO = 0.99f;
        private const int SIZE_PIES = 20;
        private TgcRay rayoVelocidad = new TgcRay();

        // saltos
        private int saltosRestantes = 0;
        private const int SALTOS_TOTALES = 2;
        private const float JUMP_SPEED = 10f; // PC Cristian

        public Personaje(string MediaDir, string shaderDir) {
            vidas = 3;
            stamina = MAX_STAMINA;
            soundDir = MediaDir + "/Sonidos/";

            dir = TGCVector3.Empty;
            vel = TGCVector3.Empty;
            var skeletalLoader = new TgcSkeletalLoader();

            mesh = skeletalLoader.loadMeshAndAnimationsFromFile(
                MediaDir + "Robot\\Robot-TgcSkeletalMesh.xml",
                MediaDir + "Robot\\",
                new[] {
                     MediaDir + "Robot\\Caminando-TgcSkeletalAnim.xml",
                     MediaDir + "Robot\\Parado-TgcSkeletalAnim.xml",
                     MediaDir + "Robot\\Correr-TgcSkeletalAnim.xml"
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
                mesh.BoundingBox.calculateBoxRadius() - 5
            );

            var posicionPies = mesh.BoundingBox.calculateBoxCenter();
            posicionPies.Y = mesh.BoundingBox.PMin.Y;

            pies = new TgcBoundingSphere(posicionPies, SIZE_PIES);

            effect = TgcShaders.loadEffect(shaderDir + "TgcSkeletalMeshShader.fx");
            effect.SetValue("g_shadowMapping", 0);
            mesh.Effect = effect;
            mesh.Technique = "DIFFUSE_MAP";


            emmiter = new ParticleEmitter(MediaDir + "pisada.png", 10);
            emmiter.MinSizeParticle = 10;
            emmiter.MaxSizeParticle = 20;
            emmiter.ParticleTimeToLive = 1;
            emmiter.CreationFrecuency = 0.5f;
            emmiter.Dispersion = 400;
            emmiter.Speed = new TGCVector3(30, 30, 30);
            emmiter.Position = pies.Position;
        }

        public void update(float deltaTime, TgcD3dInput Input) {
            checkInputs(Input);
            updateAnimations();
        }

        public void render(float deltaTime) {
            mesh.UpdateMeshTransform();
            mesh.animateAndRender(deltaTime);

            if (timerEmitter > 0.0f) {
                emmiter.render(deltaTime);
                timerEmitter -= deltaTime;
            }

            // seria un post-update
            if (dir.Length() == 0) moving = false;
        }

        public void prepareForShadowMapping(TGCVector3 lightPos, TGCVector3 lightDir,
            TGCMatrix g_mShadowProj, TGCMatrix g_lightView, Texture g_pShadowMap) {
            effect.SetValue("g_vLightPos", new Vector4(lightPos.X, lightPos.Y, lightPos.Z, 1));
            effect.SetValue("g_vLightDir", new Vector4(lightDir.X, lightDir.Y, lightDir.Z, 1));
            effect.SetValue("g_mProjLight", g_mShadowProj.ToMatrix());
            effect.SetValue("g_mViewLightProj", (g_lightView * g_mShadowProj).ToMatrix());

            effect.SetValue("g_txShadow", g_pShadowMap);

            effect.SetValue("g_shadowMapping", 1);
        }

        public void prepareForNormalRender() {
            effect.SetValue("g_shadowMapping", 0);
        }

        public void dispose() {
            mesh.Dispose();
            sound.dispose();
        }

        private void checkInputs(TgcD3dInput Input) {
            float FRAME_WALK_SPEED = WALK_SPEED;
            dir = TGCVector3.Empty;

            // caminar
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

            // correr
            if (Input.keyDown(Key.LeftShift) && !agotado && dir.Length() != 0) {
                dir.X = dir.X * MULT_CORRER;
                dir.Z = dir.Z * MULT_CORRER;
                stamina--;
                if (stamina == 0) agotado = true;
                timerEmitter = TIMER_CORTO;
            } else if (Input.keyDown(Key.LeftAlt)) {
                dir.X = dir.X * MULT_CAMINAR;
                dir.Z = dir.Z * MULT_CAMINAR;
                stamina += 2;
            } else {
                stamina++;
            }

            if (stamina > MAX_STAMINA) {
                stamina = MAX_STAMINA;
                agotado = false;
            }

            // saltar
            if (Input.keyPressed(Key.Space) && saltosRestantes > 0) {
                dir.Y = JUMP_SPEED;

                playSound(1);

                saltosRestantes--;
            }
        }

        private void updateAnimations() {
            if (enElAire) {
                // no hay una animacion para "caer"
                mesh.playAnimation("Correr", true);
            } else if (moving) {
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
            float velX = 0, velY = 0, velZ = 0;

            if (vel.X != 0) {
                if (dir.X != 0) velX = dir.X * deltaTime;
                else velX = vel.X;
            } else velX = dir.X * deltaTime;

            if (vel.Z != 0) {
                if (dir.Z != 0) velZ = dir.Z * deltaTime;
                else velZ = vel.Z;
            } else velZ = dir.Z * deltaTime;

            vel = new TGCVector3(velX, vel.Y + dir.Y * deltaTime, velZ);

            // movimiento por entorno
            TgcBoundingAxisAlignBox piso = nivel.getBoundingBoxes().Find(b => TgcCollisionUtils.testSphereAABB(pies, b));

            // actualizo para las animaciones
            enElAire = (piso == null);

            if (piso == null) {
                // si estoy en el aire
                if (vel.Y > VEL_TERMINAL) {
                    vel += gravedad * deltaTime;
                }
            } else {
                // si estoy en algun piso
                vel.Y = 0;
                aterrizar();
                modificarMovimientoSegunPiso(piso, nivel, deltaTime);
                if (nivel.esPisoRotante(piso)) {
                    int i = 1;
                };
            }

            if (dir.Y != 0) vel.Y = dir.Y;

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
            movimientoVertical(vertical, nivel, deltaTime, 0);

            resetearMovimientoSegunPiso(piso, nivel);
            emmiter.Position = pies.Position;
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

            rayoVelocidad.Origin = this.getBoundingSphere().Center;
            rayoVelocidad.Direction = movement;

            TGCVector3 aux = new TGCVector3();

            TgcBoundingAxisAlignBox paredCercana = colliders
                // tomo las aabb que colisionan
                .Where(b => TgcCollisionUtils.intersectRayAABB(rayoVelocidad, b, out aux))
                // ordeno por distancia
                .OrderBy(b => {
                    TgcCollisionUtils.intersectRayAABB(rayoVelocidad, b, out aux);
                    return aux.LengthSq();
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

        private void movimientoVertical(TGCVector3 movement, Nivel nivel, float deltaTime, int count) {
            if (count > 5) return;

            rayoVelocidad.Origin = this.getPies().Center;
            rayoVelocidad.Direction = movement;

            var colliders = nivel.getBoundingBoxes();

            TGCVector3 aux = new TGCVector3();

            TgcBoundingAxisAlignBox pisoCercano = colliders
                .Where(b => TgcCollisionUtils.intersectRayAABB(rayoVelocidad, b, out aux))
                .OrderBy(b => {
                    TgcCollisionUtils.intersectRayAABB(rayoVelocidad, b, out aux);
                    return aux.Length();
                })
                .DefaultIfEmpty(null)
                .First();

            if (pisoCercano == null) {
                translate(movement);
            } else {
                pisoHeight = pisoCercano.PMax.Y;

                TgcCollisionUtils.intersectRayAABB(rayoVelocidad, pisoCercano, out aux);

                TGCVector3 radius =
                    TGCVector3.Normalize(movement) * this.getBoundingSphere().Radius;

                TGCVector3 distance =
                    TgcCollisionUtils.closestPointAABB(this.getBoundingSphere().Center, pisoCercano) - this.getBoundingSphere().Center;

                if ((distance - radius).Length() < 0.1f) return;

                if ((radius + movement).Length() < distance.Length()) {
                    this.translate(movement);
                } else {
                    var foo = TGCVector3.Normalize(movement) * distance.Length();
                    movement = TGCVector3.Normalize(foo - distance) * WALK_SPEED * deltaTime;

                    this.movimientoVertical(movement, nivel, deltaTime, count + 1);
                }
            }
        }

        private void translate(TGCVector3 movement) {
            mesh.Move(movement);
            pies.moveCenter(movement);
            boundingSphere.moveCenter(movement);

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

        public void morir() {
            playSound(2);

            if (vidas == 0) {
                EscenaManager.getInstance().goBack();
                EscenaManager.getInstance().addScene(new GameOverEscena());
            }

            resetear();

            vidas--;


        }

        public void resetear() {
            mesh.Position = POS_ORIGEN;
            boundingSphere.setValues(mesh.BoundingBox.calculateBoxCenter(), mesh.BoundingBox.calculateBoxRadius());

            var posicionPies = mesh.BoundingBox.calculateBoxCenter();
            posicionPies.Y = mesh.BoundingBox.PMin.Y;
            pies.setValues(posicionPies, SIZE_PIES);

            dir = TGCVector3.Empty;
            vel = TGCVector3.Empty;
        }

        public int getStamina() => stamina;
        public int getVidas() => vidas;

        public void playSound(int choice) {
            string file = "";

            switch (choice) {
                case 1:
                    file = "sonidoSalto.wav";
                    break;
                case 2:
                    file = "sonidoCaida.wav";
                    break;
                case 3:
                    file = "sonidoChoque.wav";
                    break;
                default:
                    return;
            }

            if (sound != null)
                sound.dispose();

            sound = new TgcStaticSound();
            sound.loadSound(soundDir + file, Musica.getInstance().getDsDevice());
            sound.play(false);
        }
    }
}
