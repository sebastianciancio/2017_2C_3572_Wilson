
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
using System.Linq;

namespace TGC.Group.Model
{
    public class GameModel : TgcExample
    {
        // ***********************************************************
        // Parametros del HeightMap
        // ***********************************************************

        private TgcSimpleTerrain terrain;
        private const float sceneScaleY = 10f;
        private const float sceneScaleXZ = 25f;
        private Vector3 terrainCenter = new Vector3(0, 0, 0);
        private string sceneHeightmapPath;
        private string terrainTexturePath;
        private Bitmap HeightmapSize;

        // Máximo valor del Heightmap
        private int maxValorHeightmap;

        // Punto más alto ya escalado de la Escena 
        private const int MAXIMA_ALTURA_ESCENA = 1000;

        // ***********************************************************
        // Parametros del SkyBox
        // ***********************************************************

        private TgcSkyBox skyBox;
        private string skyTexturePath;
        private Vector3 skyBoxCenter = new Vector3(0, 0, 0);
        private Vector3 skyBoxSize = new Vector3(800 * sceneScaleXZ, 800 * sceneScaleXZ, 800 * sceneScaleXZ);
        private const float skyBoxSkyEpsilon = 30f;

        // ***********************************************************
        // Parametros de los elementos Mesh del Escenario
        // ***********************************************************

        private TgcSceneLoader loader;
        private bool ShowBoundingBox { get; set; }
        private string palmMeshPath;
        private string rockMeshPath;
        private string plantMeshPath;

        private TgcMesh palmModel;
        private TgcMesh rockModel;
        private TgcMesh plantModel;
        private List<TgcMesh> sceneMeshes;


        // ***********************************************************

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;

            loader = new TgcSceneLoader();
            sceneMeshes = new List<TgcMesh>();

            sceneHeightmapPath = MediaDir + "Isla\\isla_heightmap.png";
            terrainTexturePath = MediaDir + "Isla\\isla_textura2.png";
            palmMeshPath = MediaDir + "Palmera\\Palmera-TgcScene.xml";
            rockMeshPath = MediaDir + "Roca\\Roca-TgcScene.xml";
            plantMeshPath = MediaDir + "Planta3\\Planta3-TgcScene.xml";
            skyTexturePath = MediaDir + "SkyBox\\";

            HeightmapSize = new Bitmap(sceneHeightmapPath);
        }

        public override void Init()
        {
            LoadScene();
            InitCamera();

            // ***************************************************************************************
            // La ubicacion de los Mesh es en coordenadas Originales del HeightMap (sin escalado) [-256,256]
            // **************************************************************************************

            palmModel = loader.loadSceneFromFile(palmMeshPath).Meshes[0];
            CreateObjectsFromModel(palmModel, 200, new Vector3(100, 0, -90), new Vector3(0.5f, 0.5f, 0.5f), 80);

            rockModel = loader.loadSceneFromFile(rockMeshPath).Meshes[0];
            CreateObjectsFromModel(rockModel, 250, new Vector3(-56, 0, 32), new Vector3(0.5f, 0.5f, 0.5f), 50);

            plantModel = loader.loadSceneFromFile(plantMeshPath).Meshes[0];
            CreateObjectsFromModel(plantModel, 200, new Vector3(-30, 0, -70), new Vector3(0.4f, 0.4f, 0.4f), 80);

        }

        public override void Update()
        {
            PreUpdate();

            // SkyBox: Se cambia el valor por defecto del farplane para evitar cliping de farplane.
            D3DDevice.Instance.Device.Transform.Projection =
                Matrix.PerspectiveFovLH(D3DDevice.Instance.FieldOfView,
                    D3DDevice.Instance.AspectRatio,
                    D3DDevice.Instance.ZNearPlaneDistance,
                    D3DDevice.Instance.ZFarPlaneDistance * 2f);

            Camara.UpdateCamera(ElapsedTime);
        }

        public override void Render()
        {
            PreRender();
            RenderTerrain();
            RenderSceneMeshes();
            RenderHelpText();
            RenderSkyBox();
            PostRender();
        }

        public override void Dispose()
        {
            terrain.dispose();

            // Se liberan los elementos de la escena
            palmModel.dispose();
            rockModel.dispose();
            plantModel.dispose();

            // Se libera Lista de Mesh
            sceneMeshes.Clear();

            //Liberar recursos del SkyBox
            skyBox.dispose();
        }

