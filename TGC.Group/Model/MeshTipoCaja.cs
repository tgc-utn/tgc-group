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
        private List<Rayo> rayosMenosZ;
        private List<Rayo> rayosX;
        private List<Rayo> rayosMenosX;
        private List<Rayo> rayosY;
        private List<Rayo> rayosMenosY;

        public MeshTipoCaja(TgcMesh caja)
        {
            this.mesh = caja;
            this.rayosX = new List<Rayo>();
            this.rayosMenosX = new List<Rayo>();
            this.rayosY = new List<Rayo>();
            this.rayosMenosY = new List<Rayo>();
            this.rayosZ = new List<Rayo>();
            this.rayosMenosZ = new List<Rayo>();

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

            var rayoCentroCaraX = new RayoX(centroCaraX, new TGCVector3(1,0,0));
            var rayoIzqCaraX = new RayoX(new TGCVector3(centroCaraX.X, centroCaraX.Y, mesh.BoundingBox.PMin.Z), new TGCVector3(1, 0, 0));
            var rayoDerCaraX = new RayoX(new TGCVector3(centroCaraX.X, centroCaraX.Y, mesh.BoundingBox.PMax.Z), new TGCVector3(1, 0, 0));

            var rayoCaraMenosX = new RayoX(centroCaraMenosX, new TGCVector3(-1, 0, 0));
            var rayoIzqCaraMenosX = new RayoX(new TGCVector3(centroCaraMenosX.X, centroCaraMenosX.Y, mesh.BoundingBox.PMin.Z), new TGCVector3(-1, 0, 0));
            var rayoDerCaraMenosX = new RayoX(new TGCVector3(centroCaraMenosX.X, centroCaraMenosX.Y, mesh.BoundingBox.PMax.Z), new TGCVector3(-1, 0, 0));

            var rayoCaraY = new RayoY(centroCaraY, new TGCVector3(0, 1, 0));
            var rayoIzqCaraY = new RayoY(new TGCVector3(mesh.BoundingBox.PMax.X, centroCaraY.Y, centroCaraY.Z), new TGCVector3(0, 1, 0));
            var rayoDerCaraY = new RayoY(new TGCVector3(mesh.BoundingBox.PMin.X, centroCaraY.Y, centroCaraY.Z), new TGCVector3(0, 1, 0));

            var rayoCaraMenosY = new RayoY(centroCaraMenosY, new TGCVector3(0, -1, 0));
            var rayoIzqCaraMenosY = new RayoY(new TGCVector3(mesh.BoundingBox.PMax.X, centroCaraMenosY.Y, centroCaraMenosY.Z), new TGCVector3(0, -1, 0));
            var rayoDerCaraMenosY = new RayoY(new TGCVector3(mesh.BoundingBox.PMin.X, centroCaraMenosY.Y, centroCaraMenosY.Z), new TGCVector3(0, -1, 0));

            var rayoCaraZ = new RayoZ(centroCaraZ, new TGCVector3(0, 0, 1));
            var rayoIzqCaraZ = new RayoZ(new TGCVector3(mesh.BoundingBox.PMax.X, centroCaraZ.Y, centroCaraZ.Z), new TGCVector3(0, 0, 1));
            var rayoDerCaraZ = new RayoZ(new TGCVector3(mesh.BoundingBox.PMin.X, centroCaraZ.Y, centroCaraZ.Z), new TGCVector3(0, 0, 1));

            var rayoCaraMenosZ = new RayoZ(centroCaraMenosZ, new TGCVector3(0, 0, -1));
            var rayoIzqCaraMenosZ = new RayoZ(new TGCVector3(mesh.BoundingBox.PMax.X, centroCaraMenosZ.Y, centroCaraMenosZ.Z), new TGCVector3(0, 0, -1));
            var rayoDerCaraMenosZ = new RayoZ(new TGCVector3(mesh.BoundingBox.PMin.X, centroCaraMenosZ.Y, centroCaraMenosZ.Z), new TGCVector3(0, 0, -1));

            rayosY.Add(rayoCaraY); // UpCentro
            rayosY.Add(rayoIzqCaraY); // UpIzq
            rayosY.Add(rayoDerCaraY); // UpDer

            rayosMenosY.Add(rayoCaraMenosY); // Down
            rayosMenosY.Add(rayoIzqCaraMenosY); // DownIzq
            rayosMenosY.Add(rayoDerCaraMenosY); // DownDer

            rayosZ.Add(rayoCaraZ); // Front
            rayosZ.Add(rayoIzqCaraZ); // FrontIzq
            rayosZ.Add(rayoDerCaraZ); // FronDer

            rayosMenosZ.Add(rayoCaraMenosZ); // Back
            rayosMenosZ.Add(rayoIzqCaraMenosZ); // BackIzq
            rayosMenosZ.Add(rayoDerCaraMenosZ); // BackDer

            rayosMenosX.Add(rayoCaraMenosX); // Right
            rayosMenosX.Add(rayoIzqCaraMenosX); // RightIzq
            rayosMenosX.Add(rayoDerCaraMenosX); // RightDer

            rayosX.Add(rayoCentroCaraX); // Left
            rayosX.Add(rayoIzqCaraX); // LeftIzq
            rayosX.Add(rayoDerCaraX);        
        }

        public bool ChocoConFrente(Personaje personaje) {
            //this.GenerarRayos();
            return this.TesteoDeRayos(personaje, rayosZ);   
        }

        public bool ChocoALaIzquierda(Personaje personaje) {
            //this.GenerarRayos();
            return this.TesteoDeRayos(personaje, rayosX);
        }

        public bool ChocoArriba(Personaje personaje)
        {
            //this.GenerarRayos();
            return this.TesteoDeRayos(personaje, rayosY);
        }

        public bool ChocoALaDerecha(Personaje personaje) {
            //this.GenerarRayos();
            return this.TesteoDeRayos(personaje, rayosMenosX);
        }

        public bool ChocoAtras(Personaje personaje)
        {
            //this.GenerarRayos();
            return this.TesteoDeRayos(personaje, rayosMenosZ);
        }

        public bool ChocoAbajo(Personaje personaje)
        {
            //this.GenerarRayos();
            return this.TesteoDeRayos(personaje, rayosMenosY);
        }

        private bool TesteoDeRayos(Personaje personaje, List<Rayo> rayos) {
            var puntoInterseccion = TGCVector3.Empty;

            foreach (Rayo rayo in rayos)
            {
                rayo.Colisionar(personaje.Mesh);

                if (rayo.HuboColision())
                    return true;
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

        internal void RenderizaRayos()
        {
            rayosX.ForEach(rayo => { rayo.Render(); });

            rayosMenosX.ForEach(rayo => { rayo.Render(); });

            rayosY.ForEach(rayo => { rayo.Render(); });

            rayosMenosY.ForEach(rayo => { rayo.Render(); });

            rayosZ.ForEach(rayo => { rayo.Render(); });

            rayosMenosZ.ForEach(rayo => { rayo.Render(); });
        }
    }
}