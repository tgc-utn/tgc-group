using BulletSharp;
using System.Drawing;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model
{
    class ObjetoJuego: IObjetoJuego
    {
        //Objetos de meshes
        public TgcMesh    Mesh        { get { return mesh; } }
        public TGCVector3 Scale       { get { return new TGCVector3(scale); } }
        public TGCVector3 Translation { get { return new TGCVector3(translation); } }
        public TGCVector4 Rotation    { get { return new TGCVector4(rotation.X,rotation.Y,rotation.Z,rotation.W); } }
        public RigidBody  Cuerpo      { get { return cuerpo; } }

        protected BulletSharp.Math.Vector3 translation;
        protected BulletSharp.Math.Vector3 scale;

        protected TgcMesh    mesh;
        protected BulletSharp.Math.Quaternion rotation;
        protected RigidBody cuerpo;

        public ObjetoJuego(TgcMesh mesh, TGCVector3 translation=new TGCVector3(), TGCVector3 rotation=new TGCVector3(), float angle=0)
        {
            this.mesh = mesh;

            this.translation = new BulletSharp.Math.Vector3(translation.X,translation.Y,translation.Z);

            scale = new BulletSharp.Math.Vector3();

            this.rotation = new BulletSharp.Math.Quaternion( new BulletSharp.Math.Vector3(rotation.X,rotation.Y,rotation.Z)  ,angle);
        }

        public virtual void Update(float elapsedTime)
        {
            cuerpo.InterpolationWorldTransform.Decompose(out scale, out rotation, out translation);
        }

        public virtual void Render()
        {
            Mesh.Transform = new TGCMatrix(cuerpo.InterpolationWorldTransform);
            Mesh.Render();
            
            Mesh.BoundingBox.transform(Mesh.Transform);
            Mesh.BoundingBox.Render();

            RenderRigidBodyBoundingBox();
        }

        public static void RenderRigidBodyBoundingBox(RigidBody rigidBody, TGCVector3 translation, TGCVector3 scale)
        {
            BulletSharp.Math.Vector3 aabbMin = new BulletSharp.Math.Vector3();
            BulletSharp.Math.Vector3 aabbMax = new BulletSharp.Math.Vector3();
            BulletSharp.Math.Matrix aabbTransform = new BulletSharp.Math.Matrix();

            rigidBody.GetWorldTransform(out aabbTransform);

            rigidBody.CollisionShape.GetAabb(aabbTransform, out aabbMin, out aabbMax);

            TgcBoundingAxisAlignBox aabb = new TgcBoundingAxisAlignBox(new TGCVector3(aabbMin), new TGCVector3(aabbMax), translation, scale);

            aabb.setRenderColor(Color.Blue);
            aabb.Render();
        }

        public void RenderRigidBodyBoundingBox()
        {
            RenderRigidBodyBoundingBox(cuerpo, Translation, Scale);
        }

        public virtual void Dispose()
        {
            cuerpo.Dispose();
        }
    }
}
