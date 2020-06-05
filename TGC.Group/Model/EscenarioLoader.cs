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
        private List<BloqueBuilder> bloques;
        private int numeroBloques;
        private float tamanioZBloques = 2000f;
        public EscenarioLoader(String mediaDir,Nave nave)
        {
            this.mediaDir = mediaDir;
            this.nave = nave;
            setearBloques();
            GameManager.Instance.AgregarRenderizable(bloques[0].generarBloque());
            numeroBloques = 1;
        }

        public void Update(float elapsedTime)
        {
            Random rnd = new Random();
            int numeroRandom = rnd.Next(2); // devuelve un numero entre 0 y 2
            TGCVector3 posicionBloque;
            if (naveAvanzoLoSuficiente())
            {
                posicionBloque = bloques[numeroRandom].getPosicion();
                posicionBloque.Z = 1000f+ numeroBloques * tamanioZBloques;
                bloques[numeroRandom].setPosicion(posicionBloque);
                numeroBloques++;
                GameManager.Instance.AgregarRenderizable(bloques[numeroRandom].generarBloque());

            }
        }

        public void setearBloques()
        {
            bloques = new List<BloqueBuilder>();
            List<TGCVector3> positions = new List<TGCVector3>();
            positions.Add(new TGCVector3(100, -35, 200));
            positions.Add(new TGCVector3(80, -35, 1000));
            List<TGCVector3> positions2 = new List<TGCVector3>();
            positions2.Add(new TGCVector3(100, 0, 300));

            BloqueBuilder bloque = new BloqueBuilder(mediaDir, new TGCVector3(0f, 0f, 1000f), "Xwing\\10-TgcScene.xml", positions, nave);
            BloqueBuilder bloque1 = new BloqueBuilder(mediaDir, new TGCVector3(0f, 90f, 1000f), "Xwing\\death+star-TgcScene.xml", positions2, nave);
            BloqueBuilder bloque2 = new BloqueBuilder(mediaDir, new TGCVector3(0f, 90f, 1000f), "Xwing\\death+star-TgcScene.xml", new List<TGCVector3>(), nave);
            //Bloque bloque1 = new Bloque(mediaDir, new TGCVector3(0f, 100f, 3000f), "Xwing\\death+star-TgcScene.xml");
            //Bloque bloque2 = new Bloque(mediaDir, new TGCVector3(0f, 0f, 5000f), "Xwing\\TRENCH_RUN-TgcScene.xml");
            bloques.Add(bloque);
            bloques.Add(bloque1);
            bloques.Add(bloque2);
        }
        private bool naveAvanzoLoSuficiente()
        {
            float posZ = nave.GetPosicion().Z;
            if (numeroBloques == 1)
            {
                return posZ > tamanioZBloques / 2;
            }
            return posZ > tamanioZBloques * (numeroBloques-1)+tamanioZBloques/2;
        }

    }
}
