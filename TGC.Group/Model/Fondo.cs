using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Terrain;
using TGC.Core.Mathematica;


namespace TGC.Group.Model
{
    public class Fondo : TgcExample
    {
        //Herramienta para crear la caja que simula el ambiente
        private TgcSkyBox skyBox;
       
        /// <summary>
        ///     Constructor del fondo.
        /// </summary>
        /// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
        /// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
        public Fondo(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public override void Init()
        {
            skyBox = new TgcSkyBox();
            skyBox.Center = TGCVector3.Empty;
            skyBox.Size = new TGCVector3(10000, 10000, 10000);

            //especifico donde estan las texturas cargadas
            var rutaTextura = MediaDir + "Mar\\";

            //coloco cada textura en cada cara del cubo del skyBox
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, rutaTextura + "fondoMarUp.PNG");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, rutaTextura + "fondoMarDown.PNG");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, rutaTextura + "fondoMarLeft.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, rutaTextura + "fondoMarRight.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, rutaTextura + "fondoMarBack.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, rutaTextura + "fondoMarFront.jpg");

            skyBox.SkyEpsilon = 25f;

            skyBox.Init();
        }

        public override void Update()
        {
            D3DDevice.Instance.Device.Transform.Projection = TGCMatrix.PerspectiveFovLH(D3DDevice.Instance.FieldOfView, D3DDevice.Instance.AspectRatio,
                   D3DDevice.Instance.ZNearPlaneDistance, D3DDevice.Instance.ZFarPlaneDistance * 2f).ToMatrix();
            //Coloco a la camara en el centro del cubo
            skyBox.Center = Camara.Position;
        }

        public override void Render()
        {
            skyBox.Render();
        }

        public override void Dispose()
        {
            skyBox.Dispose();
        }
    }
}