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
        public TgcStaticSound sonidoCaminar;
        public TgcStaticSound sonidoFondo;

        static string mediaDir;

        public static SoundManager Instance;


        public static SoundManager getInstance()
        {
            if (Instance == null)
            {
                Instance = new SoundManager();
            }
            return Instance;
        }

        public SoundManager()
        {


            sonidoCaminar = new TgcStaticSound();
            sonidoFondo = new TgcStaticSound();

            sonidoFondo.loadSound("Sound\\muelle, continuo", DsDevice);
            sonidoCaminar.loadSound("Sound\\muelle, continuo", DsDevice);

        }

        public void playSonidoCaminar()
        {
            sonidoCaminar.play();
        }

        public void playSonidoFondo()
        {
            sonidoFondo.play();
        }

        public void dispose()
        {
            sonidoFondo.dispose();
            sonidoCaminar.dispose();
        }
    }
}
