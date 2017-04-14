//"Inclusion de librerias"
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
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
using TGC.Core.UserControls;
using TGC.Core.UserControls.Modifier;
using System.Collections.Generic;


namespace TGC.Group.Model
{
    /// <summary>
    ///     Twisted Chano, juego de autos chocadores
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

        /// <summary>
        /// Inicializacion de variables y "definicion" de los objetos
        /// </summary>

        //Velocidad de movimiento del auto
        private const float MOVEMENT_SPEED = 400f;
        //Velocidad de rotación del auto
        private const float ROTATION_SPEED = 120f;

        //Cantidad de filas
        private const int ROWS = 19;

        //Cantidad de columnas
        private const int COLUMNS = 19;

        //Tamaño cuadrante
        private const int CUADRANTE_SIZE = 300;

        //Scene principal
        private TgcScene ScenePpal;

        //Mesh del auto
        private TgcMesh Mesh { get; set; }

        //Boleano para ver si dibujamos el boundingbox
        private bool BoundingBox { get; set; }

        //Nombre del jugador 1 que se dibujara en pantalla
        public string NombreJugador1 { get;  set; }

        //Camara en tercera persona
        private TgcThirdPersonCamera CamaraInterna;

        //Tipo de cámara
        private int TipoCamara = 0;

        //Lista de Autos
        private List<TgcMesh> MeshAutos;

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
        //private List<TgcMesh> MeshFuentesAgua;
        //private TgcMesh FuenteAguaOriginal;

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

            var ScenePpalMesh = ScenePpal.Meshes[0];
            ScenePpalMesh.AutoTransformEnable = false;
            ScenePpalMesh.Transform = ScenePpalMesh.Transform * Matrix.Scaling(new Vector3(3, 1, 3));
            ScenePpalMesh.AlphaBlendEnable = true;
            ScenePpalMesh.updateBoundingBox();

            //Cargo el auto
            var SceneAuto = loader.loadSceneFromFile(MediaDir + "Vehiculos\\Auto\\Auto-TgcScene.xml");

            //Movemos el escenario un poco para arriba para que se pueda mover el auto
            Mesh = SceneAuto.Meshes[0];
        
            Mesh.AutoTransformEnable = true;
            Mesh.move(0, 0.3f, 0);

            //Camara por defecto
            CamaraInterna = new TgcThirdPersonCamera(Mesh.Position, 300, 600);
            Camara = CamaraInterna;

