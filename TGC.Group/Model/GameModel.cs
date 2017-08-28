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
using TGC.Core.Text;
using System.Collections.Generic;
using TGC.Core.Terrain;
using TGC.Core.UserControls;
using TGC.Core.UserControls.Modifier;
using TGC.Core.Sound;
using TGC.Core.Shaders;


namespace TGC.Group.Model
{
    public class GameModel : TgcExample
    {

        // ***********************************************************
        // Parametros del HeightMap
        // ***********************************************************
        
            private string currentHeightmap;
            private float currentScaleXZ;
            private float currentScaleY;
            private string currentTexture;
            private Texture terrainTexture;
            private int totalVertices;
            private VertexBuffer vbTerrain;
            
        // ***********************************************************

            //public TgcScene currentScene;
            private TgcSceneLoader loader;
            private bool ShowBoundingBox { get; set; }


        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;

            loader = new TgcSceneLoader();
        }

        public override void Init()
        {
            LoadScene();
            InitCamera();
        }

        public override void Update()
        {
            PreUpdate();
            Camara.UpdateCamera(ElapsedTime);
        }

        public override void Render()
        {
            PreRender();
            //currentScene.renderAll(ShowBoundingBox);


            //Ver si cambio el heightmap
            var selectedHeightmap = currentHeightmap;
            if (currentHeightmap != selectedHeightmap)
            {
                currentHeightmap = selectedHeightmap;
                createHeightMapMesh(D3DDevice.Instance.Device, currentHeightmap, currentScaleXZ, currentScaleY);
            }

            //Ver si cambio alguno de los valores de escala
            var selectedScaleXZ = currentScaleXZ;
            var selectedScaleY = currentScaleY;
            if (currentScaleXZ != selectedScaleXZ || currentScaleY != selectedScaleY)
            {
                currentScaleXZ = selectedScaleXZ;
                currentScaleY = selectedScaleY;
                createHeightMapMesh(D3DDevice.Instance.Device, currentHeightmap, currentScaleXZ, currentScaleY);
            }

            //Ver si cambio la textura del terreno
            var selectedTexture = currentTexture;
            if (currentTexture != selectedTexture)
            {
                currentTexture = selectedTexture;
                loadTerrainTexture(D3DDevice.Instance.Device, currentTexture);
            }

            //Render terrain
            D3DDevice.Instance.Device.SetTexture(0, terrainTexture);
            D3DDevice.Instance.Device.VertexFormat = CustomVertex.PositionTextured.Format;
            D3DDevice.Instance.Device.SetStreamSource(0, vbTerrain, 0);
            D3DDevice.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, totalVertices / 3);


            RenderHelpText();
            PostRender();
        }

        public override void Dispose()
        {
            //currentScene.disposeAll();

            vbTerrain.Dispose();
            terrainTexture.Dispose();
        }

        private void RenderHelpText()
        {
            DrawText.drawText("Camera position: \n" + Camara.Position, 0, 40, Color.OrangeRed);
            DrawText.drawText("Camera LookAt: \n" + Camara.LookAt, 200, 40, Color.OrangeRed);            
        }

        private void LoadScene()
        {
            //currentScene = loader.loadSceneFromFile(MediaDir + "Isla\\isla-TgcScene.xml");


            //Heighmap de la Isla
            currentHeightmap = MediaDir + "Isla\\isla_heightmap.png";

            currentScaleXZ = 50f;
            currentScaleY = 1.5f;
            createHeightMapMesh(D3DDevice.Instance.Device, currentHeightmap, currentScaleXZ, currentScaleY);

            //Textura de la Isla
            currentTexture = MediaDir + "Isla\\isla_textura.png";
            loadTerrainTexture(D3DDevice.Instance.Device, currentTexture);

        }

        private void InitCamera()
        {
            // Defino las matrices de Posición y LookAt
            var cameraPosition = new Vector3(10500, 400, 4500);
            var cameraLookAt = new Vector3(0, 0, -1);
            var cameraMoveSpeed = 200f;
            var cameraJumpSpeed = 200f;

            // Creo la cámara y defino la Posición y LookAt
            Camara = new TgcFpsCamera(cameraPosition,cameraMoveSpeed,cameraJumpSpeed,Input);
            Camara.SetCamera(cameraPosition, cameraLookAt);
        }




        /// <summary>
        ///     Crea y carga el VertexBuffer en base a una textura de Heightmap
        /// </summary>
        private void createHeightMapMesh(Microsoft.DirectX.Direct3D.Device d3dDevice, string path, float scaleXZ, float scaleY)
        {
            //parsear bitmap y cargar matriz de alturas
            var heightmap = loadHeightMap(path);

            //Crear vertexBuffer
            totalVertices = 2 * 3 * (heightmap.GetLength(0) - 1) * (heightmap.GetLength(1) - 1);
            vbTerrain = new VertexBuffer(typeof(CustomVertex.PositionTextured), totalVertices, d3dDevice,
                Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);

            //Crear array temporal de vertices
            var dataIdx = 0;
            var data = new CustomVertex.PositionTextured[totalVertices];

            //Iterar sobre toda la matriz del Heightmap y crear los triangulos necesarios para el terreno
            for (var i = 0; i < heightmap.GetLength(0) - 1; i++)
            {
                for (var j = 0; j < heightmap.GetLength(1) - 1; j++)
                {
                    //Crear los cuatro vertices que conforman este cuadrante, aplicando la escala correspondiente
                    var v1 = new Vector3(i * scaleXZ, heightmap[i, j] * scaleY, j * scaleXZ);
                    var v2 = new Vector3(i * scaleXZ, heightmap[i, j + 1] * scaleY, (j + 1) * scaleXZ);
                    var v3 = new Vector3((i + 1) * scaleXZ, heightmap[i + 1, j] * scaleY, j * scaleXZ);
                    var v4 = new Vector3((i + 1) * scaleXZ, heightmap[i + 1, j + 1] * scaleY, (j + 1) * scaleXZ);

                    //Crear las coordenadas de textura para los cuatro vertices del cuadrante
                    var t1 = new Vector2(i / (float)heightmap.GetLength(0), j / (float)heightmap.GetLength(1));
                    var t2 = new Vector2(i / (float)heightmap.GetLength(0), (j + 1) / (float)heightmap.GetLength(1));
                    var t3 = new Vector2((i + 1) / (float)heightmap.GetLength(0), j / (float)heightmap.GetLength(1));
                    var t4 = new Vector2((i + 1) / (float)heightmap.GetLength(0), (j + 1) / (float)heightmap.GetLength(1));

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

        /// <summary>
        ///     Cargar textura del Terreno
        /// </summary>
        private void loadTerrainTexture(Microsoft.DirectX.Direct3D.Device d3dDevice, string path)
        {
            //Rotar e invertir textura
            var b = (Bitmap)Image.FromFile(path);
            b.RotateFlip(RotateFlipType.Rotate90FlipX);
            terrainTexture = Texture.FromBitmap(d3dDevice, b, Usage.None, Pool.Managed);
        }

        /// <summary>
        ///     Cargar Bitmap y obtener el valor en escala de gris de Y
        ///     para cada coordenada (x,z)
        /// </summary>
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

    }
}