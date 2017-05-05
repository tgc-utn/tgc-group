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

        //Boleano para ver si dibujamos el boundingbox
        private bool BoundingBox { get; set; }

        //Nombre del jugador 1 que se dibujara en pantalla
        public string NombreJugador1 { get;  set; }

        //Scene principal
        private TgcScene ScenePpal;

        //Cantidad de autos enemigos
        public int CantidadDeOponentes { get; set; }

        //Cantidad de tiempo de juego
        public int TiempoDeJuego { get; set; }

        //Tipo de cámara
        private int TipoCamara = 0;

        //Lista de Autos
        public static List<TgcMesh> MeshAutos;

        //Lista de palmeras
        public static List<TgcMesh> MeshPalmeras;
        private TgcMesh PalmeraOriginal;

        //Lista de pinos
        public static List<TgcMesh> MeshPinos;
        private TgcMesh PinoOriginal;

        //Lista de rocas
        public static List<TgcMesh> MeshRocas;
        private TgcMesh RocaOriginal;

        //Lista de bananas
        public static List<TgcMesh> MeshArbolesBananas;
        private TgcMesh ArbolBananasOriginal;

        //Lista de objetos del mesh principal
        public static List<TgcMesh> MeshPrincipal;

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

            //Cargo el terreno
            ScenePpal = loader.loadSceneFromFile(MediaDir + "MAPA3-TgcScene.xml");

            TransformarMeshScenePpal(0, 3, POSICION_VERTICE);
            TransformarMeshScenePpal(1, 3, POSICION_VERTICE);
            TransformarMeshScenePpal(2, 3, POSICION_VERTICE);
            TransformarMeshScenePpal(3, 3, POSICION_VERTICE);
            TransformarMeshScenePpal(4, 3, POSICION_VERTICE);
            TransformarMeshScenePpal(5, 3, POSICION_VERTICE);

            //Cargo los objetos del mesh en una lista para después poder validar las colisiones
            GameModel.MeshPrincipal = new List<TgcMesh>();

            foreach (TgcMesh unMesh in ScenePpal.Meshes)
            {
                GameModel.MeshPrincipal.Add(unMesh);
            }

            //Cargo los jugadores y sus autos
            this.CrearJugadores(loader);

            //Creo los objetos del escenario
            this.CrearObjetos(loader);
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
            GameModel.MeshPalmeras = CrearInstancias(PalmeraOriginal, 0.75f, 0.25f, 2, MatrizPoblacion);

            //Creo pinos
            MatrizPoblacion = RandomMatrix();
            PinoOriginal = loader.loadSceneFromFile(MediaDir + "Vegetacion\\Pino\\Pino-TgcScene.xml").Meshes[0];
            GameModel.MeshPinos = CrearInstancias(PinoOriginal, 0.90f, -0.05f, 2, MatrizPoblacion);

            //Creo rocas
            MatrizPoblacion = RandomMatrix();
            RocaOriginal = loader.loadSceneFromFile(MediaDir + "Vegetacion\\Roca\\Roca-TgcScene.xml").Meshes[0];
            GameModel.MeshRocas = CrearInstancias(RocaOriginal, 0.75f, 0.30f, 2, MatrizPoblacion);

            //Creo arboles bananas
            MatrizPoblacion = RandomMatrix();
            ArbolBananasOriginal = loader.loadSceneFromFile(MediaDir + "Vegetacion\\ArbolBananas\\ArbolBananas-TgcScene.xml").Meshes[0];
            GameModel.MeshArbolesBananas = CrearInstancias(ArbolBananasOriginal, 1.50f, 0.15f, 1, MatrizPoblacion);            
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
            GameModel.MeshAutos = new List<TgcMesh>();
            this.listaJugadores = new List<Jugador>();
            this.listaJugadores.Add(new Jugador(this.NombreJugador1, MediaDir, 0));
            this.listaJugadores[0].claseAuto.SetMesh(loader.loadSceneFromFile(MediaDir + "Vehiculos\\Auto\\Auto-TgcScene.xml").Meshes[0]);
            this.listaJugadores[0].claseAuto.SetRuedas(loader);
            this.listaJugadores[0].CreateCamera();

            GameModel.MeshAutos.Add(this.listaJugadores[0].claseAuto.GetMesh());
            Camara = this.listaJugadores[0].claseCamara.GetCamera();

            if (CantidadDeOponentes >= 1)
            {
                listaJugadores.Add(new Jugador("tanque", MediaDir, 1));
                this.listaJugadores[1].claseAuto.SetMesh(loader.loadSceneFromFile(MediaDir + "Vehiculos\\TanqueFuturistaRuedas\\TanqueFuturistaRuedas-TgcScene.xml").Meshes[0]);
                this.listaJugadores[1].claseAuto.SetPositionMesh(new Vector3((-1) * (POSICION_VERTICE - CUADRANTE_SIZE * 4), 0, (POSICION_VERTICE - CUADRANTE_SIZE * 4)), false);
                this.listaJugadores[1].claseAuto.SetRuedas(loader);
                this.listaJugadores[1].CreateCamera();
                GameModel.MeshAutos.Add(this.listaJugadores[1].claseAuto.GetMesh());
            }

            if (CantidadDeOponentes >= 2)
            {
                listaJugadores.Add(new Jugador("hummer", MediaDir, 2));
                this.listaJugadores[2].claseAuto.SetMesh(loader.loadSceneFromFile(MediaDir + "Vehiculos\\Hummer\\Hummer-TgcScene.xml").Meshes[0]);
                this.listaJugadores[2].claseAuto.SetPositionMesh(new Vector3(POSICION_VERTICE - CUADRANTE_SIZE * 4, 0, POSICION_VERTICE - CUADRANTE_SIZE * 4), false);
                this.listaJugadores[2].claseAuto.SetRuedas(loader);
                this.listaJugadores[2].CreateCamera();
                GameModel.MeshAutos.Add(this.listaJugadores[2].claseAuto.GetMesh());
            }

            if (CantidadDeOponentes >= 3)
            {
                listaJugadores.Add(new Jugador("camioneta", MediaDir, 3));
                this.listaJugadores[3].claseAuto.SetMesh(loader.loadSceneFromFile(MediaDir + "Vehiculos\\Camioneta\\Camioneta-TgcScene.xml").Meshes[0]);
                this.listaJugadores[3].claseAuto.SetPositionMesh(new Vector3((POSICION_VERTICE - CUADRANTE_SIZE * 4), 0, (-1) * (POSICION_VERTICE - CUADRANTE_SIZE * 4)), true);
                this.listaJugadores[3].claseAuto.SetRuedas(loader);
                this.listaJugadores[3].CreateCamera();
                GameModel.MeshAutos.Add(this.listaJugadores[3].claseAuto.GetMesh());
            }

            if (CantidadDeOponentes >= 4)
            {
                listaJugadores.Add(new Jugador("patrullero", MediaDir, 4));
                this.listaJugadores[4].claseAuto.SetMesh(loader.loadSceneFromFile(MediaDir + "Vehiculos\\Patrullero\\Patrullero-TgcScene.xml").Meshes[0]);
                this.listaJugadores[4].claseAuto.SetPositionMesh(new Vector3((-1) * (POSICION_VERTICE - CUADRANTE_SIZE * 4), 0, (-1) * (POSICION_VERTICE - CUADRANTE_SIZE * 4)), true);
                this.listaJugadores[4].claseAuto.SetRuedas(loader);
                this.listaJugadores[4].CreateCamera();
                GameModel.MeshAutos.Add(this.listaJugadores[4].claseAuto.GetMesh());
            }
        }

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
            bool MoverRuedas = false, Avanzar = false, Frenar = false, Izquierda = false, Derecha = false, Saltar = false;

            PreUpdate();

            //Valido las teclas que se presionaron
            if ((Input.keyDown(Key.Up) || Input.keyDown(Key.W)))
            {
                Avanzar = true;
            }

            if (Input.keyDown(Key.Left) || Input.keyDown(Key.A))
            {
                Izquierda = true;
                MoverRuedas = true;
            }
            else
            {
                if (Input.keyDown(Key.Right) || Input.keyDown(Key.D))
                {
                    Derecha = true;
                    MoverRuedas = true;
                }
            }

            if ((Input.keyDown(Key.Down) || Input.keyDown(Key.S)))
            {
                Frenar = true;
            }

            if (Input.keyPressed(Key.Space))
            {
                Saltar = true;
            }

            //Activo bounding box para debug
            ActivarBoundingBox();

            //Chequea si se solicitó cambiar el tipo de camara
            CambiarDeCamara();

            //Actualizo los jugadores
            foreach (var unJugador in this.listaJugadores)
            {
                if (unJugador.GetNroJugador() == 0)
                {
                    unJugador.Update(MoverRuedas, Avanzar, Frenar, Izquierda, Derecha, Saltar, ElapsedTime);
                }
                else
                {
                    //IA!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!111
                    //unJugador.Update(ElapsedTime);
                    unJugador.Update(false, false, false, false, false, false, ElapsedTime);
                }
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
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
            PreRender();

            //Dibuja un texto por pantalla
            DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.Red);
            DrawText.drawText("Con la tecla F1 se cambia el tipo de camara. Pos [Actual]: " + TgcParserUtils.printVector3(Camara.Position), 0, 30, Color.Red);

            //Dibujamos la escena
            ScenePpal.renderAll();

            //Renderizo los objetos cargados de las listas
            //Render de BoundingBox, muy útil para debug de colisiones.
            if (BoundingBox)
            {
                RenderizarObjetos(1);

                foreach (var unJugador in this.listaJugadores)
                {
                    unJugador.claseAuto.RenderObb();
                }
            }
            else
            {
                RenderizarObjetos(0);
            }

            //Dibujo los jugadores
            foreach (var unJugador in this.listaJugadores)
            {
                unJugador.Render();
            }

            //Dibujo el reloj
            this.claseTiempo.Render();

            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        //0: render
        //1: render y render bounding box
        //2: control de colisiones
        //3: render bounding box
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
                                mesh.render();
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
                        case 3:
                            {
                                mesh.BoundingBox.render();
                            }
                            break;

                    }
                }
            }

            return false;
        }

        private void RenderizarObjetos(int unaAccion)
        {
            //Renderizar objetos del escenario
            AccionarListaMesh(MeshPrincipal, unaAccion, null);

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
                            Camara = this.listaJugadores[0].claseCamara.GetCamera();
                        }
                        break;

                    case 1:
                        {
                            Camara = new TgcRotationalCamera (this.listaJugadores[0].claseAuto.GetBBCenter(), this.listaJugadores[0].claseAuto.GetBBRadius() * 2, Input);
                        }
                        break;
                }
            }
        }

        public override void Dispose()
        {
            //Borro los objetos de los jugadores
            foreach (var unJugador in this.listaJugadores)
            {
                unJugador.Dispose();
            }

            PalmeraOriginal.dispose();
            PinoOriginal.dispose();
            RocaOriginal.dispose();
            ArbolBananasOriginal.dispose();
            ScenePpal.disposeAll();
        }
    }
}