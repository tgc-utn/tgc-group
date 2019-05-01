using TGC.Core.BoundingVolumes;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using BulletSharp;

namespace TGC.Group.Model
{
    public class Element : Collisionable
    {
        public TgcMesh Mesh { get; }
        public RigidBody RidigBody { get; set; }


        public Element(TgcMesh model)
        {
            this.Mesh = model;
           //this.RidigBody = rigidBody;
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