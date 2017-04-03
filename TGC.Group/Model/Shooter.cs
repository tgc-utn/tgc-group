using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Terrain;
using TGC.Group.Model.Cameras;

namespace TGC.Group.Model
{
    public  class Shooter : TgcExample
    {
        private string heightmapDir;
        private string terrainTextureDir;
        private TgcSimpleTerrain terreno;


        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        /// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
        /// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
        public Shooter(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public override void Init()
        {
            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;

            Camara = new FirstPersonCamera(Input);

            heightmapDir = MediaDir + "Heightmaps\\" + "arenaheightmap.jpg";
            terrainTextureDir = MediaDir + "Texturas\\" +  "arena.jpg";

            terreno = new TgcSimpleTerrain();
            terreno.loadHeightmap(heightmapDir, 5, 2, Camara.LookAt);
            terreno.loadTexture(terrainTextureDir);
        }

        public override void Update()
        {
            PreUpdate();
        }

        public override void Render()
        {
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
            PreRender();

            terreno.render();

            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        public override void Dispose()
        {
            terreno.dispose();
        }

    }
}
