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
		private TgcSkyBox skyBox;

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
			//Crear SkyBox
			skyBox = new TgcSkyBox();
			skyBox.Center = new Vector3(0, 500, 0);
			skyBox.Size = new Vector3(8000, 8000, 8000);
			var texturesPath = MediaDir + "Texturas\\Quake\\SkyBox LostAtSeaDay\\";
		
			skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "lostatseaday_up.jpg");
			skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "lostatseaday_dn.jpg");
			skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "lostatseaday_lf.jpg");
			skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "lostatseaday_rt.jpg");
			skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "lostatseaday_bk.jpg");
			skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "lostatseaday_ft.jpg");
			skyBox.Init();

            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;

			Camara = new FirstPersonCamera(new Vector3(0, 1500, 0), Input);

            heightmapDir = MediaDir + "Heightmaps\\" + "heightmap_v1.jpg";
            terrainTextureDir = MediaDir + "Texturas\\" +  "map_v2.jpg";

            terreno = new TgcSimpleTerrain();
			terreno.loadHeightmap(heightmapDir, 15, 3, Camara.LookAt);
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

			skyBox.render();

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
