using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.BoundingVolumes;
using TGC.Core.Camara;
using TGC.Core.Collision;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Terrain;
using TGC.Core.Textures;
using TGC.Group.Camera;
using TGC.Group.Collision;
using TGC.Group.Entities;
using TGC.Group.Helpers;

namespace TGC.Group.Model.Levels
{
    public class Level1Model : LevelModel
    {
        public TgcCamera Camera;
        private TgcD3dInput Input;
        private string MediaDir;
        TgcFrustum Frustum;

        private string currentHeightmap;
        private float currentScaleXZ;
        private float currentScaleY;
        private Texture terrainTexture;
        private List<TgcMesh> meshes;
        private HeightmapModel hmModel;
        private TgcSkyBox skyBox;
        private TgcPlane surfacePlane;
        private CollisionManager collisionManager;

        public Level1Model(TgcCamera camera, TgcD3dInput input, string mediaDir, TgcFrustum frustum)
        {
            Camera = camera;
            Input = input;
            MediaDir = mediaDir;
            Frustum = frustum;
        }

        public void Init()
        {
            TGCVector3 lookAt = new TGCVector3(6000, 4200f, 6600f);

            collisionManager = new CollisionManager(Input, lookAt, 0.1f);

            //Camara
            Camera = new FpsCamera(lookAt, Input, collisionManager);

            //Skybox
            LoadSkyBox();

            //Heightmap
            currentHeightmap = MediaDir + "\\Level1\\Heigthmap\\" + "hm_level1.jpg";
            currentScaleXZ = 50f;
            currentScaleY = 2f;
            hmModel = HeightmapHelper.CreateHeightMapMesh(D3DDevice.Instance.Device, currentHeightmap, currentScaleXZ, currentScaleY);

            //Texture
            var currentTexture = MediaDir + "\\Level1\\Textures\\" + "level1.PNG";
            terrainTexture = TextureHelper.LoadTerrainTexture(D3DDevice.Instance.Device, currentTexture);

            //Surface
            LoadSurface();

            //Meshes
            LoadMeshes();
        }

        private void LoadSurface()
        {
            var surfaceTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "\\Level1\\Textures\\" + "surface.PNG");
            surfacePlane = new TgcPlane(new TGCVector3(0, 4000, 0), new TGCVector3(12800, 0f, 12800), TgcPlane.Orientations.XZplane, surfaceTexture);
        }

