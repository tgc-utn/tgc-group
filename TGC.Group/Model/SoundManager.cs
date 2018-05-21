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
        private TgcMp3Player mp3Player = new TgcMp3Player();
        private static Directorio Directorio { get; set; }

        private static TgcStaticSound SonidoCaminar = new TgcStaticSound();
        private static TgcStaticSound SonidoSalto = new TgcStaticSound();

        public static SoundManager Instance;

        public static SoundManager getInstance()
        {
            if (Instance == null)
            {
                Instance = new SoundManager(Directorio);
            }
            return Instance;
        }

        public SoundManager(Directorio directorio)
        {
            Directorio = directorio;

            //Cargo archivo de sonido background mp3.
            mp3Player.closeFile();
            mp3Player.FileName = directorio.SonidoFondo;

            //Cargo sonidos estaticos.
            //SonidoSalto.loadSound(directorio.SonidoSalto, DirectSound.DsDevice);
           // SonidoCaminar.loadSound(directorio.SonidoCaminar, DirectSound.DsDevice);
        }

        public void playSonidoCaminar()
        {
           
            SonidoCaminar.play();
        }

        public void playSonidoFondo()
        {
            mp3Player.play(true);
        }

        public void dispose()
        {
            SonidoCaminar.dispose();
        }
    }
}
