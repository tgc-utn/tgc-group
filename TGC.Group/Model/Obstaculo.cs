using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    public class Obstaculo : Destruible
    {
        private string direccionDelScene;
        private TGCVector3 posicionInicial;
        private TGCMatrix MatrizTranslation;
        private TGCMatrix MatrizEscala;
        public Obstaculo(string direccionDelScene, Nave naveDelJugador,TGCVector3 posicion) : base(naveDelJugador)
        {
            this.direccionDelScene = direccionDelScene;
            this.naveDelJugador = naveDelJugador;
            this.posicionInicial = posicion;
        }

        public override void Init()
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene2 = loader.loadSceneFromFile(direccionDelScene + "\\VigaMetal\\VigaMetal-TgcScene.xml");

            //Solo nos interesa el primer modelo de esta escena (tiene solo uno)
            mainMesh = scene2.Meshes[0];
            mainMesh.Position = posicionInicial;
            MatrizTranslation = TGCMatrix.Translation(posicionInicial);
            MatrizEscala = TGCMatrix.Scaling(new TGCVector3(.2f, .2f, .2f));
            //TGCQuaternion rotation = TGCQuaternion.RotationAxis(new TGCVector3(1.0f, 0.0f, 0.0f), Geometry.DegreeToRadian(90f));
            //mainMesh.Transform = TGCMatrix.Scaling(0.1f, 0.1f, 0.1f) * TGCMatrix.RotationTGCQuaternion(rotation) * TGCMatrix.Translation(mainMesh.Position);
            mainMesh.Transform = MatrizEscala * MatrizTranslation;
        }

        public override void Render()
        {
            mainMesh.Render();
        }

        public override TgcBoundingAxisAlignBox GetBoundingBox()
        {
            return mainMesh.BoundingBox;
        }

    }
}
