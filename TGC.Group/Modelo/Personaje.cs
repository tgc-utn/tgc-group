using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;

using TGC.Core.SkeletalAnimation;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.Textures;
using TGC.Core.Collision;
using TGC.Core.SceneLoader;
using TGC.Core.Input;
using TGC.Core.Particle;


using TGC.Group.Modelo.Cajas;

namespace TGC.Group.Modelo
{
    public class Personaje
    {
        private static float vidaMaxima = 1;
        public float vida { get; set; }
        public int frutas { get; set; }
        public int mascaras { get; set; }

        public float VELOCIDAD_PERSONAJE = 1000f;
        public float VELOCIDAD_EXTRA = 0f;
        public float TIEMPO_POWER_UP = 0f;

        public TgcSkeletalMesh personajeMesh { get; }
        private Directorio directorio;

        public TgcBoundingSphere esferaPersonaje { get; set; }
        private TGCVector3 POSICION_INICIAL_ESFERA;
        private float COEFICIENTE_REDUCTIVO_ESFERA = 0.85f;
        private float RADIO_ESFERA;

        private TGCVector3 posicionInicial = new TGCVector3(-452f,0.1f, -5161f);
        
        private TGCVector3 posicionDesarrollo = new TGCVector3(-4738.616f, 1379f, -7531f);
        private DireccionPersonaje direccion = new DireccionPersonaje();

        public TGCVector3 PERSONAJE_SCALE = new TGCVector3(1f, 0.9f,1f);
       // public float PERSONAJE_ALTURA_PISO { get; set; }

        public float ultimaRotacion { get; set; }
        public TGCVector3 ultimoDesplazamiento { get; set; }

        public TGCMatrix matrizTransformacionPlataformaRotante { get; set; }

        public ParticleEmitter emisorParticulas { get; set; }

        public static string texturesPath;
        private string nitroTex = "hojaparticula.png";

        public TGCVector3 MovimientoRealActual { get; set; }


        public Personaje(Directorio directorio)
        {
            this.directorio = directorio;
            vida = vidaMaxima;
            frutas = 0;
            mascaras = 0;
            ultimaRotacion = 0;

            var skeletalLoader = new TgcSkeletalLoader();

            var pathAnimacionesPersonaje = new[] { directorio.RobotCaminando,
                                                   directorio.RobotParado,
                                                   directorio.RobotPateando,
                                                   directorio.RobotCorriendo,
                                                   directorio.RobotEmpujando};
            personajeMesh = skeletalLoader.
                            loadMeshAndAnimationsFromFile(directorio.RobotSkeletalMesh,
                                                      directorio.RobotDirectorio,
                                                      pathAnimacionesPersonaje);

            //Descomentar para ubicarlo donde se este desarrollando
            // posicionInicial = posicionDesarrollo;

            personajeMesh.AutoUpdateBoundingBox = false;
            personajeMesh.BoundingBox.transform(TGCMatrix.Translation(posicionInicial));
            //personajeMesh.Move(posicionInicial);

            RADIO_ESFERA = boundingBox().calculateBoxRadius() * COEFICIENTE_REDUCTIVO_ESFERA;
            POSICION_INICIAL_ESFERA = new TGCVector3(posicionInicial.X,posicionInicial.Y + RADIO_ESFERA,posicionInicial.Z);
            esferaPersonaje = new TgcBoundingSphere(POSICION_INICIAL_ESFERA, RADIO_ESFERA);
            //Ubica al mesh en la posicion inicial.
            personajeMesh.Transform = TGCMatrix.Scaling(PERSONAJE_SCALE) 
                                      *TGCMatrix.RotationY(FastMath.ToRad(180f))
                                      *TGCMatrix.Translation(posicionInicial);

            matrizTransformacionPlataformaRotante = TGCMatrix.Identity;
            
        }

        public bool colisionaConMesh(TgcMesh mesh) => TgcCollisionUtils.testSphereAABB(esferaPersonaje, mesh.BoundingBox);
        public bool colisionaConCaja(Caja box)
        {
            TgcBoundingAxisAlignBox boundingBoxColision = boundingBox();
            boundingBoxColision.scaleTranslate(position(), new TGCVector3(2.5f,2.5f,2.5f));
            return TgcCollisionUtils.testAABBAABB(boundingBoxColision, box.boundingBox());
        }
       public bool colisionaPorArribaDe(TgcMesh mesh)
        {
            TgcBoundingSphere esferaAuxiliar = new TgcBoundingSphere(esferaPersonaje.Center, esferaPersonaje.Radius);
            esferaAuxiliar.moveCenter(new TGCVector3(0f,-RADIO_ESFERA, 0f));
            return TgcCollisionUtils.testSphereAABB(esferaAuxiliar, mesh.BoundingBox);
        }


        public bool colisionConPisoDesnivelado(TgcMesh pisoDesnivelado)
        {
            TgcBoundingSphere esferaAuxiliar = new TgcBoundingSphere(esferaPersonaje.Center, esferaPersonaje.Radius);
            esferaAuxiliar.moveCenter(new TGCVector3(0f, -RADIO_ESFERA, 0f));
            return TgcCollisionUtils.testSphereAABB(esferaAuxiliar, pisoDesnivelado.BoundingBox);
        }

