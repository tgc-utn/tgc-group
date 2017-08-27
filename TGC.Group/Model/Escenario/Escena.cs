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

        private TgcMesh Mesh { get; set; }
        private new TgcD3dInput Input { get; set; }

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
            var d3dDevice = D3DDevice.Instance.Device;
            this.MediaDir = this.env.MediaDir;
         
            //Cargar mesh
            var Mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + "LogoTGC-TgcScene.xml").Meshes[0];

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
            Mesh.render();

        }
        public void dispose()
        {

            //Dispose del mesh.
            Mesh.dispose();
        }

       

      
    }
}