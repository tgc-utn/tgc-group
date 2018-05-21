using Microsoft.DirectX.DirectInput;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TGC.Core.Sound;
using TGC.Core.Text;

namespace TGC.Group.Model
{
    public class SoundManager : TgcDirectSound
    {
        public TgcStaticSound SonidoCaminar;
        public TgcStaticSound SonidoFondo;
        public static Directorio Directorio { get; set; }
        
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

            SonidoCaminar = new TgcStaticSound();
            SonidoFondo = new TgcStaticSound();

            SonidoFondo.loadSound(directorio.SonidoFondo, DsDevice);
            SonidoCaminar.loadSound(directorio.SonidoCaminar, DsDevice);

        }

        public void playSonidoCaminar()
        {
            SonidoCaminar.play();
        }

        public void playSonidoFondo()
        {
            SonidoFondo.play();
        }

        public void dispose()
        {
            SonidoFondo.dispose();
            SonidoCaminar.dispose();
        }
    }
}
