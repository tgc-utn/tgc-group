﻿using System;
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
using TGC.Group.Modelo.Plataformas;
using TGC.Group.SphereCollisionUtils;
using TGC.Group.Modelo.Rampas;
using TGC.Group.Modelo.Cajas;

namespace TGC.Group.Modelo
{
    public class Escenario
    {
        public TgcScene scene { get; set; }
        public Personaje personaje { get; }
        public float Ypiso { get; } = 20f;

        private List<TgcMesh> frutas;
        public List<PisoInercia> pisosInercia;
        public List<Hoguera> hogueras { get; set;}
        public List<FuegoLuz> fuegosLuz { get; set;}
        public List<Plataforma> plataformas { get; set; }
        public List<PlataformaRotante> plataformasRotantes { get; set; }

        private float danioLava = 0.009f;

        public Escenario(string pathEscenario,Personaje personaje)
        {
            var loader = new TgcSceneLoader();
            scene = loader.loadSceneFromFile(pathEscenario);
            this.personaje = personaje;
            
            frutas = Frutas();
            pisosInercia = PisosInercia();
            hogueras = Hogueras();
            fuegosLuz = FuegosLuz();

            //Obtenemos las plataformas segun su tipo de movimiento.
            plataformas = Plataformas();
            plataformasRotantes = PlataformasRotantes();
        }
        

        #region MeshLists
        public List<TgcMesh> encontrarMeshes(string clave) => scene.Meshes.FindAll(x => x.Layer == clave);
        public List<TgcMesh> ParedesMesh() => encontrarMeshes("PAREDES");
        public List<TgcMesh> RocasMesh() => encontrarMeshes("ROCAS");
        public List<TgcMesh> PisosMesh() => encontrarMeshes("PISOS");
        public List<TgcMesh> CajasMesh() => encontrarMeshes("CAJAS");
        public List<TgcMesh> SarcofagosMesh() => encontrarMeshes("SARCOFAGOS");
        public List<TgcMesh> PilaresMesh() => encontrarMeshes("PILARES");
        public List<TgcMesh> PlataformasMesh() => encontrarMeshes("PLATAFORMA");
        public List<TgcMesh> PisosResbalososMesh() => encontrarMeshes("RESBALOSOS");
        public List<TgcMesh> LavaMesh() => encontrarMeshes("LAVA");
        public List<TgcMesh> Luces() => encontrarMeshes("Luces");
        public List<TgcMesh> Frutas() => encontrarMeshes("FRUTA");
        public List<TgcMesh> Mascaras() => encontrarMeshes("MASCARA");
        public List<TgcMesh> Escalones() => encontrarMeshes("ESCALON");
        public List<TgcMesh> RampasMesh() => encontrarMeshes("RAMPA");
        public List<TgcMesh> FuegosMesh() => encontrarMeshes("FUEGO");
        public List<TgcMesh> HoguerasMesh() => encontrarMeshes("HOGUERA");
        public List<TgcMesh> MetasMesh() => encontrarMeshes("META");

        public List<TgcMesh> MeshesParaEfectoLava()
        {
            List<TgcMesh> meshesParaEfectoLava = new List<TgcMesh>();
            meshesParaEfectoLava.AddRange(LavaMesh());
            meshesParaEfectoLava.AddRange(FuegosMesh());
            meshesParaEfectoLava.AddRange(HoguerasMesh());

            return meshesParaEfectoLava;
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
            meshesColisionables.AddRange(PisosResbalososMesh());
            meshesColisionables.AddRange(PlataformasMesh());
            meshesColisionables.AddRange(LavaMesh());
            meshesColisionables.AddRange(Escalones());

            return meshesColisionables;
        }

        public List<TgcMesh> MeshesLuminosos()
        {
            List<TgcMesh> meshesBloom = new List<TgcMesh>();
            meshesBloom.AddRange(Luces());
            meshesBloom.AddRange(FuegosMesh());
            meshesBloom.AddRange(HoguerasMesh());
            meshesBloom.AddRange(LavaMesh());
            meshesBloom.AddRange(PisosResbalososMesh());
            return meshesBloom;
        }

        public List<TgcMesh> MeshesOpacos()
        {
            List<TgcMesh> opacos = new List<TgcMesh>();
            opacos.AddRange(ParedesMesh());
            opacos.AddRange(RocasMesh());
            //opacos.AddRange(PisosMesh());
            opacos.AddRange(CajasMesh());
            opacos.AddRange(SarcofagosMesh());
            opacos.AddRange(PilaresMesh());
            opacos.AddRange(PlataformasMesh());
            //opacos.AddRange(PisosResbalososMesh());
            opacos.AddRange(Frutas());
            opacos.AddRange(Mascaras());
            opacos.AddRange(Escalones());
            opacos.AddRange(RampasMesh());

            return opacos;
        }


