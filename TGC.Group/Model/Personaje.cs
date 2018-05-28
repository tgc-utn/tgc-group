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


namespace TGC.Group.Model
{
    public class Personaje
    {
        private static float vidaMaxima = 1;
        public float vida { get; set; }
        public TgcSkeletalMesh personajeMesh { get; }
        private Directorio directorio;

        public Personaje(Directorio directorio)
        {
            this.directorio = directorio;
            vida = vidaMaxima;

            var skeletalLoader = new TgcSkeletalLoader();

            var pathAnimacionesPersonaje = new[] { directorio.RobotCaminando, directorio.RobotParado, };
            personajeMesh = skeletalLoader.
                            loadMeshAndAnimationsFromFile(directorio.RobotSkeletalMesh,
                                                      directorio.RobotDirectorio,
                                                      pathAnimacionesPersonaje);

            
        }

        public bool vidaCompleta() => vida == vidaMaxima;
        public bool vivo() => vida > 0;
        #region MeshAdapter
        public TgcBoundingAxisAlignBox boundingBox() => personajeMesh.BoundingBox;

        public void playAnimation(string animation, bool playLoop) => personajeMesh.playAnimation(animation, playLoop);
        public Effect effect() => personajeMesh.Effect;
        public void effect(Effect newEffect) => personajeMesh.Effect = newEffect;
        public void technique(string newTechnique) => personajeMesh.Technique = newTechnique;

        public void position(TGCVector3 newPosition) => personajeMesh.Position = newPosition;
        public TGCVector3 position() => personajeMesh.Position;
        public TGCVector3 rotation() => personajeMesh.Rotation;
        public void RotateY(float angle) => personajeMesh.RotateY(angle);

        public void move(TGCVector3 desplazamiento) => personajeMesh.Move(desplazamiento);

        public void autoTransform(bool state) => personajeMesh.AutoTransform = state;
        public void UpdateMeshTransform() => personajeMesh.UpdateMeshTransform();
        public void changeDiffuseMaps(TgcTexture[] newDiffuseMap) => personajeMesh.changeDiffuseMaps(newDiffuseMap);

        public void animateAndRender(float elapsedTime) => personajeMesh.animateAndRender(elapsedTime);
        public TgcSkeletalMesh.MeshRenderType renderType() => personajeMesh.RenderType;
        public void render() => personajeMesh.Render();
        public void dispose() => personajeMesh.Dispose();
        #endregion
    }
}
