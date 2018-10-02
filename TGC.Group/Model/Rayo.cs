using TGC.Core.Collision;
using System.Collections;
using TGC.Core.Mathematica;
using TGC.Core.SkeletalAnimation;
using System;
using TGC.Core.Geometry;

namespace TGC.Group.Model
{
    public abstract class Rayo
    {
        protected TGCVector3 origen;
        protected TGCVector3 direccion;
        protected TgcRay ray;
        protected TGCVector3 puntoInterseccion;

        public Rayo(TGCVector3 origen, TGCVector3 direccion)
        {
            this.origen = origen;
            this.direccion = direccion;
            this.ray = new TgcRay(origen, direccion);
            this.puntoInterseccion = TGCVector3.Empty;
        }

        public bool Colisionar(TgcSkeletalMesh personaje) {
            return TgcCollisionUtils.intersectRayAABB(this.ray, personaje.BoundingBox, out this.puntoInterseccion) ;
        }

        public abstract int DistanciaMinima();

        public bool HuboColision() {
            return FastMath.Abs(this.Intervalo()) < this.DistanciaMinima();
        }

        protected abstract float Intervalo();

        internal void Render()
        {
            TgcArrow.fromDirection(this.origen, this.direccion * 15).Render();
        }
    }
}