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
        private TGCMatrix auxUltimaPos;
        private ArrayList cajasPlaya;

        // Planos de limite
        private TgcMesh planoIzq;
        private TgcMesh planoDer;
        private TgcMesh planoFront;
        private TgcMesh planoBack;

        private TgcArrow segment = new TgcArrow();
        private bool colisionoContraLimite;
        private bool colisionoContraMesh;
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

            cajasPlaya = new ArrayList();

            cajasPlaya.Add(caja1);

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
            ultimaPos = TGCMatrix.Translation(personaje.Position);
            auxUltimaPos = TGCMatrix.Identity; //TGCMatrix.Translation(personaje.Position);

            colisionoContraLimite = true;
            colisionoContraMesh = true;

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

            auxUltimaPos = ultimaPos;

            CalcularMovimiento();

            if (colisionoContraLimite)
            {
                ultimaPos = auxUltimaPos;

                CalcularColisiones();

                if (colisionoContraMesh)
                {
                    ultimaPos = auxUltimaPos;
                }

            }
            else
            {
                CalcularColisiones();

                if (colisionoContraMesh)
                {
                    ultimaPos = auxUltimaPos;
                }
                else {
                    ultimaPos *= TGCMatrix.Translation(movimiento);
                }
            }

            colisionoContraLimite = true;
            colisionoContraMesh = true;

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
                segment.Render();
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
                if (ChocoConLimite(personaje, planoIzq))
                {
                    if (ChocoConLimite(personaje, planoBack))
                    {
                        var noTeMuevasParaAtras = new TGCVector3(movimiento.X, movimiento.Y, 0);
                        NoMoverHacia(Key.A, noTeMuevasParaAtras);

                        planoBack.BoundingBox.setRenderColor(Color.AliceBlue);
                    }
                    else
                    {
                        planoBack.BoundingBox.setRenderColor(Color.Yellow);
                        NoMoverHacia(Key.A, movimiento);
                    }
                }
                else if (ChocoConLimite(personaje, planoDer))
                {
                    if (ChocoConLimite(personaje, planoBack))
                    {
                        var noTeMuevasParaAtras = new TGCVector3(movimiento.X, movimiento.Y, 0);
                        NoMoverHacia(Key.D, noTeMuevasParaAtras);
                        planoBack.BoundingBox.setRenderColor(Color.AliceBlue);
                    }
                    else
                    {
                        NoMoverHacia(Key.D, movimiento);
                        planoBack.BoundingBox.setRenderColor(Color.Yellow);
                    }
                }
                else if (ChocoConLimite(personaje, planoBack))
                {
                    NoMoverHacia(Key.S, movimiento);
                    planoBack.BoundingBox.setRenderColor(Color.AliceBlue);
                }
                else
                { // no hay colisiones contra los planos laterales
                    planoBack.BoundingBox.setRenderColor(Color.Yellow);
                    colisionoContraLimite = false;
                }

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

                personaje.playAnimation("Caminando", true); // esto creo que esta mal, si colisiono no deberia caminar.
            }
            else {
                colisionoContraLimite = false;
                personaje.playAnimation("Parado", true);
                //auxUltimaPos *= TGCMatrix.Translation(movimiento);
            }

            //if (moving)
            //{
            //    personaje.playAnimation("Caminando", true);
            //}
            //else
            //{
            //    personaje.playAnimation("Parado", true);
            //}

            camara.Target = personaje.Position;
        }

        private void NoMoverHacia(Key key, TGCVector3 vector) {
            switch(key)
            {
                case Key.A:
                    if (movimiento.X > 0) // los ejes estan al reves de como pensaba, o lo entendi mal.
                        auxUltimaPos *= TGCMatrix.Translation(0, vector.Y, vector.Z);
                    else
                        auxUltimaPos *= TGCMatrix.Translation(vector);
                    break;
                case Key.D:
                    if (movimiento.X < 0)
                        auxUltimaPos *= TGCMatrix.Translation(0, vector.Y, vector.Z);
                    else
                        auxUltimaPos *= TGCMatrix.Translation(vector);
                    break;
                case Key.S:
                    if (movimiento.Z > 0)
                        auxUltimaPos *= TGCMatrix.Translation(vector.X, vector.Y, 0);
                    else
                        auxUltimaPos *= TGCMatrix.Translation(vector);
                    break;
                case Key.W:
                    if (movimiento.Z < 0)
                        auxUltimaPos *= TGCMatrix.Translation(vector.X, vector.Y, 0);
                    else
                        auxUltimaPos *= TGCMatrix.Translation(vector);
                    break;
            }
                
        }

        private bool ChocoConLimite(TgcSkeletalMesh personaje, TgcMesh planoIzq) {
            return TgcCollisionUtils.testAABBAABB(planoIzq.BoundingBox, personaje.BoundingBox);
        }

        private void CalcularColisiones() {
            if (moving)
            {
                foreach (TgcMesh caja in cajasPlaya)
                {
                    // calcular el centro de cada cara, para armar un rayo y saber contra que cara estas colisionando.
                    // si hubo colision con la cara de adelante, no deberias poder avanzar y deberias poder empujarla
                    // si hubo colision con cara del costado, no podrias moverte hacia el costado
                    // si hubo colision de espalda, no deberias poder moverte hacia atras.
                    //mesh.BoundingBox.calculateBoxCenter

                    var velocidadCaminar = VELOCIDAD_DESPLAZAMIENTO * ElapsedTime;

                    var rayos = ArmarRayosEnCadaCara(caja); // quizas esto de armar los rayos pueda hacerse en el init asi no se hace en cada update, por ahora lo dejo aca.

                    var puntoInterseccion = TGCVector3.Empty;

                    if (TgcCollisionUtils.intersectRayAABB((TgcRay)rayos[0], personaje.BoundingBox, out puntoInterseccion))
                    {
                        NoMoverHacia(Key.S, movimiento);
                        //personaje.playAnimation("Parado", true);
                    } // esta parado encima de la caja

                    // No testeo en direccion -y porque no podrias estar abajo de la caja

                    var rayoZ = (TgcRay)rayos[2];

                    segment = TgcArrow.fromDirection(rayoZ.Origin, new TGCVector3(0, 0, 10));

                    if (TgcCollisionUtils.intersectRayAABB(rayoZ, personaje.BoundingBox, out puntoInterseccion))
                    {
                        //var bb = personaje.BoundingBox;
                        //var centroBB = bb.calculateBoxCenter();
                        //var ZCaraDelanteraPersonajeAlturaRayo = (new TGCVector3(centroBB.X, rayoZ.Origin.Y, centroBB.Z - (FastMath.Abs(bb.PMax.Z - bb.PMin.Z) / 2))).Z;
                        if (FastMath.Abs(puntoInterseccion.Z - rayoZ.Origin.Z) < 5)
                        { // tenes que empujar la caja y moverla hacia adelante.
                            NoMoverHacia(Key.W, movimiento);
                            personaje.playAnimation("Empujar", true);
                            break; // sino caga el valor del flag
                        } else
                        {
                            colisionoContraMesh = false;
                            personaje.playAnimation("Caminando", true);
                        }
                    }
                    else
                    {
                        colisionoContraMesh = false;
                        personaje.playAnimation("Caminando", true);

                    }
                }
            }
            else {
                colisionoContraMesh = false;
                personaje.playAnimation("Parado", true);
            }
        }

        private ArrayList ArmarRayosEnCadaCara(TgcMesh meshTipoCaja) {
            // el orden es el mismo que retorna el metodo computeFaces de un BB, visto de frente (hacia -z) => Up, Down, Front, Back, Right, Left
            var rayos = new ArrayList();

            // Solucion casera del centro...
            //var PMax = meshTipoCaja.BoundingBox.PMax;
            //var PMin = meshTipoCaja.BoundingBox.PMin;
            //var centro = new TGCVector3((PMax.X + PMin.X) / 2, (PMax.Y + PMin.Y) / 2, (PMax.Z + PMin.Z) / 2);
            //

            var centro = meshTipoCaja.BoundingBox.calculateBoxCenter();

            var centroCaraX = HallarCentroDeCara(meshTipoCaja, centro, "x");
            var centroCaraMenosX = HallarCentroDeCara(meshTipoCaja, centro, "-x");
            var centroCaraY = HallarCentroDeCara(meshTipoCaja, centro, "y");
            var centroCaraMenosY = HallarCentroDeCara(meshTipoCaja, centro, "-y");
            var centroCaraZ = HallarCentroDeCara(meshTipoCaja, centro, "z");
            var centroCaraMenosZ = HallarCentroDeCara(meshTipoCaja, centro, "-z");

            var rayoCaraX = new TgcRay(centroCaraX, new TGCVector3(1,0,0));
            var rayoCaraMenosX = new TgcRay(centroCaraMenosX, new TGCVector3(-1, 0, 0));
            var rayoCaraY = new TgcRay(centroCaraY, new TGCVector3(0, 1, 0));
            var rayoCaraMenosY = new TgcRay(centroCaraMenosY, new TGCVector3(0, -1, 0));
            var rayoCaraZ = new TgcRay(centroCaraZ, new TGCVector3(0, 0, 1));
            var rayoCaraMenosZ = new TgcRay(centroCaraMenosZ, new TGCVector3(0, 0, -1));

            rayos.Add(rayoCaraY); // Up
            rayos.Add(rayoCaraMenosY); // Down
            rayos.Add(rayoCaraZ); // Front
            rayos.Add(rayoCaraMenosZ); // Back
            rayos.Add(rayoCaraMenosX); // Right
            rayos.Add(rayoCaraX); // Left

            return rayos;
        }

        private TGCVector3 HallarCentroDeCara(TgcMesh meshTipoCaja, TGCVector3 centro, String dirCara)
        { // le pasas el centro para no tener que calcularlo cada vez que entras aca. en dirCara quise no pasarle un string, pero no anduvo con TGCVector3
            var PMin = meshTipoCaja.BoundingBox.PMin;
            var PMax = meshTipoCaja.BoundingBox.PMax;
            
            switch (dirCara)
            {
                case "x":
                    return new TGCVector3(centro.X + (FastMath.Abs(PMax.X - PMin.X) / 2), centro.Y, centro.Z);
                case "-x":
                    return new TGCVector3(centro.X - (FastMath.Abs(PMax.X - PMin.X) / 2), centro.Y, centro.Z);
                case "y":
                    return new TGCVector3(centro.X, centro.Y + (FastMath.Abs(PMax.Y - PMin.Y) / 2), centro.Z);
                case "-y":
                    return new TGCVector3(centro.X, centro.Y - (FastMath.Abs(PMax.Y - PMin.Y) / 2), centro.Z);
                case "z":
                    return new TGCVector3(centro.X, centro.Y, centro.Z + (FastMath.Abs(PMax.Z - PMin.Z) / 2));
                case "-z":
                    return new TGCVector3(centro.X, centro.Y, centro.Z - (FastMath.Abs(PMax.Z - PMin.Z) / 2));
                default:
                    throw new Exception("direccion invalida");
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

            foreach (TgcMesh mesh in cajasPlaya) {
                mesh.Dispose(); // mmm, no se que pasaria con las instancias...
            }
        }
    }
}