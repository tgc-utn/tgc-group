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
        private TgcScene escenaPlataformas;
        private TgcSkeletalMesh personaje;
        private GameCamera camara;
        private TGCVector3 movimiento;
        private TGCMatrix ultimaPos;

        // Planos de limite
        private TgcMesh planoIzq;
        private TgcMesh planoDer;
        private TgcMesh planoFront;
        private TgcMesh planoBack;

        // Plataformas
        private TgcMesh plataforma1;
        private TgcMesh plataforma2;

        //Transformaciones de plataforma
        private TGCMatrix transformacionBox;
        private TGCMatrix transformacionBox2;

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
            escenaPlataformas = loader.loadSceneFromFile(MediaDir + "primer-nivel\\pozo-plataformas\\tgc-scene\\plataformas\\escenarioPlataformas-TgcScene.xml");

            plataforma1 = loader.loadSceneFromFile(MediaDir + "primer-nivel\\pozo-plataformas\\tgc-scene\\plataformas\\plataforma-TgcScene.xml").Meshes[0];

            plataforma2 = plataforma1.createMeshInstance(plataforma1.Name + "2");

            plataforma1.AutoTransform = false;
            plataforma2.AutoTransform = false;

            planoIzq = loader.loadSceneFromFile(MediaDir + "primer-nivel\\pozo-plataformas\\tgc-scene\\plataformas\\planoHorizontal-TgcScene.xml").Meshes[0];
            planoIzq.AutoTransform = false;

            planoDer = planoIzq.createMeshInstance("planoDer");
            planoDer.AutoTransform = false;
            planoDer.Transform = TGCMatrix.Translation(-39, 0, 0);
            planoDer.BoundingBox.transform(planoDer.Transform);

            planoFront = loader.loadSceneFromFile(MediaDir + "primer-nivel\\pozo-plataformas\\tgc-scene\\plataformas\\planoVertical-TgcScene.xml").Meshes[0];
            planoFront.AutoTransform = false;
            planoFront.Transform = TGCMatrix.Translation(50, 0, -65);
            planoFront.BoundingBox.transform(planoFront.Transform);

            planoBack = planoFront.createMeshInstance("planoBack");
            planoBack.AutoTransform = false;
            planoBack.Transform = TGCMatrix.Translation(0, 0, 135);
            planoBack.BoundingBox.transform(planoBack.Transform);


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
            personaje.Position = new TGCVector3(0, 0, 100);
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

            AnimarPlataformas();

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

            personaje.Transform =
               TGCMatrix.Scaling(personaje.Scale)
                           * TGCMatrix.RotationYawPitchRoll(personaje.Rotation.Y, personaje.Rotation.X, personaje.Rotation.Z)
                           * TGCMatrix.Translation(personaje.Position)
                           * ultimaPos;

            personaje.BoundingBox.transform(personaje.Transform);

            personaje.animateAndRender(ElapsedTime);

            RenderizarPlataformas();

            if (BoundingBox)
            {
                planoBack.BoundingBox.Render();
                planoFront.BoundingBox.Render();
                planoIzq.BoundingBox.Render();
                planoDer.BoundingBox.Render();
                plataforma1.BoundingBox.Render();
                plataforma2.BoundingBox.Render();
                personaje.BoundingBox.Render();
            }
  
            escenaPlataformas.RenderAll();

   
            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        private void AnimarPlataformas() {
            //Muevo las plataformas
            var Mover = TGCMatrix.Translation(0, 0, -10);
            var Mover2 = TGCMatrix.Translation(0, 0, 65);

            //Punto por donde va a rotar
            var Trasladar = TGCMatrix.Translation(0, 0, 10);
            var Trasladar2 = TGCMatrix.Translation(0, 0, -10);

            //Aplico la rotacion
            var Rot = TGCMatrix.RotationX(orbitaDeRotacion);

            //Giro para que la caja quede derecha
            var RotInversa = TGCMatrix.RotationX(-orbitaDeRotacion);

            transformacionBox = Mover * Trasladar * Rot * Trasladar * RotInversa;
            transformacionBox2 = Mover2 * Trasladar2 * RotInversa * Trasladar2 * Rot;
        }

        private void RenderizarPlataformas() {
            //Dibujar la primera plataforma en pantalla
            plataforma1.Transform = transformacionBox;
            plataforma1.Render();
            plataforma1.BoundingBox.transform(plataforma1.Transform);
            plataforma1.BoundingBox.Render();

            //Dibujar la segunda plataforma en pantalla
            plataforma2.Transform = transformacionBox2;
            plataforma2.Render();
            plataforma2.BoundingBox.transform(plataforma2.Transform);
            plataforma2.BoundingBox.Render();

            //Recalculamos la orbita de rotacion
            orbitaDeRotacion += MOVEMENT_SPEED * ElapsedTime;
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
                NoMoverHacia(Key.A);

            }
            else if (ChocoConLimite(personaje, planoDer))
            {
                NoMoverHacia(Key.D);
            }
            else { // no hay colisiones contra los planos laterales
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

            if (ChocoConLimite(personaje, planoBack))
            { // HUBO CAMBIO DE ESCENARIO
              /* Aca deberiamos hacer algo como no testear mas contra las cosas del escenario anterior y testear
                  contra las del escenario actual. 
              */

                planoBack.BoundingBox.setRenderColor(Color.AliceBlue);
            }
            else {
                planoBack.BoundingBox.setRenderColor(Color.Yellow);
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

        private void NoMoverHacia(Key key) {
            switch(key)
            {
                case Key.A:
                    if (movimiento.X > 0) // los ejes estan al reves de como pensaba, o lo entendi mal.
                        ultimaPos *= TGCMatrix.Translation(0, movimiento.Y, movimiento.Z);
                    else
                        ultimaPos *= TGCMatrix.Translation(movimiento);
                    break;
                case Key.D:
                    if (movimiento.X < 0)
                        ultimaPos *= TGCMatrix.Translation(0, movimiento.Y, movimiento.Z);
                    else
                        ultimaPos *= TGCMatrix.Translation(movimiento);
                    break;
                case Key.S:
                    if (movimiento.Z > 0)
                        ultimaPos *= TGCMatrix.Translation(movimiento.X, movimiento.Y, 0);
                    else
                        ultimaPos *= TGCMatrix.Translation(movimiento);
                    break;
                case Key.W:
                    if (movimiento.Z < 0)
                        ultimaPos *= TGCMatrix.Translation(movimiento.X, movimiento.Y, 0);
                    else
                        ultimaPos *= TGCMatrix.Translation(movimiento);
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
            escenaPlataformas.DisposeAll();
            personaje.Dispose();
            planoIzq.Dispose(); // solo se borran los originales
            planoFront.Dispose(); // solo se borran los originales
            plataforma1.Dispose(); // solo se borran los originales
        }
    }
}