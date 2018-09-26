using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.SkeletalAnimation;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Textures;
using TGC.Group.Camera;
using System;

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

        //Boleano para ver si dibujamos el boundingbox
        private bool BoundingBox { get; set; }
        private const float VELOCIDAD_DESPLAZAMIENTO = 50f;
        private TgcScene escenaPlaya;
        private TgcSkeletalMesh personaje;
        private TgcMesh caja1;
        private GameCamera camara;
        private TGCVector3 movimiento;
        private TGCMatrix ultimaPos;
   
        private bool estaEnLaPlataforma1;
        private bool estaEnLaPlataforma2;

        // Planos de limite
        private TgcMesh planoIzq;
        private TgcMesh planoDer;
        private TgcMesh planoFront;
        private TgcMesh planoBack;

        //Constantes para velocidades de movimiento de plataforma
        private const float MOVEMENT_SPEED = 1f;
        private float orbitaDeRotacion;

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

            var loader = new TgcSceneLoader();
            escenaPlaya = loader.loadSceneFromFile(MediaDir + "primer-nivel\\Playa final\\Playa-TgcScene.xml");
              
            planoIzq = loader.loadSceneFromFile(MediaDir + "primer-nivel\\pozo-plataformas\\tgc-scene\\plataformas\\planoHorizontal-TgcScene.xml").Meshes[0];            
            planoIzq.AutoTransform = false;
           
            planoDer = planoIzq.createMeshInstance("planoDer");
            planoDer.AutoTransform = false;
            planoDer.Transform = TGCMatrix.Translation(-12, 0, 0) * TGCMatrix.Scaling(1, 1, 2.2f);
            planoDer.BoundingBox.transform(planoDer.Transform);

            planoIzq.Transform = TGCMatrix.Translation(14, 0, 0) * TGCMatrix.Scaling(1, 1, 2.2f);
            planoIzq.BoundingBox.transform(planoIzq.Transform);

            //planoDerechoAbajo = planoIzq.createMeshInstance("planoDer");

            planoFront = loader.loadSceneFromFile(MediaDir + "primer-nivel\\pozo-plataformas\\tgc-scene\\plataformas\\planoVertical-TgcScene.xml").Meshes[0];
            planoFront.AutoTransform = false;

            planoBack = planoFront.createMeshInstance("planoBack");
            planoBack.AutoTransform = false;
            planoBack.Transform = TGCMatrix.Translation(50, 0, 140);
            planoBack.BoundingBox.transform(planoBack.Transform);

            planoFront.Transform = TGCMatrix.Translation(50, 0, -30);
            planoFront.BoundingBox.transform(planoFront.Transform);

            caja1 = loader.loadSceneFromFile(MediaDir + "primer-nivel\\Playa final\\caja-TgcScene.xml").Meshes[0];

            //caja1.Transform = TGCMatrix.Scaling(0.001f, 0.001f, 0.001f);
            //caja1.BoundingBox.transform(caja1.Transform);

            var skeletalLoader = new TgcSkeletalLoader();
            personaje = skeletalLoader.loadMeshAndAnimationsFromFile(
                MediaDir + "primer-nivel\\pozo-plataformas\\tgc-scene\\Robot\\Robot-TgcSkeletalMesh.xml",
                MediaDir + "primer-nivel\\pozo-plataformas\\tgc-scene\\Robot\\",
                new[]
                {
                    MediaDir + "primer-nivel\\pozo-plataformas\\tgc-scene\\Robot\\Caminando-TgcSkeletalAnim.xml",
                    MediaDir + "primer-nivel\\pozo-plataformas\\tgc-scene\\Robot\\Parado-TgcSkeletalAnim.xml"
                });

            personaje.AutoTransform = false;

            //Configurar animacion inicial
            personaje.playAnimation("Parado", true);
            //Escalarlo porque es muy grande
            personaje.Position = new TGCVector3(10, 0, 60);
            personaje.Scale = new TGCVector3(0.15f, 0.15f, 0.15f);
            ultimaPos = TGCMatrix.Translation(personaje.Position);

            camara = new GameCamera(personaje.Position, 100, 200);
            Camara = camara;

        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        public override void Update()
        {
            PreUpdate();

            CalcularMovimiento();

            if (Input.keyDown(Key.Q))
            {
                BoundingBox = !BoundingBox;
            }

            camara.Target = personaje.Position;

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

            caja1.Render();

                personaje.Transform =
           TGCMatrix.Scaling(personaje.Scale)
                       * TGCMatrix.RotationYawPitchRoll(personaje.Rotation.Y, personaje.Rotation.X, personaje.Rotation.Z)
                       * TGCMatrix.Translation(personaje.Position)
                       * ultimaPos;

            personaje.BoundingBox.transform(personaje.Transform);
            
            personaje.BoundingBox.Render();

            personaje.animateAndRender(ElapsedTime);

            if (BoundingBox)
            {
                planoBack.BoundingBox.Render();
                planoFront.BoundingBox.Render();
                planoIzq.BoundingBox.Render();
                planoDer.BoundingBox.Render();
                personaje.BoundingBox.Render();
                caja1.BoundingBox.Render();
            }

            escenaPlaya.RenderAll();

   
            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        private void CalcularMovimiento()
        {
            var velocidadCaminar = VELOCIDAD_DESPLAZAMIENTO * ElapsedTime;

            //Calcular proxima posicion de personaje segun Input
            var moving = false;
            movimiento = TGCVector3.Empty;
  
            if (Input.keyDown(Key.W))
            {
                movimiento.Z = -velocidadCaminar;
                moving = true;
            }

            if (Input.keyDown(Key.S))
            {
                movimiento.Z = velocidadCaminar;
                moving = true;
            }

            if (Input.keyDown(Key.D))
            {
                movimiento.X = -velocidadCaminar;
                moving = true;
            }

            if (Input.keyDown(Key.A))
            {
                movimiento.X = velocidadCaminar;
                moving = true;
            }

            if (ChocoConLimite(personaje, planoIzq))
            {
                if (ChocoConLimite(personaje, planoBack))
                {
                    var noTeMuevasParaAtras = new TGCVector3(0, movimiento.Y, 0);
                    NoMoverHacia(Key.S, noTeMuevasParaAtras);

                    planoBack.BoundingBox.setRenderColor(Color.AliceBlue);
                }
                else {
                    planoBack.BoundingBox.setRenderColor(Color.Yellow);
                    NoMoverHacia(Key.A, movimiento);
                }
            }
            else if (ChocoConLimite(personaje, planoDer))
            {
                if (ChocoConLimite(personaje, planoBack))
                {
                    var noTeMuevasParaAtras = new TGCVector3(0, movimiento.Y, 0);
                    NoMoverHacia(Key.S, noTeMuevasParaAtras);
                    planoBack.BoundingBox.setRenderColor(Color.AliceBlue);
                }
                else {
                    NoMoverHacia(Key.D, movimiento);
                    planoBack.BoundingBox.setRenderColor(Color.Yellow);
                }
            }

            else if (ChocoConLimite(personaje, planoBack)) {
                NoMoverHacia(Key.S, movimiento);
                planoBack.BoundingBox.setRenderColor(Color.AliceBlue);
            }
            else
            { // no hay colisiones contra los planos laterales
                planoBack.BoundingBox.setRenderColor(Color.Yellow);
                ultimaPos *= TGCMatrix.Translation(movimiento);
            }

            if (ChocoConLimite(personaje, planoFront))
            { // HUBO CAMBIO DE ESCENARIO
                /* Aca deberiamos hacer algo como no testear mas contra las cosas del escenario anterior y testear
                  contra las del escenario actual. 
                */

                planoFront.BoundingBox.setRenderColor(Color.AliceBlue);
            }
            else {
                planoFront.BoundingBox.setRenderColor(Color.Yellow);
            }

            if (moving)
            {
                personaje.playAnimation("Caminando", true);
            }
            else
            {
                personaje.playAnimation("Parado", true);
            }

            camara.Target = personaje.Position;
        }

        private void NoMoverHacia(Key key, TGCVector3 vector) {
            switch(key)
            {
                case Key.A:
                    if (movimiento.X > 0) // los ejes estan al reves de como pensaba, o lo entendi mal.
                        ultimaPos *= TGCMatrix.Translation(0, vector.Y, vector.Z);
                    else
                        ultimaPos *= TGCMatrix.Translation(vector);
                    break;
                case Key.D:
                    if (movimiento.X < 0)
                        ultimaPos *= TGCMatrix.Translation(0, vector.Y, vector.Z);
                    else
                        ultimaPos *= TGCMatrix.Translation(vector);
                    break;
                case Key.S:
                    if (movimiento.Z > 0)
                        ultimaPos *= TGCMatrix.Translation(vector.X, vector.Y, 0);
                    else
                        ultimaPos *= TGCMatrix.Translation(vector);
                    break;
                case Key.W:
                    if (movimiento.Z < 0)
                        ultimaPos *= TGCMatrix.Translation(vector.X, vector.Y, 0);
                    else
                        ultimaPos *= TGCMatrix.Translation(vector);
                    break;
            }
                
        }

        private bool ChocoConLimite(TgcSkeletalMesh personaje, TgcMesh planoIzq) {
            return TgcCollisionUtils.testAABBAABB(planoIzq.BoundingBox, personaje.BoundingBox);
        }

        /// <summary>
        ///     Se llama cuando termina la ejecución del ejemplo.
        ///     Hacer Dispose() de todos los objetos creados.
        ///     Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        /// </summary>
        public override void Dispose()
        {
            //Dispose del mesh.
            escenaPlaya.DisposeAll();
            caja1.Dispose();
            personaje.Dispose();
            planoIzq.Dispose(); // solo se borran los originales
            planoFront.Dispose(); // solo se borran los originales
        }
    }
}