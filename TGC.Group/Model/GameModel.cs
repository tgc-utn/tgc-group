using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.SkeletalAnimation;
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
        private TgcSkeletalMesh personaje;
        private TgcThirdPersonCamera camaraInterna;
        private Calculos calculo = new Calculos();
        private TGCVector3 velocidad = TGCVector3.Empty;
        private TGCVector3 aceleracion = new TGCVector3(0, -100f, 0);
        //Para saber si ya salto
        private bool firstTime = true;
        //Para saber si esta en tierra
        private bool OnGround = true;
        
        //Define direccion del mesh del personaje dependiendo el movimiento
        private Personaje dirPers = new Personaje();

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        //Caja que se muestra en el ejemplo.
        private TGCBox Box { get; set; }

        //Boleano para ver si dibujamos el boundingbox
        private bool BoundingBox { get; set; }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aquí todo el código de inicialización: cargar modelos, texturas, estructuras de optimización, todo
        ///     procesamiento que podemos pre calcular para nuestro juego.
        ///     Borrar el codigo ejemplo no utilizado.
        /// </summary>
        public override void Init()
        {
            dirPers.CreateDictionary();
            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;

            //Textura de la carperta Media. Game.Default es un archivo de configuracion (Game.settings) util para poner cosas.
            //Pueden abrir el Game.settings que se ubica dentro de nuestro proyecto para configurar.
            var pathTexturaCaja = MediaDir + Game.Default.TexturaCaja;

            //Cargamos una textura, tener en cuenta que cargar una textura significa crear una copia en memoria.
            //Es importante cargar texturas en Init, si se hace en el render loop podemos tener grandes problemas si instanciamos muchas.
            var texture = TgcTexture.createTexture(pathTexturaCaja);

            //Creamos una caja 3D ubicada de dimensiones (5, 10, 5) y la textura como color.
            var size = new TGCVector3(20, 30, 40);
            //Construimos una caja según los parámetros, por defecto la misma se crea con centro en el origen y se recomienda así para facilitar las transformaciones.
            Box = TGCBox.fromSize(size, texture);
            //Posición donde quiero que este la caja, es común que se utilicen estructuras internas para las transformaciones.
            //Entonces actualizamos la posición lógica, luego podemos utilizar esto en render para posicionar donde corresponda con transformaciones.
            Box.Position = new TGCVector3(-25, 0, 0);

            //Cargo el unico mesh que tiene la escena.



            var skeletalLoader = new TgcSkeletalLoader();
            personaje =
                skeletalLoader.loadMeshAndAnimationsFromFile(
                    MediaDir + "Media\\SkeletalAnimations\\Robot\\Robot-TgcSkeletalMesh.xml",
                    MediaDir + "Media\\SkeletalAnimations\\Robot\\",
                    new[]
                    {
                        MediaDir + "Media\\SkeletalAnimations\\Robot\\Caminando-TgcSkeletalAnim.xml",
                        MediaDir + "Media\\SkeletalAnimations\\Robot\\Parado-TgcSkeletalAnim.xml"
                    });

            //Configurar animacion inicial
            personaje.playAnimation("Parado", true);
            //Posicion inicial
            personaje.Position = new TGCVector3(0, 0, 0);
            //No es recomendado utilizar autotransform en casos mas complicados, se pierde el control.
            personaje.AutoTransform = true;
            //Rotar al robot en el Init para que mire hacia el otro lado
            personaje.RotateY(3.1415f);
            //Le cambiamos la textura para diferenciarlo un poco
            personaje.changeDiffuseMaps(new[]
            {
                TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Media\\SkeletalAnimations\\Robot\\Textures\\uvw.jpg")
            });
            //Prueba de agregar al robot
            // Mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + /*"LogoTGC-TgcScene.xml"*/"Media\\ModelosTgc\\Robot\\Robot-TgcScene.xml").Meshes[0];

            //Defino una escala en el modelo logico del mesh que es muy grande.
            // Mesh.Scale = new TGCVector3(0.4f, 0.4f, 0.4f);

            //Suelen utilizarse objetos que manejan el comportamiento de la camara.
            //Lo que en realidad necesitamos gráficamente es una matriz de View.
            //El framework maneja una cámara estática, pero debe ser inicializada.
            //Posición de la camara.

            camaraInterna = new TgcThirdPersonCamera(personaje.Position, 200, -900);
            //Quiero que la camara mire hacia el origen (0,0,0).
            var lookAt = TGCVector3.Empty;
            //Configuro donde esta la posicion de la camara y hacia donde mira.
            Camara = camaraInterna;
            //Internamente el framework construye la matriz de view con estos dos vectores.
            //Luego en nuestro juego tendremos que crear una cámara que cambie la matriz de view con variables como movimientos o animaciones de escenas.
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        public override void Update()
        {
            PreUpdate();

            var velocidadCaminar = 300f;
            var velocidadCaminarID = 300f;
            var velocidadSalto = 200f;
            var coeficienteDeSalto = 0.2f;


            //Capturar Input teclado
            if (Input.keyPressed(Key.F))
            {
                BoundingBox = !BoundingBox;
            }

            var moveForward = 0f;
            var moveID = 0f;
            var moving = false;
            var movingID = false;

            //Adelante
            if (Input.keyDown(Key.W))
            {
                moveForward = -velocidadCaminar;
                moving = true;
                personaje.RotateY(dirPers.RotationAngle(Key.W));
                dirPers.turnForward();
            }

            //Atras
            if (Input.keyDown(Key.S))
            {
                moveForward = -velocidadCaminar;
                moving = true;
                personaje.RotateY(dirPers.RotationAngle(Key.S));
                dirPers.turnBackwards();
            }

            //Derecha
            if (Input.keyDown(Key.D))
            {
                moveID = velocidadCaminarID;
                movingID = true;
                personaje.RotateY(dirPers.RotationAngle(Key.D));
                dirPers.turnRight();
            }

            //Izquierda
            if (Input.keyDown(Key.A))
            {
                moveID = -velocidadCaminarID;
                movingID = true;
                personaje.RotateY(dirPers.RotationAngle(Key.A));
                dirPers.turnLeft();
            }

            if (personaje.Position.Y <= 0f)
            {
                OnGround = true;
                firstTime = true;
            }

            //Saltar
            if (Input.keyPressed(Key.Space))
            {
                if (OnGround)
                {
                    velocidad.Y = velocidadSalto;
                    OnGround = false;
                    firstTime = false;
                }
            }

            var perPos = personaje.Position;


            if (!firstTime)
            {
                velocidad = velocidad + ElapsedTime * aceleracion;
                perPos = perPos + velocidad * ElapsedTime;
                perPos.Y -= coeficienteDeSalto;

                personaje.Move(perPos - personaje.Position);
 
            }

            //Si se genera algún movimiento, se activa la animacíón
            if(moving || movingID) personaje.playAnimation("Caminando", true);
            else personaje.playAnimation("Parado", true);

            //Movimiento adelante-atrás
            if (moving) personaje.MoveOrientedY(moveForward * ElapsedTime);
            //Movimiento Izq-Der
            if (movingID) personaje.Move(new TGCVector3(moveID, 0f,0f) * ElapsedTime);
   
            //Camara sigue al personaje
            camaraInterna.Target = personaje.Position;
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
            DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.GhostWhite);
            DrawText.drawText("Con clic izquierdo subimos la camara [Actual]: " + TGCVector3.PrintVector3(Camara.Position), 0, 30, Color.GhostWhite);
           
            //Siempre antes de renderizar el modelo necesitamos actualizar la matriz de transformacion.
            //Debemos recordar el orden en cual debemos multiplicar las matrices, en caso de tener modelos jerárquicos, tenemos control total.
            Box.Transform = TGCMatrix.Scaling(Box.Scale) * TGCMatrix.RotationYawPitchRoll(Box.Rotation.Y, Box.Rotation.X, Box.Rotation.Z) * TGCMatrix.Translation(Box.Position);
            //A modo ejemplo realizamos toda las multiplicaciones, pero aquí solo nos hacia falta la traslación.
            //Finalmente invocamos al render de la caja
            Box.Render();

            //Cuando tenemos modelos mesh podemos utilizar un método que hace la matriz de transformación estándar.
            //Es útil cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones jerárquicas o complicadas.
           // Mesh.UpdateMeshTransform();
            //Render del mesh
            //Mesh.Render();

            //Render de BoundingBox, muy útil para debug de colisiones.
            if (BoundingBox)
            {
                Box.BoundingBox.Render();
          
            }

            personaje.animateAndRender(ElapsedTime);
            if (BoundingBox)
            {
                personaje.BoundingBox.Render();
            }


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
            //Dispose de la caja.
            Box.Dispose();
            personaje.Dispose();
            //Dispose del mesh.
         //   Mesh.Dispose();
        }
    }
}