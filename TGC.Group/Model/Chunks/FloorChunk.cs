using BulletSharp;
using BulletSharp.Math;
using System;
using System.Collections.Generic;
using TGC.Core.BoundingVolumes;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.Terrain;
using TGC.Core.Textures;
using TGC.Group.Model.Elements;
using TGC.Group.Model.Elements.ElementFactories;
using TGC.Group.Model.Elements.RigidBodyFactories;
using TGC.Group.Model.Resources.Meshes;
using TGC.Group.Model.Utils;
using Element = TGC.Group.Model.Elements.Element;

namespace TGC.Group.Model.Chunks
{
    public class FloorChunk : Chunk
    {
        private static readonly TgcTexture FloorTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, 
            Game.Default.MediaDirectory + Game.Default.TexturaTierra);

        private TgcPlane floor;
        private RigidBody floorRigidBody;

        public FloorChunk(TGCVector3 origin) : base(origin, AquaticPhysics.Instance)
        {
            var max = origin + DefaultSize;

            var segments = Segment.GenerateSegments(origin, max, 10);

            var divisions = (int)(DefaultSize.X / 100);

            this.CreateElements(segments, divisions);
            this.CreateFloor(origin);
            this.AddElementsToPhysicsWorld();
        }

        private void CreateElements(List<Segment> segments, int divisions)
        {
            this.Elements.AddRange(CreateCorals(segments, divisions));
            segments.ForEach(segment => this.Elements.AddRange(CreateFishes(segment, divisions)));
        }

        private void CreateFloor(TGCVector3 origin)
        {
            this.floor = new TgcPlane(origin, DefaultSize, TgcPlane.Orientations.XZplane, FloorTexture);
            this.floorRigidBody = new BoxFactory().CreatePlane(this.floor);
        }

        private static IEnumerable<Element> CreateFishes(Segment segment, int divisions)
        {
            return segment.GenerateElements(divisions / 2, SpawnRate.Of(1, 750), FishFactory.Instance);
        }

        private static IEnumerable<Element> CreateCorals(List<Segment> segments, int divisions)
        {
            var corals = segments[0].GenerateElements(divisions / 2, SpawnRate.Of(1, 25), CoralFactory.Instance);
            segments.Remove(segments[0]);
            return corals;
        }

        private new void AddElementsToPhysicsWorld()
        {
            base.AddElementsToPhysicsWorld();
            this.Physics.Add(this.floorRigidBody);
        }

        public override void Render()
        {
            base.Render();
            this.floor.updateValues();
            this.floor.Render();
        }

        public override void Dispose()
        {
            this.floorRigidBody.Dispose();
            this.floor.Dispose();
            base.Dispose();
        }
    }
}