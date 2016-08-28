using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.Utils;
using TGC.Group.Camera;

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

        ////Caja que se muestra en el ejemplo.
        //private TgcBox Box { get; set; }

        ////Mesh de TgcLogo.
        //private TgcMesh Mesh { get; set; }

        ////Boleano para ver si dibujamos el boundingbox
        //private bool BoundingBox { get; set; }

        private string terrainHeightMap;
        private string terrainTexturePath;
        private float scaleXZ;
        private float scaleY;
        private Texture terrainTexture;
        private int totalVertices;
        private VertexBuffer vbTerrain;

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

            //Heighmap del terreno
            terrainHeightMap = MediaDir + Game.Default.Heighmap;
            terrainTexturePath = MediaDir + Game.Default.TerrainTexture;
            scaleXZ = 50f;
            scaleY = 1.5f;

            //Mesh del mapa
            createHeightMapMesh(d3dDevice);

            //Textura del terreno
            loadTerrainTexture(d3dDevice);

            Camara = new FpsCamera(new Vector3(3200f, 450f, 1500f), Input);

            /*
            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;

            //Textura de la carperta Media. Game.Default es un archivo de configuracion (Game.settings) util para poner cosas.
            //Pueden abrir el Game.settings que se ubica dentro de nuestro proyecto para configurar.
            var pathTexturaCaja = MediaDir + Game.Default.TexturaCaja;

            //Cargamos una textura, tener en cuenta que cargar una textura significa crear una copia en memoria.
            //Es importante cargar texturas en Init, si se hace en el render loop podemos tener grandes problemas si instanciamos muchas.
            var texture = TgcTexture.createTexture(pathTexturaCaja);

            //Creamos una caja 3D ubicada de dimensiones (5, 10, 5) y la textura como color.
            var size = new Vector3(5, 10, 5);
            //Construimos una caja según los parámetros, por defecto la misma se crea con centro en el origen y se recomienda así para facilitar las transformaciones.
            Box = TgcBox.fromSize(size, texture);
            //Posición donde quiero que este la caja, es común que se utilicen estructuras internas para las transformaciones.
            //Entonces actualizamos la posición lógica, luego podemos utilizar esto en render para posicionar donde corresponda con transformaciones.
            Box.Position = new Vector3(-25, 0, 0);

            //Cargo el unico mesh que tiene la escena.
            Mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + "LogoTGC-TgcScene.xml").Meshes[0];
            //Defino una escala en el modelo logico del mesh que es muy grande.
            Mesh.Scale = new Vector3(0.5f, 0.5f, 0.5f);

            //Suelen utilizarse objetos que manejan el comportamiento de la camara.
            //Lo que en realidad necesitamos gráficamente es una matriz de View.
            //El framework maneja una cámara estática, pero debe ser inicializada.
            //Posición de la camara.
            var cameraPosition = new Vector3(0, 0, 125);
            //Quiero que la camara mire hacia el origen (0,0,0).
            var lookAt = Vector3.Empty;
            //Configuro donde esta la posicion de la camara y hacia donde mira.
            Camara.SetCamera(cameraPosition, lookAt);
            //Internamente el framework construye la matriz de view con estos dos vectores.
            //Luego en nuestro juego tendremos que crear una cámara que cambie la matriz de view con variables como movimientos o animaciones de escenas.
            */
        }

        private void createHeightMapMesh(Microsoft.DirectX.Direct3D.Device d3dDevice)
        {
            //parsear bitmap y cargar matriz de alturas
            var mHeightMap = loadHeightMap();

            //Crear vertexBuffer
            totalVertices = 2 * 3 * (mHeightMap.GetLength(0) - 1) * (mHeightMap.GetLength(1) - 1);
            vbTerrain = new VertexBuffer(typeof(CustomVertex.PositionTextured), totalVertices, d3dDevice,
                Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);

            //Crear array temporal de vértices
            var dataIdx = 0;
            var data = new CustomVertex.PositionTextured[totalVertices];

            //Iterar sobre toda la matriz del Heightmap y crear los triangulos necesarios para el terreno
            for (var i = 0; i < mHeightMap.GetLength(0) - 1; i++)
            {
                for (var j = 0; j < mHeightMap.GetLength(1) - 1; j++)
                {
                    //Crear los cuatro vertices que conforman este cuadrante, aplicando la escala correspondiente
                    var v1 = new Vector3(i * scaleXZ, mHeightMap[i, j] * scaleY, j * scaleXZ);
                    var v2 = new Vector3(i * scaleXZ, mHeightMap[i, j + 1] * scaleY, (j + 1) * scaleXZ);
                    var v3 = new Vector3((i + 1) * scaleXZ, mHeightMap[i + 1, j] * scaleY, j * scaleXZ);
                    var v4 = new Vector3((i + 1) * scaleXZ, mHeightMap[i + 1, j + 1] * scaleY, (j + 1) * scaleXZ);

                    //Crear las coordenadas de textura para los cuatro vertices del cuadrante
                    var t1 = new Vector2(i / (float)mHeightMap.GetLength(0), j / (float)mHeightMap.GetLength(1));
                    var t2 = new Vector2(i / (float)mHeightMap.GetLength(0), (j + 1) / (float)mHeightMap.GetLength(1));
                    var t3 = new Vector2((i + 1) / (float)mHeightMap.GetLength(0), j / (float)mHeightMap.GetLength(1));
                    var t4 = new Vector2((i + 1) / (float)mHeightMap.GetLength(0), (j + 1) / (float)mHeightMap.GetLength(1));

                    //Cargar triangulo 1
                    data[dataIdx] = new CustomVertex.PositionTextured(v1, t1.X, t1.Y);
                    data[dataIdx + 1] = new CustomVertex.PositionTextured(v2, t2.X, t2.Y);
                    data[dataIdx + 2] = new CustomVertex.PositionTextured(v4, t4.X, t4.Y);

                    //Cargar triangulo 2
                    data[dataIdx + 3] = new CustomVertex.PositionTextured(v1, t1.X, t1.Y);
                    data[dataIdx + 4] = new CustomVertex.PositionTextured(v4, t4.X, t4.Y);
                    data[dataIdx + 5] = new CustomVertex.PositionTextured(v3, t3.X, t3.Y);

                    dataIdx += 6;
                }
            }

            //Llenar todo el VertexBuffer con el array temporal
            vbTerrain.SetData(data, 0, LockFlags.None);
        }

        private int[,] loadHeightMap()
        {
            //Cargar bitmap desde el file system
            var bmpHeightMap = (Bitmap)Image.FromFile(terrainHeightMap);
            var iWidth = bmpHeightMap.Size.Width;
            var iHeight = bmpHeightMap.Size.Height;
            var mHeightmap = new int[iWidth, iHeight];

            for (var i = 0; i < iWidth; i++)
            {
                for (var j = 0; j < iHeight; j++)
                {
                    //Obtener color
                    //(j,i) invertido para primero barrer filas y después columnas
                    var clrPixel = bmpHeightMap.GetPixel(j, i);

                    //Calcular intensidad en escala de grises
                    var fIntensity = clrPixel.R * 0.299f + clrPixel.G * 0.587f + clrPixel.B * 0.114f;
                    mHeightmap[i, j] = (int)fIntensity;
                }
            }

            return mHeightmap;
        }

        private void loadTerrainTexture(Microsoft.DirectX.Direct3D.Device d3dDevice)
        {
            var bmpBitmap = (Bitmap)Image.FromFile(terrainTexturePath);
            bmpBitmap.RotateFlip(RotateFlipType.Rotate90FlipX);
            terrainTexture = Texture.FromBitmap(d3dDevice, bmpBitmap, Usage.None, Pool.Managed);
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        public override void Update()
        {
            PreUpdate();

            ////Capturar Input teclado
            //if (Input.keyPressed(Key.F))
            //{
            //    BoundingBox = !BoundingBox;
            //}

            ////Capturar Input Mouse
            if (Input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Como ejemplo podemos hacer un movimiento simple de la cámara.
                //En este caso le sumamos un valor en Y
                Camara.SetCamera(Camara.Position + new Vector3(0, 10f, 0), Camara.LookAt);
                //Ver ejemplos de cámara para otras operaciones posibles.

                //Si superamos cierto Y volvemos a la posición original.
                if (Camara.Position.Y > 300f)
                {
                    Camara.SetCamera(new Vector3(Camara.Position.X, 0f, Camara.Position.Z), Camara.LookAt);
                }
            }
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
            createHeightMapMesh(D3DDevice.Instance.Device);

            DrawText.drawText("Camera pos: " + TgcParserUtils.printVector3(Camara.Position), 5, 20, Color.Red);
            DrawText.drawText("Camera LookAt: " + TgcParserUtils.printVector3(Camara.LookAt), 5, 40, Color.Red);

            //Render terrain
            D3DDevice.Instance.Device.SetTexture(0, terrainTexture);
            D3DDevice.Instance.Device.VertexFormat = CustomVertex.PositionTextured.Format;
            D3DDevice.Instance.Device.SetStreamSource(0, vbTerrain, 0);
            D3DDevice.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, totalVertices / 3);

            PostRender();

            ////Dibuja un texto por pantalla
            //DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.OrangeRed);
            //DrawText.drawText(
            //    "Con clic izquierdo subimos la camara [Actual]: " + TgcParserUtils.printVector3(Camara.Position), 0, 30,
            //    Color.OrangeRed);

            ////Siempre antes de renderizar el modelo necesitamos actualizar la matriz de transformacion.
            ////Debemos recordar el orden en cual debemos multiplicar las matrices, en caso de tener modelos jerárquicos, tenemos control total.
            //Box.Transform = Matrix.Scaling(Box.Scale) *
            //                Matrix.RotationYawPitchRoll(Box.Rotation.Y, Box.Rotation.X, Box.Rotation.Z) *
            //                Matrix.Translation(Box.Position);
            ////A modo ejemplo realizamos toda las multiplicaciones, pero aquí solo nos hacia falta la traslación.
            ////Finalmente invocamos al render de la caja
            //Box.render();

            ////Cuando tenemos modelos mesh podemos utilizar un método que hace la matriz de transformación estándar.
            ////Es útil cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones jerárquicas o complicadas.
            //Mesh.UpdateMeshTransform();
            ////Render del mesh
            //Mesh.render();

            ////Render de BoundingBox, muy útil para debug de colisiones.
            //if (BoundingBox)
            //{
            //    Box.BoundingBox.render();
            //    Mesh.BoundingBox.render();
            //}

            ////Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            //PostRender();
        }

        /// <summary>
        ///     Se llama cuando termina la ejecución del ejemplo.
        ///     Hacer Dispose() de todos los objetos creados.
        ///     Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        /// </summary>
        public override void Dispose()
        {
            vbTerrain.Dispose();
            terrainTexture.Dispose();
            //Dispose de la caja.
            //Box.dispose();
            ////Dispose del mesh.
            //Mesh.dispose();
        }
    }
}