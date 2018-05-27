using System.Collections.Generic;
using System.Linq;
using TGC.Core.BoundingVolumes;
using TGC.Core.Geometry;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Niveles {
    public abstract class Nivel {
        protected List<TgcPlane> pisosNormales;
        protected List<TgcPlane> pisosResbaladizos;
        protected List<TgcPlane> pMuerte;
        protected List<Caja> cajas;
        protected List<Plataforma> pEstaticas;
        protected List<PlataformaDesplazante> pDesplazan;
        protected List<PlataformaRotante> pRotantes;
        protected List<PlataformaAscensor> pAscensor;

        public Nivel(string mediaDir) {
            pisosNormales = new List<TgcPlane>();
            pisosResbaladizos = new List<TgcPlane>();
            cajas = new List<Caja>();
            pEstaticas = new List<Plataforma>();
            pDesplazan = new List<PlataformaDesplazante>();
            pRotantes = new List<PlataformaRotante>();
            pAscensor = new List<PlataformaAscensor>();

            // TODO: ver si estos son necesarios
            pMuerte = new List<TgcPlane>();

            init(mediaDir);
        }

        public abstract void init(string mediaDir);

        public void update(float deltaTime) {
            getUpdateables().ForEach(r => r.Update(deltaTime));
        }

        public void render() {
            getRenderizables().ForEach(r => r.Render());
        }

        public abstract void dispose();

        /* TODO: puede traer problemas de performance
         * tener una lista con todos los renderizables aparte?
         * agrega mas estado... pero mucho mas performante */
        protected List<IRenderObject> getRenderizables() {
            return new List<IRenderObject>().Concat(pisosNormales)
                .Concat(pisosResbaladizos)
                .Concat(pMuerte)
                .Concat(cajas)
                .Concat(pEstaticas)
                .Concat(pDesplazan)
                .Concat(pRotantes)
                .Concat(pAscensor)
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
            // list.AddRange(cajas.Select(caja => caja.getCuerpo()).ToArray());
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

        public List<Caja> getCajas() {
            return cajas;
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

    }
}
