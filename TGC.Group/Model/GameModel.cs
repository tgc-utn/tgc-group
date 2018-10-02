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
using System.Collections.Generic;

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
        public bool BoundingBox { get; set; }
        private const float VELOCIDAD_DESPLAZAMIENTO = 50f;
        private TgcScene escenaPlaya;
        private Personaje personaje = new Personaje();
        private TgcMesh caja1;
        private GameCamera camara;
        private TGCVector3 movimiento;
        private ArrayList meshesColisionables;
        private TGCMatrix movimientoCaja;
        private Dictionary<string, Escenario> escenarios;
        private Escenario escenarioActual;
        

        // Solo para mostrar
        private MeshTipoCaja caja1Mesh;
        //

        private TgcArrow segment = new TgcArrow();

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

            escenarios = new Dictionary<string, Escenario>();

            escenarios["playa"] = new EscenarioPlaya(this);

            escenarioActual = escenarios["playa"];
            var loader = new TgcSceneLoader();
            caja1 = loader.loadSceneFromFile(MediaDir + "primer-nivel\\Playa final\\caja-TgcScene.xml").Meshes[0];
            caja1.AutoTransform = false;
            caja1.Transform = TGCMatrix.Translation(10, 0, 0);
            movimientoCaja = caja1.Transform;
            
            caja1.BoundingBox.transform(caja1.Transform);

            personaje.Init(this);

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

            movimientoCaja = TGCMatrix.Identity;

            // Agrego a la lista de meshes colisionables tipo caja, todas las cosas del pedazo de escenario donde estoy contra las que puedo colisionar.
            caja1Mesh = new MeshTipoCaja(caja1);

            meshesColisionables = new ArrayList();

            meshesColisionables.Add(caja1Mesh);
            // 

            personaje.Update();

            movimiento = personaje.movimiento;

            CalcularMovimiento();

            CalcularColisiones();

            personaje.Movete(movimiento);

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

            caja1.Transform *= movimientoCaja;
            caja1.BoundingBox.transform(caja1.Transform);

            caja1.Render();

            personaje.Render();

            if (BoundingBox)
            {
                caja1Mesh.RenderizaRayos();
                
                caja1.BoundingBox.Render();
            }

            escenarioActual.Render();

            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        private void CalcularMovimiento()
        {
            if (personaje.moving)
            {
                //personaje.playAnimation("Caminando", true); // esto creo que esta mal, si colisiono no deberia caminar.

                if (ChocoConLimite(personaje, escenarioActual.planoIzq))
                    NoMoverHacia(Key.A);

                if (ChocoConLimite(personaje, escenarioActual.planoBack))
                {
                    NoMoverHacia(Key.S);
                    escenarioActual.planoBack.BoundingBox.setRenderColor(Color.AliceBlue);
                }
                else
                { // esto no hace falta despues
                    escenarioActual.planoBack.BoundingBox.setRenderColor(Color.Yellow);
                }

                if (ChocoConLimite(personaje, escenarioActual.planoDer))
                    NoMoverHacia(Key.D);

                if (ChocoConLimite(personaje, escenarioActual.planoFront))
                { // HUBO CAMBIO DE ESCENARIO
                  /* Aca deberiamos hacer algo como no testear mas contra las cosas del escenario anterior y testear
                    contra las del escenario actual. 
                  */

                    escenarioActual.planoFront.BoundingBox.setRenderColor(Color.AliceBlue);
                }
                else
                {
                    escenarioActual.planoFront.BoundingBox.setRenderColor(Color.Yellow);
                }

                if (ChocoConLimite(personaje, escenarioActual.planoPiso))
                {
                    if (movimiento.Y < 0)
                        {
                            movimiento.Y = 0; // Ojo, que pasa si quiero saltar desde arriba de la plataforma?
                            personaje.ColisionoEnY();
                        }
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

        private bool ChocoConLimite(Personaje personaje, TgcMesh plano) {
            return TgcCollisionUtils.testAABBAABB(plano.BoundingBox, personaje.BoundingBox);
        }

        private void CalcularColisiones() {
            if (personaje.moving)
            {
                foreach (MeshTipoCaja caja in meshesColisionables)
                {
                    if (caja.ChocoConFrente(personaje))
                    {
                        movimientoCaja = TGCMatrix.Translation(0, 0, movimiento.Z*3); // + distancia minima del rayo
                        break;
                    }
                    else if (caja.ChocoAtras(personaje))
                    {
                        NoMoverHacia(Key.S);
                        break;
                    }
                    else if (caja.ChocoALaIzquierda(personaje))
                    {
                        NoMoverHacia(Key.D);
                        break;
                    }
                    else if (caja.ChocoALaDerecha(personaje))
                    {
                        NoMoverHacia(Key.A);
                        break;
                    }
                    else if (caja.ChocoArriba(personaje))
                    {
                        if (movimiento.Y < 0)
                        {
                            movimiento.Y = 0; // Ojo, que pasa si quiero saltar desde arriba de la plataforma?
                            personaje.ColisionoEnY();
                        }
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
            //escenarioActual.DisposeAll();
            personaje.Dispose();
            escenarioActual.planoIzq.Dispose(); // solo se borran los originales
            escenarioActual.planoFront.Dispose(); // solo se borran los originales
            escenarioActual.planoPiso.Dispose();
            caja1.Dispose();

            //foreach (TgcMesh mesh in meshesColisionables) {
            //    mesh.Dispose(); // mmm, no se que pasaria con las instancias...
            //} // recontra TODO
        }
    }
}