            //Creo los objetos del escenario
            CrearAutos(loader);
            CrearObjetos(loader);
        }

        private void CrearObjetos(TgcSceneLoader loader)
        {
            int[,] MatrizPoblacion;

            //Creo palmeras
            MatrizPoblacion = RandomMatrix();
            PalmeraOriginal = loader.loadSceneFromFile(MediaDir + "Vegetacion\\Palmera\\Palmera-TgcScene.xml").Meshes[0];
            MeshPalmeras = CrearInstancias(PalmeraOriginal, 0.75f, 1.15f, 2, MatrizPoblacion);

            //Creo pinos
            MatrizPoblacion = RandomMatrix();
            PinoOriginal = loader.loadSceneFromFile(MediaDir + "Vegetacion\\Pino\\Pino-TgcScene.xml").Meshes[0];
            MeshPinos = CrearInstancias(PinoOriginal, 0.90f, 1.15f, 2, MatrizPoblacion);

            //Creo rocas
            MatrizPoblacion = RandomMatrix();
            RocaOriginal = loader.loadSceneFromFile(MediaDir + "Vegetacion\\Roca\\Roca-TgcScene.xml").Meshes[0];
            MeshRocas = CrearInstancias(RocaOriginal, 0.75f, 1.15f, 2, MatrizPoblacion);

            //Creo arboles bananas
            MatrizPoblacion = RandomMatrix();
            ArbolBananasOriginal = loader.loadSceneFromFile(MediaDir + "Vegetacion\\ArbolBananas\\ArbolBananas-TgcScene.xml").Meshes[0];
            MeshArbolesBananas = CrearInstancias(ArbolBananasOriginal, 1.50f, 1.15f, 1, MatrizPoblacion);

            //Creo barriles de polvora
            MatrizPoblacion = RandomMatrix();
            BarrilPolvoraOriginal = loader.loadSceneFromFile(MediaDir + "Objetos\\BarrilPolvora\\BarrilPolvora-TgcScene.xml").Meshes[0];
            MeshBarrilesPolvora = CrearInstancias(BarrilPolvoraOriginal, 0.75f, 1.15f, 1, MatrizPoblacion);

            //Creo carretillas
            MatrizPoblacion = RandomMatrix();
            CarretillaOriginal = loader.loadSceneFromFile(MediaDir + "Objetos\\Carretilla\\Carretilla-TgcScene.xml").Meshes[0];
            MeshCarretillas = CrearInstancias(CarretillaOriginal, 0.20f, 1.15f, 1, MatrizPoblacion);

            //Creo contenedores
            MatrizPoblacion = RandomMatrix();
            ContenedorOriginal = loader.loadSceneFromFile(MediaDir + "Objetos\\Contenedor\\Contenedor-TgcScene.xml").Meshes[0];
            MeshContenedores = CrearInstancias(ContenedorOriginal, 1.5f, 1.15f, 1, MatrizPoblacion);

            //Creo fuentes de agua
            //MatrizPoblacion = RandomMatrix();
            //FuenteAguaOriginal = loader.loadSceneFromFile(MediaDir + "Objetos\\FuenteAgua\\FuenteAgua-TgcScene.xml").Meshes[0];
            //MeshFuentesAgua = CrearInstancias(FuenteAguaOriginal, 1, 25, 1, MatrizPoblacion);

            //Creo lockers
            MatrizPoblacion = RandomMatrix();
            LockerOriginal = loader.loadSceneFromFile(MediaDir + "Muebles\\LockerMetal\\LockerMetal-TgcScene.xml").Meshes[0];
            MeshLockers = CrearInstancias(LockerOriginal, 1, 1.15f, 1, MatrizPoblacion);

            //Creo expendedores bebidas
            MatrizPoblacion = RandomMatrix();
            ExpendedorBebidaOriginal = loader.loadSceneFromFile(MediaDir + "Muebles\\ExpendedorDeBebidas\\ExpendedorDeBebidas-TgcScene.xml").Meshes[0];
            MeshExpendedoresBebidas = CrearInstancias(ExpendedorBebidaOriginal, 0.50f, 1.15f, 1, MatrizPoblacion);

            //Creo cajas de municiones
            MatrizPoblacion = RandomMatrix();
            CajaMunicionesOriginal = loader.loadSceneFromFile(MediaDir + "Armas\\CajaMuniciones\\CajaMuniciones-TgcScene.xml").Meshes[0];
            MeshCajasMuniciones = CrearInstancias(CajaMunicionesOriginal, 1, 1.15f, 1, MatrizPoblacion);
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

        private void CrearAutos(TgcSceneLoader loader)
        {
            System.Random randomNumber = new System.Random();

            var tanque = loader.loadSceneFromFile(MediaDir + "Vehiculos\\TanqueFuturistaRuedas\\TanqueFuturistaRuedas-TgcScene.xml").Meshes[0].createMeshInstance("tanque");
            var hummer = loader.loadSceneFromFile(MediaDir + "Vehiculos\\Hummer\\Hummer-TgcScene.xml").Meshes[0].createMeshInstance("hummer");
            var camioneta = loader.loadSceneFromFile(MediaDir + "Vehiculos\\Camioneta\\Camioneta-TgcScene.xml").Meshes[0].createMeshInstance("camioneta");
            var patrullero = loader.loadSceneFromFile(MediaDir + "Vehiculos\\Patrullero\\Patrullero-TgcScene.xml").Meshes[0].createMeshInstance("buggy");

            tanque.Transform = tanque.Transform * Matrix.Translation(new Vector3(-2000,0,2000));
            tanque.BoundingBox.transform(tanque.Transform);

            hummer.Transform = hummer.Transform * Matrix.Translation(new Vector3(2000, 0, 2000));
            hummer.BoundingBox.transform(hummer.Transform);

            camioneta.Transform = camioneta.Transform * Matrix.RotationY(180 * (FastMath.PI / 180));
            camioneta.Transform = camioneta.Transform * Matrix.Translation(new Vector3(2000,0,-2000));
            camioneta.BoundingBox.transform(camioneta.Transform);

            patrullero.Transform = patrullero.Transform * Matrix.RotationY(180 * (FastMath.PI / 180));
            patrullero.Transform = patrullero.Transform * Matrix.Translation(new Vector3(-2000,0,-2000));
            patrullero.BoundingBox.transform(patrullero.Transform);

            MeshAutos = new List<TgcMesh>();
            //Auto principal
            MeshAutos.Add(Mesh);

            //Otros autos con IA?
            MeshAutos.Add(tanque);
            MeshAutos.Add(hummer);
            MeshAutos.Add(camioneta);
            MeshAutos.Add(patrullero);
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

                            //Roto el objeto aleatoriamente
                            instance.Transform = Matrix.RotationY((randomNumber.Next(1, 180)) * FastMath.PI / 180);

                            //Lo agrando y traslado al borde del terreno
                            instance.Transform = instance.Transform * Matrix.Scaling(new Vector3(scale, scale, scale)) *
                                                                       Matrix.Translation(new Vector3(3000, ejeZ, -3000));


                            //Lo posiciono en una posición aleatoria
                            instance.Transform = instance.Transform * Matrix.Translation(
                                                                        new Vector3((-1) * randomNumber.Next(j * CUADRANTE_SIZE, (j + 1) * CUADRANTE_SIZE), 0,
                                                                                    randomNumber.Next(i * CUADRANTE_SIZE, (i + 1) * CUADRANTE_SIZE)));

                            instance.BoundingBox.transform(instance.Transform);
                            
                            //Valido si pisa a otro objeto que ya existe
                            if (ValidarColisionCrearInstancias(instance, ListaMesh))
                            {
                                //Hubo colision, no creo el objeto
                                instance.dispose();
                                continue;                                
                            }
                            ///////////////////////////////////////////////////////////////////////////////////

                            //Le activo el alpha para que se vea mejor
                            instance.AlphaBlendEnable = true;

                            //Lo agrego a la lista para después renderizarlo
                            ListaMesh.Add(instance);
                        }
                    }
                }
            }

            return ListaMesh;
        }

        private bool ValidarColisionCrearInstancias(TgcMesh unaInstancia, List<TgcMesh> ListaMeshActual)
        {
            //Valido la colisión para cada lista de objetos que tenga
            if (AccionarListaMesh(ListaMeshActual, 2, unaInstancia) ||
                AccionarListaMesh(MeshPalmeras, 2, unaInstancia) ||
                AccionarListaMesh(MeshPinos, 2, unaInstancia) ||
                AccionarListaMesh(MeshRocas, 2, unaInstancia) ||
                AccionarListaMesh(MeshArbolesBananas, 2, unaInstancia) ||
                AccionarListaMesh(MeshBarrilesPolvora, 2, unaInstancia) ||
                AccionarListaMesh(MeshCarretillas, 2, unaInstancia) ||
                AccionarListaMesh(MeshContenedores, 2, unaInstancia) ||
                AccionarListaMesh(MeshLockers, 2, unaInstancia) ||
                AccionarListaMesh(MeshExpendedoresBebidas, 2, unaInstancia) ||
                AccionarListaMesh(MeshCajasMuniciones, 2, unaInstancia) ||
                AccionarListaMesh(MeshAutos, 2, unaInstancia)
                )
            {
                return true;
            }

            return false;
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
            DrawText.drawText("Jugador 1: " + this.NombreJugador1, 1200, 10, Color.LightYellow);

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
                foreach (var mesh in ScenePpal.Meshes)
                {
                    mesh.BoundingBox.render();
                }

                RenderizarObjetos(1);
            }            

            //Renderizo los objetos cargados de las listas
            RenderizarObjetos(0);

            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        //0: render
        //1: render bounding box
        //2: control de colisiones
        private bool AccionarListaMesh(List<TgcMesh> unaListaMesh, int unaAccion, TgcMesh unaInstancia)
        {
            //Si no está sin inicializar, realizo una acción
            if (unaListaMesh != null)
            {
                foreach (var mesh in unaListaMesh)
                {
                    switch (unaAccion)
                    {
                        case 0:
                            {
                                mesh.render(); 
                            }
                            break;

                        case 1:
                            {
                                mesh.BoundingBox.render();
                            }
                            break;

                        case 2:
                            {
                                if (TgcCollisionUtils.classifyBoxBox(mesh.BoundingBox, unaInstancia.BoundingBox) != TgcCollisionUtils.BoxBoxResult.Afuera)
                                {
                                    return true;
                                }
                            }
                            break;

                    }
                }
            }

            return false;
        }

        private void RenderizarObjetos(int unaAccion)
        {
            //Renderizar Autos
            AccionarListaMesh(MeshAutos, unaAccion, null);

            //Renderizar palmeras
            AccionarListaMesh(MeshPalmeras, unaAccion, null);

            //Renderizar arbol bananas
            AccionarListaMesh(MeshArbolesBananas, unaAccion, null);
            
            //Renderizar pinos
            AccionarListaMesh(MeshPinos, unaAccion, null);

            //Renderizar rocas
            AccionarListaMesh(MeshRocas, unaAccion, null);

            //Renderizar barriles de polvora
            AccionarListaMesh(MeshBarrilesPolvora, unaAccion, null);

            //Renderizar Carretillas
            AccionarListaMesh(MeshCarretillas, unaAccion, null);

            //Renderizar Contenedores
            AccionarListaMesh(MeshContenedores, unaAccion, null);

            //Renderizar Fuentes de Agua
            //RenderListaMesh(MeshPinos, unaAccion, null);

            //Renderizar Lockers
            AccionarListaMesh(MeshLockers, unaAccion, null);

            //Renderizar Expendedores Bebidas
            AccionarListaMesh(MeshExpendedoresBebidas, unaAccion, null);

            //Renderizar Cajas Municiones
            AccionarListaMesh(MeshCajasMuniciones, unaAccion, null);
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
                            //CamaraInterna = new TgcThirdPersonCamera(Mesh.Position, 300, 400);
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
            var moveForward = 0f;
            float rotate = 0;
            var moving = false;
            var rotating = false;


            //Movernos de izquierda a derecha, sobre el eje X.
            if (Input.keyDown(Key.Left) || Input.keyDown(Key.A))
            {
                rotate = -ROTATION_SPEED;
                rotating = true;
            }
            else if (Input.keyDown(Key.Right) || Input.keyDown(Key.D))
            {
                rotate = ROTATION_SPEED;
                rotating = true;
            }

            //Movernos adelante y atras, sobre el eje Z.
            if (Input.keyDown(Key.Up) || Input.keyDown(Key.W))
            {
                moveForward = -MOVEMENT_SPEED;
                moving = true;
            }
            else if (Input.keyDown(Key.Down) || Input.keyDown(Key.S))
            {
                moveForward = MOVEMENT_SPEED;
                moving = true;
            }

            if (rotating)
            {
                var rotAngle = (rotate * ElapsedTime) * (FastMath.PI / 180);
                Mesh.rotateY(rotAngle);
                CamaraInterna.rotateY(rotAngle);
            }
            if (moving)
            {
                //Guardar posicion original antes de cambiarla
                var originalPos = Mesh.Position;

                //Multiplicar movimiento por velocidad y elapsedTime
                movement *= MOVEMENT_SPEED * ElapsedTime;
                Mesh.moveOrientedY(moveForward * ElapsedTime);

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
        }

        /// <summary>
        ///     Se llama cuando termina la ejecución del ejemplo.
        ///     Hacer Dispose() de todos los objetos creados.
        ///     Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        /// </summary>
        public override void Dispose()
        {
            PalmeraOriginal.dispose();

            foreach (var mesh in MeshAutos)
            {
                mesh.dispose();
            }
               
            PinoOriginal.dispose();
            RocaOriginal.dispose();
            ArbolBananasOriginal.dispose();
            BarrilPolvoraOriginal.dispose();
            CarretillaOriginal.dispose();
            ContenedorOriginal.dispose();
            LockerOriginal.dispose();
            ExpendedorBebidaOriginal.dispose();
            CajaMunicionesOriginal.dispose();

            //FuenteAguaOriginal.dispose();
            ScenePpal.disposeAll();
        }
    }
}