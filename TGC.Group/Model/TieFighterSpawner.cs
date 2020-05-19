using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model
{
    class TieFighterSpawner
    {
        private String mediaDir;
        private Nave nave;

        public TieFighterSpawner(String mediaDir, Nave nave)
        {
            this.mediaDir = mediaDir;
            this.nave = nave;
        }

        public void Update(float elapsedTime)
        {
            //logica spawn de TieFighters!
        }
    }
}
