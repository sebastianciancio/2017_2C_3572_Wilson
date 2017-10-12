using Microsoft.DirectX;
using System.Drawing;
using TGC.Core.SceneLoader;
using TGC.Core.Utils;
using System.Collections.Generic;
using TGC.Core.Terrain;
using System;
using TGC.Core.Collision;
using TGC.Group.Model.Escenario.Objetos;
using TGC.Group.Model.Escenario;
using TGC.Examples.Optimization.Quadtree;
using TGC.Core.BoundingVolumes;
using Microsoft.DirectX.Direct3D;

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

        public TgcSkyBox[] skyBoxGame;
        private string skyTexturePath;
        private Vector3 skyBoxCenter;
        private Vector3 skyBoxSize;
        private float skyBoxSkyEpsilon;

        // ***********************************************************
        // Parametros de Quadtree
        // ***********************************************************

        public TgcBoundingAxisAlignBox terrainBoundingBox;
        private CustomVertex.PositionColored[] triangle;
        private Quadtree quadtree;

        // ***********************************************************
        // Parametros de los elementos Mesh del Escenario
        // ***********************************************************

        private TgcSceneLoader loader;
        private bool ShowBoundingBox { get; set; }

        //private string palmMeshPath;
        //private string rockMeshPath;
        //private string plantMeshPath;
        //private string arbolMeshPath;
        //private string arbolFrutalMeshPath;
        //private string frutaMeshPath;
        //private string pinoMeshPath;
        //private string palm2MeshPath;
        //private string palm3MeshPath;

        private Palmera palmModel;
        private Roca rockModel;
        //private Planta plantModel;
        private Arbol arbolModel;
        //private TgcMesh arbolFrutalModel;
        private Fruta frutaModel;
        private Pino pinoModel;
        //private TgcMesh palm2Model;
        //private TgcMesh palm3Model;

        public int totalMeshesRenderizados;

        // ***********************************************************
        public List<TgcMesh> SceneMeshes { get; set; }
        public List<ObjetoEscena> Destroyables { get; set; }
        private GameModel env;

        // ***********************************************************

        public Escenario(GameModel env)
        {
            this.env = env;
            skyBoxGame = new TgcSkyBox[4];

            sceneHeightmapPath = env.MediaDir + "Isla\\isla_heightmap.png";
            terrainTexturePath = env.MediaDir + "Isla\\pasto_textura.png";
            skyTexturePath = env.MediaDir + "SkyBox\\";

            //palmMeshPath = env.MediaDir + "Palmera\\Palmera-TgcScene.xml";
            //rockMeshPath = env.MediaDir + "Roca\\Roca-TgcScene.xml";
            //arbolMeshPath = env.MediaDir + "ArbolSelvatico\\ArbolSelvatico-TgcScene.xml";
            //frutaMeshPath = env.MediaDir + "Fruta\\Fruta-TgcScene.xml";
            //pinoMeshPath = env.MediaDir + "Pino\\Pino-TgcScene.xml";

            //plantMeshPath = env.MediaDir + "Planta3\\Planta3-TgcScene.xml";
            //arbolFrutalMeshPath = env.MediaDir + "ArbustoFruta\\Peach-TgcScene.xml";
            //palm2MeshPath = env.MediaDir + "Palmera2\\Palmera2-TgcScene.xml";
            //palm3MeshPath = env.MediaDir + "Palmera3\\Palmera3-TgcScene.xml";
        }

        public void Init()
        {
            // Inicializo las Escalas
            SceneScaleY = 40f;
            SceneScaleXZ = 200f;

            // Creo el SkyBox
            CreateSkyBox();

            //Cargar Heightmap y textura de la Escena
            HeightmapSize = new Bitmap(sceneHeightmapPath);
            terrain = new TgcSimpleTerrain();
            terrain.AlphaBlendEnable = true;
            terrain.loadHeightmap(sceneHeightmapPath, SceneScaleXZ, SceneScaleY, terrainCenter);
            terrain.loadTexture(terrainTexturePath);

            // La ubicacion de los Mesh es en coordenadas Originales del HeightMap (sin escalado) [-256,256]
            SceneMeshes = new List<TgcMesh>();
            Destroyables = new List<ObjetoEscena>();
            loader = new TgcSceneLoader();

            rockModel = new Roca(env);
            CreateObjectsFromModel(rockModel.mesh, 200, new Vector3(-160, 0, 20), new Vector3(0.9f, 0.9f, 0.9f), 90, new float[] { 30f, 35f, 40f, 45f });

            palmModel = new Palmera(env);
            CreateObjectsFromModel(palmModel.mesh, 150, new Vector3(-70, 0, -70), new Vector3(0.5f, 0.5f, 0.5f), 180, new float[] { 60f, 65f, 70f, 75f });

            arbolModel = new Arbol(env);
            //CreateObjectsFromModel(arbolModel.mesh, 40, new Vector3(75, 0, -75), new Vector3(0.8f, 0.8f, 0.8f), 75, new float[] { 50f, 55f, 60f, 65f });

            frutaModel = new Fruta(env);
            CreateObjectsFromModel(frutaModel.mesh, 70, new Vector3(-90, 0, 75), new Vector3(0.8f, 0.8f, 0.8f), 120, new float[] { 2f, 2f, 2f, 2f });

            pinoModel = new Pino(env);
            CreateObjectsFromModel(pinoModel.mesh, 70, new Vector3(-75, 0, 75), new Vector3(0.8f, 0.8f, 0.8f), 120, new float[] { 50, 55f, 60f, 65f });

            //plantModel = loader.loadSceneFromFile(plantMeshPath).Meshes[0];
            //CreateObjectsFromModel(plantModel, 70, new Vector3(75, 0, -75), new Vector3(0.8f, 0.8f, 0.8f), 75, new float[] { 50f, 55f, 60f, 65f });

            //palm2Model = loader.loadSceneFromFile(palm2MeshPath).Meshes[0];
            //CreateObjectsFromModel(palm2Model, 70, new Vector3(-20, 0, -50), new Vector3(0.8f, 0.8f, 0.8f), 80, new float[] { 10f, 15f, 20f, 25f });

            //arbolFrutalModel = loader.loadSceneFromFile(arbolFrutalMeshPath).Meshes[0];
            //CreateObjectsFromModel(arbolFrutalModel, 30, new Vector3(-75, 0, 75), new Vector3(0.8f, 0.8f, 0.8f), 50, new float[] { 50, 55f, 60f, 65f });

            //palm3Model = loader.loadSceneFromFile(palm3MeshPath).Meshes[0];
            //CreateObjectsFromModel(palm3Model, 70, new Vector3(-10, 0, -60), new Vector3(0.8f, 0.8f, 0.8f), 80, new float[] { 10f, 15f, 20f, 25f });


            //Crear Quadtree: Defino el BoundinBox del Escenario
            triangle = new CustomVertex.PositionColored[5];
            triangle[0] = new CustomVertex.PositionColored(-256* SceneScaleXZ, 0, 0, Color.Green.ToArgb());
            triangle[1] = new CustomVertex.PositionColored(0, 0, 256 * SceneScaleXZ, Color.Green.ToArgb());
            triangle[2] = new CustomVertex.PositionColored(0, 300 * SceneScaleY, 0, Color.Green.ToArgb());

            triangle[3] = new CustomVertex.PositionColored(256 * SceneScaleXZ, 0, 0, Color.Green.ToArgb());
            triangle[4] = new CustomVertex.PositionColored(0, 0, -256 * SceneScaleXZ, Color.Green.ToArgb());

            terrainBoundingBox =
                TgcBoundingAxisAlignBox.computeFromPoints(new[]
                {triangle[0].Position, triangle[1].Position, triangle[2].Position,triangle[3].Position,triangle[4].Position});

            quadtree = new Quadtree();
            quadtree.create(SceneMeshes, terrainBoundingBox);
            quadtree.createDebugQuadtreeMeshes();
        }

        private void cambioHorario()
        {
            env.usoHorario = 0;
            if (env.horaDelDia < 2)
            {
                env.horaDelDia++;
            }
            else
            {
                env.horaDelDia = 0;
            }
        }

        public void Update(float elapsedTime)
        {
            // Para determinar el momento del día
            env.usoHorario += elapsedTime;

            //Actualizar Skybox (se genera efecto de paso del día)
            ActualizarSkyBox();

            // Actualizo el momento del día (dia o noche)
            if (env.usoHorario > 190) cambioHorario();
        }

        public void Render(float elapsedTime)
        {
            skyBoxGame[env.horaDelDia].render();
            terrain.render();
            RenderSceneMeshes();
            quadtree.render(env.Frustum, true);
        }

        public void Dispose()
        {
            // Se libera el Terreno
            terrain.dispose();

            // Se liberan los elementos de la escena
            foreach (var mesh in SceneMeshes)
            {
                mesh.dispose();
            }

            foreach (var dest in Destroyables)
            {
                dest.Dispose();
            }

            // Se libera Lista de Mesh
            SceneMeshes.Clear();

            //Liberar recursos del SkyBox
            skyBoxGame[env.horaDelDia].dispose();
        }

        private void CreateObjectsFromModel(TgcMesh model, int count, Vector3 center, Vector3 scale, int sparse, float[] scalaVariableObjetos)
        {
            var rnd = new Random();

            // TODO: buscar una mejor forma de tener una distribucion pareja
            var rows = (int)Math.Sqrt(count);
            var cols = (int)Math.Sqrt(count);

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

        private void CreateDestroyables(ObjetoEscena obj, int count, Vector3 center, Vector3 scale, int sparse, float[] scalaVariableObjetos)
        {
            var rnd = new Random();

            // TODO: buscar una mejor forma de tener una distribucion pareja
            var rows = (int)Math.Sqrt(count);
            var cols = (int)Math.Sqrt(count);

            float[] scalaRotacionObjetos = { FastMath.QUARTER_PI, FastMath.PI, FastMath.PI_HALF, FastMath.TWO_PI };

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    var instance = obj.mesh.createMeshInstance(obj.mesh.Name + i + "_" + j);

                    // Escalo el objeto en forma Random
                    instance.Scale = scale * scalaVariableObjetos[rnd.Next(0, scalaVariableObjetos.Length)];

                    var x = center.X + rnd.Next(-sparse, sparse);
                    var z = center.Z + rnd.Next(-sparse, sparse);

                    // Posiciono el objeto en el Escenario
                    instance.Position = new Vector3(x * SceneScaleXZ, CalcularAlturaTerreno(x, z) * SceneScaleY, z * SceneScaleXZ);
                    instance.rotateY(scalaRotacionObjetos[rnd.Next(0, scalaRotacionObjetos.Length)]);

                    // Lo guardo en una Lista de Objetos que están en el Escenario
                    SceneMeshes.Add(instance);
                }
            }

            Destroyables.Add(obj);
        }

        private void CreateSkyBox()
        {
            // Inicializo el SkyBox
            skyBoxCenter = new Vector3(0, 0, 0);
            skyBoxSize = new Vector3(3600 * SceneScaleXZ, 3600 * SceneScaleXZ, 3600 * SceneScaleXZ);
            skyBoxSkyEpsilon = 7800f;

            // Creo los SkyBox según el momento del día

            // DIA
            var i = 0;
            skyBoxGame[i] = new TgcSkyBox();
            skyBoxGame[i].Center = skyBoxCenter;
            skyBoxGame[i].Size = skyBoxSize;

            //Configurar las texturas para cada una de las 6 caras
            skyBoxGame[i].setFaceTexture(TgcSkyBox.SkyFaces.Up, skyTexturePath + "lostatseaday_up.jpg");
            skyBoxGame[i].setFaceTexture(TgcSkyBox.SkyFaces.Down, skyTexturePath + "lostatseaday_dn.jpg");
            skyBoxGame[i].setFaceTexture(TgcSkyBox.SkyFaces.Left, skyTexturePath + "lostatseaday_lf.jpg");
            skyBoxGame[i].setFaceTexture(TgcSkyBox.SkyFaces.Right, skyTexturePath + "lostatseaday_rt.jpg");
            skyBoxGame[i].setFaceTexture(TgcSkyBox.SkyFaces.Front, skyTexturePath + "lostatseaday_bk.jpg");
            skyBoxGame[i].setFaceTexture(TgcSkyBox.SkyFaces.Back, skyTexturePath + "lostatseaday_ft.jpg");


            skyBoxGame[i].SkyEpsilon = skyBoxSkyEpsilon;

            //Inicializa todos los valores para crear el SkyBox
            skyBoxGame[i].Init();

            // TARDE
            i = 1;
            skyBoxGame[i] = new TgcSkyBox();
            skyBoxGame[i].Center = skyBoxCenter;
            skyBoxGame[i].Size = skyBoxSize;

            //Configurar las texturas para cada una de las 6 caras
            skyBoxGame[i].setFaceTexture(TgcSkyBox.SkyFaces.Up, skyTexturePath + "lostatseaday_tarde_up.jpg");
            skyBoxGame[i].setFaceTexture(TgcSkyBox.SkyFaces.Down, skyTexturePath + "lostatseaday_dn.jpg");
            skyBoxGame[i].setFaceTexture(TgcSkyBox.SkyFaces.Left, skyTexturePath + "lostatseaday_tarde_lf.jpg");
            skyBoxGame[i].setFaceTexture(TgcSkyBox.SkyFaces.Right, skyTexturePath + "lostatseaday_tarde_rt.jpg");
            skyBoxGame[i].setFaceTexture(TgcSkyBox.SkyFaces.Front, skyTexturePath + "lostatseaday_tarde_bk.jpg");
            skyBoxGame[i].setFaceTexture(TgcSkyBox.SkyFaces.Back, skyTexturePath + "lostatseaday_tarde_ft.jpg");
            skyBoxGame[i].SkyEpsilon = skyBoxSkyEpsilon;

            //Inicializa todos los valores para crear el SkyBox
            skyBoxGame[i].Init();

            // NOCHE
            i = 2;
            skyBoxGame[i] = new TgcSkyBox();
            skyBoxGame[i].Center = skyBoxCenter;
            skyBoxGame[i].Size = skyBoxSize;

            //Configurar las texturas para cada una de las 6 caras
            skyBoxGame[i].setFaceTexture(TgcSkyBox.SkyFaces.Up, skyTexturePath + "lostatseaday_noche_up.jpg");
            skyBoxGame[i].setFaceTexture(TgcSkyBox.SkyFaces.Down, skyTexturePath + "lostatseaday_noche_dn.jpg");
            skyBoxGame[i].setFaceTexture(TgcSkyBox.SkyFaces.Left, skyTexturePath + "lostatseaday_noche_lf.jpg");
            skyBoxGame[i].setFaceTexture(TgcSkyBox.SkyFaces.Right, skyTexturePath + "lostatseaday_noche_rt.jpg");
            skyBoxGame[i].setFaceTexture(TgcSkyBox.SkyFaces.Front, skyTexturePath + "lostatseaday_noche_bk.jpg");
            skyBoxGame[i].setFaceTexture(TgcSkyBox.SkyFaces.Back, skyTexturePath + "lostatseaday_noche_ft.jpg");
            skyBoxGame[i].SkyEpsilon = skyBoxSkyEpsilon;

            //Inicializa todos los valores para crear el SkyBox
            skyBoxGame[i].Init();
        }

        // Se actualiza el SkyBox (se genera efecto de paso del día)
        public void ActualizarSkyBox()
        {
            foreach (TgcMesh face in skyBoxGame[env.horaDelDia].Faces)
            {
                face.AutoTransformEnable = true;
                face.rotateY(env.ElapsedTime / 30);
            }
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
            //Analizar cada objeto contra el Frustum - con fuerza bruta
            totalMeshesRenderizados = 0;
            foreach (var mesh in SceneMeshes)
            {
                //Nos ocupamos solo de las mallas habilitadas
                if (mesh.Enabled)
                {
                    //Solo mostrar la malla si colisiona contra el Frustum
                    var r = TgcCollisionUtils.classifyFrustumAABB(env.Frustum, mesh.BoundingBox);
                    if (r != TgcCollisionUtils.FrustumResult.OUTSIDE)
                    {
                        mesh.render();
                        totalMeshesRenderizados++;
                    }
                }
            }

            foreach (var dest in Destroyables)
            {
                //Solo mostrar la malla si colisiona contra el Frustum
                var r = TgcCollisionUtils.classifyFrustumAABB(env.Frustum, dest.mesh.BoundingBox);
                if (r != TgcCollisionUtils.FrustumResult.OUTSIDE)
                {
                    dest.Render();
                    totalMeshesRenderizados++;
                }
            }
        }
    }
}