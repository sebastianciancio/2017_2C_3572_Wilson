
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
using System;

namespace TGC.Group.Model
{
    public class GameModel : TgcExample
    {
        // ***********************************************************
        // Parametros del HeightMap
        // ***********************************************************

        private const float sceneScaleY = 5f;
        private const float sceneScaleXZ = 20f;
        private Texture terrainTexture;
        private VertexBuffer vbTerrain;
        private int totalVertices;

        // ***********************************************************

        private TgcSceneLoader loader;
        private bool ShowBoundingBox { get; set; }

        private string sceneHeightmapPath;
        private string terrainTexturePath;
        private string palmMeshPath;
        private string rockMeshPath;
        private string plantMeshPath;

        private TgcMesh palmModel;
        private TgcMesh rockModel;
        private TgcMesh plantModel;
        private List<TgcMesh> sceneMeshes;

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;

            loader = new TgcSceneLoader();
            sceneMeshes = new List<TgcMesh>();

            sceneHeightmapPath = MediaDir + "Isla\\isla_heightmap.png";
            terrainTexturePath = MediaDir + "Isla\\isla_textura.png";
            palmMeshPath = MediaDir + "Palmera\\Palmera-TgcScene.xml";
            rockMeshPath = MediaDir + "Roca\\Roca-TgcScene.xml";
            plantMeshPath = MediaDir + "Planta3\\Planta3-TgcScene.xml";
        }

        public override void Init()
        {
            LoadScene();
            InitCamera();

            palmModel = loader.loadSceneFromFile(palmMeshPath).Meshes[0];
            CreateObjectsFromModel(palmModel, 200, new Vector3(7700, 500, 4300), new Vector3(0.5f, 0.5f, 0.5f), 1000);

            rockModel = loader.loadSceneFromFile(rockMeshPath).Meshes[0];
            CreateObjectsFromModel(rockModel, 150, new Vector3(3500, 630, 5000), new Vector3(1.5f, 1.5f, 1.5f), 800);

            plantModel = loader.loadSceneFromFile(plantMeshPath).Meshes[0];
            CreateObjectsFromModel(plantModel, 180, new Vector3(5000, 590, 3000), new Vector3(1, 1, 1), 1200);
        }

        public override void Update()
        {
            PreUpdate();
            Camara.UpdateCamera(ElapsedTime);
        }

        public override void Render()
        {
            PreRender();
            RenderTerrain();
            RenderSceneMeshes();
            RenderHelpText();
            PostRender();
        }

        public override void Dispose()
        {
            vbTerrain.Dispose();
            terrainTexture.Dispose();
            palmModel.dispose();
            rockModel.dispose();
        }

        private void CreateObjectsFromModel(TgcMesh model, int count, Vector3 center, Vector3 scale, int sparse)
        {
            var rnd = new Random();

            // TODO: buscar una forma no tan chota de tener una distribucion pareja
            var rows = (int)Math.Sqrt(count);
            var cols = (int)Math.Sqrt(count);

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    var instance = model.createMeshInstance(model.Name + i + "_" + j);

                    instance.AutoTransformEnable = true;

                    // var heightMap = LoadHeightMap(sceneHeightmapPath);

                    var x = center.X + rnd.Next(-sparse, sparse);
                    var y = center.Y; // TODO: AJUSTAR ESTA VARIABLE A LA ALTURA DEL HEIGHTMAP EN LA POSICION ACTUAL
                    var z = center.Z + rnd.Next(-sparse, sparse);

                    instance.move(x, y, z);
                    instance.Scale = scale;

                    sceneMeshes.Add(instance);
                }
            }
        }

        private void RenderSceneMeshes()
        {
            foreach (var mesh in sceneMeshes)
            {
                mesh.render();
            }
        }

        private void RenderTerrain()
        {
            D3DDevice.Instance.Device.SetTexture(0, terrainTexture);
            D3DDevice.Instance.Device.VertexFormat = CustomVertex.PositionTextured.Format;
            D3DDevice.Instance.Device.SetStreamSource(0, vbTerrain, 0);
            D3DDevice.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, totalVertices / 3);
        }

        private void RenderHelpText()
        {
            DrawText.drawText("Camera position: \n" + Camara.Position, 0, 20, Color.OrangeRed);
            DrawText.drawText("Camera LookAt: \n" + Camara.LookAt, 0, 100, Color.OrangeRed);
            DrawText.drawText("Mesh count: \n" + sceneMeshes.Count, 0, 180, Color.OrangeRed);
        }

        private void LoadScene()
        {
            createHeightMapMesh(D3DDevice.Instance.Device, sceneHeightmapPath, sceneScaleXZ, sceneScaleY);
            LoadTerrainTexture(D3DDevice.Instance.Device, terrainTexturePath);
        }

        private void LoadTerrainTexture(Microsoft.DirectX.Direct3D.Device d3dDevice, string path)
        {
            //Rotar e invertir textura
            var b = (Bitmap)Image.FromFile(path);
            b.RotateFlip(RotateFlipType.Rotate90FlipX);
            terrainTexture = Texture.FromBitmap(d3dDevice, b, Usage.None, Pool.Managed);
        }

        private void InitCamera()
        {
            // Defino las matrices de Posición y LookAt
            var cameraPosition = new Vector3(10500, 1600, 4500);
            var cameraLookAt = new Vector3(0, 0, -1);
            var cameraMoveSpeed = 500f;
            var cameraJumpSpeed = 500f;

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
            var heightmap = LoadHeightMap(path);

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
        ///     Cargar Bitmap y obtener el valor en escala de gris de Y
        ///     para cada coordenada (x,z)
        /// </summary>
        private int[,] LoadHeightMap(string path)
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