        private void LoadSkyBox()
        {
            //Crear SkyBox
            skyBox = new TgcSkyBox();
            skyBox.Center = new TGCVector3(3200, 0, 3200);
            skyBox.Size = new TGCVector3(12700, 10000, 12700);
            var texturesPath = MediaDir + "\\Level1\\Textures\\SkyBox\\";
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "up.PNG");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "others.PNG");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "left.PNG");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "right.PNG");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "others.PNG");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "others.PNG");
            skyBox.Init();
        }

        private void LoadMeshes()
        {
            meshes = new List<TgcMesh>();

            LoadBoat();
            LoadPillarCorals();
            LoadRocks();
            LoadFishes();
            LoadCorals();
            LoadShip();
        }

        private void LoadBoat()
        {
            string pathBoat = MediaDir + "\\Meshes\\boat\\boat-TgcScene.xml";

            var loader = new TgcSceneLoader();
            var scene = loader.loadSceneFromFile(pathBoat);

            var position = new TGCVector3(6000, 3800, 6000);
            var scale = new TGCVector3(15, 5, 15);

            foreach (var mesh in scene.Meshes)
            {
                mesh.Position = position;
                mesh.Scale = scale;

                meshes.Add(mesh);

                //The boat has collision
                collisionManager.AddCollisionMesh(mesh);
            }
        }

        private void LoadShip()
        {
            string pathShip = MediaDir + "\\Meshes\\ship\\ship-TgcScene.xml";

            var loader = new TgcSceneLoader();
            var scene = loader.loadSceneFromFile(pathShip);

            var position = new TGCVector3(10000f, 150f, 800f);
            var scale = new TGCVector3(5, 5, 5);
            var rotation = new TGCVector3(-(FastMath.PI / 8), -(FastMath.PI / 4), 0);

            foreach (var mesh in scene.Meshes)
            {
                mesh.Position = position;
                mesh.Scale = scale;
                mesh.Rotation = rotation;

                meshes.Add(mesh);
            }
        }

        private void LoadCorals()
        {
            
        }

        private void LoadFishes()
        {
            var rnd = new Random();
            string pathFish = MediaDir + "\\Meshes\\fish\\fish-TgcScene.xml";

            var loader = new TgcSceneLoader();
            var originalMesh = loader.loadSceneFromFile(pathFish).Meshes[0];

            var xMax = 12000f;
            var zMax = 12000f;
            var yMax = 2000f;
            var cant = 100;

            for (int i = 0; i < cant; i++)
            {
                var posX = xMax * (float)rnd.NextDouble();
                var posZ = zMax * (float)rnd.NextDouble();
                var posY = yMax * (float)rnd.NextDouble() + 20f;

                var position = new TGCVector3(posX, posY * currentScaleY, posZ);
                var scale = new TGCVector3(10, 10, 10);

                var fish = originalMesh.createMeshInstance(originalMesh.Name + $"_{i}");

                fish.Transform = TGCMatrix.Scaling(scale) * TGCMatrix.Translation(position);

                meshes.Add(fish);
            }
        }

        private void LoadRocks()
        {
            var rnd = new Random();
            string pathRock = MediaDir + "\\Meshes\\rock\\Roca-TgcScene.xml";

            var loader = new TgcSceneLoader();
            var originalMesh = loader.loadSceneFromFile(pathRock).Meshes[0];

            var xMax = 12000f;
            var zMax = 12000f;
            var cant = 10;

            for (int i = 0; i < cant; i++)
            {
                var posX = xMax * (float)rnd.NextDouble();
                var posZ = zMax * (float)rnd.NextDouble();
                var posY = hmModel.HeightmapData[Convert.ToInt32(posX/currentScaleXZ), Convert.ToInt32(posZ/currentScaleXZ)];

                var position = new TGCVector3(posX, posY*currentScaleY, posZ);
                var scale = new TGCVector3(i * 1.5f, i, i * 1.5f);

                var rock = originalMesh.createMeshInstance(originalMesh.Name + $"_{i}");

                rock.Transform = TGCMatrix.Scaling(scale) * TGCMatrix.Translation(position);

                meshes.Add(rock);
            }
        }

        private void LoadPillarCorals()
        {
            string pathPillarCoral = MediaDir + "\\Meshes\\pillar_coral\\pillar_coral-TgcScene.xml";

            var loader = new TgcSceneLoader();
            var originalMesh = loader.loadSceneFromFile(pathPillarCoral).Meshes[0];

            var pillar1 = originalMesh.createMeshInstance(originalMesh.Name + "_1");
            pillar1.Transform = TGCMatrix.Scaling(new TGCVector3(50f, 20f, 50f)) * TGCMatrix.Translation(new TGCVector3(800f, 0f, 500f));

            var pillar2 = originalMesh.createMeshInstance(originalMesh.Name + "_2");
            pillar2.Transform = TGCMatrix.Scaling(new TGCVector3(20f, 10f, 20f)) * TGCMatrix.Translation(new TGCVector3(10000f, 360f, 10000f));

            meshes.AddRange(new TgcMesh[] { pillar1, pillar2});
        }

        public override void Update(float elapsedTime)
        {
        }

        public override void Render(float elapsedTime)
        {
            //Render SkyBox
            skyBox.Render();

            //Render terrain
            D3DDevice.Instance.Device.SetTexture(0, terrainTexture);
            D3DDevice.Instance.Device.VertexFormat = CustomVertex.PositionTextured.Format;
            D3DDevice.Instance.Device.SetStreamSource(0, hmModel.Terrain, 0);
            D3DDevice.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, hmModel.TotalVertex / 3);

            //Render Surface
            surfacePlane.Render();

            //var currentPosition = collisionManager.update(elapsedTime, Input);
            collisionManager.Render();

            //Render Meshes
            foreach (var mesh in meshes)
            {
                mesh.Render();
            }
        }

        public void Dispose()
        {
            skyBox.Dispose();
            hmModel.Terrain.Dispose();
            terrainTexture.Dispose();
            surfacePlane.Dispose();
            collisionManager.Dispose();

            //Dispose de Meshes
            foreach (TgcMesh mesh in meshes)
            {
                mesh.Dispose();
            }
        }
    }
}
