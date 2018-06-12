using Microsoft.DirectX.DirectInput;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TGC.Core.Sound;
using TGC.Core.Text;

namespace TGC.Group.Model
{
    public class SoundManager
    {
        private static TgcDirectSound DirectSound = new TgcDirectSound();
        private TgcMp3Player mp3BackgroundPlayer = new TgcMp3Player();
        private static Directorio Directorio { get; set; }

        private static TgcStaticSound SonidoCaminar = new TgcStaticSound();
        private static TgcStaticSound SonidoSalto = new TgcStaticSound();

        private TgcMp3Player mp3SaltosPlayer = new TgcMp3Player();

        public SoundManager(Directorio directorio,Microsoft.DirectX.DirectSound.Device dsDevice)
        {
            Directorio = directorio;

            //Cargo archivo de sonido background mp3.
            mp3BackgroundPlayer.closeFile();
            mp3BackgroundPlayer.FileName = directorio.SonidoFondo;

            //Cargo sonidos estaticos.
            SonidoSalto.loadSound(directorio.SonidoSalto, dsDevice);
            SonidoCaminar.loadSound(directorio.SonidoCaminar, dsDevice);

            //mp3SaltosPlayer.closeFile();
            //mp3SaltosPlayer.FileName = directorio.SonidoSalto;
        }

        public void playSonidoCaminar()
        {
            //mp3PasosPlayer.play(true);
            //SonidoCaminar.play();
        }

        public void stopSonidoCaminar()
        {
            SonidoCaminar.stop();
        }

        public void playSonidoSaltar()
        {
            //mp3SaltosPlayer.play(true);
            //SonidoSalto.play(false);
        }

        public void playSonidoFondo()
        {
           // mp3BackgroundPlayer.play(true);
        }

        public void dispose()
        {
           // SonidoCaminar.dispose();
        }
    }
}
