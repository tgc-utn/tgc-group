using BulletSharp.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using TGC.Core.Camara;
using TGC.Core.Collision;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Group.Model.Chunks;
using TGC.Group.Model.Elements;
using TGC.Group.Model.Elements.ElementFactories;
using TGC.Group.Model.Elements.RigidBodyFactories;
using TGC.Group.Model.Input;
using TGC.Group.Model.Resources.Meshes;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model
{
    internal class World
    {
        public const int RenderRadius = 5;
        public const int UpdateRadius = RenderRadius + 1;
        private const int InteractionRadius = 490000; // Math.pow(700, 2)
        
        private readonly Dictionary<TGCVector3, Chunk> chunks;
        private readonly List<Entity> entities;
        private readonly WaterSurface waterSurface;
        public Element SelectableElement { get; private set; }

        public World(TGCVector3 initialPoint)
        {
            this.chunks = new Dictionary<TGCVector3, Chunk>();
            
            this.entities = new List<Entity>();

            this.waterSurface = new WaterSurface(initialPoint);
            
            AddChunk(initialPoint);
            AddShark();
        }

        protected void AddShark()
        {
            var mesh = SharkMesh.All()[0];
            mesh.Position = new TGCVector3(30, 1000, -2000);
            mesh.UpdateMeshTransform();
            var rigidBody = new CapsuleFactory().CreateShark(mesh);
            TGCVector3 scaled = new TGCVector3(10, 10, 10);
            AquaticPhysics.Instance.Add(rigidBody);
            var shark = new Shark(mesh, rigidBody);
            this.entities.Add(shark);
        }

        private Chunk AddChunk(TGCVector3 origin)
        {
            var chunk = Chunk.ByYAxis(origin);
            
            this.chunks.Add(origin, chunk);

            this.entities.AddRange(chunk.Init());

            return chunk;
        }

        public List<Collisionable> GetCollisionables()
        {
            var res = new List<Collisionable>();

            res.AddRange(this.entities);

            foreach (var chunk in this.chunks.Values)
            {
                res.AddRange(chunk.Elements);
            }

            return res;
        }

        private List<Chunk> GetChunksByRadius(TGCVector3 origin, int radius)
        {
            var toUpdate = new List<Chunk>();
            var intOrigin = new TGCVector3(
                (int)(origin.X/Chunk.DefaultSize.X), 
                (int)(origin.Y/Chunk.DefaultSize.Y), 
                (int)(origin.Z/Chunk.DefaultSize.Z));

            for (var i = -radius; i <= radius; i++)
            {
                for (var j = -radius; j <= radius; j++)
                {
                    for (var k = -radius; k <= radius; k++)
                    {
                        var position = new TGCVector3(
                            Chunk.DefaultSize.X * (intOrigin.X + i),
                            Chunk.DefaultSize.Y * (intOrigin.Y + j),
                            Chunk.DefaultSize.Z * (intOrigin.Z + k));
                        
                        toUpdate.Add(this.chunks.ContainsKey(position) ? this.chunks[position] : AddChunk(position));
                    }
                }
            }
            
            return toUpdate;
        }
        
        private List<Chunk> ToUpdate(TGCVector3 cameraPosition)
        {
            return GetChunksByRadius(cameraPosition, UpdateRadius);
        }

        private List<Chunk> ToRender(TGCVector3 cameraPosition)
        {
            return GetChunksByRadius(cameraPosition, RenderRadius);
        }

        public void Update(Camera camera)
        {
            var toUpdate = ToUpdate(camera.Position);
            toUpdate.ForEach(chunk => chunk.Update(camera));
            this.SelectableElement = GetSelectableElement(camera, toUpdate);
            this.entities.ForEach(entity => entity.Update(camera));
        }
        
        public void Render(TgcCamera camera)
        {
            ToRender(camera.Position).ForEach(chunk => chunk.Render());
            this.entities.ForEach(entity => entity.Render());
            this.waterSurface.Render(camera.Position);
        }

        public void RenderBoundingBox(TgcCamera camera)
        {
            ToRender(camera.Position).ForEach(chunk => chunk.RenderBoundingBox());
        }

        public void Dispose()
        {
            this.chunks.Values.ToList().ForEach(chunk => chunk.Dispose());
            this.entities.ForEach(entity => entity.Dispose());
        }
        private static Element GetSelectableElement(TgcCamera camera, List<Chunk> toUpdate)
        {            
            var direction = camera.LookAt - camera.Position;
            direction.Normalize();

            var intersectedElements =
                toUpdate
                    .SelectMany(chunk => chunk.Elements)
                    .ToList()
                    .FindAll(element => element.isIntersectedBy(new TgcRay(camera.Position, direction)));
            
            intersectedElements.Sort((element1, element2) => 
                (int) TGCVector3.LengthSq(camera.Position, element1.Mesh.Position) -
                (int) TGCVector3.LengthSq(camera.Position, element2.Mesh.Position));

            return intersectedElements.Find(element => 
                Math.Abs(TGCVector3.LengthSq(camera.Position, element.Mesh.Position)) < InteractionRadius);
        }
        public void Remove(Element selectableElement)
        {
            foreach (var chunk in this.chunks.Values)
            {
                chunk.Remove(selectableElement);
            }
        }
    }
}
