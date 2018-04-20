using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.SkeletalAnimation;
using TGC.Core.Collision;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model
{
    class Escenario
    {
       public TgcScene scene { get; set; }

        public Escenario(string pathEscenario)
        {
            var loader = new TgcSceneLoader();
            scene = loader.loadSceneFromFile(pathEscenario);
        }

        public List<TgcMesh> Paredes()=> scene.Meshes.FindAll(x => x.Layer == "PAREDES");

        public List<TgcMesh> Rocas()=> scene.Meshes.FindAll(x => x.Layer == "ROCAS");
        
        public TgcMesh Piso() =>scene.Meshes.Find(x => x.Name == "Piso");
        
        public List<TgcMesh> MeshesColisionables()
        {
            return Paredes().Concat(Rocas()).ToList();
        }

        public void RenderizarBoundingBoxes()
        {
            this.Paredes().ForEach(x => x.BoundingBox.Render());
            this.Rocas().ForEach(x => x.BoundingBox.Render());
        }

        public void RenderAll() => scene.RenderAll();
        
        public void DisposeAll() => scene.DisposeAll();
        
        public TgcBoundingAxisAlignBox BoundingBox() => scene.BoundingBox;

    }
}
