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
using TGC.Group.Model.Camara;
using TGC.Core.Camara;


namespace TGC.Group.Model.EscenarioGame
{
    public class Escenario
    {
        private string MediaDir;
        private float ElapsedTime;
        private float sceneScaleXZ;
        private float sceneScaleY;
        private TgcCamera Camara;

        // ***********************************************************
        // Parametros del HeightMap
        // ***********************************************************

        private TgcSimpleTerrain terrain;
        private Vector3 terrainCenter = new Vector3(0, 0, 0);
        private string sceneHeightmapPath;
        private string terrainTexturePath;
        private Bitmap HeightmapSize;

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

        private TgcScene loaderScene;

        private TgcMesh palmModel;
        private TgcMesh rockModel;
        private TgcMesh plantModel;
        private TgcMesh arbolModel;
        private TgcMesh arbolFrutalModel;
        private TgcMesh frutaModel;
        private TgcMesh pinoModel;
        private TgcMesh palm2Model;
        private TgcMesh palm3Model;

        private List<TgcMesh> sceneMeshes;

        // ***********************************************************

        private static Escenario myInstance;
        private TgcD3dInput input;
        private TgcText2D draw;

        public static Escenario getInstance()
        {
            return myInstance;
        }


        public Escenario(GameModel env,TgcD3dInput input,TgcText2D draw)
        {    
            // Declaro las propiedades
            this.MediaDir = env.MediaDir;
            this.ElapsedTime = env.ElapsedTime;
            this.draw = draw;
            this.input = input;
            this.sceneScaleXZ = env.sceneScaleXZ;
            this.sceneScaleY = env.sceneScaleY;
            myInstance = this;
            var d3dDevice = D3DDevice.Instance.Device;
            this.Camara = env.Camara;


            // Inicializo el SkyBox
            skyBoxCenter = new Vector3(0, 128 * sceneScaleY, 0);
            skyBoxSize = new Vector3(1100 * sceneScaleXZ, 1100 * sceneScaleXZ, 1100 * sceneScaleXZ);
            skyBoxSkyEpsilon = 30f;



            loader = new TgcSceneLoader();
            sceneMeshes = new List<TgcMesh>();

            sceneHeightmapPath = this.MediaDir + "Isla\\isla_heightmap.png";
            terrainTexturePath = this.MediaDir + "Isla\\isla_textura.png";
            palmMeshPath = this.MediaDir + "Palmera\\Palmera-TgcScene.xml";
            rockMeshPath = this.MediaDir + "Roca\\Roca-TgcScene.xml";
            plantMeshPath = this.MediaDir + "Planta3\\Planta3-TgcScene.xml";
            arbolMeshPath = this.MediaDir + "ArbolSelvatico\\ArbolSelvatico-TgcScene.xml";
            arbolFrutalMeshPath = this.MediaDir + "ArbustoFruta\\Peach-TgcScene.xml";
            frutaMeshPath = this.MediaDir + "Fruta\\Fruta-TgcScene.xml";
            pinoMeshPath = this.MediaDir + "Pino\\Pino-TgcScene.xml";
            palm2MeshPath = this.MediaDir + "Palmera2\\Palmera2-TgcScene.xml";
            palm3MeshPath = this.MediaDir + "Palmera3\\Palmera3-TgcScene.xml";

            skyTexturePath = this.MediaDir + "SkyBox\\";

            HeightmapSize = new Bitmap(sceneHeightmapPath);

            LoadScene();

            // ***************************************************************************************
            // La ubicacion de los Mesh es en coordenadas Originales del HeightMap (sin escalado) [-256,256]
            // **************************************************************************************

            palmModel = loader.loadSceneFromFile(palmMeshPath).Meshes[0];
            CreateObjectsFromModel(palmModel, 200, new Vector3(100, 0, -90), new Vector3(0.5f, 0.5f, 0.5f), 80);

            rockModel = loader.loadSceneFromFile(rockMeshPath).Meshes[0];
            CreateObjectsFromModel(rockModel, 250, new Vector3(-56, 0, 32), new Vector3(0.9f, 0.9f, 0.9f), 50);

            plantModel = loader.loadSceneFromFile(plantMeshPath).Meshes[0];
            CreateObjectsFromModel(plantModel, 30, new Vector3(-30, 0, -70), new Vector3(0.8f, 0.8f, 0.8f), 80);

            arbolModel = loader.loadSceneFromFile(arbolMeshPath).Meshes[0];
            CreateObjectsFromModel(arbolModel, 30, new Vector3(-30, 0, -10), new Vector3(0.8f, 0.8f, 0.8f), 80);

            arbolFrutalModel = loader.loadSceneFromFile(arbolFrutalMeshPath).Meshes[0];
            CreateObjectsFromModel(arbolFrutalModel, 30, new Vector3(-50, 0, -20), new Vector3(0.8f, 0.8f, 0.8f), 80);

            frutaModel = loader.loadSceneFromFile(frutaMeshPath).Meshes[0];
            CreateObjectsFromModel(frutaModel, 30, new Vector3(-40, 0, -30), new Vector3(0.8f, 0.8f, 0.8f), 80);

            pinoModel = loader.loadSceneFromFile(pinoMeshPath).Meshes[0];
            CreateObjectsFromModel(pinoModel, 30, new Vector3(-30, 0, -40), new Vector3(0.8f, 0.8f, 0.8f), 80);

            palm2Model = loader.loadSceneFromFile(palm2MeshPath).Meshes[0];
            CreateObjectsFromModel(palm2Model, 30, new Vector3(-20, 0, -50), new Vector3(0.8f, 0.8f, 0.8f), 80);

            palm3Model = loader.loadSceneFromFile(palm3MeshPath).Meshes[0];
            CreateObjectsFromModel(palm3Model, 30, new Vector3(-10, 0, -60), new Vector3(0.8f, 0.8f, 0.8f), 80);


            // **************************************************************************************
        }


        public void Update()
        {
            
        }

        public void Render()
        {
            RenderTerrain();
            RenderSceneMeshes();
            RenderHelpText();
            RenderSkyBox();
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
                    instance.Position = new Vector3( x * sceneScaleXZ ,CalcularAlturaTerreno(x, z) * sceneScaleY, z * sceneScaleXZ);
                    instance.rotateY(scalaRotacionObjetos[rnd.Next(0, scalaRotacionObjetos.Length)]);
                    instance.AlphaBlendEnable = true;

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
            // Cargo el SkyBox
            CreateSkyBox();

            // Cargo HUD
            //spriteDrawer = new Drawer2D();
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
            
            this.draw.drawText("Camera position: \n" + Camara.Position, 0, 20, Color.OrangeRed);
            this.draw.drawText("Camera LookAt: \n" + Camara.LookAt, 0, 100, Color.OrangeRed);
            this.draw.drawText("Mesh count: \n" + sceneMeshes.Count, 0, 180, Color.OrangeRed);
            this.draw.drawText("Camera (Coordenada X Original): \n" + ((Camara.Position.X / sceneScaleXZ) - (HeightmapSize.Width / 2)), 200, 20, Color.OrangeRed);
            this.draw.drawText("Camera (Coordenada Z Original): \n" + ((Camara.Position.Z / sceneScaleXZ) + (HeightmapSize.Width / 2)), 200, 100, Color.OrangeRed);
        }



    }
}