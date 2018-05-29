using System.Runtime.InteropServices;
using System.Text;
using TGC.Core.Sound;

namespace TGC.Group.Model {
    class Musica {
        private static Musica instance;
        private TgcMp3Player musicaDeFondo;

        private Musica() {
            musicaDeFondo = new TgcMp3Player();
        }

        public static Musica getInstance() {
            if (instance == null) instance = new Musica();
            return instance;
        }

        public void setMusica(string path) {
            musicaDeFondo.FileName = path;
        }

        public void play() {
            // mciSendString("open file " + musicaDeFondo.FileName + "type mpegvideo alias main", null, 0, 0);
            musicaDeFondo.play(true);
        }

        public void pause() {
            musicaDeFondo.pause();
        }

        public void setVolume(int volume) {
            mciSendString("setaudio main volume to " + volume * 100, null, 0, 0);
        }

        [DllImport("winmm.dll")]
        public static extern int mciSendString(string lpstrCommand,
            StringBuilder lpstrReturnString, int uReturnLengh, int hwndCallback);

    }
}
