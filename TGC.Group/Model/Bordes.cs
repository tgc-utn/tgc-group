using BulletSharp;
using TGC.Core.BoundingVolumes;
using TGC.Core.BulletPhysics;
using TGC.Core.Mathematica;
using System.Collections.Generic;

namespace TGC.Group.Model
{


    class Bordes: IObjetoJuego
    {
        public List<RigidBody> paredes { get; set; }

        public Bordes(TgcBoundingAxisAlignBox boundingBox)
        {
            paredes = new List<RigidBody>();

            TGCVector3 size;
            TGCVector3 translate;
            TGCVector3 bbSize = boundingBox.calculateSize();

            //Atras
            translate = new TGCVector3();
            translate.X = boundingBox.PMin.X;
            translate.Y = boundingBox.PMax.Y;
            translate.Z = boundingBox.PMin.Z;
            size = new TGCVector3();
            size.X = bbSize.X;
            size.Y = bbSize.Y;
            size.Z = 1;
            paredes.Add(BulletRigidBodyFactory.Instance.CreateBox(size, 1f, translate, 0f, 0f, 0f, 1f, true));

            //Adelante
            translate = new TGCVector3();
            translate.X = boundingBox.PMin.X;
            translate.Y = boundingBox.PMax.Y;
            translate.Z = boundingBox.PMax.Z;
            size = new TGCVector3();
            size.X = bbSize.X;
            size.Y = bbSize.Y;
            size.Z = 1;
            paredes.Add(BulletRigidBodyFactory.Instance.CreateBox(size, 1f, translate, 0f, 0f, 0f, 1f, true));

            //Izquierda
            translate = new TGCVector3();
            translate.X = boundingBox.PMin.X;
            translate.Y = boundingBox.PMax.Y;
            translate.Z = boundingBox.PMin.Z;
            size = new TGCVector3();
            size.X = 1;
            size.Y = bbSize.Y;
            size.Z = bbSize.Z;

            //Derecha
            translate = new TGCVector3();
            translate.X = boundingBox.PMax.X;
            translate.Y = boundingBox.PMax.Y;
            translate.Z = boundingBox.PMin.Z;
            size = new TGCVector3();
            size.X = 1;
            size.Y = bbSize.Y;
            size.Z = bbSize.Z;
            paredes.Add(BulletRigidBodyFactory.Instance.CreateBox( size, 1f, boundingBox.PMin, 0f, 0f, 0f, 1f, true));
        }

        public void Render()
        {
            foreach (RigidBody pared in paredes)
            {
                BulletSharp.Math.Vector3 scale = new BulletSharp.Math.Vector3();
                BulletSharp.Math.Vector3 translation = new BulletSharp.Math.Vector3();
                BulletSharp.Math.Quaternion rotation = new BulletSharp.Math.Quaternion();

                pared.InterpolationWorldTransform.Decompose(out scale, out rotation, out translation);

                ObjetoJuego.RenderRigidBodyBoundingBox(pared, new TGCVector3(translation),new TGCVector3(scale));
            }
        }

        public void Update(float elapsedTime)
        {
        }

        public void Dispose()
        {
            foreach (RigidBody pared in paredes)
            {
                pared.Dispose();
            }
        }
    }
}
