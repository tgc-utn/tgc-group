using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;

namespace TGC.Group.Model
{
    public class GameModel : TGCExample
    {
        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        /// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
        /// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        private TgcScene Escena { get; set; }

        private TgcMesh Cancha { get; set; }
        private TgcMesh Arco { get; set; }
        private TgcMesh Pelota { get; set; }

        private List<Jugador> jugadores = new List<Jugador>();

        private TGCVector3 LookAt;
        private TGCVector3 CameraPosition;

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aquí todo el código de inicialización: cargar modelos, texturas, estructuras de optimización, todo
        ///     procesamiento que podemos pre calcular para nuestro juego.
        ///     Borrar el codigo ejemplo no utilizado.
        /// </summary>
        public override void Init()
        {
            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;

            //Creamos una caja 3D ubicada de dimensiones (5, 10, 5) y la textura como color.
            var size = new TGCVector3(5, 10, 5);

            Escena = new TgcSceneLoader().loadSceneFromFile(MediaDir + "Cancha2-TgcScene.xml");
            Cancha = Escena.Meshes[0];
            Arco = Escena.Meshes[1];
            TgcMesh Auto = Escena.Meshes[2];
            TgcMesh Tractor = Escena.Meshes[5];
            TgcMesh Patrullero = Escena.Meshes[3];
            TgcMesh Tanque = Escena.Meshes[4];

            jugadores.Add(new Jugador(Auto, new TGCVector3(-20, 0, 100), new TGCVector3(0, 0, 0)));
            jugadores.Add(new Jugador(Tractor, new TGCVector3(0, 0, -30), new TGCVector3(0, FastMath.PI, 0)));
            jugadores.Add(new Jugador(Patrullero, new TGCVector3(0, 0, 30), new TGCVector3(0, 0, 0)));
            jugadores.Add(new Jugador(Tanque, new TGCVector3(20, 0, -100), new TGCVector3(0, FastMath.PI, 0)));

            Pelota = Escena.getMeshByName("Pelota");


            LookAt = new TGCVector3(TGCVector3.Empty);
            CameraPosition = new TGCVector3(0, 100, 225);
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        public override void Update()
        {
            PreUpdate();


            LookAt.X += Input.XposRelative * 0.5f;
            LookAt.Y += Input.YposRelative * 0.5f;
            Camera.SetCamera(CameraPosition, LookAt);

            if (Input.keyDown(Key.W))
            {
                CameraPosition.Z += 0.5f * ElapsedTime;
            }
            if (Input.keyDown(Key.S))
            {
                CameraPosition.Z -= 0.5f * ElapsedTime;
            }

            PostUpdate();
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aquí todo el código referido al renderizado.
        ///     Borrar todo lo que no haga falta.
        /// </summary>
        public override void Render()
        {
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
            PreRender();

            Arco.Rotation = new TGCVector3(0, 0, 0);
            Arco.UpdateMeshTransform();
            Arco.Render();

            Arco.Rotation = new TGCVector3(0, FastMath.PI, 0);
            Arco.UpdateMeshTransform();
            Arco.Render();

            Pelota.UpdateMeshTransform();
            Pelota.Render();

            //Cuando tenemos modelos mesh podemos utilizar un método que hace la matriz de transformación estándar.
            //Es útil cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones jerárquicas o complicadas.
            Cancha.UpdateMeshTransform();
            Cancha.Render();

            foreach (var jugador in jugadores)
            {
                jugador.Render();
            }

            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        /// <summary>
        ///     Se llama cuando termina la ejecución del ejemplo.
        ///     Hacer Dispose() de todos los objetos creados.
        ///     Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        /// </summary>
        public override void Dispose()
        {
            Escena.DisposeAll();
        }
    }
}