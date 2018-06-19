using Microsoft.DirectX.DirectInput;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TGC.Core.Sound;
using TGC.Core.Text;

namespace TGC.Group.Modelo
{
    public class SoundManager
    {
        private static TgcDirectSound DirectSound = new TgcDirectSound();

        private static Directorio Directorio { get; set; }

        private static TgcStaticSound SonidoCaminar = new TgcStaticSound();
        private static TgcStaticSound SonidoSalto = new TgcStaticSound();
        private static TgcStaticSound SonidoMoneda = new TgcStaticSound();
        private static TgcStaticSound SonidoDanio = new TgcStaticSound();


        private TgcMp3Player mp3BackgroundPlayer = new TgcMp3Player();
        private TgcMp3Player mp3SaltosPlayer = new TgcMp3Player();
        private TgcMp3Player mp3FruitPlayer = new TgcMp3Player();

        public bool sonido { get; set; }  = true;

        public SoundManager(Directorio directorio,Microsoft.DirectX.DirectSound.Device dsDevice)
        {
            Directorio = directorio;

            //Cargo archivo de sonido background mp3.
            mp3BackgroundPlayer.closeFile();
            mp3BackgroundPlayer.FileName = directorio.SonidoFondo;

            mp3FruitPlayer.closeFile();
            mp3FruitPlayer.FileName = directorio.SonidoFruta;

            //Cargo sonidos estaticos.
            SonidoSalto.loadSound(directorio.SonidoSalto, dsDevice);
            SonidoMoneda.loadSound(directorio.SonidoMoneda, dsDevice);
            SonidoDanio.loadSound(directorio.SonidoDanio, dsDevice);
            SonidoCaminar.loadSound(directorio.SonidoCaminar, dsDevice);

            //mp3SaltosPlayer.closeFile();
            //mp3SaltosPlayer.FileName = directorio.SonidoSalto;
        }
        public void actualizarEstado()
        {
            sonido = !sonido;
            if (sonido) reanudarSonidos();
            else pauseSonidos();
            
        }

        public void pauseSonidos()
        {
            mp3BackgroundPlayer.pause();
            stopSonidoCaminar();
        }

        public void reanudarSonidos()
        {
            mp3BackgroundPlayer.resume();
        }

        public void playSonidoCaminar()
        {
           if(sonido) SonidoCaminar.play();
        }

        public void stopSonidoCaminar()
        {
            SonidoCaminar.stop();
        }

        public void playSonidoSaltar()
        {
            if(sonido)SonidoSalto.play(false);
        }

        public void playSonidoFondo()
        {
          if(sonido)mp3BackgroundPlayer.play(true);
        }

        public void playSonidoDanio()
        {
            if (sonido) SonidoDanio.play();
        }

        public void playSonidoFruta()
        {
            if (sonido)
            {
                //mp3FruitPlayer.play(false);
                SonidoMoneda.stop();
                SonidoMoneda.play(false);
            }
        }

        public void playSonidoMoneda()
        {
            if (sonido)
            {
            SonidoMoneda.stop();
            SonidoMoneda.play(false);
            }
        }

        public void dispose()
        {
           SonidoCaminar.dispose();
        }
    }
}
