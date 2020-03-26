using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace TGC.Group.Model
{
    public class PicaraCaja : IRenderizable
    {

        private readonly string mediaDir;
        private readonly TGCVector3 posicionInicial;

        public PicaraCaja(string mediaDir, TGCVector3 posicionInicial)
        {
            this.mediaDir = mediaDir;
            this.posicionInicial = posicionInicial;
        }
        
        private TGCBox CuboMeme { get; set; } 

        public void Init()
        {
            var pathTexturaCaja = mediaDir + "luciano.jpg";
            var texture = TgcTexture.createTexture(pathTexturaCaja);
            var size = new TGCVector3(-2, 2, 2);

            CuboMeme = TGCBox.fromSize(size, texture); 
            CuboMeme.Position = posicionInicial;
        }

        /// <summary>
        /// Estos 3 metodos de Update, Render y Dispose se llaman igual que los de GameModel pero no son llamados cada frame automaticamente.
        /// Lo que quiero decir es que lo unico que tienen en comun es el nombre y la logica que por convencion llevan adentro.
        /// El Update y el Render NO tienen que usar PreUpdate() y PostUpdate() ( o PreRender() y PostRender() ) al principio y al final de los metodos.
        /// <summary>

        public void Update(float elapsedTime) 
        {
            var movement = new TGCVector3(0, 0, -1);
            movement *= 50f * elapsedTime;
            CuboMeme.Position = CuboMeme.Position + movement;
            CuboMeme.Transform = TGCMatrix.Translation(CuboMeme.Position);
        }
        public void Render()
        {
            CuboMeme.Render();
        }
        public void Dispose()
        {
            CuboMeme.Dispose();
        }
    }
}