using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model.GameWorld
{
    public class WorldMap
    {
        private TgcScene       scene;
        private List<TgcMesh>  walls;
        private List<TgcMesh>  doors;
        private List<TgcMesh>  elements;

        private static float[,,] roomPositions = new float[,,]
        {
            {{0f, 0f,     650f},  {0f, (float)Math.PI * 0.5f, 0f}},
            {{650f, 0f,   650f},  {0f, 0f, 0f}},
            {{-650f, 0f,  650f},  {0f, 0f, 0f}},
            {{0f, 0f,    -650f},  {0f, 0f, 0f}},
            {{650f, 0f,  -650f},  {0f, 0f, 0f}},
            {{-650f, 0f, -650f},  {0f, 0f, 0f}},            
            {{650f,  0f,    0f},  {0f, 0f, 0f}},
            {{-650f, 0f,    0f},  {0f, 0f, 0f}},
        };

        private static float[,,] sofaInstances = new float[,,]
        {
            {{0f, 0f, 800f},    {0f, 0f, 0f}},
            {{650f, 0f, -156f}, {0f, 1f, 0f}},            
        };

        private static float[,,] lockerInstances = new float[,,]
        {
            {{1064.47352f, 0f, 0f}, {0f, 0f, 0f}},
            {{-1055.6674f, 0f, -1055.6674f},  {0f, -1f, 0f}},
        };

        private static float[,,] wardrobeInstances = new float[,,]
        {            
            {{420f, 0f, 620f},   {0f, 1/2f, 0f}},
            {{150f, 0f, -1065f}, {0f, 1f, 0f}},
        };

        private static float[,,] tableInstances = new float[,,]
        {
            {{-788f, 0f, 0f},   {0f, 3/2f, 0f}},
            {{0f, 0f, -788f},  {0f, 0f, 0f}},
            {{605f, 0f, -380f},  {0f, 1f, 0f}},
        };

        private static float[,,] torchInstances = new float[,,]
        {
            {{1080f, 50f, 330f},  {-0.5f, 0f, 0f}},
            {{-1080f, 50f, 330f}, {-0.5f, 1f, 0f}},
            {{-1080f, 50f, -330f}, {-0.5f, 1f, 0f}},
            {{1080f, 50f, -330f},  {-0.5f, 0f, 0f}},

            {{330, 50f, 1080f},  {-0.5f, -0.5f, 0f}},
            {{-330f, 50f, 1080f}, {-0.5f, -0.5f, 0f}},
            {{330f, 50f, -1080f}, {-0.5f, 0.5f, 0f}},
            {{-330f, 50f, -1080f},  {-0.5f, +0.5f, 0f}},
        };

        private bool alphaBlendEnable;

        public WorldMap(string mediaPath)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            this.scene            = loader.loadSceneFromFile(mediaPath + "/Scene-TgcScene.xml");
            this.walls            = new List<TgcMesh>();
            this.doors            = new List<TgcMesh>();
            this.elements         = new List<TgcMesh>();


            this.createRoomWallInstances();
            this.rotateAndScaleScenario();
            this.createRoomInstances();

            this.createElementInstances(loader, mediaPath + "/Sillon-TgcScene.xml",      sofaInstances,     "Sofa",     1.25f);
            this.createElementInstances(loader, mediaPath + "/Mesa-TgcScene.xml",        tableInstances,    "Table",    2.75f);
            this.createElementInstances(loader, mediaPath + "/Placard-TgcScene.xml",     wardrobeInstances, "Wardrobe", 2.5f);
            this.createElementInstances(loader, mediaPath + "/LockerMetal-TgcScene.xml", lockerInstances,   "Locker",   2.5f);
            this.createElementInstances(loader, mediaPath + "/Torch-TgcScene.xml",       torchInstances,    "Torch",    2f);
        }

        protected void createRoomWallInstances()
        {
            int meshCount = this.scene.Meshes.Count;
            for (int index = 0; index < meshCount; index++)
            {
                if (this.scene.Meshes[index].Name == "RoomWall01")
                {
                    TgcMesh sideWall = this.scene.Meshes[index].createMeshInstance("RoomWall02", new Vector3(0f, 0f, 0f), new Vector3(0f, (float)Math.PI / 2, 0f), new Vector3(1f, 1f, 1f));
                    TgcMesh frontWall = this.scene.Meshes[index].createMeshInstance("RoomWall03", new Vector3(-10f, 0f, 410f), new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f));

                    sideWall.UpdateMeshTransform();
                    frontWall.UpdateMeshTransform();
                    sideWall.updateBoundingBox();
                    frontWall.updateBoundingBox();
                    sideWall.BoundingBox.setRenderColor(Color.Red);
                    frontWall.BoundingBox.setRenderColor(Color.Green);
                    
                    this.scene.Meshes.Add(sideWall);
                    this.scene.Meshes.Add(frontWall);
                    break;
                }
            }
        }

        protected void rotateAndScaleScenario()
        {

            int meshCount = this.scene.Meshes.Count;
            for (int index = 0; index < meshCount; index++)
            {

                this.scene.Meshes[index].Scale = new Vector3(10f, 10f, 10f);
                this.scene.Meshes[index].rotateX(-(float)Math.PI / 2);


                this.scene.Meshes[index].BoundingBox = TGCUtils.updateMeshBoundingBox(this.scene.Meshes[index]);
                

                this.scene.Meshes[index].UpdateMeshTransform();
                if (this.scene.Meshes[index].Name.Contains(Game.Default.WallMeshIdentifier))
                {
                    this.walls.Add(this.scene.Meshes[index]);
                }
                else if (this.scene.Meshes[index].Name.Contains(Game.Default.DoorMeshIdentifier))
                {
                    this.doors.Add(this.scene.Meshes[index]);
                }
            }
        }

        protected void createRoomInstances()
        {
            TgcMesh roomFloor  = null;
            TgcMesh roomWall01 = null;
            TgcMesh roomWall02 = null;
            TgcMesh roomWall03 = null;
            TgcMesh roomWall04 = null;
            TgcMesh roomWall05 = null;
            TgcMesh roomDoor   = null;

            int index = 0;
            while(roomFloor == null || roomWall01 == null || roomWall02 == null || roomWall03 == null || roomWall04 == null || roomWall05 == null || roomDoor == null)
            {
                switch(this.scene.Meshes[index].Name)
                {
                    case "RoomWall01":
                        roomWall01 = this.scene.Meshes[index];
                        break;
                    case "RoomWall02":
                        roomWall02 = this.scene.Meshes[index];
                        break;
                    case "RoomWall03":
                        roomWall03 = this.scene.Meshes[index];
                        break;
                    case "RoomWall04":
                        roomWall04 = this.scene.Meshes[index];
                        break;
                    case "RoomWall05":
                        roomWall05 = this.scene.Meshes[index];
                        break;
                    case "RoomDoor":
                        roomDoor = this.scene.Meshes[index];
                        break;
                    case "RoomFloor":
                        roomFloor = this.scene.Meshes[index];
                        break;
                }
                index++;
            }
            Vector3 scale = new Vector3(0f, 0f, 0f);
            
            
            for(index = 0; index < roomPositions.GetLength(0); index++)
            {
                Vector3 rotations = new Vector3(roomPositions[index,1,0], roomPositions[index,1,1], roomPositions[index,1,2]);
                Vector3 positions = new Vector3(roomPositions[index,0,0], roomPositions[index,0,1], roomPositions[index,0,2]);

                this.scene.Meshes.Add(TGCUtils.createInstanceFromMesh(ref roomFloor, roomFloor.Name + index.ToString(), positions, rotations, scale));                
                this.scene.Meshes.Add(TGCUtils.createInstanceFromMesh(ref roomWall01, roomWall01.Name + index.ToString(), positions, rotations, scale));
                this.scene.Meshes.Add(TGCUtils.createInstanceFromMesh(ref roomWall02, roomWall02.Name + index.ToString(), positions, rotations, scale));
                this.scene.Meshes.Add(TGCUtils.createInstanceFromMesh(ref roomWall03, roomWall03.Name + index.ToString(), positions, rotations, scale));
                this.scene.Meshes.Add(TGCUtils.createInstanceFromMesh(ref roomWall04, roomWall04.Name + index.ToString(), positions, rotations, scale));
                this.scene.Meshes.Add(TGCUtils.createInstanceFromMesh(ref roomWall05, roomWall05.Name + index.ToString(), positions, rotations, scale));
                this.scene.Meshes.Add(TGCUtils.createInstanceFromMesh(ref roomDoor, roomDoor.Name + index.ToString(), positions, rotations, scale));
            }
            
        }

        static Predicate<TgcMesh> byName(string name)
        {
            return delegate (TgcMesh mesh)
            {
                return mesh.Name == name;
            };
        }
        
        protected void createElementInstances(TgcSceneLoader loader, string path, float[,,] instances, string prefix, float scale)
        {
            TgcScene tempScene = loader.loadSceneFromFile(path);
            int meshCount = tempScene.Meshes.Count;
            for(int meshIndex = 0; meshIndex < meshCount; meshIndex++)
            {
                TgcMesh element = tempScene.Meshes[meshIndex];
                element.Scale = new Vector3(scale, scale, scale);
                int count = instances.GetLength(0);
                float PI = (float)Math.PI;
                for (int index = 0; index < count; index++)
                {
                    TgcMesh instance = element.createMeshInstance(
                        prefix + index.ToString(),
                        new Vector3(instances[index, 0, 0], instances[index, 0, 1], instances[index, 0, 2]),
                        new Vector3(PI * instances[index, 1, 0], PI * instances[index, 1, 1], PI * instances[index, 1, 2]),
                        new Vector3(scale, scale, scale));
                    instance.UpdateMeshTransform();
                    this.elements.Add(instance);
                }
            }
        }

        public bool AlphaBlendEnable
        {
            get
            {
                return false;
            }
            set
            {
                this.alphaBlendEnable = value;
            }
        }

        public void dispose()
        {
            this.scene.disposeAll();

            int elementCount = this.elements.Count;
            for(int index = 0; index < elementCount; index++)
            {
                this.elements.ElementAt(index).dispose();
            }
        }


        public void render()
        {
            this.scene.renderAll(true);

            int elementCount = this.elements.Count;
            for (int index = 0; index < elementCount; index++)
            {
                this.elements.ElementAt(index).render();
            }
        }

        public List<TgcMesh> Walls
        {
            get{return this.walls;}
        }

        public List<TgcMesh> Doors
        {
            get{return this.doors;}
        }

    }
}
