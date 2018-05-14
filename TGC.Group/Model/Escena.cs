using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Vehiculos;
using TGC.Group.Model.Vehiculos.Estados;

namespace TGC.Group.Model
{
    class Escena
    {
        private static Escena instance;
        private String mediaDir = "";

        private Objeto scene;
        //habitacion
        private Objeto cajaZapatillas, sillon, escoba, cama, mesaDeLuz, ropero, armario, escritorio, sillaEscritorio, cubito, lego, toma, pelota;
        //cocina
        private Objeto mesadaCocina1, cajaCocina, dispenser, mesaCocina, heladera, tacho, sillaCocina1, sillaCocina2, sillaCocina3, muebleCocina, libroCocina1, libroCocina2, libroCocina3;
        //banio
        private Objeto bathtub, inodoro, cepillo, esponja, jabon, banqueta, espejo;

        private List<Objeto> objetosEscenario = new List<Objeto>();

        private Escena()
        {
        }

        public static Escena getInstance()
        {
            if (instance == null)
            {
                instance = new Escena();
            }

            return instance;
        }

        public void init(string mediaDir)
        {
            this.mediaDir = mediaDir;

            TgcSceneLoader loader = new TgcSceneLoader();
            this.scene = this.dameMesh("Texturas\\Habitacion\\escenaFinal-TgcScene.xml", new TGCVector3(1, 1, 1), new TGCVector3(0, 0, 0), new TGCVector3(0, 0, 0));
            //cocina
            this.cama = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Cama\\Cama-TgcScene.xml", new TGCVector3(1, 1, 1), new TGCVector3(0, FastMath.PI, 0), new TGCVector3(-36f, 0, -124f));
            this.mesaDeLuz = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\MesaDeLuz\\MesaDeLuz-TgcScene.xml", new TGCVector3(1, 1, 1), new TGCVector3(0, FastMath.PI, 0), new TGCVector3(22f, 0, -158f));
            this.ropero = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Placard\\Placard-TgcScene.xml", new TGCVector3(1, 1, 1), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(-205f, 0, -122f));
            this.armario = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Armario\\Armario-TgcScene.xml", new TGCVector3(1, 1, 1), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(-30f, 0, 110f));
            this.escritorio = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Escritorio\\Escritorio-TgcScene.xml", new TGCVector3(1, 1, 1), new TGCVector3(0, 0, 0), new TGCVector3(183f, 0f, -107f));
            this.sillaEscritorio = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\SillaEscritorio\\SillaEscritorio-TgcScene.xml", new TGCVector3(0.5f, 0.3f, 0.5f), new TGCVector3(0, 0, 0), new TGCVector3(151f, -1f, -101f));
            this.dispenser = this.dameMesh("MeshCreator\\Meshes\\Cocina\\DispenserAgua\\DispenserAgua-TgcScene.xml", new TGCVector3(1f, 1f, 1f), new TGCVector3(0, 0, 0), new TGCVector3(-32f, -1f, 360f));
            this.mesaCocina = this.dameMesh("MeshCreator\\Meshes\\Cocina\\Mesa\\Mesa-TgcScene.xml", new TGCVector3(1f, 1f, 1f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(94f, 0f, 335f));
            this.heladera = this.dameMesh("MeshCreator\\Meshes\\Cocina\\Heladera\\Heladera-TgcScene.xml", new TGCVector3(1f, 1f, 1f), new TGCVector3(0, 0, 0), new TGCVector3(190f, -1f, 180f));
            this.tacho = this.dameMesh("MeshCreator\\Meshes\\Cocina\\Tacho\\Tacho-TgcScene.xml", new TGCVector3(0.6f, 0.6f, 0.6f), new TGCVector3(0, 0, 0), new TGCVector3(-30f, -1f, 165f));
            this.sillaCocina1 = this.dameMesh("MeshCreator\\Meshes\\Cocina\\Silla\\silla-TgcScene.xml", new TGCVector3(0.4f, 0.4f, 0.4f), new TGCVector3(0, -FastMath.QUARTER_PI, 0), new TGCVector3(116f, 0f, 295f));
            this.sillaCocina2 = this.dameMesh("MeshCreator\\Meshes\\Cocina\\Silla\\silla-TgcScene.xml", new TGCVector3(0.4f, 0.4f, 0.4f), new TGCVector3(0, -FastMath.QUARTER_PI, 0), new TGCVector3(90f, 0f, 265f));
            this.sillaCocina3 = this.dameMesh("MeshCreator\\Meshes\\Cocina\\Silla\\silla-TgcScene.xml", new TGCVector3(0.4f, 0.4f, 0.4f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(140f, 0f, 335f));
            this.muebleCocina = this.dameMesh("MeshCreator\\Meshes\\Cocina\\Mueble\\Mueble-TgcScene.xml", new TGCVector3(1f, 1f, 0.5f), new TGCVector3(0, 0, 0), new TGCVector3(10f, -1f, 357f));
            this.libroCocina1 = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Libros\\Comun\\Comun-TgcScene.xml", new TGCVector3(0.15f, 0.15f, 0.15f), new TGCVector3(0, 0, 0), new TGCVector3(110f, 23.85f, 290f));
            this.libroCocina2 = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Libros\\Comun\\Comun-TgcScene.xml", new TGCVector3(0.15f, 0.15f, 0.15f), new TGCVector3(0, -FastMath.QUARTER_PI, 0), new TGCVector3(110f, 25.1f, 290f));
            this.libroCocina3 = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Libros\\Arquitectura\\Arquitectura-TgcScene.xml", new TGCVector3(0.15f, 0.15f, 0.15f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(110f, 26.35f, 290f));
            this.cajaCocina = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Caja\\Caja-TgcScene.xml", new TGCVector3(0.25f, 0.26f, 0.25f), new TGCVector3(0, -FastMath.QUARTER_PI, 0), new TGCVector3(65f, 0f, 235f));
            this.mesadaCocina1 = this.dameMesh("MeshCreator\\Meshes\\Cocina\\Mueble\\Mueble-TgcScene.xml", new TGCVector3(1f, 1f, 0.5f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(202f, 0f, 342f));
            this.toma = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Toma\\Toma-TgcScene.xml", new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, 0f, FastMath.PI_HALF), new TGCVector3(-110f, 15f, 144f));

            //banio
            this.jabon = this.dameMesh("MeshCreator\\Meshes\\Bathroom\\Jabon\\Jabon-TgcScene.xml", new TGCVector3(0.7f, 0.7f, 0.7f), new TGCVector3(0, 0, 0), new TGCVector3(-75f, 0f, 265f));
            this.bathtub = this.dameMesh("MeshCreator\\Meshes\\Bathroom\\Bathtub\\Bathtub-TgcScene.xml", new TGCVector3(1.5f, 1.8f, 1.5f), new TGCVector3(0, 0, 0), new TGCVector3(-164f, 0f, 270f));
            this.inodoro = this.dameMesh("MeshCreator\\Meshes\\Bathroom\\InodoroCuadrado\\InodoroCuadrado-TgcScene.xml", new TGCVector3(2f, 3f, 3f), new TGCVector3(0, 0, 0), new TGCVector3(-85, 0f, 175));
            this.cepillo = this.dameMesh("MeshCreator\\Meshes\\Bathroom\\Cepillo\\Cepillo-TgcScene.xml", new TGCVector3(1.4f, 1.4f, 1.4f), new TGCVector3(0, FastMath.QUARTER_PI, 0), new TGCVector3(-125, 0f, 170f));
            this.esponja = this.dameMesh("MeshCreator\\Meshes\\Bathroom\\Esponja\\Esponja-TgcScene.xml", new TGCVector3(1f, 1f, 1f), new TGCVector3(0, FastMath.QUARTER_PI, 0), new TGCVector3(-165, 0f, 190));
            this.banqueta = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Banqueta\\Banqueta-TgcScene.xml", new TGCVector3(0.4f, 0.35f, 0.4f), new TGCVector3(0, 0, 0), new TGCVector3(-170, 0f, 215));
            this.espejo = this.dameMesh("MeshCreator\\Meshes\\Bathroom\\Espejo\\Espejo-TgcScene.xml", new TGCVector3(1f, 1f, 1f), new TGCVector3(0, 0, 0), new TGCVector3(-90f, 0f, 298f));

            //habitacion
            this.cajaZapatillas = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\CajaZapatillas\\CajaZapatillas-TgcScene.xml", new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(100f, 0f, -147f));
            this.sillon = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Sillon\\Sillon-TgcScene.xml", new TGCVector3(1f, 1f, 1f), new TGCVector3(0f, FastMath.PI+FastMath.PI_HALF, 0f), new TGCVector3(-180f, -0.5f, 20f));
            this.escoba = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Escoba\\Escoba-TgcScene.xml", new TGCVector3(1.3f, 1.3f, 1.3f), new TGCVector3(FastMath.PI_HALF, 0, 0), new TGCVector3(-105f, 1f, -60f));
            this.cubito = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Rubik\\Rubik-TgcScene.xml", new TGCVector3(0.4f, 0.4f, 0.4f), new TGCVector3(0, 0, 0), new TGCVector3(100f, 0f, -100f));
            this.lego = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-1\\Rastis-1-TgcScene.xml", new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0, FastMath.PI_HALF), new TGCVector3(-10f, 1f, 3f));
            this.lego = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-1\\Rastis-1-TgcScene.xml", new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 1f, 0), new TGCVector3(-75f, 0f, -100f));
            this.lego = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-1\\Rastis-1-TgcScene.xml", new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0f, FastMath.PI_HALF), new TGCVector3(75f, 1f, 10f));

            this.lego = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-2\\Rastis-2-TgcScene.xml", new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 1f, 0f), new TGCVector3(45f, 0f, 20f));
            this.lego = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-2\\Rastis-2-TgcScene.xml", new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 2f, 0f), new TGCVector3(40f, 0f, -20f));
            this.lego = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-2\\Rastis-2-TgcScene.xml", new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 3f, 0f), new TGCVector3(-50f, 0f, 20f));

            this.lego = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-4\\Rastis-4-TgcScene.xml", new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0f, 0f), new TGCVector3(22f, 0f, 0f));
            this.lego = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-4\\Rastis-4-TgcScene.xml", new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 2f, 0f), new TGCVector3(-100f, 0f, 45f));
            this.lego = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-4\\Rastis-4-TgcScene.xml", new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 1f, 0f), new TGCVector3(100f, 0f, 50f));

            this.lego = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-6\\Rastis-6-TgcScene.xml", new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 1f, 0f), new TGCVector3(0f, 0f, -50f));
            this.lego = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-6\\Rastis-6-TgcScene.xml", new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 1f, 0f), new TGCVector3(-120f, 0f, -10f));
            this.lego = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-6\\Rastis-6-TgcScene.xml", new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 1f, 0f), new TGCVector3(-200f, 0f, -200f));

            this.toma = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Toma\\Toma-TgcScene.xml", new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, 0f, FastMath.PI_HALF), new TGCVector3(60f, 15f, 378f));
            this.toma = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Toma\\Toma-TgcScene.xml", new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, 0f, FastMath.PI_HALF), new TGCVector3(60f, 15f, 144f));

            this.pelota = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Pelota\\Pelota-TgcScene.xml", new TGCVector3(0.4f, 0.4f, 0.4f), new TGCVector3(0, 0f, 0), new TGCVector3(153f, 0f, 85f));
            
            
            
            //this.toalla = this.dameMesh("MeshCreator\\Meshes\\Bathroom\\Toalla\\Toalla-TgcScene.xml", new TGCVector3(1f, 1f, 1f), new TGCVector3(0, FastMath.QUARTER_PI, 0), new TGCVector3(-90, 0f, 245));

        }

        private Objeto dameMesh(string ruta, TGCVector3 escala, TGCVector3 rotacion, TGCVector3 traslado)
        {
            TgcScene tgcScene = new TgcSceneLoader().loadSceneFromFile(mediaDir + ruta);

            TGCMatrix matrixEscalado = TGCMatrix.Scaling(escala);
            TGCMatrix matrixRotacionX = TGCMatrix.RotationX(rotacion.X);
            TGCMatrix matrixRotacionY = TGCMatrix.RotationY(rotacion.Y);
            TGCMatrix matrixRotacionZ = TGCMatrix.RotationZ(rotacion.Z);
            TGCMatrix matrixRotacion = matrixRotacionX * matrixRotacionY * matrixRotacionZ;
            TGCMatrix matrixTraslacion = TGCMatrix.Translation(traslado);
            TGCMatrix transformacion = matrixEscalado * matrixRotacion * matrixTraslacion;

            Objeto nuevoObjeto = new Objeto(tgcScene.Meshes, transformacion);
            objetosEscenario.Add(nuevoObjeto);

            return nuevoObjeto;

        }

        public void render()
        {
            foreach (Objeto objeto in objetosEscenario)
            {
                objeto.RenderBoundingBox();
                objeto.Render();
            }
        }

        public void dispose()
        {
            foreach (Objeto objeto in objetosEscenario)
            {
                objeto.Dispose();
            }
        }

        public Camioneta calculateCollisions(Camioneta auto)
        {
            //bool collide = false;
            TgcMesh collider = null;
            auto.mesh.BoundingBox.setRenderColor(Color.Yellow);
            foreach (Objeto objeto in objetosEscenario)
            {
                if ((collider = objeto.TestColision(auto.mesh)) != null)
                {
                    auto.mesh.BoundingBox.setRenderColor(Color.Red);
                    collider.BoundingBox.setRenderColor(Color.Red);
                    auto.SetVelocidadActual(-auto.GetVelocidadActual() * 4);
                    auto.SetEstado(new Backward(auto));
                }

                else
                {
                    objeto.SetColorBoundingBox(Color.Yellow);
                }

            }

            return auto;
        }
    }
}
