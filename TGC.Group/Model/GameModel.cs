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
using System.Collections;

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
        private ArrayList meshesColisionables;
        private TGCMatrix movimientoCaja;
        private TGCMatrix auxMovimientoCaja;

        // Solo para mostrar
        private MeshTipoCaja caja1Mesh;
        //

        // Planos de limite
        private TgcMesh planoIzq;
        private TgcMesh planoDer;
        private TgcMesh planoFront;
        private TgcMesh planoBack;

        private TgcArrow segment = new TgcArrow();
        private bool moving;

        //Constantes para velocidades de movimiento de plataforma
        private const float MOVEMENT_SPEED = 1f;

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
            planoDer.Transform = TGCMatrix.Translation(-12, 0, -22) * TGCMatrix.Scaling(1, 1, 2.2f);
            planoDer.BoundingBox.transform(planoDer.Transform);

            planoIzq.Transform = TGCMatrix.Translation(25, 0, -22) * TGCMatrix.Scaling(1, 1, 2.2f);
            planoIzq.BoundingBox.transform(planoIzq.Transform);

            //planoDerechoAbajo = planoIzq.createMeshInstance("planoDer");

            planoFront = loader.loadSceneFromFile(MediaDir + "primer-nivel\\pozo-plataformas\\tgc-scene\\plataformas\\planoVertical-TgcScene.xml").Meshes[0];
            planoFront.AutoTransform = false;

            planoBack = planoFront.createMeshInstance("planoBack");
            planoBack.AutoTransform = false;
            planoBack.Transform = TGCMatrix.Translation(50, 0, 70);
            planoBack.BoundingBox.transform(planoBack.Transform);

            planoFront.Transform = TGCMatrix.Translation(50, 0, -197);
            planoFront.BoundingBox.transform(planoFront.Transform);

            caja1 = loader.loadSceneFromFile(MediaDir + "primer-nivel\\Playa final\\caja-TgcScene.xml").Meshes[0];
            caja1.AutoTransform = false;
            caja1.Transform = TGCMatrix.Translation(10, 0, 0);
            movimientoCaja = caja1.Transform;
            
            caja1.BoundingBox.transform(caja1.Transform);

            var skeletalLoader = new TgcSkeletalLoader();
            personaje = skeletalLoader.loadMeshAndAnimationsFromFile(
                MediaDir + "primer-nivel\\pozo-plataformas\\tgc-scene\\Robot\\Robot-TgcSkeletalMesh.xml",
                MediaDir + "primer-nivel\\pozo-plataformas\\tgc-scene\\Robot\\",
                new[]
                {
                    MediaDir + "primer-nivel\\pozo-plataformas\\tgc-scene\\Robot\\Caminando-TgcSkeletalAnim.xml",
                    MediaDir + "primer-nivel\\pozo-plataformas\\tgc-scene\\Robot\\Parado-TgcSkeletalAnim.xml",
                    MediaDir + "primer-nivel\\pozo-plataformas\\tgc-scene\\Robot\\Empujar-TgcSkeletalAnim.xml"
                });

            personaje.AutoTransform = false;

            //Configurar animacion inicial
            personaje.playAnimation("Parado", true);
            //Escalarlo porque es muy grande
            personaje.Position = new TGCVector3(10, 0, 60);
            personaje.Scale = new TGCVector3(0.15f, 0.15f, 0.15f);
            ultimaPos = TGCMatrix.Translation(new TGCVector3(10, 0, 60));

            BoundingBox = true;

            camara = new GameCamera(personaje.Position, 60, 200);
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

            auxMovimientoCaja = TGCMatrix.Identity;

            // Agrego a la lista de meshes colisionables tipo caja, todas las cosas del pedazo de escenario donde estoy contra las que puedo colisionar.
            caja1Mesh = new MeshTipoCaja(caja1);

            meshesColisionables = new ArrayList();

            meshesColisionables.Add(caja1Mesh);
            // 

            CalcularMovimiento();

            CalcularColisiones();

            ultimaPos *= TGCMatrix.Translation(movimiento);

            movimientoCaja *= auxMovimientoCaja;

            if (Input.keyDown(Key.Q))
            {
                BoundingBox = !BoundingBox;
            }

            camara.Target = ultimaPos.Origin;

            PostUpdate();
        }

        //private TGCMatrix CopiarMatriz4x4(TGCMatrix original) {
        //    var copia = TGCMatrix.Identity;

        //    copia.Origin = original.Origin;

        //    copia.M11 = original.M11;
        //    copia.M12 = original.M12;
        //    copia.M13 = original.M13;
        //    copia.M14 = original.M14;

        //    copia.M21 = original.M21;
        //    copia.M22 = original.M22;
        //    copia.M23 = original.M23;
        //    copia.M24 = original.M24;

        //    copia.M31 = original.M31;
        //    copia.M32 = original.M32;
        //    copia.M33 = original.M33;
        //    copia.M34 = original.M34;

        //    copia.M41 = original.M41;
        //    copia.M42 = original.M42;
        //    copia.M43 = original.M43;
        //    copia.M44 = original.M44;

        //    return copia;
        //}

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aquí todo el código referido al renderizado.
        ///     Borrar todo lo que no haga falta.
        /// </summary>
        public override void Render()
        {
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
            PreRender();

            caja1.Transform = movimientoCaja;
            caja1.BoundingBox.transform(caja1.Transform);

            caja1.Render();

                personaje.Transform =
           TGCMatrix.Scaling(personaje.Scale)
                       * TGCMatrix.RotationYawPitchRoll(personaje.Rotation.Y, personaje.Rotation.X, personaje.Rotation.Z)
                       * TGCMatrix.Translation(personaje.Position) // esto es para que inicalmente arranque donde tiene que arrancar, creo.
                       * ultimaPos;

            personaje.BoundingBox.transform(personaje.Transform);
            
            personaje.BoundingBox.Render();

            personaje.animateAndRender(ElapsedTime);

            if (BoundingBox)
            {
                caja1Mesh.RenderizaRayos();
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
            personaje.playAnimation("Parado", true);

            var velocidadCaminar = VELOCIDAD_DESPLAZAMIENTO * ElapsedTime;

            //Calcular proxima posicion de personaje segun Input
            moving = false;
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

            if (moving)
            {
                //personaje.playAnimation("Caminando", true); // esto creo que esta mal, si colisiono no deberia caminar.

                if (ChocoConLimite(personaje, planoIzq))
                    NoMoverHacia(Key.A);

                if (ChocoConLimite(personaje, planoBack))
                {
                    NoMoverHacia(Key.S);
                    planoBack.BoundingBox.setRenderColor(Color.AliceBlue);
                }
                else
                { // esto no hace falta despues
                    planoBack.BoundingBox.setRenderColor(Color.Yellow);
                }

                if (ChocoConLimite(personaje, planoDer))
                    NoMoverHacia(Key.D);
               
                if (ChocoConLimite(personaje, planoFront))
                { // HUBO CAMBIO DE ESCENARIO
                  /* Aca deberiamos hacer algo como no testear mas contra las cosas del escenario anterior y testear
                    contra las del escenario actual. 
                  */

                    planoFront.BoundingBox.setRenderColor(Color.AliceBlue);
                }
                else
                {
                    planoFront.BoundingBox.setRenderColor(Color.Yellow);
                }

            }
        }

        private void NoMoverHacia(Key key) { 
            switch(key)
            {
                case Key.A:
                    if (movimiento.X > 0) // los ejes estan al reves de como pensaba, o lo entendi mal.
                        movimiento.X = 0;
                    break;
                case Key.D:
                    if (movimiento.X < 0)
                        movimiento.X = 0;
                    break;
                case Key.S:
                    if (movimiento.Z > 0)
                        movimiento.Z = 0;
                    break;
                case Key.W:
                    if (movimiento.Z < 0)
                        movimiento.Z = 0;
                    break;
            }
                
        }

        private bool ChocoConLimite(TgcSkeletalMesh personaje, TgcMesh plano) {
            return TgcCollisionUtils.testAABBAABB(plano.BoundingBox, personaje.BoundingBox);
        }

        private void CalcularColisiones() {
            if (moving)
            {
                foreach (MeshTipoCaja caja in meshesColisionables)
                {
                    if (caja.ChocoConFrente(personaje))
                    {
                        auxMovimientoCaja = TGCMatrix.Translation(0, 0, movimiento.Z);
                        personaje.playAnimation("Empujar", true);
                        break;
                    }
                    else if (caja.ChocoArriba(personaje))
                    {
                        auxMovimientoCaja = TGCMatrix.Translation(0, 0, 0);
                        movimiento.Y = 0; // Ojo, que pasa si quiero saltar desde arriba de la plataforma
                        break;
                    }
                    else if (caja.ChocoAtras(personaje))
                    {
                        auxMovimientoCaja = TGCMatrix.Translation(0, 0, 0);
                        NoMoverHacia(Key.S);
                        break;
                    }
                    else if (caja.ChocoALaIzquierda(personaje))
                    {
                        auxMovimientoCaja = TGCMatrix.Translation(0, 0, 0);
                        NoMoverHacia(Key.D);
                        break;
                    }
                    else if (caja.ChocoALaDerecha(personaje))
                    {
                        auxMovimientoCaja = TGCMatrix.Translation(0, 0, 0);
                        NoMoverHacia(Key.A);
                        break;
                    }
                }
            }
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
            personaje.Dispose();
            planoIzq.Dispose(); // solo se borran los originales
            planoFront.Dispose(); // solo se borran los originales
            caja1.Dispose();

            //foreach (TgcMesh mesh in meshesColisionables) {
            //    mesh.Dispose(); // mmm, no se que pasaria con las instancias...
            //} // recontra TODO
        }
    }
}