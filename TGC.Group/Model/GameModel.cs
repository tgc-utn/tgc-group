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
using TGC.Group.Model.Clases2D;

namespace TGC.Group.Model
{
    public class GameModel : TGCExample
    {
        private EscenarioLoader escenarioLoader;
        private TieFighterSpawner tieFighterSpawner;
        private MenuPrincipal menuPrincipal;
        private InputDelJugador input;
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

            input = new InputDelJugador(Input);

            Nave naveDelJuego = new Nave(MediaDir, posicionInicialDeNave, input);
            GameManager.Instance.AgregarRenderizable(naveDelJuego);

            Camara camaraDelJuego = new Camara(posicionInicialDeNave, 10, -50, naveDelJuego);
            Camera = camaraDelJuego;
            GameManager.Instance.Camara = camaraDelJuego;

            escenarioLoader = new EscenarioLoader(MediaDir, naveDelJuego);
            tieFighterSpawner = new TieFighterSpawner(MediaDir, naveDelJuego);
            /*
            Obstaculo obstaculo = new Obstaculo(MediaDir, naveDelJuego, new TGCVector3(100, -15, 720));
            GameManager.Instance.AgregarRenderizable(obstaculo);

            Skybox skybox = new Skybox(MediaDir, camaraDelJuego);
            GameManager.Instance.AgregarRenderizable(skybox);
            */
            
            //Cursor.Hide();

            menuPrincipal = new MenuPrincipal(MediaDir,input);
        }

        public override void Update()
        {
            PreUpdate();
            if (input.HayInputDePausa())
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
            menuPrincipal.DibujarMenu();
            //Siempre los sprites se dibujan luego de todos los Render
            PostRender();
        }

        public override void Dispose()
        {
            GameManager.Instance.Dispose();
            menuPrincipal.Dispose();
        }
    }
}