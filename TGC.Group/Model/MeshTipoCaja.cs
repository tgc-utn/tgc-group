using TGC.Core.SceneLoader;
using TGC.Core.SkeletalAnimation;
using TGC.Core.Mathematica;
using TGC.Core.Collision;
using System.Collections;
using System.Collections.Generic;
using System;
//using

namespace TGC.Group.Model
{
    public class MeshTipoCaja
    {
        private TgcMesh mesh;
        private List<Rayo> rayosZ;
        private List<Rayo> rayosX;
        private List<Rayo> rayosY;

        public MeshTipoCaja(TgcMesh caja)
        {
            this.mesh = caja;
            this.rayosX = new List<Rayo>();
            this.rayosY = new List<Rayo>();
            this.rayosZ = new List<Rayo>();
            GenerarRayos();
        }

        private void GenerarRayos() {

            // el orden es el mismo que retorna el metodo computeFaces de un BB, visto de frente (hacia -z) => Up, Down, Front, Back, Right, Left
            var rayos = new List<Rayo>();

            // Solucion casera del centro...
            //var PMax = meshTipoCaja.BoundingBox.PMax;
            //var PMin = meshTipoCaja.BoundingBox.PMin;
            //var centro = new TGCVector3((PMax.X + PMin.X) / 2, (PMax.Y + PMin.Y) / 2, (PMax.Z + PMin.Z) / 2);
            //

            var centro = mesh.BoundingBox.calculateBoxCenter();

            var centroCaraX = HallarCentroDeCara("x");
            var centroCaraMenosX = HallarCentroDeCara("-x");
            var centroCaraY = HallarCentroDeCara("y");
            var centroCaraMenosY = HallarCentroDeCara("-y");
            var centroCaraZ = HallarCentroDeCara("z");
            var centroCaraMenosZ = HallarCentroDeCara("-z");

            var rayoCaraX = new RayoX(centroCaraX, new TGCVector3(1,0,0));
            var rayoCaraMenosX = new RayoX(centroCaraMenosX, new TGCVector3(-1, 0, 0));
            var rayoCaraY = new RayoY(centroCaraY, new TGCVector3(0, 1, 0));
            var rayoCaraMenosY = new RayoY(centroCaraMenosY, new TGCVector3(0, -1, 0));
            var rayoCaraZ = new RayoZ(centroCaraZ, new TGCVector3(0, 0, 1));
            var rayoCaraMenosZ = new RayoZ(centroCaraMenosZ, new TGCVector3(0, 0, -1));

            rayosY.Add(rayoCaraY); // Up
            rayosY.Add(rayoCaraMenosY); // Down
            rayosZ.Add(rayoCaraZ); // Front
            rayosZ.Add(rayoCaraMenosZ); // Back
            rayosX.Add(rayoCaraMenosX); // Right
            rayosX.Add(rayoCaraX); // Left
        
        }

        public bool ChocoConFrente(TgcSkeletalMesh personaje) {
            
            var puntoInterseccion = TGCVector3.Empty;

            foreach (Rayo rayo in rayosZ) {

                rayo.Colisionar(personaje);
                
                if(rayo.HuboColision()) {
                    return true;
                }
               
            }

            return false;
        }

        private TGCVector3 HallarCentroDeCara(String dirCara)
        { // le pasas el centro para no tener que calcularlo cada vez que entras aca. en dirCara quise no pasarle un string, pero no anduvo con TGCVector3
            var PMin = mesh.BoundingBox.PMin;
            var PMax = mesh.BoundingBox.PMax;

            var centro = mesh.BoundingBox.calculateBoxCenter();

            switch (dirCara)
            {
                case "x":
                    return new TGCVector3(centro.X + (FastMath.Abs(PMax.X - PMin.X) / 2), centro.Y, centro.Z);
                case "-x":
                    return new TGCVector3(centro.X - (FastMath.Abs(PMax.X - PMin.X) / 2), centro.Y, centro.Z);
                case "y":
                    return new TGCVector3(centro.X, centro.Y + (FastMath.Abs(PMax.Y - PMin.Y) / 2), centro.Z);
                case "-y":
                    return new TGCVector3(centro.X, centro.Y - (FastMath.Abs(PMax.Y - PMin.Y) / 2), centro.Z);
                case "z":
                    return new TGCVector3(centro.X, centro.Y, centro.Z + (FastMath.Abs(PMax.Z - PMin.Z) / 2));
                case "-z":
                    return new TGCVector3(centro.X, centro.Y, centro.Z - (FastMath.Abs(PMax.Z - PMin.Z) / 2));
                default:
                    throw new Exception("direccion invalida");
            }
        }
    }
}