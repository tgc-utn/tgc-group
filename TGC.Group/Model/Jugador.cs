using BulletSharp;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BulletPhysics;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Example;

namespace TGC.Group.Model
{
    class Jugador
    {
        public TgcMesh Mesh { get; }
        public TGCVector3 Position { get; set; }
        public TGCVector3 Rotation { get; set; }

        public RigidBody CocheBody { get; }

        public Jugador(TgcMesh mesh, TGCVector3 position, TGCVector3 rotation)
        {
            Mesh = mesh;
            Position = position;
            Rotation = rotation;
            Position += new TGCVector3(0, 10, 0);
            CocheBody = BulletRigidBodyFactory.Instance.CreateBox(Mesh.Scale, 1f, Position, 0f, 0f, 0f, 1f, true);
        }

        public void Update()
        {
            /*if(TGCExample.Input)
            CocheBody.Velo*/
        }

        public void Render()
        {
            Mesh.Transform = new TGCMatrix(CocheBody.InterpolationWorldTransform);
            Mesh.BoundingBox.transform(Mesh.Transform);
            Mesh.Render();
            Mesh.BoundingBox.Render();
        }


    }
}