        public void transformar()
        {
            //Es la posicion del centro de la esfera, pero restandole el radio de la esfera en el eje Y
            TGCVector3 posicionActual = new TGCVector3(esferaPersonaje.Center.X, esferaPersonaje.Center.Y-RADIO_ESFERA, esferaPersonaje.Center.Z);
            
            float anguloRotado = (personajeMesh.Rotation.Y + FastMath.ToRad(180f));
            personajeMesh.Transform =  TGCMatrix.Scaling(PERSONAJE_SCALE)
                                      *TGCMatrix.RotationY(anguloRotado)
                                      *TGCMatrix.Translation(posicionActual)
                                      *matrizTransformacionPlataformaRotante;
            personajeMesh.BoundingBox.transform(TGCMatrix.Translation(posicionActual) * matrizTransformacionPlataformaRotante);
            
        }

        #region Movimientos
        public float Velocidad()
        {
            return VELOCIDAD_PERSONAJE + VELOCIDAD_EXTRA;
        }

        public void aumentarVelocidad(float velocidadExtra,float tiempo)
        {
            VELOCIDAD_EXTRA += velocidadExtra;
            TIEMPO_POWER_UP += tiempo;
            emisorParticulas = new ParticleEmitter(texturesPath + nitroTex, 15);
            emisorParticulas.Position = position();
            emisorParticulas.MinSizeParticle = 1f;
            emisorParticulas.MaxSizeParticle = 2;
            emisorParticulas.ParticleTimeToLive = 1f;
            emisorParticulas.CreationFrecuency = 0.01f;
            emisorParticulas.Dispersion = 50;
            emisorParticulas.Speed = new TGCVector3(65, 20, 15);
        }

        public void actualizarValores(float elapsedTime)
        {
            if(TIEMPO_POWER_UP > 0) TIEMPO_POWER_UP -= elapsedTime;
            else
            {
                VELOCIDAD_EXTRA = 0;
                emisorParticulas = null;
            }
            
        }

        public bool rotar(TgcD3dInput Input,Key key)
        {
            bool moving = false;
            //Adelante
            if (Input.keyDown(Key.W)) moving = RotateMesh(Key.W);
            //Atras
            if (Input.keyDown(Key.S)) moving = RotateMesh(Key.S);
            //Derecha
            if (Input.keyDown(Key.D)) moving = RotateMesh(Key.D);
            //Izquierda
            if (Input.keyDown(Key.A)) moving = RotateMesh(Key.A);
            //UpLeft
            if (Input.keyDown(Key.W) && Input.keyDown(Key.A)) moving = RotateMesh(Key.W, Key.A);
            //UpRight
            if (Input.keyDown(Key.W) && Input.keyDown(Key.D)) moving = RotateMesh(Key.W, Key.D);
            //DownLeft
            if (Input.keyDown(Key.S) && Input.keyDown(Key.A)) moving = RotateMesh(Key.S, Key.A);
            //DownRight
            if (Input.keyDown(Key.S) && Input.keyDown(Key.D)) moving = RotateMesh(Key.S, Key.D);

            return moving;
        }

        public bool RotateMesh(Key input)
        {
            
            RotateY(direccion.RotationAngle(input));
            return true;
        }
        public bool RotateMesh(Key i1, Key i2)
        {
            
            RotateY(direccion.RotationAngle(i1, i2));
            return true;
        }
        #endregion

        #region MeshAdapter
        public TgcBoundingAxisAlignBox boundingBox() => personajeMesh.BoundingBox;
        public void playAnimation(string animation, bool playLoop) => personajeMesh.playAnimation(animation, playLoop);
        public Microsoft.DirectX.Direct3D.Effect effect() => personajeMesh.Effect;
        public void effect(Microsoft.DirectX.Direct3D.Effect newEffect) => personajeMesh.Effect = newEffect;
        public void technique(string newTechnique) => personajeMesh.Technique = newTechnique;
        public void position(TGCVector3 newPosition) => esferaPersonaje = new TgcBoundingSphere(newPosition, RADIO_ESFERA);
        public TGCVector3 position() => esferaPersonaje.Position;
        public TGCVector3 rotation() =>  personajeMesh.Rotation;
        public void RotateY(float angle) => personajeMesh.RotateY(angle);
        public void move(TGCVector3 desplazamiento)
        {
           //personajeMesh.BoundingBox.transform(TGCMatrix.Translation(desplazamiento));
            //personajeMesh.Move(desplazamiento);
          }        
       
        public TGCMatrix transform() => personajeMesh.Transform;
        public void transform(TGCMatrix transformacion) => personajeMesh.Transform *= transformacion;
        public void autoTransform(bool state) => personajeMesh.AutoTransform = state;
        public void UpdateMeshTransform() => personajeMesh.UpdateMeshTransform();
        public void changeDiffuseMaps(TgcTexture[] newDiffuseMap) => personajeMesh.changeDiffuseMaps(newDiffuseMap);
        public void animateAndRender(float elapsedTime) => personajeMesh.animateAndRender(elapsedTime);
        public TgcSkeletalMesh.MeshRenderType renderType() => personajeMesh.RenderType;

        public void render(float ElapsedTime)
        {
            personajeMesh.Render();
            if(emisorParticulas != null)
            {
                emisorParticulas.render(ElapsedTime);
            }
        }
        public void dispose() => personajeMesh.Dispose();
        #endregion

        #region Estado
        public bool vidaCompleta() => vida == vidaMaxima;
        public bool vivo() => vida > 0;
        public void aumentarVida(float aumento)
        {
            vida += aumento;
        }

        public void aumentarFrutas()
        {
            frutas++;
        }
        public void aumentarMascaras()
        {
            mascaras++;
        }
        #endregion
    }
}
