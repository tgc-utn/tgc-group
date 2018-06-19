using Microsoft.DirectX.Direct3D;
using System.Collections.Generic;
using System.Linq;
using TGC.Core.BoundingVolumes;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.Terrain;
using TGC.Core.Textures;

namespace TGC.Group.Model.Niveles {

    public abstract class Nivel {

        protected const float VELPEPE = 200f;

        protected List<TgcTexture> texturasUsadas;
        protected List<TgcPlane> pisosNormales;
        protected List<TgcPlane> pisosResbaladizos;
        protected List<TgcPlane> pMuerte;
        protected List<Caja> cajas;
        protected List<Plataforma> pEstaticas;
        protected List<PlataformaDesplazante> pDesplazan;
        protected List<PlataformaRotante> pRotantes;
        protected List<PlataformaAscensor> pAscensor;
        protected List<TgcMesh> decorativos;
        protected List<TgcMesh> rampas;
        protected List<TgcBoundingAxisAlignBox> aabbDeDecorativos;
        protected List<TgcBoundingAxisAlignBox> aabbSegmentoRampa;
        protected TgcSceneLoader loaderDeco;
        protected TGCBox lfBox;
        protected TgcSkyBox skyBox;
        protected string pathSkyBox;
        public Nivel siguienteNivel;

        protected Effect smEffect;

        public Nivel(string mediaDir) {

            texturasUsadas = new List<TgcTexture>();
            pisosNormales = new List<TgcPlane>();
            pisosResbaladizos = new List<TgcPlane>();
            cajas = new List<Caja>();
            pEstaticas = new List<Plataforma>();
            pDesplazan = new List<PlataformaDesplazante>();
            pRotantes = new List<PlataformaRotante>();
            pAscensor = new List<PlataformaAscensor>();
            decorativos = new List<TgcMesh>();
            rampas = new List<TgcMesh>();
            aabbDeDecorativos = new List<TgcBoundingAxisAlignBox>();
            aabbSegmentoRampa = new List<TgcBoundingAxisAlignBox>();
            loaderDeco = new TgcSceneLoader();

            // TODO: ver si estos son necesarios
            pMuerte = new List<TgcPlane>();

            init(mediaDir);
        }

        public abstract void init(string mediaDir);

        public void update(float deltaTime) {
            getUpdateables().ForEach(r => r.Update(deltaTime));
        }

        public void render() {
            smEffect.SetValue("k_ls", 0.2f);
            getRenderizables().ForEach(r => r.Render());

            smEffect.SetValue("k_ls", 0.5f);
            getRenderizablesBrillantes().ForEach(r => r.Render());

            if (lfBox != null) lfBox.BoundingBox.Render();
            aabbDeDecorativos.ForEach(r => r.Render());

            if (skyBox != null) {
                skyBox.Render();
            }
        }

        public void dispose() {
            foreach (var textura in texturasUsadas) {
                textura.dispose();
            }

            getRenderizables().ForEach(r => r.Dispose());

            skyBox.Dispose();
        }

        public void setEffect(Effect e) {
            this.smEffect = e;
            pisosNormales.ForEach(p => p.Effect = e);
            pisosResbaladizos.ForEach(p => p.Effect = e);
            cajas.ForEach(c => c.setEffect(e));
            pEstaticas.ForEach(c => c.setEffect(e));
            pDesplazan.ForEach(c => c.setEffect(e));
            pRotantes.ForEach(c => c.setEffect(e));
            pAscensor.ForEach(c => c.setEffect(e));
            decorativos.ForEach(d => d.Effect = e);
        }

        public void setTechnique(string s) {
            pisosNormales.ForEach(p => p.Technique = s);
            pisosResbaladizos.ForEach(p => p.Technique = s);
            cajas.ForEach(c => c.setTechnique(s));
            pEstaticas.ForEach(c => c.setTechnique(s));
            pDesplazan.ForEach(c => c.setTechnique(s));
            pRotantes.ForEach(c => c.setTechnique(s));
            pAscensor.ForEach(c => c.setTechnique(s));
            decorativos.ForEach(d => d.Technique = s);
        }

