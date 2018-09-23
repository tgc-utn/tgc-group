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
        private TgcScene scene;
        private TgcSkeletalMesh personaje;
        private GameCamera camara;
        private TGCMatrix desplazamiento;
        //private TgcPlane plano;
        private TgcMesh planoIzq;
        private TgcMesh planoDer;
        private TgcMesh planoFront;
        private TgcMesh planoBack;
        //private TgcBoundingAxisAlignBox nuevoBB;


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
            scene = loader.loadSceneFromFile(MediaDir + "primer-nivel\\pozo-plataformas\\tgc-scene\\plataformas\\plataformas-TgcScene.xml");

            //scene.Meshes.ForEach((mesh) => { Console.WriteLine(mesh.Name); });

            //plano = new TgcPlane(new TGCVector3(19, -3, 300),
            //                     new TGCVector3(100, 100, -1000),
            //                     TgcPlane.Orientations.YZplane,
            //                     TgcTexture.createTexture(MediaDir + "primer-nivel\\texturas\\barro2.jpg"), 0, 0);

            //plano = new TGCPlane(19, -3, 300, 0);

            //planoIzq = TGCBox.fromSize(new TGCVector3(0, 10, -20), new TGCVector3(15, 15, 15), Color.Violet);

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
            //planoFront.Move(50,0,-65);
            // quise crear una instancia de planoIzq y rotarla pero no hubo forma de rotar el BB, segun un mail, esta hecho para que no rote, tambien probe calcularlo de nuevo y mostrarlo, pero no renderiza.


            //planoFront.AutoUpdateBoundingBox = true;
            //planoFront.BoundingBox.transform(TGCMatrix.RotationY(FastMath.PI_HALF));
            //rotacionFront = TGCMatrix.RotationX(FastMath.PI_HALF/*TGCMatrix.RotationX(FastMath.PI_HALF)*/);
            //planoFront.UpdateMeshTransform();
            //nuevoBB = planoFront.createBoundingBox();
            //planoFront.updateBoundingBox();

            planoBack = planoFront.createMeshInstance("planoBack");
            planoBack.AutoTransform = false;
            planoBack.Transform = TGCMatrix.Translation(0, 0, 135);
            planoBack.BoundingBox.transform(planoBack.Transform);
            //planoBack.Move(0, 0, 135); // por que el mov. de este es relativo al del otro? no son instancias separadas ? 


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

            camara = new GameCamera(personaje.Position, 100, 200);
            //var cameraPosition = new TGCVector3(0, 0, 200);
            //Quiero que la camara mire hacia el origen (0,0,0).
            //var lookAt = TGCVector3.Empty;
            //Configuro donde esta la posicion de la camara y hacia donde mira.
            //Camara.SetCamera(cameraPosition, lookAt);
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

          

            //box.Transform =
            //    TGCMatrix.Scaling(box.Scale)
            //                * TGCMatrix.RotationYawPitchRoll(box.Rotation.Y, box.Rotation.X, box.Rotation.Z)
            //                * TGCMatrix.Translation(box.Position);

            //planoIzq.Render();
            //planoFront.Transform = rotacionFront;
            //planoFront.updateBoundingBox();
            //planoFront.UpdateMeshTransform();
            //nuevoBB.Render();
            planoBack.BoundingBox.Render();
            planoFront.BoundingBox.Render();
            planoIzq.BoundingBox.Render();
            planoDer.BoundingBox.Render();


            personaje.Transform =
                TGCMatrix.Scaling(personaje.Scale)
                            * TGCMatrix.RotationYawPitchRoll(personaje.Rotation.Y, personaje.Rotation.X, personaje.Rotation.Z)
                            * TGCMatrix.Translation(personaje.Position);
                            //* desplazamiento;


            personaje.animateAndRender(ElapsedTime);

            personaje.BoundingBox.Render();

            scene.RenderAll();

            //Siempre antes de renderizar el modelo necesitamos actualizar la matriz de transformacion.
            //Debemos recordar el orden en cual debemos multiplicar las matrices, en caso de tener modelos jerárquicos, tenemos control total.
            //Box.Transform = TGCMatrix.Scaling(Box.Scale) * TGCMatrix.RotationYawPitchRoll(Box.Rotation.Y, Box.Rotation.X, Box.Rotation.Z) * TGCMatrix.Translation(Box.Position);


            //A modo ejemplo realizamos toda las multiplicaciones, pero aquí solo nos hacia falta la traslación.
            //Finalmente invocamos al render de la caja
            //Box.Render();

            //Cuando tenemos modelos mesh podemos utilizar un método que hace la matriz de transformación estándar.
            //Es útil cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones jerárquicas o complicadas.
            //Mesh.UpdateMeshTransform();
            //Render del mesh
            //Mesh.Render();

            //Render de BoundingBox, muy útil para debug de colisiones.
            if (BoundingBox)
            {
                //Box.BoundingBox.Render();
                //Mesh.BoundingBox.Render();
            }

            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        private void CalcularMovimiento()
        {
            var velocidadCaminar = VELOCIDAD_DESPLAZAMIENTO * ElapsedTime;

            //Calcular proxima posicion de personaje segun Input
            var moving = false;
            var movement = TGCVector3.Empty;
            
            if (Input.keyDown(Key.W))
            {
                movement.Z = -velocidadCaminar;
                moving = true;
            }

            if (Input.keyDown(Key.S))
            {
                movement.Z = velocidadCaminar;
                moving = true;
            }

            if (Input.keyDown(Key.D))
            {
                movement.X = -velocidadCaminar;
                moving = true;
            }

            if (Input.keyDown(Key.A))
            {
                movement.X = velocidadCaminar;
                moving = true;
            }

            var current_pos = personaje.Position;

            //desplazamiento = TGCMatrix.Translation(movement.X, movement.Y, movement.Z);
            personaje.Move(movement);

            //if (TgcCollisionUtils.testAABBAABB(planoIzq.BoundingBox, personaje.BoundingBox))
            if(ChocoConLimite(personaje, planoIzq) || ChocoConLimite(personaje, planoDer))
            {
                //plano.Color = Color.Red;
                //plano.updateValues();

                //desplazamiento = TGCMatrix.Translation(current_pos.X, current_pos.Y, current_pos.Z);

                //personaje.Position = current_pos;
                
                personaje.Move(-movement.X, -movement.Y, -movement.Z);
                
                //planoIzq.updateValues();
            }
            else
            {
                //plano.Color = Color.Violet;
                //plano.updateValues();
                //Console.WriteLine("NO COLISIONNNNNNN");
                //planoIzq.updateValues();
               
                
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

                //Aplicar movimiento, internamente suma valores a la posicion actual del mesh.
             
            }
            else
            {
                personaje.playAnimation("Parado", true);
            }

            camara.Target = personaje.Position;
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
            scene.DisposeAll();
            personaje.Dispose();
            planoIzq.Dispose();
            planoDer.Dispose();
        }
    }
}