using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.Utils;
using TGC.Core.Camara;
using System.Collections.Generic;
using TGC.Core.Terrain;
using TGC.Core.UserControls;
using TGC.Core.UserControls.Modifier;
using TGC.Core.BoundingVolumes;
using TGC.Examples.Camara;

namespace TGC.Group.Model
{

    // Objeto Escena
    public class Escena
    {
        // ************************************************************************
        // Declaracion de Variables
        // ************************************************************************

            private string MediaDir;
            private TgcSceneLoader loader;
            private GameModel env;
            private new TgcD3dInput Input { get; set; }

            private TgcScene currentScene;

        // ************************************************************************

        private static Escena myInstance;

        public static Escena getInstance()
        {
            return myInstance;
        }

        public Escena(GameModel env)
        {
            //Device de DirectX para crear primitivas.
            this.env = env;
            myInstance = this;
            this.MediaDir = this.env.MediaDir;
            var d3dDevice = D3DDevice.Instance.Device;


            //Cargar escena con herramienta TgcSceneLoader
            var loader = new TgcSceneLoader();
            currentScene = loader.loadSceneFromFile(MediaDir + "Isla\\isla-TgcScene.xml");

            //Ajustar camara en base al tamano del objeto
            var Camara = new TgcRotationalCamera(currentScene.BoundingBox.calculateBoxCenter(), currentScene.BoundingBox.calculateBoxRadius() * 2, Input);

        }

        public void Update()
        {
            
        }

        public void Render()
        {
            //Renderizar escena entera sin mostrar los contornos
            currentScene.renderAll(false);

        }
        public void dispose()
        {
            //Dispose del mesh.
            currentScene.disposeAll();
        }

       

      
    }
}