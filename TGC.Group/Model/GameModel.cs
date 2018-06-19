using Microsoft.DirectX.Direct3D;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Core.Sound;
using TGC.Core.Terrain;
using TGC.Core.Textures;
using TGC.Group.Model.Escenas;
using TGC.Group.Model.Scenes;

namespace TGC.Group.Model {

    public class GameModel : TgcExample {

        private TgcSkyBox skyBox;


        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir) {

            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;

        }

        public override void Init() {

            // Escena escenaInicial = new InicioEscena();
            var escenaInicial = new GameEscena();
            EscenaManager.getInstance().setMediaDir(MediaDir);
            EscenaManager.getInstance().setShaderDir(ShadersDir);
            EscenaManager.getInstance().addScene(escenaInicial);

            var pathSkyBoxCaras = MediaDir + "\\SkyBoxFaces\\";

            Musica.getInstance().setMusica(MediaDir + "\\NsanityBeach.mp3");
            Musica.getInstance().setDeathSound(MediaDir + "\\deathSound.mp3");
            Musica.getInstance().playDeFondo();

            D3DDevice.Instance.ParticlesEnabled = true;
            D3DDevice.Instance.EnableParticles();

        }

        public override void Update() {

            PreUpdate();

            EscenaManager.getInstance().update(ElapsedTime, Input, Camara);

            PostUpdate();
        }

        public override void Render() {

            /*
            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.CadetBlue, 1.0f, 0);
            D3DDevice.Instance.Device.BeginScene();
            TexturesManager.Instance.clearAll();
            */

            PreRender();

            EscenaManager.getInstance().render(ElapsedTime);

            PostRender();
            /*
            RenderAxis();
            RenderFPS();
            D3DDevice.Instance.Device.EndScene();
            D3DDevice.Instance.Device.Present();
            */
        }

        public override void Dispose() {

            EscenaManager.getInstance().dispose();

        }
    }
}