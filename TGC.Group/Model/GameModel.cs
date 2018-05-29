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

        private TgcMp3Player musicaDeFondo;

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir) {

            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;

        }

        public override void Init() {

            Escena escenaInicial = new InicioEscena();
            EscenaManager.getInstance().setMediaDir(MediaDir);
            EscenaManager.getInstance().addScene(escenaInicial);

            var pathSkyBoxCaras = MediaDir + "\\SkyBoxFaces\\";

            // Inicializar SkyBox
            skyBox = new TgcSkyBox();
            skyBox.Center = new TGCVector3(0, 200, 0);
            skyBox.Size = new TGCVector3(2500, 900, 28000);
            skyBox.Color = Color.Azure;
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, pathSkyBoxCaras + "arriba.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, pathSkyBoxCaras + "abajo.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, pathSkyBoxCaras + "derecha.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, pathSkyBoxCaras + "izquierda.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, pathSkyBoxCaras + "frente.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, pathSkyBoxCaras + "atras.jpg");
            skyBox.SkyEpsilon = 25f;
            skyBox.Init();

            // Setear musica de fondo

            musicaDeFondo = new TgcMp3Player();
            musicaDeFondo.FileName = (MediaDir + "\\NsanityBeach.mp3");
            musicaDeFondo.play(true);

        }

        public override void Update() {

            PreUpdate();

            EscenaManager.getInstance().update(ElapsedTime, Input, Camara);

            PostUpdate();
        }

        public override void Render() {

            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.CadetBlue, 1.0f, 0);
            D3DDevice.Instance.Device.BeginScene();
            TexturesManager.Instance.clearAll();

            EscenaManager.getInstance().render(ElapsedTime);

            skyBox.Render();

            RenderAxis();
            RenderFPS();
            D3DDevice.Instance.Device.EndScene();
            D3DDevice.Instance.Device.Present();
        }

        public override void Dispose() {

            EscenaManager.getInstance().dispose();
            skyBox.Dispose();

        }
    }
}