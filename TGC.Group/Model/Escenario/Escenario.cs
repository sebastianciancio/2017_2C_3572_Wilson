using Microsoft.DirectX;
using System.Drawing;
using TGC.Core.SceneLoader; 
using TGC.Core.Utils;
using System.Collections.Generic;
using TGC.Core.Terrain;
using System;


namespace TGC.Group.Model.EscenarioGame
{
    public class Escenario
    {
        public float SceneScaleXZ { get; set; }
        public float SceneScaleY { get; set; }

        public float Temperatura { get; set; }

        // ***********************************************************
        // Parametros del HeightMap
        // ***********************************************************

        private TgcSimpleTerrain terrain;
        private Vector3 terrainCenter = new Vector3(0, 0, 0);
        private string sceneHeightmapPath;
        private string terrainTexturePath;
        public Bitmap HeightmapSize { get; set; }

        // ***********************************************************
        // Parametros del SkyBox
        // ***********************************************************

        private TgcSkyBox skyBox;
        private string skyTexturePath;
        private Vector3 skyBoxCenter;
        private Vector3 skyBoxSize;
        private float skyBoxSkyEpsilon;

        // ***********************************************************
        // Parametros de los elementos Mesh del Escenario
        // ***********************************************************

        private TgcSceneLoader loader;
        private bool ShowBoundingBox { get; set; }
        private string palmMeshPath;
        private string rockMeshPath;
        private string plantMeshPath;
        private string arbolMeshPath;
        private string arbolFrutalMeshPath;
        private string frutaMeshPath;
        private string pinoMeshPath;
        private string palm2MeshPath;
        private string palm3MeshPath;

        private TgcMesh palmModel;
        private TgcMesh rockModel;
        private TgcMesh plantModel;
        private TgcMesh arbolModel;
        private TgcMesh arbolFrutalModel;
        private TgcMesh frutaModel;
        private TgcMesh pinoModel;
        private TgcMesh palm2Model;
        private TgcMesh palm3Model;

        public List<TgcMesh> SceneMeshes { get; set; }
        private GameModel env;        

        // ***********************************************************

        public Escenario(GameModel env)
        {
            this.env = env;

            sceneHeightmapPath = env.MediaDir + "Isla\\isla_heightmap.png";
            terrainTexturePath = env.MediaDir + "Isla\\isla_textura.png";
            palmMeshPath = env.MediaDir + "Palmera\\Palmera-TgcScene.xml";
            rockMeshPath = env.MediaDir + "Roca\\Roca-TgcScene.xml";
            plantMeshPath = env.MediaDir + "Planta3\\Planta3-TgcScene.xml";
            arbolMeshPath = env.MediaDir + "ArbolSelvatico\\ArbolSelvatico-TgcScene.xml";
            arbolFrutalMeshPath = env.MediaDir + "ArbustoFruta\\Peach-TgcScene.xml";
            frutaMeshPath = env.MediaDir + "Fruta\\Fruta-TgcScene.xml";
            pinoMeshPath = env.MediaDir + "Pino\\Pino-TgcScene.xml";
            palm2MeshPath = env.MediaDir + "Palmera2\\Palmera2-TgcScene.xml";
            palm3MeshPath = env.MediaDir + "Palmera3\\Palmera3-TgcScene.xml";
            skyTexturePath = env.MediaDir + "SkyBox\\";
        }

