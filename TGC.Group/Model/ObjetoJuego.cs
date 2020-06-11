using BulletSharp;
using System.Drawing;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;

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
        public TgcBoundingAxisAlignBox AABB { get; private set; }

        protected BulletSharp.Math.Vector3 translation;
        protected BulletSharp.Math.Vector3 scale;

        protected TgcMesh    mesh;
        protected BulletSharp.Math.Quaternion rotation;
        protected RigidBody cuerpo;

        public float Ka { get; set; }
        public float Kd { get; set; }
        public float Ks { get; set; }
        public float shininess { get; set; }

        public ObjetoJuego(TgcMesh mesh, TGCVector3 translation=new TGCVector3(), TGCVector3 rotation=new TGCVector3(), float angle=0)
        {
            this.mesh = mesh;

            this.translation = new BulletSharp.Math.Vector3(translation.X,translation.Y,translation.Z);

            scale = new BulletSharp.Math.Vector3();

            this.rotation = new BulletSharp.Math.Quaternion(new BulletSharp.Math.Vector3(rotation.X,rotation.Y,rotation.Z), angle);

            AABB = new TgcBoundingAxisAlignBox();

            //mesh.Effect = TGCShaders.Instance.LoadEffect(@"C:\Users\jakss\Documents\TGC\2020_1C_3051_GroutingLeague\TGC.Group\Shaders\CustomShaders.fx");
            //mesh.Technique = "SOL";
        }

        public virtual void Update(float elapsedTime)
        {
            cuerpo.InterpolationWorldTransform.Decompose(out scale, out rotation, out translation);
            UpdateAABB();
        }

        protected void UpdateAABB()
        {
            BulletSharp.Math.Vector3 aabbMin = new BulletSharp.Math.Vector3();
            BulletSharp.Math.Vector3 aabbMax = new BulletSharp.Math.Vector3();
            BulletSharp.Math.Matrix aabbTransform = new BulletSharp.Math.Matrix();

            cuerpo.GetWorldTransform(out aabbTransform);

            cuerpo.CollisionShape.GetAabb(aabbTransform, out aabbMin, out aabbMax);

            AABB = new TgcBoundingAxisAlignBox(new TGCVector3(aabbMin), new TGCVector3(aabbMax), new TGCVector3(translation), new TGCVector3(scale));
        }

        public bool CheckCollideWith(ObjetoJuego objeto) => TgcCollisionUtils.testAABBAABB(this.AABB, objeto.AABB);

        public virtual void Render()
        {
            Mesh.Transform = new TGCMatrix(cuerpo.WorldTransform);
            Mesh.Render();

            Mesh.BoundingBox.transform(Mesh.Transform);
            Mesh.BoundingBox.Render();

            RenderRigidBodyBoundingBox();
        }
        public virtual void Render(Luz luz)
        {
            Mesh.Effect.SetValue("lightColor", TGCVector3.TGCVector3ToFloat3Array(new TGCVector3(luz.Color.R, luz.Color.G, luz.Color.B)));
            Mesh.Effect.SetValue("lightPosition", TGCVector3.TGCVector3ToFloat3Array(luz.Translation));
            Mesh.Effect.SetValue("Ka", Ka);
            Mesh.Effect.SetValue("Kd", Kd);
            Mesh.Effect.SetValue("Ks", Ks);
            Mesh.Effect.SetValue("shininess", shininess);
            Render();
        }

        public void RenderRigidBodyBoundingBox()
        {
            AABB.setRenderColor(Color.Blue);
            AABB.Render();
        }
        public virtual void Dispose()
        {
            cuerpo.Dispose();
        }
    }
}
