using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TGC.Core.SceneLoader;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;

namespace TGC.Group.Modelo.Cajas
{
    public class Caja
    {
        public TgcMesh cajaMesh;
        public Escenario escenario;

        float danioTNT = 0.1f;
        float velocidadNITRO = 1000f;
        float tiempoPowerUp = 5f;

        public Caja(TgcMesh cajaMesh, Escenario escenario)
        {
            this.cajaMesh = cajaMesh;
            this.escenario = escenario;

        }

        public void afectar(Personaje personaje)
        {
            if (cajaMesh.Name == "TNT")
            {
                influirDanio(personaje);
                escenario.eliminarObjeto(cajaMesh);
            }

            if (cajaMesh.Name == "NITRO")
            {
                aumentarVelocidad(personaje);
                escenario.eliminarObjeto(cajaMesh);
            }
        }

        public void influirDanio(Personaje personaje)
        {
            personaje.aumentarVida(-danioTNT);
        }

        public void aumentarVelocidad(Personaje personaje)
        {
            personaje.aumentarVelocidad(velocidadNITRO, tiempoPowerUp);
        }

        #region MeshAdapter
        public TgcBoundingAxisAlignBox boundingBox() => cajaMesh.BoundingBox;
        public void Move(TGCVector3 desplazamiento) => cajaMesh.Move(desplazamiento);
        #endregion
    }
}
