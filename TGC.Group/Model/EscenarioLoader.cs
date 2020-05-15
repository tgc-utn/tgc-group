using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    public class EscenarioLoader
    {
        private String mediaDir;
        private Nave nave;
        private List<Bloque> bloques;
        public EscenarioLoader(String mediaDir,Nave nave)
        {
            this.mediaDir = mediaDir;
            this.nave = nave;
            setearBloques();
            GameManager.Instance.AgregarRenderizable(bloques[0]);
        }

        public void Update(float elapsedTime)
        {

        }

        public void setearBloques()
        {
            bloques = new List<Bloque>();
            List<TGCVector3> positions = new List<TGCVector3>();
            positions.Add(new TGCVector3(100, -35, 200));
            positions.Add(new TGCVector3(80, -35, 1000));
            Bloque bloque = new Bloque(mediaDir, new TGCVector3(0f, 0f, 1000f), "Xwing\\TRENCH_RUN-TgcScene.xml", positions, nave);
            //Bloque bloque1 = new Bloque(mediaDir, new TGCVector3(0f, 100f, 3000f), "Xwing\\death+star-TgcScene.xml");
            //Bloque bloque2 = new Bloque(mediaDir, new TGCVector3(0f, 0f, 5000f), "Xwing\\TRENCH_RUN-TgcScene.xml");
            bloques.Add(bloque);
            //bloques.Add(bloque1);
            //bloques.Add(bloque2);
        }


    }
}
