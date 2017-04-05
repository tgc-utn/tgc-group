using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.GameWorld
{
    public class WorldMap
    {
        private TgcScene       scene;
        private List<TgcMesh>  walls;
        private List<TgcMesh>  doors;
        private List<TgcMesh>  elements;

        private static float[][2][3] sofaInstances = 
        {
            {{0f, 0f, 0f},   {0f, 0f, 1f}},
            {{0f, 65f, 0f},  {0f, 0f, 0f}},
            {{65f, 0f, 0f},  {0f, 0f, 3/2f}},
            {{65f, 65f, 0f}, {0f, 0f, 0f}},
        };

        private static float[][2][3] lockerInstances =
        {
            {{0f, 0f, 0f},   {0f, 0f, 1f}},
            {{0f, 65f, 0f},  {0f, 0f, 0f}},
            {{65f, 0f, 0f},  {0f, 0f, 3/2f}},
            {{65f, 65f, 0f}, {0f, 0f, 0f}},
        };

        private static float[][2][3] wardrobeInstances =
        {
            {{0f, 0f, 0f},   {0f, 0f, 1f}},
            {{0f, 65f, 0f},  {0f, 0f, 0f}},
            {{65f, 0f, 0f},  {0f, 0f, 3/2f}},
            {{65f, 65f, 0f}, {0f, 0f, 0f}},
        };

        private static float[][2][3] tableInstances =
        {
            {{0f, 0f, 0f},   {0f, 0f, 1f}},
            {{0f, 65f, 0f},  {0f, 0f, 0f}},
            {{65f, 0f, 0f},  {0f, 0f, 3/2f}},
            {{65f, 65f, 0f}, {0f, 0f, 0f}},
        };

        private bool alphaBlendEnable;

        public WorldMap(string mediaPath)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            this.scene            = loader.loadSceneFromFile(mediaPath + "/Scene-TgcScene.xml");
            this.doors            = new List<TgcMesh>();
            this.elements         = new List<TgcMesh>();

            int meshCount         = this.scene.Meshes.Count;
            for(int i = 0; i < meshCount; i++)
            {
                this.scene.Meshes[i].rotateX(-(float)Math.PI / 2);
                if(this.scene.Meshes[i].Name.Contains(Game.Default.WallIdentifier))
                {
                    this.walls.Add(this.scene.Meshes[i]);
                }
                else if(this.scene.Meshes[i].Name.Contains(Game.Default.DoorIdentifier))
                {
                    this.doors.Add(this.scene.Meshes[i]);
                }
            }


            this.createElementInstances(loader, mediaPath + "/Sillon-TgcScene.xml",      sofaInstances,     "Sofa");
            this.createElementInstances(loader, mediaPath + "/Mesa-TgcScene.xml",        tableInstances,    "Table");
            this.createElementInstances(loader, mediaPath + "/Placard-TgcScene.xml",     wardrobeInstances, "Wardrobe");
            this.createElementInstances(loader, mediaPath + "/LockerMetal-TgcScene.xml", lockerInstances,   "Locker");

        }

        protected void createElementInstances(TgcSceneLoader loader, string path, float[][2][3] instances, string prefix)
        {
            TgcMesh element = loader.loadSceneFromFile(path).Meshes[0];

            int count = instances.GetLength(0);
            float PI = Math.PI;
            for(int index = 0; index < count; index++)
            {
                this.elements.Add(element.clone(prefix + (index.ToString())));
                int elementIndex = this.elements.Count - 1;
                this.elements.ElementAt(elementIndex).move(instances[index][0][0], instances[index][0][1], instances[index][0][2]);
                this.elements.ElementAt(elementIndex).rotateX(PI * instances[index][1][0]);
                this.elements.ElementAt(elementIndex).rotateY(PI * instances[index][1][1]);
                this.elements.ElementAt(elementIndex).rotateZ(PI * instances[index][1][2]);
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
            this.scene.renderAll();

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
