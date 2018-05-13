using System.Text;
using System.Threading.Tasks;
using TGC.Core.Sound;
using Microsoft.DirectX.DirectSound;

namespace TGC.Group.Model
{
    class ConceptosGlobales
    {

        private static ConceptosGlobales instance;
        private string mediaDir;
        private Device dispositivoDeAudio;

        private ConceptosGlobales()
        {
        }

        public static ConceptosGlobales getInstance()
        {
            if (instance == null)
            {
                instance = new ConceptosGlobales();
            }

            return instance;
        }

        public void SetMediaDir(string mediaDir)
        {
            this.mediaDir = mediaDir;
        }

        public string GetMediaDir()
        {
            return this.mediaDir;
        }

        public void SetDispositivoDeAudio(Device dispositivo)
        {
            this.dispositivoDeAudio = dispositivo;
        }

        public Device GetDispositivoDeAudio()
        {
            return this.dispositivoDeAudio;
        }
    }
}