        /* TODO: puede traer problemas de performance
         * tener una lista con todos los renderizables aparte?
         * agrega mas estado... pero mucho mas performante */
        protected List<IRenderObject> getRenderizables() {
            return new List<IRenderObject>().Concat(pisosNormales)
                .Concat(pMuerte)
                .Concat(cajas)
                .Concat(pEstaticas)
                .Concat(pDesplazan)
                .Concat(pRotantes)
                .Concat(pAscensor)
                .Concat(rampas)
                .ToList();
        }

        protected List<IRenderObject> getRenderizablesBrillantes() {
            return new List<IRenderObject>()
                .Concat(pisosResbaladizos)
                .Concat(decorativos)
                .ToList();
        }

        protected List<IUpdateable> getUpdateables() {
            return new List<IUpdateable>()
                .Concat(pDesplazan)
                .Concat(pRotantes)
                .Concat(pAscensor)
                .ToList();
        }

        public List<TgcBoundingAxisAlignBox> getBoundingBoxes() {
            var list = new List<TgcBoundingAxisAlignBox>();
            list.AddRange(getPisos().ToArray());
            list.AddRange(cajas.Select(caja => caja.getSuperior()).ToArray());
            //list.AddRange(cajas.Select(caja => caja.getCuerpo()).ToArray());
            list.AddRange(pEstaticas.Select(plataforma => plataforma.getAABB()).ToArray());
            list.AddRange(pDesplazan.Select(desplazante => desplazante.getAABB()).ToArray());
            list.AddRange(pRotantes.Select(rotante => rotante.getAABB()).ToArray());
            list.AddRange(pAscensor.Select(ascensor => ascensor.getAABB()).ToArray());
            list.AddRange(aabbDeDecorativos);
            list.AddRange(aabbSegmentoRampa);

            return list;
        }

        public List<TgcBoundingAxisAlignBox> getPisos() {
            var list = new List<TgcBoundingAxisAlignBox>();
            list.AddRange(pisosNormales.Select(piso => piso.BoundingBox).ToArray());
            list.AddRange(pisosResbaladizos.Select(piso => piso.BoundingBox).ToArray());
            list.AddRange(pEstaticas.Select(caja => caja.getAABB()).ToArray());
            list.AddRange(pDesplazan.Select(caja => caja.getAABB()).ToArray());
            list.AddRange(pRotantes.Select(caja => caja.getAABB()).ToArray());
            list.AddRange(pAscensor.Select(caja => caja.getAABB()).ToArray());

            return list;
        }

        public List<TgcBoundingAxisAlignBox> getEstaticos() {
            var list = new List<TgcBoundingAxisAlignBox>();
            list.AddRange(pEstaticas.Select(caja => caja.getAABB()).ToArray());

            return list;
        }

        public List<Caja> getCajas() {
            return cajas;
        }

        public TgcBoundingAxisAlignBox getLFBox() {
            if (lfBox == null) return null;
            return lfBox.BoundingBox;
        }

        public bool esPisoResbaladizo(TgcBoundingAxisAlignBox piso) {
            return pisosResbaladizos.Select(p => p.BoundingBox).Contains(piso);
        }

        public bool esPisoDesplazante(TgcBoundingAxisAlignBox piso) {
            return pDesplazan.Select(p => p.getAABB()).Contains(piso);
        }

        public bool esPisoRotante(TgcBoundingAxisAlignBox piso) {
            return pRotantes.Select(p => p.getAABB()).Contains(piso);
        }

        public bool esPisoAscensor(TgcBoundingAxisAlignBox piso) {
            return pAscensor.Select(p => p.getAABB()).Contains(piso);
        }

        public PlataformaDesplazante getPlataformaDesplazante(TgcBoundingAxisAlignBox piso) {
            return pDesplazan.Find(p => p.getAABB() == piso);
        }

        public PlataformaRotante getPlataformaRotante(TgcBoundingAxisAlignBox piso) {
            return pRotantes.Find(p => p.getAABB() == piso);
        }

        public PlataformaAscensor getPlataformaAscensor(TgcBoundingAxisAlignBox piso) {
            return pAscensor.Find(p => p.getAABB() == piso);
        }

        public List<TgcBoundingAxisAlignBox> getDeathPlanes() {
            return pMuerte.Select(p => p.BoundingBox).ToList();
        }

