using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.Utils;
using TGC.Camara;
using TGC.Core.Collision;
using TGC.UtilsGroup;
using TGC.Core.Geometry;
using System;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model
{
    //Twisted Chano, juego de autos chocadores
    public class GameModel : TgcExample
    {
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        //Cantidad de filas
        private const int ROWS = 30;

        //Cantidad de columnas
        private const int COLUMNS = 30;

        //Tamaño cuadrante
        private const int CUADRANTE_SIZE = 600;

        //Posicion vertices
        private const int POSICION_VERTICE = 9000;

        //Scene principal
        private TgcScene ScenePpal;

        //Boleano para ver si dibujamos el boundingbox
        private bool BoundingBox { get; set; }

        //Nombre del jugador 1 que se dibujara en pantalla
        public string NombreJugador1 { get;  set; }

        //Cantidad de autos enemigos
        public int CantidadDeOponentes { get; set; }

        //Cantidad de tiempo de juego
        public int TiempoDeJuego { get; set; }

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

        //Lista de bananas
        private List<TgcMesh> MeshArbolesBananas;
        private TgcMesh ArbolBananasOriginal;

        //Lista de objetos del mesh principal
        private List<TgcMesh> MeshPrincipal;
        
        //Seteo de camara
        private float oh;
        private float of;
        private Vector3 d;

        //Jugadores
        private List <Jugador> listaJugadores;

        //Tiempo
        private HUDTiempo claseTiempo;

        //Variable de fin de modelo
        public bool finModelo { get; set; }

        public override void Init()
        {
            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;
            var loader = new TgcSceneLoader();
            
            //Cargo la clase de Tiempo
            this.claseTiempo = new HUDTiempo(MediaDir, this.TiempoDeJuego);

            //Cargo los jugadores y sus autos
            

            //Cargo el terreno
            ScenePpal = loader.loadSceneFromFile(MediaDir + "MAPA3-TgcScene.xml");

            TransformarMeshScenePpal(0, 3, POSICION_VERTICE);
            TransformarMeshScenePpal(1, 3, POSICION_VERTICE);
            TransformarMeshScenePpal(2, 3, POSICION_VERTICE);
            TransformarMeshScenePpal(3, 3, POSICION_VERTICE);
            TransformarMeshScenePpal(4, 3, POSICION_VERTICE);
            TransformarMeshScenePpal(5, 3, POSICION_VERTICE);

            //Cargo los objetos del mesh en una lista para después poder validar las colisiones
            MeshPrincipal = new List<TgcMesh>();

            foreach (TgcMesh unMesh in ScenePpal.Meshes)
            {
                if (unMesh.Name.IndexOf("Floor") != -1)
                    continue;

                if (unMesh.Name.IndexOf("Pasto") != -1)
                    continue;

                MeshPrincipal.Add(unMesh);
            }

            //Cargo los autos
            this.CrearAutos(loader);


            //Camara por defecto
            CamaraInterna = new TgcThirdPersonCamera(this.listaJugadores[0].claseAuto.GetPosition(), 120, 280);
            
            oh = CamaraInterna.OffsetHeight;
            of = CamaraInterna.OffsetForward;
            d = CamaraInterna.TargetDisplacement;
            Camara = CamaraInterna;

            //Creo los objetos del escenario
            CrearObjetos(loader);
        }

        

        private void TransformarMeshScenePpal (int index, float escala, float desplazamiento)
        {
            var unMesh = ScenePpal.Meshes[index];

            unMesh.AutoTransformEnable = false;

            unMesh.Transform = unMesh.Transform * Matrix.Scaling(new Vector3(escala, 1, escala))
                                                                * Matrix.Translation(new Vector3((-1) * desplazamiento, 0, (-1) * desplazamiento));
            unMesh.BoundingBox.transform(unMesh.Transform);

            unMesh.AlphaBlendEnable = true;
        }

        private void CrearObjetos(TgcSceneLoader loader)
        {
            int[,] MatrizPoblacion;

            //Creo palmeras
            MatrizPoblacion = RandomMatrix();
            PalmeraOriginal = loader.loadSceneFromFile(MediaDir + "Vegetacion\\Palmera\\Palmera-TgcScene.xml").Meshes[0];
            MeshPalmeras = CrearInstancias(PalmeraOriginal, 0.75f, 0.25f, 2, MatrizPoblacion);

            //Creo pinos
            MatrizPoblacion = RandomMatrix();
            PinoOriginal = loader.loadSceneFromFile(MediaDir + "Vegetacion\\Pino\\Pino-TgcScene.xml").Meshes[0];
            MeshPinos = CrearInstancias(PinoOriginal, 0.90f, -0.05f, 2, MatrizPoblacion);

            //Creo rocas
            MatrizPoblacion = RandomMatrix();
            RocaOriginal = loader.loadSceneFromFile(MediaDir + "Vegetacion\\Roca\\Roca-TgcScene.xml").Meshes[0];
            MeshRocas = CrearInstancias(RocaOriginal, 0.75f, 0.30f, 2, MatrizPoblacion);

            //Creo arboles bananas
            MatrizPoblacion = RandomMatrix();
            ArbolBananasOriginal = loader.loadSceneFromFile(MediaDir + "Vegetacion\\ArbolBananas\\ArbolBananas-TgcScene.xml").Meshes[0];
            MeshArbolesBananas = CrearInstancias(ArbolBananasOriginal, 1.50f, 0.15f, 1, MatrizPoblacion);
            /*
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
            MeshCajasMuniciones = CrearInstancias(CajaMunicionesOriginal, 1, 1.15f, 1, MatrizPoblacion);*/
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

        private void CrearJugadores(TgcSceneLoader loader)
        {
            System.Random randomNumber = new System.Random();

            //Creo la lista de jugadores y sus autos
            this.listaJugadores = new List<Jugador>();
            this.listaJugadores.Add(new Jugador(this.NombreJugador1, MediaDir, 0));
            this.listaJugadores[0].claseAuto.SetMesh(loader.loadSceneFromFile(MediaDir + "Vehiculos\\Auto\\Auto-TgcScene.xml").Meshes[0]);

            if (CantidadDeOponentes >= 1)
            {
                listaJugadores.Add(new Jugador("tanque", MediaDir, 1));
                this.listaJugadores[1].claseAuto.SetMesh(loader.loadSceneFromFile(MediaDir + "Vehiculos\\TanqueFuturistaRuedas\\TanqueFuturistaRuedas-TgcScene.xml").Meshes[0]);
                this.listaJugadores[1].claseAuto.SetPositionMesh(new Vector3((-1) * (POSICION_VERTICE - CUADRANTE_SIZE * 4), 0, (POSICION_VERTICE - CUADRANTE_SIZE * 4)), false);
            }

            if (CantidadDeOponentes >= 2)
            {
                listaJugadores.Add(new Jugador("hummer", MediaDir, 2));
                this.listaJugadores[2].claseAuto.SetMesh(loader.loadSceneFromFile(MediaDir + "Vehiculos\\Hummer\\Hummer-TgcScene.xml").Meshes[0]);
                this.listaJugadores[1].claseAuto.SetPositionMesh(new Vector3(POSICION_VERTICE - CUADRANTE_SIZE * 4, 0, POSICION_VERTICE - CUADRANTE_SIZE * 4), false);
            }

            if (CantidadDeOponentes >= 3)
            {
                listaJugadores.Add(new Jugador("camioneta", MediaDir, 3));
                this.listaJugadores[3].claseAuto.SetMesh(loader.loadSceneFromFile(MediaDir + "Vehiculos\\Camioneta\\Camioneta-TgcScene.xml").Meshes[0]);
                this.listaJugadores[1].claseAuto.SetPositionMesh(new Vector3((POSICION_VERTICE - CUADRANTE_SIZE * 4), 0, (-1) * (POSICION_VERTICE - CUADRANTE_SIZE * 4)), true);
            }

            if (CantidadDeOponentes >= 4)
            {
                listaJugadores.Add(new Jugador("patrullero", MediaDir, 4));
                this.listaJugadores[4].claseAuto.SetMesh(loader.loadSceneFromFile(MediaDir + "Vehiculos\\Patrullero\\Patrullero-TgcScene.xml").Meshes[0]);
                this.listaJugadores[1].claseAuto.SetPositionMesh(new Vector3((-1) * (POSICION_VERTICE - CUADRANTE_SIZE * 4), 0, (-1) * (POSICION_VERTICE - CUADRANTE_SIZE * 4)), true);
            }
        }

        //Crea instancias de un objeto
        private List<TgcMesh> CrearInstancias(TgcMesh unObjeto, float scale, float ejeZ, int cantidadObjetos, int[,] MatrizPoblacion)
        {
            List<TgcMesh> ListaMesh = new List<TgcMesh>();
            System.Random randomNumber = new System.Random();
            Matrix unaBoundingBoxMatrix;
            Matrix unaTranslation;

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
                            instance.AutoUpdateBoundingBox = true;

                            //Roto el objeto aleatoriamente
                            instance.Transform = Matrix.RotationY((randomNumber.Next(1, 180)) * FastMath.PI / 180);

                            /*
                            instance.Transform = instance.Transform * Matrix.Scaling(new Vector3(scale, scale, scale)) *
                                                                       Matrix.Translation(new Vector3(200, ejeZ, -10));
                            */

                            //Calculo el tamaño del bounding box
                            unaBoundingBoxMatrix = instance.Transform * Matrix.Scaling(new Vector3(0.15f, 0.8f, 0.15f)) *
                                                                       Matrix.Translation(new Vector3(POSICION_VERTICE, ejeZ, (-1) * POSICION_VERTICE));

                            //Lo agrando y traslado al borde del terreno
                            instance.Transform = instance.Transform * Matrix.Scaling(new Vector3(scale, scale, scale)) *
                                                                       Matrix.Translation(new Vector3(POSICION_VERTICE, ejeZ, (-1) * POSICION_VERTICE));

                            //Calculo la matriz de traslación aleatoria
                            unaTranslation = Matrix.Translation(new Vector3((-1) * randomNumber.Next(j * CUADRANTE_SIZE, (j + 1) * CUADRANTE_SIZE), 0,
                                                                 randomNumber.Next(i * CUADRANTE_SIZE, (i + 1) * CUADRANTE_SIZE)));
                            
                            //Lo posiciono en una posición aleatoria
                            instance.Transform = instance.Transform * unaTranslation;

                            //Posiciono el bounding box alterado en el mismo lugar que el mesh
                            unaBoundingBoxMatrix = unaBoundingBoxMatrix * unaTranslation;

                            //Muevo el bounding box
                            instance.BoundingBox.transform(instance.Transform);
                            
                            //Valido si pisa a otro objeto que ya existe
                            if (ValidarColisionCrearInstancias(instance, ListaMesh))
                            {
                                //Hubo colision, no creo el objeto
                                instance.dispose();
                                continue;                                
                            }
                            ///////////////////////////////////////////////////////////////////////////////////

                            //Para determinados objetos, le meto el bounding alterado
                            if ((unObjeto.Name == "Palmera") || (unObjeto.Name == "Pino") || (unObjeto.Name == "ArbolBananas"))
                            {
                                instance.BoundingBox.transform(unaBoundingBoxMatrix);
                            }

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
                AccionarListaMesh(MeshAutos, 2, unaInstancia) ||
                AccionarListaMesh(MeshPrincipal, 2, unaInstancia)
                )
            {
                return true;
            }

            return false;
        }

        private bool ValidarColisionObjetos(TgcMesh unaInstancia)
        {
            //Valido la colisión para cada lista de objetos que tenga
            if (AccionarListaMesh(MeshPalmeras, 2, unaInstancia) ||
                AccionarListaMesh(MeshPinos, 2, unaInstancia) ||
                AccionarListaMesh(MeshRocas, 2, unaInstancia) ||
                AccionarListaMesh(MeshArbolesBananas, 2, unaInstancia) ||
                AccionarListaMesh(MeshAutos, 2, unaInstancia) ||
                AccionarListaMesh(MeshPrincipal, 2, unaInstancia)
                )
            {
                return true;
            }

            return false;
        }

        public override void Update()
        {
            PreUpdate();

            //Activo bounding box para debug
            ActivarBoundingBox();

            //Chequea si se solicitó cambiar el tipo de camara
            CambiarDeCamara();

            //Maneja el movimiento del auto teniendo en cuenta la posición de los otros objetos
            MoverAutoConColisiones();

            //Actualizo los jugadores
            foreach (var unJugador in this.listaJugadores)
            {
                unJugador.Update();
            }

            //Actualizo el tiempo
            this.claseTiempo.Update();

            //Chequeo el fin del modelo
            if (this.claseTiempo.GetFinDeJuego() && Input.keyDown(Key.X))
            {
                this.finModelo = true;
            }
        }

        public override void Render()
        {
            Vector3 positionc;
            Vector3 targetc;

            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
            PreRender();

            //Dibuja un texto por pantalla
            DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.Red);
            DrawText.drawText("Con la tecla F1 se cambia el tipo de camara. Pos [Actual]: " + TgcParserUtils.printVector3(Camara.Position), 0, 30, Color.Red);

            DrawText.drawText("Jugador 1: " + this.NombreJugador1, 0, 40, Color.LightYellow);
            DrawText.drawText("Velocidad: " + this.MOVEMENT_SPEED, 0, 50, Color.LightYellow);
            
            CamaraInterna.CalculatePositionTarget(out positionc, out targetc);
            DrawText.drawText("Posición cámara: " + positionc + " " + targetc, 0, 60, Color.DarkGreen );

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
                RenderizarObjetos(1);
                foreach (var mesh in ScenePpal.Meshes)
                {
                    mesh.BoundingBox.render();
                }

                ObbMesh.render();
                RenderizarObjetos(1);
            }

            //Renderizo los objetos cargados de las listas
            RenderizarObjetos(0);

            //Dibujo los jugadores
            foreach (var unJugador in this.listaJugadores)
            {
                unJugador.Render();
            }

            //Dibujo el reloj
            this.claseTiempo.Render();

            //Dibujo las ruedas
            DibujarRuedas();

            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        private void DibujarRuedas()
        {
            float ro, alfa_rueda, rotacionRueda;
            float posicion_x;
            float posicion_y;

            //Posiciono las ruedas
            for (int i = 0; i < 4; i++)
            {
                ro = FastMath.Sqrt(dx[i] * dx[i] + dy[i] * dy[i]);

                alfa_rueda = FastMath.Asin(dx[i] / ro);

                if (i == 0 || i == 2)
                {
                    alfa_rueda += FastMath.PI;
                }

                posicion_x = FastMath.Sin(alfa_rueda + Mesh.Rotation.Y) * ro;
                posicion_y = FastMath.Cos(alfa_rueda + Mesh.Rotation.Y) * ro;

                RuedasJugador1[i].Position = (new Vector3(posicion_x, 7.5f, posicion_y) + Mesh.Position);

                rotacionRueda = 0;

                if (i == 0 || i == 2)
                {
                    rotacionRueda = 0.5f * Math.Sign (rotate);
                }

                //Si no aprieta para los costados, dejo la rueda derecha (por ahora, esto se puede modificar)
                if (Input.keyDown(Key.Left) || Input.keyDown(Key.A) || Input.keyDown(Key.Right) || Input.keyDown(Key.D))
                    RuedasJugador1[i].Rotation = new Vector3(rotacionVertical, Mesh.Rotation.Y + rotacionRueda, 0f);
                else
                    RuedasJugador1[i].Rotation = new Vector3(rotacionVertical, Mesh.Rotation.Y, 0f);
            }

            for (int i = 0; i < 4; i++)
            {
                RuedasJugador1[i].render();
            }
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

                                if (mesh != unaInstancia)
                                {
                                    if (TgcCollisionUtils.classifyBoxBox(mesh.BoundingBox, unaInstancia.BoundingBox) != TgcCollisionUtils.BoxBoxResult.Afuera)
                                    {
                                        return true;
                                    }
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

        public void MoverAutoConColisiones()
        {
            //Declaramos un vector de movimiento inicializado en cero.
            //El movimiento sobre el suelo es sobre el plano XZ.
            //Sobre XZ nos movemos con las flechas del teclado o con las letas WASD.
            var movement = new Vector3(0, 0, 0);
            var moveForward = 0f;
            rotate = 0;
            var moving = false;
            var rotating = false;
            var stationary = false;

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
            if ((Input.keyDown(Key.Up) || Input.keyDown(Key.W)))
            {
                moveForward += -Acelerar(200f);
                moving = true;
            }

            if ((Input.keyDown(Key.Down) || Input.keyDown(Key.S)))
            {
                moveForward += -Acelerar(-250f);
                moving = true;
            }

            //El auto dejo de acelerar e ira frenando de apoco 
            if (!Input.keyDown(Key.Down) && !Input.keyDown(Key.S) && !Input.keyDown(Key.Up) && !Input.keyDown(Key.W))
            {
                moveForward = -Acelerar(0f);
                moving = true;
            }

            if (rotating)
            {
                var rotAngle = (MOVEMENT_SPEED * 0.2f * Math.Sign (rotate) * ElapsedTime) * (FastMath.PI / 180);
                Mesh.rotateY(rotAngle);
                CamaraInterna.rotateY(rotAngle);
                ObbMesh.rotate(new Vector3(0, rotAngle, 0));
            }

            if (Input.keyPressed(Key.Space) && !falling)
            {
                jumping = true;
            }

            if (jumping)
            {
                Mesh.move(0, 100 * ElapsedTime * 2, 0);

                if (Mesh.Position.Y >= ALTURA_SALTO)
                {
                    jumping = false;
                    falling = true;
                }
            }

            if (falling)
            {
                Mesh.move(0, -100 * ElapsedTime * 3, 0);

                if (Mesh.Position.Y < 0.5f)
                {
                    Mesh.move(0, 0.5f - Mesh.Position.Y, 0);
                    falling = false;
                }
            }

            if(MOVEMENT_SPEED > 30 || MOVEMENT_SPEED < -30)
            {
                moving = true;
            }

            if (MOVEMENT_SPEED < 30 || MOVEMENT_SPEED > -30)
            {
                stationary = true;
            }

            if (moving)
            {
                //Guardar posicion original antes de cambiarla
                var originalPos = Mesh.Position;

                //Multiplicar movimiento por velocidad y elapsedTime
                movement *= MOVEMENT_SPEED * ElapsedTime;

                rotacionVertical -= MOVEMENT_SPEED * ElapsedTime / 60;

                Mesh.moveOrientedY(moveForward * ElapsedTime);
                ObbMesh.Center = Mesh.Position;

                if (DetectarColisionesObb())
                {
                    Mesh.Position = originalPos;
                    MOVEMENT_SPEED = (-1) * Math.Sign(MOVEMENT_SPEED) * Math.Abs(MOVEMENT_SPEED * 0.3f);

                    Mesh.moveOrientedY((-1) * moveForward * ElapsedTime);
                }

                //Hacer que la camara en 3ra persona se ajuste a la nueva posicion del objeto
                CamaraInterna.Target = Mesh.Position;

                ajustarPosicionDeCamara();

            }

            if (stationary)
            {
                ajustarPosicionDeCamara();
            }
        }

        private void ajustarPosicionDeCamara()
        {
            Vector3 position;
            Vector3 target;
            Vector3 q;
            float distSq = 0;

            CamaraInterna.OffsetHeight = oh-10;
            CamaraInterna.OffsetForward = of;
            CamaraInterna.TargetDisplacement = d;
            
            //Pedirle a la camara cual va a ser su proxima posicion
            CamaraInterna.CalculatePositionTarget(out position, out target);

            //Detectar colisiones entre el segmento de recta camara-personaje y todos los objetos del escenario
            
            var minDistSq = FastMath.Pow2(of) ;
            foreach (TgcMesh unMesh in ScenePpal.Meshes)
            {
                //Hay colision del segmento camara-personaje y el objeto
                if (TgcCollisionUtils.intersectSegmentAABB(target, position, unMesh.BoundingBox, out q))
                {
                    //Si hay colision, guardar la que tenga menor distancia
                    distSq = Vector3.Subtract(q, target).LengthSq();

                    //Hay dos casos singulares, puede que tengamos mas de una colision hay que quedarse con el menor offset.
                    //Si no dividimos la distancia por 2 se acerca mucho al target.
                    minDistSq = FastMath.Min(distSq/2, minDistSq);
                }
            }

            //Acercar la camara hasta la minima distancia de colision encontrada (pero ponemos un umbral maximo de cercania)
            var newOffsetForward = FastMath.Sqrt(minDistSq);

            if (FastMath.Abs(newOffsetForward) < 10)
            {
                newOffsetForward = 10;
            }
            CamaraInterna.OffsetForward = newOffsetForward;

            //Asignar la ViewMatrix haciendo un LookAt desde la posicion final anterior al centro de la camara
            CamaraInterna.CalculatePositionTarget(out position, out target);
            CamaraInterna.SetCamera(position, target);
        }

        private bool DetectarColisionesObb()
        {
            List<bool> booleanosColision = new List<bool>();
            List<TgcMesh> allMesh = new List<TgcMesh>();
            allMesh.AddRange(MeshPinos);
            allMesh.AddRange(MeshRocas);
            allMesh.AddRange(MeshPalmeras);
            allMesh.AddRange(MeshArbolesBananas);
            allMesh.AddRange(ScenePpal.Meshes);
            allMesh.AddRange(MeshAutos);

            foreach (var unMesh in allMesh)
            {
                if ((unMesh != Mesh) && (unMesh.Name != "Room-1-Roof-0") && (unMesh.Name != "Room-1-Floor-0") &&
                    (unMesh.Name != "Pasto") && (unMesh.Name != "Plane_5")) //siempre que el mesh sea distinto al auto sino colisionara con el mismo
                booleanosColision.Add(TgcCollisionUtils.testObbAABB(ObbMesh, unMesh.BoundingBox)); //me fijo si hubo alguna colision este booleano lo meto en una lista
            }

            return booleanosColision.Find(valor => valor.Equals(true)); // me fijo si alguno de la lista dio true
        }

        private float Acelerar(float aceleracion) 
        {
            if ((MOVEMENT_SPEED < MAX_SPEED))
            {
                MOVEMENT_SPEED = MOVEMENT_SPEED + ((aceleracion + ObtenerRozamiento()) * ElapsedTime);

                if (MOVEMENT_SPEED > Math.Abs (MAX_SPEED))
                    MOVEMENT_SPEED = MAX_SPEED - 1;

                return MOVEMENT_SPEED;
            }
            else return MOVEMENT_SPEED;
        }

        private float ObtenerRozamiento()
        {
            if (MOVEMENT_SPEED > 0) return (-ROZAMIENTO);
            if (MOVEMENT_SPEED < 0) return ROZAMIENTO;
            else return 0;
        }

        private float Desacelerar()
        {
            if (MOVEMENT_SPEED <= 0) return 0;
            else return MOVEMENT_SPEED -= ROZAMIENTO * ElapsedTime;

        }

        public Vector3 PuntoMedio(TgcMesh mesh)
        {
            Vector3[] vertices = mesh.getVertexPositions();
            long cantidad = vertices.GetLongLength(0);
            float X = 0, Y = 0, Z = 0;
            for (int i = 0; i < cantidad; i++)
            {
                X += vertices[i].X;
                Y += vertices[i].Y;
                Z += vertices[i].Z;
            }
            X /= cantidad;
            Y /= cantidad;
            Z /= cantidad;
            var vectorMedio = new Vector3(X , Y , Z);
            return vectorMedio;
        }

        public Vector3 PuntoMedioDeLosExtremos(TgcMesh mesh)
        {
            Vector3[] vertices = mesh.getVertexPositions();
            long cantidad = vertices.GetLongLength(0);
            float XMinimo = vertices[0].X;
            float YMinimo = vertices[0].Y;
            float ZMinimo = vertices[0].Z;
            float XMaximo = vertices[0].X;
            float YMaximo = vertices[0].Y;
            float ZMaximo = vertices[0].Z;
            for (int i = 0; i < cantidad; i++)
            {
                if (vertices[i].X < XMinimo)
                    XMinimo = vertices[i].X;
                if (vertices[i].X > XMaximo)
                    XMaximo = vertices[i].X;
                if (vertices[i].Y < YMinimo)
                    YMinimo = vertices[i].X;
                if (vertices[i].Y > YMaximo)
                    YMaximo = vertices[i].X;
                if (vertices[i].Z < ZMinimo)
                    ZMinimo = vertices[i].X;
                if (vertices[i].Z > ZMaximo)
                    ZMaximo = vertices[i].X;
            }
            float X = (XMinimo + XMaximo) / 2;
            float Y = (YMinimo + YMaximo) / 2;
            float Z = (ZMinimo + ZMaximo) / 2;
            var vectorMedio = new Vector3(X, Y, Z);
            return vectorMedio;
        }
        
        public override void Dispose()
        {
            //Borro los objetos de los jugadores
            foreach (var unJugador in this.listaJugadores)
            {
                unJugador.Dispose();
            }

            foreach (var mesh in MeshAutos)
            {
                mesh.dispose();
            }

            PalmeraOriginal.dispose();
            PinoOriginal.dispose();
            RocaOriginal.dispose();
            ArbolBananasOriginal.dispose();

            foreach (var mesh in RuedasJugador1)
            {
                mesh.dispose();
            }

            ScenePpal.disposeAll();
        }
    }
}