        public void Init()
        {
            SceneScaleY = 400f;
            SceneScaleXZ = 2000f;

            // Inicializo el SkyBox
            skyBoxCenter = new Vector3(0, 0, 0);
            skyBoxSize = new Vector3(500 * SceneScaleXZ, 500 * SceneScaleXZ, 500 * SceneScaleXZ);
            skyBoxSkyEpsilon = 30f;
            CreateSkyBox();

            //Cargar Heightmap y textura de la Escena
            HeightmapSize = new Bitmap(sceneHeightmapPath);
            terrain = new TgcSimpleTerrain();
            terrain.loadHeightmap(sceneHeightmapPath, SceneScaleXZ, SceneScaleY, terrainCenter);
            terrain.loadTexture(terrainTexturePath);
            terrain.AlphaBlendEnable = true;

            // La ubicacion de los Mesh es en coordenadas Originales del HeightMap (sin escalado) [-256,256]
            SceneMeshes = new List<TgcMesh>();
            loader = new TgcSceneLoader();

            palmModel = loader.loadSceneFromFile(palmMeshPath).Meshes[0];
            CreateObjectsFromModel(palmModel, 400, new Vector3(100, 0, -90), new Vector3(0.5f, 0.5f, 0.5f), 80);

            rockModel = loader.loadSceneFromFile(rockMeshPath).Meshes[0];
            CreateObjectsFromModel(rockModel, 350, new Vector3(-56, 0, 32), new Vector3(0.9f, 0.9f, 0.9f), 50);

            plantModel = loader.loadSceneFromFile(plantMeshPath).Meshes[0];
            CreateObjectsFromModel(plantModel, 70, new Vector3(-30, 0, -70), new Vector3(0.8f, 0.8f, 0.8f), 80);

            arbolModel = loader.loadSceneFromFile(arbolMeshPath).Meshes[0];
            CreateObjectsFromModel(arbolModel, 70, new Vector3(-30, 0, -10), new Vector3(0.8f, 0.8f, 0.8f), 80);

            arbolFrutalModel = loader.loadSceneFromFile(arbolFrutalMeshPath).Meshes[0];
            CreateObjectsFromModel(arbolFrutalModel, 70, new Vector3(-50, 0, -20), new Vector3(0.8f, 0.8f, 0.8f), 80);

            frutaModel = loader.loadSceneFromFile(frutaMeshPath).Meshes[0];
            CreateObjectsFromModel(frutaModel, 70, new Vector3(-40, 0, -30), new Vector3(0.8f, 0.8f, 0.8f), 80);

            pinoModel = loader.loadSceneFromFile(pinoMeshPath).Meshes[0];
            CreateObjectsFromModel(pinoModel, 70, new Vector3(-30, 0, -40), new Vector3(0.8f, 0.8f, 0.8f), 80);

            palm2Model = loader.loadSceneFromFile(palm2MeshPath).Meshes[0];
            CreateObjectsFromModel(palm2Model, 70, new Vector3(-20, 0, -50), new Vector3(0.8f, 0.8f, 0.8f), 80);

            palm3Model = loader.loadSceneFromFile(palm3MeshPath).Meshes[0];
            CreateObjectsFromModel(palm3Model, 70, new Vector3(-10, 0, -60), new Vector3(0.8f, 0.8f, 0.8f), 80);
        }

        public void Update()
        {

        }

        public void Render()
        {
            skyBox.render();
            RenderSceneMeshes();
            terrain.render();
        }

        public void Dispose()
        {
            // Se libera el Terreno
            terrain.dispose();

            // Se liberan los elementos de la escena
            palmModel.dispose();
            rockModel.dispose();
            plantModel.dispose();
            arbolModel.dispose();
            arbolFrutalModel.dispose();
            frutaModel.dispose();
            pinoModel.dispose();
            palm2Model.dispose();
            palm3Model.dispose();

            // Se libera Lista de Mesh
            SceneMeshes.Clear();

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
            float[] scalaRotacionObjetos = { FastMath.QUARTER_PI, FastMath.PI, FastMath.PI_HALF, FastMath.TWO_PI };

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
                    instance.Position = new Vector3( x * SceneScaleXZ ,CalcularAlturaTerreno(x, z) * SceneScaleY, z * SceneScaleXZ);
                    instance.rotateY(scalaRotacionObjetos[rnd.Next(0, scalaRotacionObjetos.Length)]);
                    instance.AlphaBlendEnable = true;

                    // Lo guardo en una Lista de Objetos que están en el Escenario
                    SceneMeshes.Add(instance);
                }
            }
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

        private void RenderSceneMeshes()
        {
            foreach (var mesh in SceneMeshes)
            {
                mesh.render();
            }
        }
    }
}