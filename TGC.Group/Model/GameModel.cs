using Microsoft.DirectX.DirectInput;
using System.Drawing;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Examples.Camara;


namespace TGC.Group.Model
{
    public class GameModel : TGCExample
    {
        private EscenarioLoader escenarioLoader;
        private TieFighterSpawner tieFighterSpawner;
        private Drawer2D drawer;
        private CustomSprite menuPrincipalSprite;
        private CustomSprite FlechitaSeleccion;
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }


        public override void Init()
        {
            //Debe empezar pausado
            GameManager.Instance.PausarJuego();

            var posicionInicialDeNave = new TGCVector3(105, -15, -250);

            InputDelJugador input = new InputDelJugador(Input);

            Nave naveDelJuego = new Nave(MediaDir, posicionInicialDeNave, input);
            GameManager.Instance.AgregarRenderizable(naveDelJuego);

            Camara camaraDelJuego = new Camara(posicionInicialDeNave, 10, -50, naveDelJuego);
            Camera = camaraDelJuego;
            GameManager.Instance.Camara = camaraDelJuego;

            escenarioLoader = new EscenarioLoader(MediaDir, naveDelJuego);
            tieFighterSpawner = new TieFighterSpawner(MediaDir, naveDelJuego);

            Obstaculo obstaculo = new Obstaculo(MediaDir, naveDelJuego, new TGCVector3(100, -15, 720));
            GameManager.Instance.AgregarRenderizable(obstaculo);

            Skybox skybox = new Skybox(MediaDir, camaraDelJuego);
            GameManager.Instance.AgregarRenderizable(skybox);
            
            /*
            //Cursor.Hide();
            Nave naveDelJuego = new Nave(MediaDir, TGCVector3.Empty, new InputDelJugador(Input));
            drawer = new Drawer2D();
            menuPrincipalSprite = new CustomSprite();
            menuPrincipalSprite.Bitmap = new CustomBitmap(MediaDir + "InicioMenu.png", D3DDevice.Instance.Device);
            FlechitaSeleccion = new CustomSprite();
            FlechitaSeleccion.Bitmap = new CustomBitmap(MediaDir + "Flechita.png", D3DDevice.Instance.Device);
            //Ubicarlo centrado en la pantalla
            var textureSize = menuPrincipalSprite.Bitmap.Size;
            //menuPrincipalSprite.Position = new TGCVector2(FastMath.Max(D3DDevice.Instance.Width / 2 - textureSize.Width / 2, 0), FastMath.Max(D3DDevice.Instance.Height / 2 - textureSize.Height / 2, 0));
            menuPrincipalSprite.Position = new TGCVector2(-290,-150);
            FlechitaSeleccion.Position = new TGCVector2(D3DDevice.Instance.Width / 4, FastMath.Max(D3DDevice.Instance.Height / 2,0)+140);
            */
        }

        public override void Update()
        {
            PreUpdate();
            if (Input.keyDown(Key.Escape))
                GameManager.Instance.ReanudarOPausarJuego();
            GameManager.Instance.Update(ElapsedTime);
            escenarioLoader.Update(ElapsedTime);
            tieFighterSpawner.Update(ElapsedTime);
            PostUpdate();
        }


        public override void Render()
        {
            PreRender();
            GameManager.Instance.Render();
            /*
            drawer.BeginDrawSprite();
            drawer.DrawSprite(menuPrincipalSprite);
            drawer.DrawSprite(FlechitaSeleccion);
            drawer.EndDrawSprite();
            */
            PostRender();
        }

        public override void Dispose()
        {
            GameManager.Instance.Dispose();
            menuPrincipalSprite.Dispose();
        }
    }
}