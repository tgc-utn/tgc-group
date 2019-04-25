using System;
using TGC.Core.BoundingVolumes;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;


namespace TGC.Group.Model
{
    class Element : Collisionable
    {
        public TgcMesh Mesh { get; set; }


        public Element(TGCVector3 origin, TgcMesh model)
        {
            this.Mesh = model;
            model.Position = origin;
        }

        public Element(TGCVector3 origin, TgcMesh model, TGCVector3 scale) : this(origin,model)
        {
            this.Mesh.Scale = scale;
        }

        public void Update()
        {
            return;
        }

        public void Render()
        {
            //Cuando tenemos modelos mesh podemos utilizar un método que hace la matriz de transformación estándar.
            //Es útil cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones jerárquicas o complicadas.
            Mesh.UpdateMeshTransform();
            Mesh.Render();
            return;
        }

        public void Dispose()
        {
            Mesh.Dispose();
            return;
        }

        public override TgcBoundingAxisAlignBox getCollisionVolume()
        {
            return Mesh.BoundingBox;
        }
    }
}