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


namespace TGC.Group.Model
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

        private TGCVector3 posicionInicial = new TGCVector3(400,20f, -900);
        private TGCVector3 posicionDesarrollo = new TGCVector3(-4738.616f, 1379f, -7531f);

        public TGCVector3 PERSONAJE_SCALE = new TGCVector3(1f, 0.9f,1f);

        public float ultimaRotacion { get; set; }
        public TGCVector3 ultimoDesplazamiento { get; set; }

        

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
            //posicionInicial = posicionDesarrollo;

            RADIO_ESFERA = boundingBox().calculateBoxRadius() * COEFICIENTE_REDUCTIVO_ESFERA;
            POSICION_INICIAL_ESFERA = new TGCVector3(posicionInicial.X,posicionInicial.Y + RADIO_ESFERA,posicionInicial.Z);

            //Ubica al mesh en la posicion inicial.
            personajeMesh.Transform = TGCMatrix.Scaling(PERSONAJE_SCALE) 
                                      *TGCMatrix.RotationY(FastMath.ToRad(180f))
                                      *TGCMatrix.Translation(posicionInicial);
        }

        public bool colisionaConBoundingBox(TgcMesh mesh) => TgcCollisionUtils.testSphereAABB(esferaPersonaje, mesh.BoundingBox);

        public void inicializarEsferaColisionante() => esferaPersonaje = new TgcBoundingSphere(POSICION_INICIAL_ESFERA,RADIO_ESFERA);
        

        public void transformar()
        {
            //Es la posicion del centro de la esfera, pero restandole el radio de la esfera en el eje Y
            TGCVector3 posicionActual = new TGCVector3(esferaPersonaje.Center.X, esferaPersonaje.Center.Y - RADIO_ESFERA, esferaPersonaje.Center.Z);

            personajeMesh.Transform = TGCMatrix.RotationY(personajeMesh.Rotation.Y + FastMath.ToRad(180f))
                                      * TGCMatrix.Translation(posicionActual + ultimoDesplazamiento);
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
        public void move(TGCVector3 desplazamiento) =>ultimoDesplazamiento = desplazamiento;
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
