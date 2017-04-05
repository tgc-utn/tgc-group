using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.Utils;
using TGC.Camara;
using TGC.Core.Collision;
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

        //Velocidad de movimiento del auto
        private const float MOVEMENT_SPEED = 200f;

        //Cantidad de filas
        private const int ROWS = 5;

        //Cantidad de columnas
        private const int COLUMNS = 5;

        //Tamaño cuadrante
        private const int CUADRANTE_SIZE = 400;

        //Scene principal
        private TgcScene ScenePpal;

        //Mesh del auto
        private TgcMesh Mesh { get; set; }

        //Boleano para ver si dibujamos el boundingbox
        private bool BoundingBox { get; set; }

        //Camara en tercera persona
        private TgcThirdPersonCamera CamaraInterna;

        //Tipo de cámara
        private int TipoCamara = 0;

        //Lista de palmeras
        private List<TgcMesh> MeshPalmeras;
        private TgcMesh PalmeraOriginal;

        //Lista de pinos
        private List<TgcMesh> MeshPinos;
        private TgcMesh PinoOriginal;

        //Lista de rocas
        private List<TgcMesh> MeshRocas;
        private TgcMesh RocaOriginal;

        //Lista de rocas
        private List<TgcMesh> MeshArbolesBananas;
        private TgcMesh ArbolBananasOriginal;

        //Lista de barriles de polvora
        private List<TgcMesh> MeshBarrilesPolvora;
        private TgcMesh BarrilPolvoraOriginal;

        //Lista de carretillas
        private List<TgcMesh> MeshCarretillas;
        private TgcMesh CarretillaOriginal;

        //Lista de contenedores
        private List<TgcMesh> MeshContenedores;
        private TgcMesh ContenedorOriginal;

        //Lista de fuentes de agua
        private List<TgcMesh> MeshFuentesAgua;
        private TgcMesh FuenteAguaOriginal;

        //Lista de lockers
        private List<TgcMesh> MeshLockers;
        private TgcMesh LockerOriginal;

        //Lista de expendedores bebidas
        private List<TgcMesh> MeshExpendedoresBebidas;
        private TgcMesh ExpendedorBebidaOriginal;

        //Lista de cajas de municiones
        private List<TgcMesh> MeshCajasMuniciones;
        private TgcMesh CajaMunicionesOriginal;

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

            //Cargo el terreno
            ScenePpal = loader.loadSceneFromFile(MediaDir + "MAPA-TgcScene.xml");

            //Cargo el auto
            var SceneAuto = loader.loadSceneFromFile(MediaDir + "Vehiculos\\Auto\\Auto-TgcScene.xml");

            //Movemos el escenario un poco para arriba para que se pueda mover el auto
            Mesh = SceneAuto.Meshes[0];
            Mesh.AutoTransformEnable = true;
            Mesh.move(0, 0.05f, 0);

            //Camara por defecto
            CamaraInterna = new TgcThirdPersonCamera(Mesh.Position, 300, 400);
            Camara = CamaraInterna;

            //Creo los objetos del escenario
            CrearObjetos(loader);           

        }

        private void CrearObjetos(TgcSceneLoader loader)
        {
            int[,] MatrizPoblacion;

            //Creo palmeras
            MatrizPoblacion = RandomMatrix();
            PalmeraOriginal = loader.loadSceneFromFile(MediaDir + "Vegetacion\\Palmera\\Palmera-TgcScene.xml").Meshes[0];
            MeshPalmeras = CrearInstancias(PalmeraOriginal, 0.75f, 0, 2, MatrizPoblacion);

            //Creo pinos
            MatrizPoblacion = RandomMatrix();
            PinoOriginal = loader.loadSceneFromFile(MediaDir + "Vegetacion\\Pino\\Pino-TgcScene.xml").Meshes[0];
            MeshPinos = CrearInstancias(PinoOriginal, 0.90f, 0, 1, MatrizPoblacion);

            //Creo rocas
            MatrizPoblacion = RandomMatrix();
            RocaOriginal = loader.loadSceneFromFile(MediaDir + "Vegetacion\\Roca\\Roca-TgcScene.xml").Meshes[0];
            MeshRocas = CrearInstancias(RocaOriginal, 0.75f, 0, 1, MatrizPoblacion);

            //Creo arboles bananas
            MatrizPoblacion = RandomMatrix();
            ArbolBananasOriginal = loader.loadSceneFromFile(MediaDir + "Vegetacion\\ArbolBananas\\ArbolBananas-TgcScene.xml").Meshes[0];
            MeshArbolesBananas = CrearInstancias(ArbolBananasOriginal, 1.50f, 0, 2, MatrizPoblacion);

            //Creo barriles de polvora
            MatrizPoblacion = RandomMatrix();
            BarrilPolvoraOriginal = loader.loadSceneFromFile(MediaDir + "Objetos\\BarrilPolvora\\BarrilPolvora-TgcScene.xml").Meshes[0];
            MeshBarrilesPolvora = CrearInstancias(BarrilPolvoraOriginal, 0.75f, 0, 1, MatrizPoblacion);

            //Creo carretillas
            MatrizPoblacion = RandomMatrix();
            CarretillaOriginal = loader.loadSceneFromFile(MediaDir + "Objetos\\Carretilla\\Carretilla-TgcScene.xml").Meshes[0];
            MeshCarretillas = CrearInstancias(CarretillaOriginal, 0.20f, 0, 1, MatrizPoblacion);

            //Creo contenedores
            MatrizPoblacion = RandomMatrix();
            ContenedorOriginal = loader.loadSceneFromFile(MediaDir + "Objetos\\Contenedor\\Contenedor-TgcScene.xml").Meshes[0];
            MeshContenedores = CrearInstancias(ContenedorOriginal, 1.5f, 0, 1, MatrizPoblacion);

            //Creo fuentes de agua
            MatrizPoblacion = RandomMatrix();
            FuenteAguaOriginal = loader.loadSceneFromFile(MediaDir + "Objetos\\FuenteAgua\\FuenteAgua-TgcScene.xml").Meshes[0];
            MeshFuentesAgua = CrearInstancias(FuenteAguaOriginal, 1, 25, 1, MatrizPoblacion);

            //Creo lockers
            MatrizPoblacion = RandomMatrix();
            LockerOriginal = loader.loadSceneFromFile(MediaDir + "Muebles\\LockerMetal\\LockerMetal-TgcScene.xml").Meshes[0];
            MeshLockers = CrearInstancias(LockerOriginal, 1, 0, 1, MatrizPoblacion);

            //Creo expendedores bebidas
            MatrizPoblacion = RandomMatrix();
            ExpendedorBebidaOriginal = loader.loadSceneFromFile(MediaDir + "Muebles\\ExpendedorDeBebidas\\ExpendedorDeBebidas-TgcScene.xml").Meshes[0];
            MeshExpendedoresBebidas = CrearInstancias(ExpendedorBebidaOriginal, 0.50f, 0, 1, MatrizPoblacion);

            //Creo cajas de municiones
            MatrizPoblacion = RandomMatrix();
            CajaMunicionesOriginal = loader.loadSceneFromFile(MediaDir + "Armas\\CajaMuniciones\\CajaMuniciones-TgcScene.xml").Meshes[0];
            MeshCajasMuniciones = CrearInstancias(CajaMunicionesOriginal, 1, 0, 1, MatrizPoblacion);
        }

        private int[,] RandomMatrix()
        {
            int[,] MatrizPoblacion = new int[ROWS, COLUMNS];
            System.Random randomNumber = new System.Random();

            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLUMNS; j++)
                {
                    MatrizPoblacion[i, j] = randomNumber.Next(0, 2);
                }
            }

            return MatrizPoblacion;
        }

        //Crea instancias de un objeto
        private List<TgcMesh> CrearInstancias(TgcMesh unObjeto, float scale, float ejeZ, int cantidadObjetos, int[,] MatrizPoblacion)
        {
            List<TgcMesh> ListaMesh = new List<TgcMesh>();
            System.Random randomNumber = new System.Random();

            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLUMNS; j++)
                {
                    if (MatrizPoblacion[i, j] == 1)
                    {
                        for (int z = 0; z < cantidadObjetos; z++)
                        {
                            //Crear instancia de modelo
                            var instance = unObjeto.createMeshInstance(unObjeto.Name + i + "_" + j);

                            instance.AutoTransformEnable = false;


                            instance.Transform = Matrix.Scaling(new Vector3(scale, scale, scale)) *
                                                                        Matrix.Translation(new Vector3(1000, ejeZ, -1000));

                            instance.Transform = instance.Transform * Matrix.Translation(
                                                                        new Vector3((-1) * randomNumber.Next(j * CUADRANTE_SIZE, (j + 1) * CUADRANTE_SIZE), 0,
                                                                                    randomNumber.Next(i * CUADRANTE_SIZE, (i + 1) * CUADRANTE_SIZE)));

                            ListaMesh.Add(instance);
                        }
                    }
                }
            }

            return ListaMesh;
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        public override void Update()
        {
            PreUpdate();

            //Activo bounding box para debug
            ActivarBoundingBox();

            //Chequea si se solicitó cambiar el tipo de camara
            CambiarDeCamara();

            //Acciona la vista con el movimiento del mouse
            MoverCamaraConMouse();

            //Maneja el movimiento del auto teniendo en cuenta la posición de los otros objetos
            MoverAutoConColisiones();            
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
            DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.Red);
            DrawText.drawText("Con la tecla F1 se cambia el tipo de camara. Pos [Actual]: " + TgcParserUtils.printVector3(Camara.Position), 0, 30, Color.Red);

            //Siempre antes de renderizar el modelo necesitamos actualizar la matriz de transformacion.
            //Debemos recordar el orden en cual debemos multiplicar las matrices, en caso de tener modelos jerárquicos, tenemos control total.
            /*Box.Transform = Matrix.Scaling(Box.Scale) *
                            Matrix.RotationYawPitchRoll(Box.Rotation.Y, Box.Rotation.X, Box.Rotation.Z) *
                            Matrix.Translation(Box.Position);
            //A modo ejemplo realizamos toda las multiplicaciones, pero aquí solo nos hacia falta la traslación.
            //Finalmente invocamos al render de la caja
            Box.render();
            */
            //Cuando tenemos modelos mesh podemos utilizar un método que hace la matriz de transformación estándar.
            //Es útil cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones jerárquicas o complicadas.
            Mesh.UpdateMeshTransform();

            //Render del mesh
            Mesh.render();

            //Dibujamos la escena
            ScenePpal.renderAll();

            //Render de BoundingBox, muy útil para debug de colisiones.
            if (BoundingBox)
            {
                Mesh.BoundingBox.render();
            }

            foreach (var mesh in ScenePpal.Meshes)
            {
                mesh.BoundingBox.render();
            }

            //Renderizo los objetos cargados de las listas
            RenderizarObjetos();

            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        private void RenderizarObjetos()
        {
            //Renderizar palmeras
            foreach (var mesh in MeshPalmeras)
            {
                mesh.render();
            }

            //Renderizar rocas
            foreach (var mesh in MeshPinos)
            {
                mesh.render();
            }

            //Renderizar pinos
            foreach (var mesh in MeshRocas)
            {
                mesh.render();
            }

            //Renderizar pinos
            foreach (var mesh in MeshArbolesBananas)
            {
                mesh.render();
            }

            //Renderizar barriles de polvora
            foreach (var mesh in MeshBarrilesPolvora)
            {
                mesh.render();
            }

            //Renderizar Carretillas
            foreach (var mesh in MeshCarretillas)
            {
                mesh.render();
            }

            //Renderizar Contenedores
            foreach (var mesh in MeshContenedores)
            {
                mesh.render();
            }

            //Renderizar Fuentes de Agua
            foreach (var mesh in MeshFuentesAgua)
            {
                mesh.render();
            }

            //Renderizar Lockers
            foreach (var mesh in MeshLockers)
            {
                mesh.render();
            }

            //Renderizar Expendedores Bebidas
            foreach (var mesh in MeshExpendedoresBebidas)
            {
                mesh.render();
            }

            //Renderizar Cajas Municiones
            foreach (var mesh in MeshCajasMuniciones)
            {
                mesh.render();
            }
        }

        public void ActivarBoundingBox()
        {
            //Capturar Input teclado
            if (Input.keyPressed(Key.F))
            {
                //Activa o desactiva el Bounding Box
                BoundingBox = !BoundingBox;
            }
        }

        public void CambiarDeCamara()
        {
            if (Input.keyPressed(Key.F1))
            {
                TipoCamara++;

                if (TipoCamara >= 2)
                    TipoCamara = 0;

                switch (TipoCamara)
                {
                    case 0:
                        {
                            CamaraInterna = new TgcThirdPersonCamera(Mesh.Position, 200, 300);
                            Camara = CamaraInterna;
                        }
                        break;

                    case 1:
                        {
                            Camara = new TgcRotationalCamera(Mesh.BoundingBox.calculateBoxCenter(), Mesh.BoundingBox.calculateBoxRadius() * 2, Input);
                        }
                        break;
                }
            }
        }

        public void MoverCamaraConMouse()
        {
            //Capturar Input Mouse
            if (Input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Ver....
                /*
                //En este caso le sumamos un valor en Y
                Camara.SetCamera(Camara.Position + new Vector3(0, 10f, 0), Camara.LookAt);
                //Ver ejemplos de cámara para otras operaciones posibles.

                //Si superamos cierto Y volvemos a la posición original.
                if (Camara.Position.Y > 300f)
                {
                    Camara.SetCamera(new Vector3(Camara.Position.X, 0f, Camara.Position.Z), Camara.LookAt);
                }
                */
            }
        }
        
        public void MoverAutoConColisiones()
        {
            //Declaramos un vector de movimiento inicializado en cero.
            //El movimiento sobre el suelo es sobre el plano XZ.
            //Sobre XZ nos movemos con las flechas del teclado o con las letas WASD.
            var movement = new Vector3(0, 0, 0);

            //Movernos de izquierda a derecha, sobre el eje X.
            if (Input.keyDown(Key.Left) || Input.keyDown(Key.A))
            {
                movement.X = 1;
            }
            else if (Input.keyDown(Key.Right) || Input.keyDown(Key.D))
            {
                movement.X = -1;
            }

            //Movernos adelante y atras, sobre el eje Z.
            if (Input.keyDown(Key.Up) || Input.keyDown(Key.W))
            {
                movement.Z = -1;
            }
            else if (Input.keyDown(Key.Down) || Input.keyDown(Key.S))
            {
                movement.Z = 1;
            }

            //Guardar posicion original antes de cambiarla
            var originalPos = Mesh.Position;

            //Multiplicar movimiento por velocidad y elapsedTime
            movement *= MOVEMENT_SPEED * ElapsedTime;
            Mesh.move(movement);

            //El framework posee la clase TgcCollisionUtils con muchos algoritmos de colisión de distintos tipos de objetos.
            //Por ejemplo chequear si dos cajas colisionan entre sí, o dos esferas, o esfera con caja, etc.
            var collisionFound = false;

            foreach (var mesh in ScenePpal.Meshes)
            {
                //Los dos BoundingBox que vamos a testear
                var mainMeshBoundingBox = Mesh.BoundingBox;
                var sceneMeshBoundingBox = mesh.BoundingBox;

                //Ejecutar algoritmo de detección de colisiones
                var collisionResult = TgcCollisionUtils.classifyBoxBox(mainMeshBoundingBox, sceneMeshBoundingBox);

                //Hubo colisión con un objeto. Guardar resultado y abortar loop.
                if (collisionResult != TgcCollisionUtils.BoxBoxResult.Encerrando)
                {
                    collisionFound = true;
                    break;
                }
            }

            //Si hubo alguna colisión, entonces restaurar la posición original del mesh
            if (collisionFound)
            {
                Mesh.Position = originalPos;
            }

            //Hacer que la camara en 3ra persona se ajuste a la nueva posicion del objeto
            CamaraInterna.Target = Mesh.Position;
        }

        /// <summary>
        ///     Se llama cuando termina la ejecución del ejemplo.
        ///     Hacer Dispose() de todos los objetos creados.
        ///     Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        /// </summary>
        public override void Dispose()
        {
            PalmeraOriginal.dispose();
            PinoOriginal.dispose();
            RocaOriginal.dispose();
            ArbolBananasOriginal.dispose();
            BarrilPolvoraOriginal.dispose();
            CarretillaOriginal.dispose();
            ContenedorOriginal.dispose();
            FuenteAguaOriginal.dispose();
            LockerOriginal.dispose();
            ExpendedorBebidaOriginal.dispose();
            CajaMunicionesOriginal.dispose();
            ScenePpal.disposeAll();
            Mesh.dispose();
        }
    }
}