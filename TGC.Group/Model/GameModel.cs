using Microsoft.DirectX.DirectInput;
using System.Drawing;
using System.Linq;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.SkeletalAnimation;
using TGC.Core.Textures;

namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo para implementar el TP.
    ///     Inicialmente puede ser renombrado o copiado para hacer más ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar el modelo que instancia GameForm <see cref="Form.GameForm.InitGraphics()" />
    ///     line 97.
    /// </summary>
    public class GameModel : TgcExample
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

        // Mesh del jugador.
        private TgcSkeletalMesh meshJugador { get; set; }

        // Escena de la ciudad.
        private TgcScene scene { get; set; }

        private TGCVector3 cameraOffset;

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

            //Cargo el unico mesh que tiene la escena.
            string humanDir = MediaDir + "BasicHuman\\";

            // directorio del mesh base del humano
            string humanMeshDir = humanDir + "BasicHuman-TgcSkeletalMesh.xml";

            // prepara la lista de animaciones
            // TODO: poner el resto de las animaciones?
            string[] humanAnims = {
                "StandBy"
            };
            string[] humanAnimsDir = new string[humanAnims.Length];

            // convierto de nombre de animacion a directorio
            humanAnimsDir = humanAnims.Select(s => humanDir + "Animations\\" + s + "-TgcSkeletalAnim.xml").ToArray<string>();

            // cargo el humano
            meshJugador = new TgcSkeletalLoader().loadMeshAndAnimationsFromFile(humanMeshDir, humanAnimsDir);
            meshJugador.buildSkletonMesh();
            //Defino una escala en el modelo logico del mesh que es muy grande.
            meshJugador.playAnimation(humanAnims[0]);
            meshJugador.Position = new TGCVector3(0, 10, 0);

            scene = new TgcSceneLoader().loadSceneFromFile(MediaDir + "Ciudad\\Ciudad-TgcScene.xml");

            cameraOffset = new TGCVector3(0, 200, 150);
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        public override void Update()
        {
            PreUpdate();

            // seteo la posicion de la camara
            var cameraPosition = meshJugador.Position + cameraOffset;
            var lookAt = meshJugador.Position;
            Camara.SetCamera(cameraPosition, lookAt);

            TGCVector3 movement = new TGCVector3(0, 0, 0);
            if (Input.keyDown(Key.UpArrow)) {
                movement.Z -= 1;
            } else if (Input.keyDown(Key.DownArrow)) {
                movement.Z += 1;
            }

            movement = movement * 1000 * ElapsedTime;
            meshJugador.Move(movement);

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

            //Dibuja un texto por pantalla
            DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.OrangeRed);
            DrawText.drawText("Con clic izquierdo subimos la camara [Actual]: " + TGCVector3.PrintVector3(Camara.Position), 0, 30, Color.OrangeRed);

            scene.RenderAll();

            //Cuando tenemos modelos mesh podemos utilizar un método que hace la matriz de transformación estándar.
            //Es útil cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones jerárquicas o complicadas.
            meshJugador.UpdateMeshTransform();
            //Render del mesh
            meshJugador.Render();

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
            meshJugador.Dispose();
            scene.DisposeAll();
        }
    }
}