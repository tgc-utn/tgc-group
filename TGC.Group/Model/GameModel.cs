using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Camara;
using TGC.Core.Collision;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Group.Camara;

namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo para implementar el TP.
    ///     Inicialmente puede ser renombrado o copiado para hacer m·s ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar el modelo que instancia GameForm <see cref="Form.GameForm.InitGraphics()" />
    ///     line 97.
    /// </summary>
    public class GameModel : TGCExample {
        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        /// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
        /// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir) {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        private TgcScene scene;
        private TgcCamera Camara { get; set; }

        //Boleano para ver si dibujamos el boundingbox
        private bool BoundingBox { get; set; }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aquÅEtodo el cÛdigo de inicializaciÛn: cargar modelos, texturas, estructuras de optimizaciÛn, todo
        ///     procesamiento que podemos pre calcular para nuestro juego.
        ///     Borrar el codigo ejemplo no utilizado.
        /// </summary>
        public override void Init() {
           
            scene = new TgcSceneLoader().loadSceneFromFile(MediaDir + "MapaV1-TgcScene.xml");
            Camera = new TgcFpsCamera(new TGCVector3(0, 50, 0), Input);

        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la lÛgica de computo del modelo, asÅEcomo tambiÈn verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        public override void Update() {
            PreUpdate();

            //Capturar Input teclado
            if (Input.keyPressed(Key.F)) {
                BoundingBox = !BoundingBox;
            }

            var input = Input;

            PostUpdate();
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aquÅEtodo el cÛdigo referido al renderizado.
        ///     Borrar todo lo que no haga falta.
        /// </summary>
        public override void Render() {
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones seg˙n nuestra conveniencia.
            PreRender(); 

            scene.RenderAll();
            //Dibuja un texto por pantalla
            DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.OrangeRed);

            if (BoundingBox)
            {
                foreach (var mesh in scene.Meshes) 
                {
                    mesh.BoundingBox.Render();
                }

            }


            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        /// <summary>
        ///     Se llama cuando termina la ejecuciÛn del ejemplo.
        ///     Hacer Dispose() de todos los objetos creados.
        ///     Es muy importante liberar los recursos, sobretodo los gr·ficos ya que quedan bloqueados en el device de video.
        /// </summary>
        public override void Dispose()
        {
            scene.DisposeAll();
        }
    }
}