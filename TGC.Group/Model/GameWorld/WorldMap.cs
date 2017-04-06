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

        private bool alphaBlendEnable;

        public WorldMap(string mediaPath)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            this.scene            = loader.loadSceneFromFile(mediaPath + "/Scene-TgcScene.xml");
            this.walls            = new List<TgcMesh>();
            this.doors            = new List<TgcMesh>();
            this.elements         = new List<TgcMesh>();

            int meshCount         = this.scene.Meshes.Count;
            for(int index = 0; index < meshCount; index++)
            {
                // WHY THE HELL THE FRAMEWORK WON'T UPDATE BB'S ROTATION AAAAAAAAAAAAA
                Matrix scaleMatrix    = new Matrix();
                Matrix rotationMatrix = new Matrix();
                rotationMatrix.RotateX(-(float)Math.PI / 2);
                scaleMatrix.Scale(new Vector3(10f, 10f, 10f));
                scaleMatrix.Multiply(rotationMatrix);

                
                this.scene.Meshes[index].Scale = new Vector3(10f, 10f, 10f);
                this.scene.Meshes[index].Rotation = new Vector3(-(float)Math.PI / 2, 0f, 0f);
                // WHYYYYYYYYYY
                this.scene.Meshes[index].BoundingBox.transform(scaleMatrix);
                
                if (this.scene.Meshes[index].Name.Contains(Game.Default.WallMeshIdentifier))
                {
                    this.walls.Add(this.scene.Meshes[index]);
                }
                else if(this.scene.Meshes[index].Name.Contains(Game.Default.DoorMeshIdentifier))
                {
                    this.doors.Add(this.scene.Meshes[index]);
                }
            }


            this.createElementInstances(loader, mediaPath + "/Sillon-TgcScene.xml",      sofaInstances,     "Sofa",     1.25f);
            this.createElementInstances(loader, mediaPath + "/Mesa-TgcScene.xml",        tableInstances,    "Table",    2.75f);
            this.createElementInstances(loader, mediaPath + "/Placard-TgcScene.xml",     wardrobeInstances, "Wardrobe", 2.5f);
            this.createElementInstances(loader, mediaPath + "/LockerMetal-TgcScene.xml", lockerInstances,   "Locker",   2.5f);

        }

        protected void createElementInstances(TgcSceneLoader loader, string path, float[,,] instances, string prefix, float scale)
        {
            TgcMesh element = loader.loadSceneFromFile(path).Meshes[0];
            element.Scale = new Vector3(scale, scale, scale);
            int count = instances.GetLength(0);
            float PI = (float)Math.PI;
            for(int index = 0; index < count; index++)
            {
                this.elements.Add(element.clone(prefix + (index.ToString())));
                int elementIndex = this.elements.Count - 1;
                this.elements.ElementAt(elementIndex).rotateX(PI * instances[index, 1, 0]);
                this.elements.ElementAt(elementIndex).rotateY(PI * instances[index, 1, 1]);
                this.elements.ElementAt(elementIndex).rotateZ(PI * instances[index, 1, 2]);
                this.elements.ElementAt(elementIndex).move(instances[index,0,0], instances[index,0,1], instances[index,0,2]);              
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
            for(int i = 0; i < scene.Meshes.Count; i++)
            {
                this.scene.Meshes[i].BoundingBox.render();
            }


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
