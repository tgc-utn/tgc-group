using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.SceneLoader;
using TGC.Core.Utils;

namespace TGC.Group.Model
{
    class Auto
    {
        //Altura del salto
        private const float ALTURA_SALTO = 75f;

        //Velocidad de movimiento del auto
        private float MOVEMENT_SPEED = 0f;

        //Rozamiento del piso
        private float ROZAMIENTO = 100f;

        //Velocidad Maxima
        private const float MAX_SPEED = 1200f;

        //Velocidad de rotación del auto
        private const float ROTATION_SPEED = 120f;

        //Mesh del auto
        private TgcMesh Mesh { get; set; }

        //BoudingBox Obb del auto.
        private TgcBoundingOrientedBox ObbMesh;

        //Ruedas
        TgcMesh ruedaDerechaDelanteraMesh;
        TgcMesh ruedaDerechaTraseraMesh;
        TgcMesh ruedaIzquierdaDelanteraMesh;
        TgcMesh ruedaIzquierdaTraseraMesh;

        //Lista de ruedas
        List<TgcMesh> RuedasJugador;

        //Rotación del auto
        float rotate = 0;
        float rotacionVertical = 0;

        //Posición de las ruedas
        List<float> dx = new List<float> { 23, -23, -23, 23 };
        List<float> dy = new List<float> { -30, 32, -31, 30 };

        //Estado de salto
        private bool falling = false;
        private bool jumping = false;

        //Nro. de Jugador
        private int NroJugador;

        //MediaDir
        private string MediaDir;

        public Auto(string MediaDir, int NroJugador)
        {
            this.NroJugador = NroJugador;
        }

        public void SetMesh(TgcMesh unMesh)
        {
            //Guardo el mesh
            this.Mesh = unMesh;

            //Movemos el auto un poco para arriba para que se pueda mover
            this.Mesh.AutoTransformEnable = true;
            this.Mesh.move(0, 0.5f, 0);
            this.Mesh.updateBoundingBox();

            //Cargo el bouding box obb del auto a partir de su AABB
            this.ObbMesh = TgcBoundingOrientedBox.computeFromAABB(Mesh.BoundingBox);
        }

        public Vector3 GetPosition ()
        {
            return this.Mesh.Position;    
        }

        public void SetPositionMesh(Vector3 unVector, bool rotar)
        {
            if (rotar)
            {
                this.Mesh.Transform = this.Mesh.Transform * Matrix.RotationY(180 * (FastMath.PI / 180));
            }

            this.Mesh.Transform = this.Mesh.Transform * Matrix.Translation(unVector);
        }

        public void SetRuedas(TgcSceneLoader loader)
        {
            //Cargo las ruedas de los autos
            ruedaDerechaDelanteraMesh = loader.loadSceneFromFile(MediaDir + "Vehiculos\\Auto_Rueda_Derecha-TgcScene.xml").Meshes[0];
            ruedaDerechaTraseraMesh = loader.loadSceneFromFile(MediaDir + "Vehiculos\\Auto_Rueda_Derecha-TgcScene.xml").Meshes[0];
            ruedaIzquierdaDelanteraMesh = loader.loadSceneFromFile(MediaDir + "Vehiculos\\Auto_Rueda_Izquierda-TgcScene.xml").Meshes[0];
            ruedaIzquierdaTraseraMesh = loader.loadSceneFromFile(MediaDir + "Vehiculos\\Auto_Rueda_Izquierda-TgcScene.xml").Meshes[0];

            ruedaDerechaDelanteraMesh.AutoTransformEnable = true;
            ruedaDerechaDelanteraMesh.Scale = new Vector3(0.5f, 0.5f, 0.5f);

            ruedaDerechaTraseraMesh.AutoTransformEnable = true;
            ruedaDerechaTraseraMesh.Scale = new Vector3(0.5f, 0.5f, 0.5f);

            ruedaIzquierdaDelanteraMesh.AutoTransformEnable = true;
            ruedaIzquierdaDelanteraMesh.Scale = new Vector3(0.5f, 0.5f, 0.5f);

            ruedaIzquierdaTraseraMesh.AutoTransformEnable = true;
            ruedaIzquierdaTraseraMesh.Scale = new Vector3(0.5f, 0.5f, 0.5f);

            RuedasJugador = new List<TgcMesh> { ruedaDerechaDelanteraMesh, ruedaDerechaTraseraMesh, ruedaIzquierdaDelanteraMesh, ruedaIzquierdaTraseraMesh };
        }
    }
}
