using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class TieFighterSpawner
    {
        private String mediaDir;
        private Nave nave;
        private float tiempoTranscurrido;
        private float tiempoSiguienteSpawn;

        public TieFighterSpawner(String mediaDir, Nave nave)
        {
            this.mediaDir = mediaDir;
            this.nave = nave;
            this.tiempoSiguienteSpawn = 2.5f;
            this.tiempoTranscurrido = 0f;
        }

        public void Update(float elapsedTime)
        {
            if (GameManager.Instance.Pause)
                return;
            Random rnd = new Random();
            tiempoTranscurrido += elapsedTime;
            if(tiempoTranscurrido > tiempoSiguienteSpawn)
            {
                tiempoTranscurrido = 0;
                SpawnTieFighter();
                tiempoSiguienteSpawn = rnd.Next(4, 8);
            }
            //numero random generado cuando crea una tie fighter
            //logica spawn de TieFighters!
        }
        private void SpawnTieFighter()
        {
            TGCVector3 posicionTie = new TGCVector3(nave.GetPosicion());
            posicionTie.Z += 600f;
            TieFighter tieFighter = new TieFighter(mediaDir, posicionTie, nave);
            GameManager.Instance.AgregarRenderizable(tieFighter);
        }
    }
}
