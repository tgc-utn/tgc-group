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
            //creo los meshes de las ruedas y luego agrego cada object Rueda(mesh,posicion) a la lista de ruedas;
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcMesh ruedaIzquierda = loader.loadSceneFromFile(mediaDir + "meshCreator\\meshes\\Vehiculos\\Camioneta\\Rueda\\Rueda-TgcScene.xml").Meshes[0];
            TgcMesh ruedaDerecha = ruedaIzquierda.clone("ruedaDerecha");
            TgcMesh ruedaTraseraIzquierda = ruedaIzquierda.clone("ruedaTraseraIzquierda");
            TgcMesh ruedaTraseraDerecha = ruedaIzquierda.clone("ruedaTraseraDerecha");
            ruedas.Add(new Rueda(ruedaIzquierda, new TGCVector3(3.2f, 0f, 1.6f)));
            ruedas.Add(new Rueda(ruedaDerecha, new TGCVector3(3.2f, 0f, -1.6f)));
            ruedas.Add(new Rueda(ruedaTraseraIzquierda, new TGCVector3(-2.7f, 0, 1.6f)));
            ruedas.Add(new Rueda(ruedaTraseraDerecha, new TGCVector3(-2.7f, 0, -1.6f)));

            foreach (var rueda in ruedas)
            {
                rueda.Transform(TGCVector3.transform(posicion(), transformacion), vectorAdelante, TGCVector3.Cross(vectorAdelante, new TGCVector3(0, 1, 0)));
            }
        }


    }
}
