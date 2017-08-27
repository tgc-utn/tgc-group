using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.Utils;
using TGC.Core.Camara;
using System.Collections.Generic;
using TGC.Core.Terrain;
using TGC.Core.UserControls;
using TGC.Core.UserControls.Modifier;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model
{

    // Objeto Escena
    public class Escena
    {
        private string MediaDir;
        private TgcSceneLoader loader;
        private GameModel env;

        private new TgcD3dInput Input { get; set; }


        //Caja que se muestra en el ejemplo.
        private TgcBox Box { get; set; }

        private TgcMesh Mesh { get; set; }


        //Constantes para velocidades de movimiento
        private const float ROTATION_SPEED = 0.5f;
        private const float MOVEMENT_SPEED = 5f;
        private int TiempoRetardo = 3;
        private int contadorDeCiclos = 0;
        private float currentMoveDir = 1f;

        private static Escena myInstance;

        public static Escena getInstance()
        {
            return myInstance;
        }

        public Escena(GameModel env)
        {
            //Device de DirectX para crear primitivas.
            this.env = env;
            myInstance = this;
            this.MediaDir = this.env.MediaDir;
            var d3dDevice = D3DDevice.Instance.Device;


            //Cargamos una textura, tener en cuenta que cargar una textura significa crear una copia en memoria.
            //Es importante cargar texturas en Init, si se hace en el render loop podemos tener grandes problemas si instanciamos muchas.
            var texture = TgcTexture.createTexture(MediaDir + Game.Default.TexturaCaja);

            //Creamos una caja 3D ubicada de dimensiones (5, 10, 5) y la textura como color.
            var size = new Vector3(5, 10, 5);
            //Construimos una caja según los parámetros, por defecto la misma se crea con centro en el origen y se recomienda así para facilitar las transformaciones.
            Box = TgcBox.fromSize(size, texture);

            //Posición donde quiero que este la caja, es común que se utilicen estructuras internas para las transformaciones.
            //Entonces actualizamos la posición lógica, luego podemos utilizar esto en render para posicionar donde corresponda con transformaciones.
            Box.Position = new Vector3(-25, 0, 0);



            //Cargar mesh
            Mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + "LogoTGC-TgcScene.xml").Meshes[0];

            //Defino una escala en el modelo logico del mesh que es muy grande.
            Mesh.Scale = new Vector3(0.5f, 0.5f, 0.5f);

            //Centrar camara rotacional respecto a este mesh
            var Camara = new TgcRotationalCamera(Mesh.BoundingBox.calculateBoxCenter(),
                Mesh.BoundingBox.calculateBoxRadius() * 2, Input);
        }


        public void Update()
        {
            
        }

        public void Render()
        {
            //Cuando tenemos modelos mesh podemos utilizar un método que hace la matriz de transformación estándar.
            //Es útil cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones jerárquicas o complicadas.
            //Mesh.UpdateMeshTransform();
            //Render del mesh
            //Mesh.render();

            //Siempre antes de renderizar el modelo necesitamos actualizar la matriz de transformacion.
            //Debemos recordar el orden en cual debemos multiplicar las matrices, en caso de tener modelos jerárquicos, tenemos control total.
            Box.Transform = Matrix.Scaling(Box.Scale) *
                            Matrix.RotationYawPitchRoll(Box.Rotation.Y, Box.Rotation.X, Box.Rotation.Z) *
                            Matrix.Translation(Box.Position);
            //A modo ejemplo realizamos toda las multiplicaciones, pero aquí solo nos hacia falta la traslación.
            //Finalmente invocamos al render de la caja
            Box.render();

        }
        public void dispose()
        {

            //Dispose del mesh.
            //Mesh.dispose();

            //Dispose de la caja.
            Box.dispose();
        }

       

      
    }
}