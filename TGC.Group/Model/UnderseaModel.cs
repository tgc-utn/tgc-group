using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Group.Camera;

namespace TGC.Group.Model
{
    public class UnderseaModel : TgcExample
    {
        private TgcScene tgcScene;
        private TgcPlane floorMesh;
        private string currentHeightmap;
        private float currentScaleXZ;
        private float currentScaleY;
        private int totalVertices;
        private VertexBuffer vbTerrain;
        private string currentTexture;
        private Texture terrainTexture;

        public UnderseaModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public override void Init()
        {
            //Camara
            Camara = new FpsCamera(new TGCVector3(3675f, 495f, 418f), Input);
            //Camara.SetCamera(new TGCVector3(3675f, 495f, 418f), new TGCVector3(3715f, 494f, 429f));

            //Heightmap
            currentHeightmap = MediaDir + "\\Level1\\Heigthmap\\" + "hm_level1.jpg";
            currentScaleXZ = 50f;
            currentScaleY = 1.5f;
            createHeightMapMesh(D3DDevice.Instance.Device, currentHeightmap, currentScaleXZ, currentScaleY);

            //Texture
            currentTexture = MediaDir + "\\Level1\\Textures\\" + "level1.PNG";
            loadTerrainTexture(D3DDevice.Instance.Device, currentTexture);

            //Scene
            var loader = new TgcSceneLoader();
            tgcScene = loader.loadSceneFromFile(MediaDir + "\\Level1\\Meshes\\vg_level1-TgcScene.xml");

            //var floorTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "\\Scene\\Textures\\Captura.PNG");
            //floorMesh = new TgcPlane(new TGCVector3(-2000, 0, -2000), new TGCVector3(4000, 0f, 4000), TgcPlane.Orientations.XZplane, floorTexture);
        }

        private void createHeightMapMesh(Device d3dDevice, string path, float scaleXZ, float scaleY)
        {
            //parsear bitmap y cargar matriz de alturas
            var heightmap = loadHeightMap(path);

            //Crear vertexBuffer
            totalVertices = 2 * 3 * (heightmap.GetLength(0) - 1) * (heightmap.GetLength(1) - 1);
            vbTerrain = new VertexBuffer(typeof(CustomVertex.PositionTextured), totalVertices, d3dDevice, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);

            //Crear array temporal de vertices
            var dataIdx = 0;
            var data = new CustomVertex.PositionTextured[totalVertices];

            //Iterar sobre toda la matriz del Heightmap y crear los triangulos necesarios para el terreno
            for (var i = 0; i < heightmap.GetLength(0) - 1; i++)
            {
                for (var j = 0; j < heightmap.GetLength(1) - 1; j++)
                {
                    //Crear los cuatro vertices que conforman este cuadrante, aplicando la escala correspondiente
                    var v1 = new TGCVector3(i * scaleXZ, heightmap[i, j] * scaleY, j * scaleXZ);
                    var v2 = new TGCVector3(i * scaleXZ, heightmap[i, j + 1] * scaleY, (j + 1) * scaleXZ);
                    var v3 = new TGCVector3((i + 1) * scaleXZ, heightmap[i + 1, j] * scaleY, j * scaleXZ);
                    var v4 = new TGCVector3((i + 1) * scaleXZ, heightmap[i + 1, j + 1] * scaleY, (j + 1) * scaleXZ);

                    //Crear las coordenadas de textura para los cuatro vertices del cuadrante
                    var t1 = new TGCVector2(i / (float)heightmap.GetLength(0), j / (float)heightmap.GetLength(1));
                    var t2 = new TGCVector2(i / (float)heightmap.GetLength(0), (j + 1) / (float)heightmap.GetLength(1));
                    var t3 = new TGCVector2((i + 1) / (float)heightmap.GetLength(0), j / (float)heightmap.GetLength(1));
                    var t4 = new TGCVector2((i + 1) / (float)heightmap.GetLength(0), (j + 1) / (float)heightmap.GetLength(1));

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

        private int[,] loadHeightMap(string path)
        {
            //Cargar bitmap desde el FileSystem
            var bitmap = (Bitmap)Image.FromFile(path);
            var width = bitmap.Size.Width;
            var height = bitmap.Size.Height;
            var heightmap = new int[width, height];

            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    //Obtener color
                    //(j, i) invertido para primero barrer filas y despues columnas
                    var pixel = bitmap.GetPixel(j, i);

                    //Calcular intensidad en escala de grises
                    var intensity = pixel.R * 0.299f + pixel.G * 0.587f + pixel.B * 0.114f;
                    heightmap[i, j] = (int)intensity;
                }
            }

            return heightmap;
        }

        private void loadTerrainTexture(Device d3dDevice, string path)
        {
            //Rotar e invertir textura
            var b = (Bitmap)Image.FromFile(path);
            b.RotateFlip(RotateFlipType.Rotate90FlipX);
            terrainTexture = Texture.FromBitmap(d3dDevice, b, Usage.None, Pool.Managed);
        }

        public override void Update()
        {
            PreUpdate();
            PostUpdate();
        }

        public override void Render()
        {
            PreRender();

            //Render terrain
            D3DDevice.Instance.Device.SetTexture(0, terrainTexture);
            D3DDevice.Instance.Device.VertexFormat = CustomVertex.PositionTextured.Format;
            D3DDevice.Instance.Device.SetStreamSource(0, vbTerrain, 0);
            D3DDevice.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, totalVertices / 3);

            tgcScene.RenderAll();

            PostRender();
        }

        public override void Dispose()
        {
            vbTerrain.Dispose();
            terrainTexture.Dispose();
            tgcScene.DisposeAll();
        }
    }
}
