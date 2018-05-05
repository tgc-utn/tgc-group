using Microsoft.DirectX.DirectInput;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.SkeletalAnimation;

namespace TGC.Group.Model {
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
        private TgcSkeletalMesh personaje;
        private TGCVector3 velocidadPersonaje;

        // Escena de la ciudad.
        private TgcScene scene;

        private TGCVector3 cameraOffset;
        private const float WALK_SPEED = 5f;
        private bool moving = false;

        public override void Init() {
            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;

            var skeletalLoader = new TgcSkeletalLoader();
            personaje = skeletalLoader.loadMeshAndAnimationsFromFile(


                MediaDir + "Robot\\Robot-TgcSkeletalMesh.xml",
                MediaDir + "Robot\\",
                new[]
                {
                     MediaDir + "Robot\\Caminando-TgcSkeletalAnim.xml",
                     MediaDir + "Robot\\Parado-TgcSkeletalAnim.xml"
                });

            personaje.buildSkletonMesh();
            personaje.playAnimation("Parado", true);
            personaje.Scale = new TGCVector3(0.5f, 0.5f, 0.5f);
            personaje.Position = new TGCVector3(300, -85, 0);

            scene = new TgcSceneLoader().loadSceneFromFile(MediaDir + "Jungla\\Jungla -TgcScene.xml");

            cameraOffset = new TGCVector3(0, 200, 400);

            velocidadPersonaje = TGCVector3.Empty;
        }


        public override void Update() {
            PreUpdate();

            Camara.SetCamera(personaje.Position + cameraOffset, personaje.Position);

            if (Input.keyDown(Key.W) || Input.keyDown(Key.UpArrow)) {
                velocidadPersonaje.Z = -WALK_SPEED;
                moving = true;
            }

            if (Input.keyDown(Key.S) || Input.keyDown(Key.DownArrow)) {
                velocidadPersonaje.Z = WALK_SPEED;
                moving = true;
            }

            if (Input.keyDown(Key.D) || Input.keyDown(Key.RightArrow)) {
                velocidadPersonaje.X = -WALK_SPEED;
                moving = true;
            }

            if (Input.keyDown(Key.A) || Input.keyDown(Key.LeftArrow)) {
                velocidadPersonaje.X = WALK_SPEED;
                moving = true;
            }

            if (moving) {
                personaje.playAnimation("Caminando", true);
            } else {
                personaje.playAnimation("Parado", true);
            }

            //Aplicar movimiento hacia adelante o atras segun la orientacion actual del Mesh
            var lastPos = personaje.Position;

            personaje.Move(velocidadPersonaje);
            velocidadPersonaje.X = 0;
            velocidadPersonaje.Z = 0;
            moving = false;

            PostUpdate();
        }

        public override void Render() {
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
            PreRender();

            scene.RenderAll();

            //Cuando tenemos modelos mesh podemos utilizar un método que hace la matriz de transformación estándar.
            //Es útil cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones jerárquicas o complicadas.
            personaje.UpdateMeshTransform();
            //Render del mesh
            personaje.animateAndRender(ElapsedTime);

            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        public override void Dispose() {
            personaje.Dispose();
            scene.DisposeAll();
        }
    }
}