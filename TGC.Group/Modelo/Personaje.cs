using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.DirectX.Direct3D;

using TGC.Core.SkeletalAnimation;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.Textures;
using TGC.Core.Collision;
using TGC.Core.SceneLoader;


namespace TGC.Group.Modelo
{
    public class Personaje
    {
        private static float vidaMaxima = 1;
        public float vida { get; set; }
        public int frutas { get; set; }
        public int mascaras { get; set; }

        public TgcSkeletalMesh personajeMesh { get; }
        private Directorio directorio;

        public TgcBoundingSphere esferaPersonaje { get; set; }
        private TGCVector3 POSICION_INICIAL_ESFERA;
        private float COEFICIENTE_REDUCTIVO_ESFERA = 0.85f;
        private float RADIO_ESFERA;

        private TGCVector3 posicionInicial = new TGCVector3(400,0.1f, -900);
        private TGCVector3 posicionDesarrollo = new TGCVector3(-4738.616f, 1379f, -7531f);

        public TGCVector3 PERSONAJE_SCALE = new TGCVector3(1f, 0.9f,1f);
       // public float PERSONAJE_ALTURA_PISO { get; set; }

        public float ultimaRotacion { get; set; }
        public TGCVector3 ultimoDesplazamiento { get; set; }

        public TGCMatrix matrizTransformacionPlataformaRotante { get; set; }
        

        public Personaje(Directorio directorio)
        {
            this.directorio = directorio;
            vida = vidaMaxima;
            frutas = 0;
            mascaras = 0;
            ultimaRotacion = 0;

            var skeletalLoader = new TgcSkeletalLoader();

            var pathAnimacionesPersonaje = new[] { directorio.RobotCaminando, directorio.RobotParado, };
            personajeMesh = skeletalLoader.
                            loadMeshAndAnimationsFromFile(directorio.RobotSkeletalMesh,
                                                      directorio.RobotDirectorio,
                                                      pathAnimacionesPersonaje);

            //Descomentar para ubicarlo donde se este desarrollando
            // posicionInicial = posicionDesarrollo;

            //personajeMesh.AutoUpdateBoundingBox = false;
            //personajeMesh.BoundingBox.transform(TGCMatrix.Scaling(PERSONAJE_SCALE) *TGCMatrix.Translation(posicionInicial));
            
            move(posicionInicial);

            
            RADIO_ESFERA = boundingBox().calculateBoxRadius() * COEFICIENTE_REDUCTIVO_ESFERA;
            POSICION_INICIAL_ESFERA = new TGCVector3(posicionInicial.X,posicionInicial.Y + RADIO_ESFERA,posicionInicial.Z);
            esferaPersonaje = new TgcBoundingSphere(POSICION_INICIAL_ESFERA, RADIO_ESFERA);
            //Ubica al mesh en la posicion inicial.
            personajeMesh.Transform = TGCMatrix.Scaling(PERSONAJE_SCALE) 
                                      *TGCMatrix.RotationY(FastMath.ToRad(180f))
                                      *TGCMatrix.Translation(posicionInicial);

           
            
            matrizTransformacionPlataformaRotante = TGCMatrix.Identity;
            
        }

        public bool colisionaConBoundingBox(TgcMesh mesh) => TgcCollisionUtils.testSphereAABB(esferaPersonaje, mesh.BoundingBox);
        public bool colisionaConCaja(TgcMesh box)
        {
            TgcBoundingAxisAlignBox boundingBoxColision = boundingBox();
            boundingBoxColision.scaleTranslate(position(), new TGCVector3(2.5f,2.5f,2.5f));
            return TgcCollisionUtils.testAABBAABB(boundingBoxColision, box.BoundingBox);
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
            //esferaAuxiliar.moveCenter(new TGCVector3(0f,, 0f));
            return TgcCollisionUtils.testSphereAABB(esferaAuxiliar, pisoDesnivelado.BoundingBox);
        }

        public void transformar()
        {
            //Es la posicion del centro de la esfera, pero restandole el radio de la esfera en el eje Y
            TGCVector3 posicionActual = new TGCVector3(esferaPersonaje.Center.X, esferaPersonaje.Center.Y - RADIO_ESFERA, esferaPersonaje.Center.Z);
            float anguloRotado = (personajeMesh.Rotation.Y + FastMath.ToRad(180f));
            personajeMesh.Transform =  TGCMatrix.Scaling(PERSONAJE_SCALE)
                                      *TGCMatrix.RotationY(anguloRotado)
                                      *TGCMatrix.Translation(posicionActual + ultimoDesplazamiento)
                                      *matrizTransformacionPlataformaRotante;
        }

        #region MeshAdapter
        public TgcBoundingAxisAlignBox boundingBox() => personajeMesh.BoundingBox;
        public void playAnimation(string animation, bool playLoop) => personajeMesh.playAnimation(animation, playLoop);
        public Effect effect() => personajeMesh.Effect;
        public void effect(Effect newEffect) => personajeMesh.Effect = newEffect;
        public void technique(string newTechnique) => personajeMesh.Technique = newTechnique;
        public void position(TGCVector3 newPosition) => esferaPersonaje = new TgcBoundingSphere(newPosition, RADIO_ESFERA);
        public TGCVector3 position() => esferaPersonaje.Position;
        public TGCVector3 rotation() =>  personajeMesh.Rotation;
        public void RotateY(float angle) => personajeMesh.RotateY(angle);
        public void move(TGCVector3 desplazamiento)
        {
            ultimoDesplazamiento = desplazamiento;
            //personajeMesh.
            personajeMesh.Move(desplazamiento);
          }        
       
        public TGCMatrix transform() => personajeMesh.Transform;
        public void transform(TGCMatrix transformacion) => personajeMesh.Transform *= transformacion;
        public void autoTransform(bool state) => personajeMesh.AutoTransform = state;
        public void UpdateMeshTransform() => personajeMesh.UpdateMeshTransform();
        public void changeDiffuseMaps(TgcTexture[] newDiffuseMap) => personajeMesh.changeDiffuseMaps(newDiffuseMap);
        public void animateAndRender(float elapsedTime) => personajeMesh.animateAndRender(elapsedTime);
        public TgcSkeletalMesh.MeshRenderType renderType() => personajeMesh.RenderType;
        public void render() => personajeMesh.Render();
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
