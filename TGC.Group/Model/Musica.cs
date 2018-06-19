using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.DirectX.DirectSound;
using TGC.Core.Sound;

namespace TGC.Group.Model {
    class Musica {
        private static Musica instance;
        private TgcMp3Player musicaDeFondo;
        private Device dsDevice;

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

        public void playDeFondo() {
            musicaDeFondo.play(true);
        }

        public void setDsDevice(Device dsDevice) {
            this.dsDevice = dsDevice;
        }

        public Device getDsDevice() => dsDevice;

        public void pause() {
            musicaDeFondo.pause();
        }
    }
}