        public List<TgcBoundingAxisAlignBox> MeshesColisionablesBB()
        {
            return MeshesColisionables().FindAll(mesh => mesh.Name != "PlataformaRotante").Select(mesh => mesh.BoundingBox).ToList();
        }
        
        public List<TgcMesh> MeshesColisionablesSin(TgcMesh box)
        {
            List<TgcMesh> obstaculos = MeshesColisionables();
            var indice = -1;
            indice = obstaculos.FindIndex(mesh => mesh.Name == box.Name);

            if (indice != -1) obstaculos.RemoveAt(indice);
            return obstaculos;
        }

        public List<TgcBoundingAxisAlignBox> MeshesColisionablesBBSin(TgcMesh box)
        {
            List<TgcMesh> obstaculos = MeshesColisionablesSin(box);
            var indice = -1;
            indice = obstaculos.FindIndex(mesh => mesh.Name == box.Name);

            if (indice != -1) obstaculos.RemoveAt(indice);
            return obstaculos.Select(mesh => mesh.BoundingBox).ToList();
        }



        #endregion

        #region Colisiones
        public bool colisionConPilar()
        {
            return this.PilaresMesh().Exists(mesh => personaje.colisionaConMesh(mesh));
        }

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

        public bool colisionEscenario()
        {
            
            return this.MeshesColisionables().FindAll(mesh => mesh.Layer != "CAJAS" && mesh.Layer != "PISOS").Find(mesh => personaje.colisionaConMesh(mesh)) != null;
        }

        public bool colisionDeSalto()
        {
            
            List<TgcMesh> colisionablesSalto = new List<TgcMesh>();
            colisionablesSalto.AddRange(MeshesColisionables());
            colisionablesSalto.AddRange(RampasMesh());
            return colisionablesSalto.Exists(mesh => personaje.colisionaPorArribaDe(mesh));
        }

        public Caja obtenerColisionCajaPersonaje()
        {
            return this.Cajas().Find(caja=>personaje.colisionaConCaja(caja));
        }

        public Rampa obtenerColisionRampaPersonaje()
        {
            return this.Rampas().Find(rampa => personaje.colisionConPisoDesnivelado(rampa.rampaMesh));
        }

        public PisoInercia obtenerColisionPisoInerciaPersonaje()
        {
            return this.pisosInercia.Find(piso => personaje.colisionaPorArribaDe(piso.pisoMesh));
        }

        public bool colisionaConMeta()
        {
            return this.MetasMesh().Exists(meta => personaje.colisionaConMesh(meta));
        }

        

       
        #endregion

        #region Personaje

        public bool personajeSobreMascara()
        {
            return Mascaras().Exists(mascara => personaje.colisionaConMesh(mascara));
        }

        public void eliminarMascaraColisionada()
        {
            TgcMesh mascaraColisionada = Mascaras().Find(mascara => personaje.colisionaConMesh(mascara));
            eliminarObjeto(mascaraColisionada);
        }

        public void eliminarObjeto(TgcMesh mesh)
        {
            mesh.Enabled = false;
            scene.Meshes.Remove(mesh);
            Juego.octree.modelos.Remove(mesh);
        }


        public TgcMesh obtenerFrutaColisionada()
        {
            return Frutas().Find(fruta => personaje.colisionaConMesh(fruta));
        }

        public void eliminarFrutaColisionada()
        {
            TgcMesh frutaColisionada = Frutas().Find(fruta => personaje.colisionaConMesh(fruta));
            eliminarObjeto(frutaColisionada);
        }

        public bool personajeSobreLava()
        {
            var auxiliarBoundingBox = personaje.boundingBox();
            auxiliarBoundingBox.move(new TGCVector3(0, -Ypiso, 0));
            return LavaMesh().Exists(lava => TgcCollisionUtils.testAABBAABB(lava.BoundingBox, auxiliarBoundingBox));

        }

        public bool personajeSobreDesnivel()
        {
            return RampasMesh().Exists(pisoDesnivelado => personaje.colisionConPisoDesnivelado(pisoDesnivelado));
        }

        public void quemarPersonaje()
        {
            personaje.vida -= danioLava ;
        }
        #endregion

        #region MeshToClassAdapters

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

