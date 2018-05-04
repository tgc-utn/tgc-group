using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Vehiculos
{
    class Camioneta : Vehiculo
    {
        public Camioneta(string mediaDir, TGCVector3 posicionInicial) : base(mediaDir, posicionInicial)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcMesh ruedaIzquierda = loader.loadSceneFromFile(mediaDir + "meshCreator\\meshes\\Vehiculos\\Camioneta\\Rueda\\Rueda-TgcScene.xml").Meshes[0];
            TgcMesh ruedaDerecha = ruedaIzquierda.clone("ruedaDerecha");
            TgcMesh ruedaTraseraIzquierda = ruedaIzquierda.clone("ruedaTraseraIzquierda");
            TgcMesh ruedaTraseraDerecha = ruedaIzquierda.clone("ruedaTraseraDerecha");
            ruedaIzquierda.Move(new TGCVector3(1.7f,0f,3.15f) + posicionInicial);
            ruedaDerecha.Move(new TGCVector3(-1.7f, 0f, 3.15f) + posicionInicial);
            ruedaTraseraIzquierda.Move(new TGCVector3(1.7f, 0, -2.75f) + posicionInicial);
            ruedaTraseraDerecha.Move(new TGCVector3(-1.7f, 0, -2.75f) + posicionInicial);
            List<TgcMesh> ruedas = new List<TgcMesh>();
            ruedas.Add(ruedaTraseraIzquierda);
            ruedas.Add(ruedaTraseraDerecha);
            this.ruedas = new Ruedas(ruedaIzquierda, ruedaDerecha, ruedas);
        }
    }
}
