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
using TGC.Group.Model.AI;

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

        

        public List<TgcMesh> encontrarMeshes(string clave) => scene.Meshes.FindAll(x => x.Layer == clave);
        public List<TgcMesh> ParedesMesh() => encontrarMeshes("PAREDES");
        public List<TgcMesh> RocasMesh() => encontrarMeshes("ROCAS");
        public List<TgcMesh> PisosMesh() => encontrarMeshes("PISOS");
        public List<TgcMesh> CajasMesh() => encontrarMeshes("CAJAS");
        public List<TgcMesh> SarcofagosMesh() => encontrarMeshes("SARCOFAGOS");
        public List<TgcMesh> PilaresMesh() => encontrarMeshes("PILARES");
        public List<TgcMesh> PlataformasMesh() => encontrarMeshes("PLATAFORMA");
        public List<TgcMesh> ResbalososMesh() => encontrarMeshes("RESBALOSOS");
        public List<TgcMesh> LavaMesh() => encontrarMeshes("LAVA");

        public bool colisionaConPiso(TgcMesh mesh)
        {
            return PisosMesh().Exists(piso => TgcCollisionUtils.testAABBAABB(piso.BoundingBox, mesh.BoundingBox));
        }
        public bool colisionaConLava(TgcMesh mesh)
        {
            return LavaMesh().Exists(lava => TgcCollisionUtils.testAABBAABB(lava.BoundingBox, mesh.BoundingBox));
        }
        public bool colisionaConPared(TgcMesh mesh)
        {
            return ParedesMesh().Exists(pared => TgcCollisionUtils.testAABBAABB(pared.BoundingBox, mesh.BoundingBox));
        }

        public bool colisionaCon(TgcMesh mesh)
        {
            return MeshesColisionablesSin("PLATAFORMA").Exists(x => TgcCollisionUtils.testAABBAABB(x.BoundingBox, mesh.BoundingBox));
        }

        private int coeficienteRotacion = 1;

        public List<Plataforma> Plataformas()
        {
            List<Plataforma> plataformas = new List<Plataforma>();

            foreach (TgcMesh plataformaMesh in PlataformasMesh())
            {
                
                Plataforma plataforma;

                if (plataformaMesh.Name == "PlataformaY") plataforma = new PlataformaY(plataformaMesh, this);
                else if (plataformaMesh.Name == "PlataformaX") plataforma = new PlataformaX(plataformaMesh, this);
                else if (plataformaMesh.Name == "PlataformaZ") plataforma = new PlataformaZ(plataformaMesh, this);
                else if (plataformaMesh.Name == "PlataformaRotante")
                {
                    coeficienteRotacion *= -1;
                    plataforma = new PlataformaRotante(plataformaMesh, this, coeficienteRotacion);
                }
                else plataforma = new Plataforma(plataformaMesh, this);

                plataformas.Add(plataforma);
                
            }

            return plataformas;
        }
        public List<PlataformaRotante> PlataformasRotantes()
        {
            List<PlataformaRotante> plataformas = new List<PlataformaRotante>();
            foreach(PlataformaRotante plataforma in Plataformas().FindAll(plat => plat.plataformaMesh.Name == "PlataformaRotante"))
            {
                plataformas.Add(plataforma);
            }
            return plataformas;
        }

        public List<TgcMesh> MeshesColisionables()
        {
            List<TgcMesh> meshesColisionables = new List<TgcMesh>();
            meshesColisionables.AddRange(ParedesMesh());
            meshesColisionables.AddRange(RocasMesh());
            meshesColisionables.AddRange(PisosMesh());
            meshesColisionables.AddRange(CajasMesh());
            meshesColisionables.AddRange(SarcofagosMesh());
            meshesColisionables.AddRange(PilaresMesh());
            meshesColisionables.AddRange(ResbalososMesh());
            meshesColisionables.AddRange(PlataformasMesh());
            
            return meshesColisionables;
        }


        public List<TgcBoundingAxisAlignBox> MeshesColisionablesBB()
        {
            return MeshesColisionables().FindAll(mesh => mesh.Name != "PlataformaRotante").Select(mesh => mesh.BoundingBox).ToList();
        }

        public List<TgcMesh> ObjetosColisionablesConCajas()
        {
            return ParedesMesh().Concat(RocasMesh())
                                .Concat(CajasMesh())
                                .Concat(SarcofagosMesh())
                                .Concat(PilaresMesh())
                                .ToList();
        }

        public List<TgcMesh> ObstaculosColisionablesConCamara()
        {
            List<TgcMesh> obstaculos = new List<TgcMesh>();
            obstaculos.AddRange(ParedesMesh());
            obstaculos.AddRange(RocasMesh());

            return obstaculos;
        }

        public void RenderizarBoundingBoxes()
        {
            MeshesColisionables().ForEach(mesh => BoundingBoxRender(mesh));
           
        }
        private void BoundingBoxRender(TgcMesh mesh)
        {
            if (mesh.Name != "PlataformaRotante") mesh.BoundingBox.Render();
        }

        public void RenderAll() => scene.RenderAll();
        
        public void DisposeAll() => scene.DisposeAll();
        
        public TgcBoundingAxisAlignBox BoundingBox() => scene.BoundingBox;

        public List<TgcBoundingAxisAlignBox> MeshesColisionablesBBSin(TgcMesh box)
        {
            List<TgcMesh> obstaculos = MeshesColisionables();
            var indice = -1;
            indice = obstaculos.FindIndex(mesh => mesh.Name == box.Name);

            if (indice != -1) obstaculos.RemoveAt(indice);
            return obstaculos.ConvertAll(mesh => mesh.BoundingBox);
        }

        public List<TgcMesh> MeshesColisionablesSin(string layer)
        {
            List<TgcMesh> obstaculos = MeshesColisionables();

            obstaculos.RemoveAll(mesh => mesh.Layer == layer);
            
            return obstaculos;
        }




    }
}
