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
        private TGCMatrix movimientoCaja;
        private TGCMatrix auxMovimientoCaja;

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
            caja1.AutoTransform = false;
            movimientoCaja = TGCMatrix.Translation(caja1.Position);

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
            ultimaPos = TGCMatrix.Translation(new TGCVector3(10, 0, 60));
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

            auxUltimaPos = TGCMatrix.Identity; //CopiarMatriz4x4(ultimaPos);
            //auxUltimaPos = ultimaPos; // tengo que pasarle una copia de ultimaPos, no la referencia, sino al modificar auxUltimaPos se modifica ultimaPos y visceversa
            auxMovimientoCaja = TGCMatrix.Identity;

            CalcularMovimiento();

            if (colisionoContraLimite)
            {
                ultimaPos *= auxUltimaPos; 
                //CopiarMatriz4x4(auxUltimaPos, ultimaPos);

                CalcularColisiones();

                if (colisionoContraMesh)
                {
                    ultimaPos *= auxUltimaPos;

                    movimientoCaja *= auxMovimientoCaja; //TGCMatrix.Translation(0, 0, movimiento.Z);
                    //CopiarMatriz4x4(auxUltimaPos, ultimaPos);
                }
            }
                
            else
            {
                CalcularColisiones();

                if (colisionoContraMesh)
                {
                    ultimaPos *= auxUltimaPos;
                    // SI NO ASIGNAS IDENTITY PASA ESTO: 
                    //el problema es que en esta linea de arriba la transformacion en Z de auxUltimaPos (M43) deberia ser cero, pero es igual al valor de ultimaPos
                    // SI ASIGNAS IDENTITY PASA ESTO (como esta ahora): 
                    //auxUltimaPos tiene bien el M43 en 0, pero ultimaPos ya tiene el valor nuevo de Z, y eso no deberia pasar, deberia tener el viejo...

                    movimientoCaja *= auxMovimientoCaja;
                    //CopiarMatriz4x4(auxUltimaPos, ultimaPos);
                }
                else {
                    // Ahora esta entrando aca luego de que colisiona con la mesh, 
                    // como que detecta la colision solo una vez, pero si se queda en el lugar ya no la detecta, 
                    // la vuelve a detectar cuando se mueve, pero ya es tarde porque ya paso por aca 
                    // y el valor de ultimaPos ya quedo actualizado, por eso pasa lo que pasa.
                    // Igual creo que esta cayendo aca porque en el debuggeo en un momento entre testeo y testeo deja de moverse posta, pero
                    // lo que no entiendo es por que pasa cuando lo corres normal
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
                
                //personaje.playAnimation("Parado", true);
                
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

                    // Armar los rayos de cada cara
                    var meshTipoCajaConRayos = new MeshTipoCaja(caja);

                    colisionoContraMesh = meshTipoCajaConRayos.ChocoConFrente(personaje);

                    if (colisionoContraMesh)
                    {
                        auxMovimientoCaja = TGCMatrix.Translation(0, 0, movimiento.Z);
                        personaje.playAnimation("Empujar", true);
                        break;
                    }
                    else {
                        auxMovimientoCaja = TGCMatrix.Translation(0,0,0);
                        personaje.playAnimation("Caminando", true);
                    }
                }
            }
            else {
                colisionoContraMesh = false;
                personaje.playAnimation("Parado", true);

            }
        }

        //private void ColisionConRayo(TgcRay rayo, Key key, String eje, TGCVector3 movimientoMesh) {
        //    var puntoInterseccion = TGCVector3.Empty;

        //    switch (eje) {
        //        case "x":

        //    }

        //    if (TgcCollisionUtils.intersectRayAABB(rayo, personaje.BoundingBox, out puntoInterseccion))
        //    {
        //        if (FastMath.Abs(puntoInterseccion.X - rayo.Origin.X) < 1)
        //        { // tenes que empujar la mesh y moverla hacia adelante.
        //            NoMoverHacia(key, movimiento);
        //            personaje.playAnimation("Parado", true);
        //            auxMovimientoCaja = TGCMatrix.Translation(0, 0, 0);
        //        }
        //        else
        //        {
        //            colisionoContraMesh = false;
        //            personaje.playAnimation("Caminando", true);
        //        }

        //    } // Esta a arriba del mesh
        //}

        

        

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