        private void CreateObjectsFromModel(TgcMesh model, int count, Vector3 center, Vector3 scale, int sparse)
        {
            var rnd = new Random();

            // TODO: buscar una mejor forma de tener una distribucion pareja
            var rows = (int)Math.Sqrt(count);
            var cols = (int)Math.Sqrt(count);


            float[] scalaVariableObjetos = { 1f, 1.5f, 2f, 2.5f };

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    var instance = model.createMeshInstance(model.Name + i + "_" + j);
                    instance.AutoTransformEnable = true;

                    // Escalo el objeto en forma Random
                    instance.Scale = scale * scalaVariableObjetos[rnd.Next(0, scalaVariableObjetos.Length)];

                    var x = center.X + rnd.Next(-sparse, sparse);
                    var z = center.Z + rnd.Next(-sparse, sparse);

                    // Posiciono el objeto en el Escenario
                    instance.Position = new Vector3( x * sceneScaleXZ ,CalcularAlturaTerreno(x, z) * sceneScaleY, z * sceneScaleXZ);

                    // Lo guardo en una Lista de Objetos que están en el Escenario
                    sceneMeshes.Add(instance);
                }
            }
        }

        private void LoadScene()
        {
            //Cargar Heightmap y textura de la Escena
            terrain = new TgcSimpleTerrain();
            terrain.loadHeightmap(sceneHeightmapPath, sceneScaleXZ, sceneScaleY, terrainCenter);
            terrain.loadTexture(terrainTexturePath);
            terrain.AlphaBlendEnable = true;

            // Calculo el máximo valor del Heightmap
            maxValorHeightmap = terrain.HeightmapData.Cast<int>().Max();

            // Cargo el SkyBox
            CreateSkyBox();
        }

        private void CreateSkyBox()
        {
            //Crear SkyBox
            skyBox = new TgcSkyBox();
            skyBox.Center = skyBoxCenter;
            skyBox.Size = skyBoxSize;

            //Configurar las texturas para cada una de las 6 caras
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, skyTexturePath + "lostatseaday_up.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, skyTexturePath + "lostatseaday_dn.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, skyTexturePath + "lostatseaday_lf.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, skyTexturePath + "lostatseaday_rt.jpg");

            //Hay veces es necesario invertir las texturas Front y Back si se pasa de un sistema RightHanded a uno LeftHanded
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, skyTexturePath + "lostatseaday_bk.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, skyTexturePath + "lostatseaday_ft.jpg");
            skyBox.SkyEpsilon = skyBoxSkyEpsilon;

            //Inicializa todos los valores para crear el SkyBox
            skyBox.Init();
        }

        // Las coordenas x,z son Originales (sin Escalado) y el z devuelto es Original también (sin Escalado)
        public float CalcularAlturaTerreno(float x, float z)
        {
            // Calculo las coordenadas en la Matriz de Heightmap
            var pos_i = x + (HeightmapSize.Width / 2);
            var pos_j = z + (HeightmapSize.Width / 2);

            var pi = (int)pos_i;
            var fracc_i = pos_i - pi;
            var pj = (int)pos_j;
            var fracc_j = pos_j - pj;

            if (pi < 0)
                pi = 0;
            else if (pi > (HeightmapSize.Width-1))
                pi = (HeightmapSize.Width-1);

            if (pj < 0)
                pj = 0;
            else if (pj > (HeightmapSize.Width-1))
                pj = (HeightmapSize.Width-1);

            var pi1 = pi + 1;
            var pj1 = pj + 1;
            if (pi1 > (HeightmapSize.Width-1))
                pi1 = (HeightmapSize.Width-1);
            if (pj1 > (HeightmapSize.Width-1))
                pj1 = (HeightmapSize.Width-1);

            // 2x2 percent closest filtering usual:
            var H0 = terrain.HeightmapData[pi, pj];
            var H1 = terrain.HeightmapData[pi1, pj];
            var H2 = terrain.HeightmapData[pi, pj1];
            var H3 = terrain.HeightmapData[pi1, pj1];
            var H = (H0 * (1 - fracc_i) + H1 * fracc_i) * (1 - fracc_j) +
                    (H2 * (1 - fracc_i) + H3 * fracc_i) * fracc_j;

            return H;
        }

        private void InitCamera()
        {
            // Usar Coordenadas Originales del HeightMap [-256,256]
            var posicionCamaraX = 0;
            var posicionCamaraZ = 0;

            var cameraPosition = new Vector3(posicionCamaraX * sceneScaleXZ, (CalcularAlturaTerreno(posicionCamaraX, posicionCamaraZ) + 1 )* sceneScaleY, posicionCamaraZ * sceneScaleXZ);
            var cameraLookAt = new Vector3(0, 0, -1);
            var cameraMoveSpeed = 500f;
            var cameraJumpSpeed = 500f;

            // Creo la cámara y defino la Posición y LookAt
            Camara = new TgcFpsCamera(cameraPosition,cameraMoveSpeed,cameraJumpSpeed,Input);
            Camara.SetCamera(cameraPosition, cameraLookAt);
        }

        private void RenderSkyBox()
        {
            //Renderizar SkyBox
            skyBox.render();
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
            terrain.render();
        }

        private void RenderHelpText()
        {
            DrawText.drawText("Camera position: \n" + Camara.Position, 0, 20, Color.OrangeRed);
            DrawText.drawText("Camera LookAt: \n" + Camara.LookAt, 0, 100, Color.OrangeRed);
            DrawText.drawText("Mesh count: \n" + sceneMeshes.Count, 0, 180, Color.OrangeRed);
        }

    }
}