        public void agregarPisoNormal(TGCVector3 origen, TGCVector3 tamanio, TgcTexture textura) {
            var piso = new TgcPlane(origen, tamanio, TgcPlane.Orientations.XZplane, textura);
            pisosNormales.Add(piso);
        }

        public void agregarPisoResbaladizo(TGCVector3 origen, TGCVector3 tamanio, TgcTexture textura) {
            var piso = new TgcPlane(origen, tamanio, TgcPlane.Orientations.XZplane, textura);
            pisosResbaladizos.Add(piso);
        }

        public void agregarPared(TGCVector3 centro, TGCVector3 tamanio, TgcTexture textura) {
            var pared = new Plataforma(centro, tamanio, textura);
            pEstaticas.Add(pared);
        }

        public void inicializarSkyBox(string pathCaras) {
            skyBox = new TgcSkyBox();
            skyBox.Center = new TGCVector3(0, 0, 4000);
            skyBox.Size = new TGCVector3(18000, 1800, 18000);
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, pathCaras + "arriba.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, pathCaras + "abajo.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, pathCaras + "derecha.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, pathCaras + "izquierda.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, pathCaras + "frente.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, pathCaras + "atras.jpg");
            skyBox.SkyEpsilon = 25f;
            skyBox.Init();
        }

        public void cargarDecorativo(TgcMesh unDecorativo, TgcScene unaEscena, TGCVector3 posicion, TGCVector3 escala, float rotacion) {
            unDecorativo = unaEscena.Meshes[0];
            unDecorativo.Position = posicion;
            unDecorativo.Scale = escala;
            unDecorativo.RotateY(rotacion);
            decorativos.Add(unDecorativo);
            aabbDeDecorativos.Add(unDecorativo.BoundingBox);
        }

        public void agregarEscalera(TGCVector3 origen, int cantidadEscalones, TGCVector3 tamanioEscalon, TgcTexture textura) {
            //NOTA: origen debe ser el CENTRO del primer escalon
            var alto = tamanioEscalon.Y;
            var largo = tamanioEscalon.Z; //profundidad
            var centroEscalon = origen;
            var escalon = new Plataforma(centroEscalon, tamanioEscalon, textura);
            pEstaticas.Add(escalon);
            int i;
            for (i = 2; i <= cantidadEscalones; i++) {
                centroEscalon.Y += alto;
                centroEscalon.Z -= largo;
                escalon = new Plataforma(centroEscalon, tamanioEscalon, textura);
                pEstaticas.Add(escalon);
            }
        }

        /* VER: Para agregar una rampa al escenario
        public void agregarRampa(TGCVector3 centro, float ancho, float largo, TgcTexture textura)
        {
            var tamanio = new TGCVector3(ancho, 50, largo);
            var rampa = new Plataforma(centro, tamanio, textura, FastMath.QUARTER_PI);
            pEstaticas.Add(rampa);
        }*/

        //NOTA: Abstraccion para crear rampas de tamaño unico
        //NO ANDA BIEN. Mejorar
        public void agregarRampa(TGCVector3 inicio, TgcTexture textura) {
            var centroEscalon = inicio;
            var escalon = new Plataforma(inicio, new TGCVector3(100, 5.773f, 10), textura); //5.773 para hacer plano a 30º
            aabbSegmentoRampa.Add(escalon.getAABB());
            int i;
            for (i = 2; i <= 14; i++) //15 escalones en total, rampa de 86.595f de alto y 150 de profundidad
            {
                centroEscalon.Y += 5.773f;
                centroEscalon.Z -= 10;
                escalon = new Plataforma(inicio, new TGCVector3(100, 5.773f, 10), textura);
                aabbSegmentoRampa.Add(escalon.getAABB());
            }
            var centroUltimo = centroEscalon;
            //Plano se dibuja desde una esquina, lo ajusto
            centroUltimo.X -= 50;
            centroUltimo.Y += 2.8865f;
            centroUltimo.Z -= 5;
            var superficieRampa = new TgcPlane(centroUltimo, new TGCVector3(100, 0, 173.2013f), TgcPlane.Orientations.XZplane, textura);
            superficieRampa.toMesh("superficieRampa").Transform = TGCMatrix.RotationX((FastMath.PI) / 6);
            rampas.Add(superficieRampa.toMesh("superficieRampa"));
        }

    }
}
