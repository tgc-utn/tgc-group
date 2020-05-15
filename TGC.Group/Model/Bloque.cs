using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Bloque:IRenderizable
    {
        private readonly string mediaDir;
        private readonly TGCVector3 posicionInicial;

        private TGCMatrix ScaleMatrix;
        private TGCMatrix TranslationMatrix;
        private TgcScene Scene;
        private String nombreMapa;
        private List<TGCVector3> posicionesTorretas;
        private Nave nave;

        public Bloque(string mediaDir, TGCVector3 posicionInicial,String nombreMapa,List<TGCVector3> posiciones,Nave nave)
        {
            this.mediaDir = mediaDir;
            this.posicionInicial = posicionInicial;
            this.nombreMapa = nombreMapa;
            this.posicionesTorretas = posiciones;
            this.nave = nave;
        }
        public void Init()
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            Scene = new TgcSceneLoader().loadSceneFromFile(mediaDir + nombreMapa);
            ScaleMatrix = TGCMatrix.Scaling(20f, 20f, 20f);
            TranslationMatrix = TGCMatrix.Translation(posicionInicial);
            instanciarTorretas();
        }

        private void instanciarTorretas()
        {
            this.posicionesTorretas.
                ForEach(delegate (TGCVector3 tor) {
                    Torreta torreta = new Torreta(mediaDir,tor,nave);
                    GameManager.Instance.AgregarRenderizable(torreta); 
                });
        }

        public void Update(float elapsedTime)
        {
            Scene.Meshes.ForEach(delegate (TgcMesh mesh) { mesh.Transform = ScaleMatrix*TranslationMatrix; });
        }

        public void Render()
        {
            Scene.RenderAll();


        }
        public void Dispose()
        {
            Scene.DisposeAll();
        }
    }
}
