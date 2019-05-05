using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model.Elements
{
    abstract class ElementFactory
    {
        private static Random Random = new Random();
        private readonly List<TgcMesh> Meshes;
        private readonly IRigidBodyFactory BodyFactory;

        public ElementFactory(List<TgcMesh> meshes, IRigidBodyFactory rigidBodyFactory)
        {
            Meshes = meshes;
            BodyFactory = rigidBodyFactory;
        }

        public Element Create(Cube cube)
        {
            var mesh = CreateCompletedMesh(cube);
            var rigidBody = BodyFactory.Create(mesh);
            return CreateSpecificElement(mesh, rigidBody);
        }

        private TgcMesh CreateCompletedMesh(Cube cube)
        {
            var newMesh = CreateSimpleMesh();
            newMesh.Scale = ScaleOfBoxToBox(newMesh.BoundingBox, cube);
            newMesh.Position = cube.PMin;
            newMesh.UpdateMeshTransform();
            return newMesh;
        }
        private TGCVector3 ScaleOfBoxToBox(TgcBoundingAxisAlignBox boundingBox, Cube scaleCube)
        {
            var boundingBoxMax = boundingBox.PMax - boundingBox.PMin;
            var scaleBoxMax = scaleCube.PMax - scaleCube.PMin;

            var minScale = new[]
            {
                scaleBoxMax.X / boundingBoxMax.X,
                scaleBoxMax.Y / boundingBoxMax.Y,
                scaleBoxMax.Z / boundingBoxMax.Z
            }.Min();

            return new TGCVector3(minScale, minScale, minScale);
        }

        private TgcMesh CreateSimpleMesh()
        {
            var mesh = GetRandomMesh();
            return mesh.createMeshInstance(mesh.Name);
        }

        private TgcMesh GetRandomMesh()
        {
            return Meshes[Random.Next(Meshes.Count())];
        }

        protected abstract Element CreateSpecificElement(TgcMesh mesh, BulletSharp.RigidBody rigidBody);
    }

}
