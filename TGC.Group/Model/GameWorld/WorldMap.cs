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
        private TgcScene rooms;
        private List<TgcMesh>  elements;
        
        
        private bool alphaBlendEnable;

        public WorldMap(string mediaPath)
        {
            TgcSceneLoader loader = new TgcSceneLoader();

            this.rooms = loader.loadSceneFromFile(mediaPath + "/Scene-TgcScene.xml");

            int meshCount = this.rooms.Meshes.Count;
            for (int i = 0; i < meshCount; i++)
            {
                rooms.Meshes[i].rotateX(-(float)Math.PI / 2);
                if (rooms.Meshes[i].Name.Contains("zzz"))
                {
                    rooms.Meshes[i].rotateY((float)Math.PI);
                }
            }



            elements = new List<TgcMesh>();


            /*
            elements.Add(loader.loadSceneFromFile(mediaPath + "/LockerMetal/LockerMetal-TgcScene.xml").Meshes[0]);
            elements.Add(elements.ElementAt(0).clone("esd"));
            elements.ElementAt(1).move(new Vector3(15f, 15f, 0f));
            */
            

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



            this.rooms.disposeAll();

            int elementCount = this.elements.Count;
            for(int index = 0; index < elementCount; index++)
            {
                this.elements.ElementAt(index).dispose();
            }
        }


        public void render()
        {
            this.rooms.renderAll();

            int elementCount = this.elements.Count;
            for (int index = 0; index < elementCount; index++)
            {
                this.elements.ElementAt(index).render();
            }
        }
    }
}
