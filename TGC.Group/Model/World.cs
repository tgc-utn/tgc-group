using System;
using System.Collections.Generic;
using System.Linq;
using BulletSharp;
using BulletSharp.Math;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Camara;
using TGC.Core.Collision;
using TGC.Core.Mathematica;
using TGC.Core.Terrain;
using TGC.Group.Model.Elements;
using TGC.Group.Model.Elements.RigidBodyFactories;
using TGC.Group.Model.Resources.Meshes;
using Chunk = TGC.Group.Model.Chunks.Chunk;
using Element = TGC.Group.Model.Elements.Element;

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
        public TgcSimpleTerrain Floor { get; set; }
        
        public World(TGCVector3 initialPoint)
        {
            chunks = new Dictionary<TGCVector3, Chunk>();
            
            entities = new List<Entity>();

            waterSurface = new WaterSurface(initialPoint);
            
            AddChunk(initialPoint);
            AddShark();
            AddHeightMap();
        }

        private void AddHeightMap()
        {
            Floor = new TgcSimpleTerrain();
            Floor.loadHeightmap(Game.Default.MediaDirectory + "Heightmap3.jpg", 1000, 100, 
                new TGCVector3(0,-100,0));
            Floor.loadTexture(Game.Default.MediaDirectory + Game.Default.TexturaTierra);
            CreateSurfaceFromHeighMap(Floor.getData());

        }
        
        public RigidBody CreateSurfaceFromHeighMap(CustomVertex.PositionTextured[] triangleDataVB)
        {
            //Triangulos
            var triangleMesh = new TriangleMesh();
            int i = 0;
            TGCVector3 vector0;
            TGCVector3 vector1;
            TGCVector3 vector2;

            while (i < triangleDataVB.Length)
            {
                vector0 = new TGCVector3(triangleDataVB[i].X, triangleDataVB[i].Y, triangleDataVB[i].Z);
                vector1 = new TGCVector3(triangleDataVB[i + 1].X, triangleDataVB[i + 1].Y, triangleDataVB[i + 1].Z);
                vector2 = new TGCVector3(triangleDataVB[i + 2].X, triangleDataVB[i + 2].Y, triangleDataVB[i + 2].Z);

                i = i + 3;

                triangleMesh.AddTriangle(vector0.ToBulletVector3(), vector1.ToBulletVector3(), vector2.ToBulletVector3());
            }

            CollisionShape meshCollisionShape = new BvhTriangleMeshShape(triangleMesh, true);
            var meshMotionState = new DefaultMotionState();
            var meshRigidBodyInfo = new RigidBodyConstructionInfo(0, meshMotionState, meshCollisionShape);
            RigidBody meshRigidBody = new RigidBody(meshRigidBodyInfo);
            AquaticPhysics.Instance.Add(meshRigidBody);

            return meshRigidBody;
        }

        protected void AddShark()
        {
            var mesh = SharkMesh.All()[0];
            mesh.Position = new TGCVector3(30, 1000, -2000);
            mesh.UpdateMeshTransform();
            
            var rigidBody = new CapsuleFactory().CreateShark(mesh); ;
            AquaticPhysics.Instance.Add(rigidBody);
            
            
            entities.Add(new Shark(mesh, rigidBody));
        }

        private Chunk AddChunk(TGCVector3 origin)
        {
            var chunk = Chunk.ByYAxis(origin);
            
            chunks.Add(origin, chunk);

            entities.AddRange(chunk.Init());

            return chunk;
        }

        public List<Collisionable> GetCollisionables()
        {
            var res = new List<Collisionable>();

            res.AddRange(entities);

            foreach (var chunk in chunks.Values)
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
                        
                        toUpdate.Add(chunks.ContainsKey(position) ? chunks[position] : AddChunk(position));
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
            SelectableElement = GetSelectableElement(camera, toUpdate);
            entities.ForEach(entity => entity.Update(camera));
        }
        
        public void Render(TgcCamera camera)
        {
            ToRender(camera.Position).ForEach(chunk => chunk.Render());
            entities.ForEach(entity => entity.Render());
            waterSurface.Render(camera.Position);
            Floor.Render();
        }

        public void RenderBoundingBox(TgcCamera camera)
        {
            ToRender(camera.Position).ForEach(chunk => chunk.RenderBoundingBox());
        }

        public void Dispose()
        {
            chunks.Values.ToList().ForEach(chunk => chunk.Dispose());
            entities.ForEach(entity => entity.Dispose());
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
            foreach (var chunk in chunks.Values)
            {
                chunk.Remove(selectableElement);
            }
        }
    }
}