        public List<Rampa> Rampas()
        {
            List<Rampa> rampas = new List<Rampa>();

            foreach (TgcMesh rampaMesh in RampasMesh())
            {

                Rampa rampa;

                if (rampaMesh.Name == "RAMPAX") rampa = new RampaX(rampaMesh, this);
                else  rampa = new RampaZ(rampaMesh, this);
                rampas.Add(rampa);

            }

            return rampas;
        }

        public List<Caja> Cajas()
        {
            List<Caja> cajas = new List<Caja>();

            foreach (TgcMesh cajaMesh in CajasMesh())
            {

                Caja caja;

                if(cajaMesh.Name == "TNT") caja = new CajaTnt(cajaMesh, this);
                else if(cajaMesh.Name == "NITRO") caja = new CajaNitro(cajaMesh, this);
                else caja = new Caja(cajaMesh, this);
                cajas.Add(caja);

            }

            return cajas;
        }

        public List<PisoInercia> PisosInercia()
        {
            List<PisoInercia> pisosInercia = new List<PisoInercia>();

            foreach (TgcMesh pisoInercia in PisosResbalososMesh())
            {

                PisoInercia piso = new PisoInercia(pisoInercia);
                pisosInercia.Add(piso);

            }

            return pisosInercia;
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

        public List<Hoguera> Hogueras()
        {
            List<Hoguera> hogueras = new List<Hoguera>();
            foreach (TgcMesh mesh in HoguerasMesh())
            {
                hogueras.Add(new Hoguera(mesh, 1));
            }
            return hogueras;

           
        }
        public List<FuegoLuz> FuegosLuz()
        {
            List<FuegoLuz> fuegosLuz = new List<FuegoLuz>();
            foreach (TgcMesh mesh in FuegosMesh())
            {
                fuegosLuz.Add(new FuegoLuz(mesh));
            }
            return fuegosLuz;
        }

        #endregion

        public List<TgcMesh> FuentesDeLuz()
        {
            List<TgcMesh> fuentesDeLuz = new List<TgcMesh>();
            fuentesDeLuz.AddRange(Luces());
            fuentesDeLuz.AddRange(LavaMesh());
            fuentesDeLuz.AddRange(FuegosMesh());
            fuentesDeLuz.AddRange(HoguerasMesh());


            return fuentesDeLuz;
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
           Rampas().ForEach(rampa => BoundingBoxRender(rampa.rampaMesh));
        }
        private void BoundingBoxRender(TgcMesh mesh)
        {
            if (mesh.Name != "PlataformaRotante") mesh.BoundingBox.Render();
        }

        public void RenderAll() => scene.RenderAll();
        
        public void DisposeAll() => scene.DisposeAll();
        
        public TgcBoundingAxisAlignBox BoundingBox() => scene.BoundingBox;

        

        public List<TgcMesh> MeshesColisionablesSin(string layer)
        {
            List<TgcMesh> obstaculos = MeshesColisionables();

            obstaculos.RemoveAll(mesh => mesh.Layer == layer);
            
            return obstaculos;
        }


        public TgcMesh obtenerFuenteLuzCercana(TGCVector3 pos, float maxDistance)
        {
            var minDist = float.MaxValue;
            TgcMesh minLight = null;

            foreach (var light in FuentesDeLuz())
            {
                var distSq = TGCVector3.LengthSq(pos - light.BoundingBox.calculateBoxCenter());
                if (distSq < minDist)
                {
                    minDist = distSq;
                    minLight = light;
                }
            }

            if (minLight != null)
            {
                if (maxDistance != 0 && TGCVector3.LengthSq(pos - minLight.BoundingBox.calculateBoxCenter()) > (maxDistance * maxDistance))
                {
                    return null;
                }
            }
            return minLight;
        }

        public Hoguera getClosestBonfire(TGCVector3 pos, float maxDistance)
        {
            var minDist = float.MaxValue;
            TgcMesh minF = null;

            foreach (var fuego in HoguerasMesh())
            {
                var distSq = TGCVector3.LengthSq(pos - fuego.BoundingBox.calculateBoxCenter());
                if (distSq < minDist)
                {
                    minDist = distSq;
                    minF = fuego;
                }
            }

            if (minF != null)
            {
                if (maxDistance != 0 && TGCVector3.LengthSq(pos - minF.BoundingBox.calculateBoxCenter()) > (maxDistance * maxDistance))
                {
                    return null;
                }
                foreach(Hoguera h in hogueras)
                {
                    if(h.MeshHoguera.GetHashCode() == minF.GetHashCode())
                    {
                        return h;
                    }
                }
            }
            return null;
        }

    }
}
