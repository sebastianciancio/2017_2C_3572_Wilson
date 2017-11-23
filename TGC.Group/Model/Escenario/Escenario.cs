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
using TGC.Core.Shaders;
using TGC.Core.Fog;
using TGC.Core.Geometry;
using TGC.Core.Textures;
using TGC.Core.Direct3D;
using TGC.Core.Particle;
using static TGC.Core.Geometry.TgcPlane;

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

        public TerrenoCustom terrain;
        public TerrenoCustom mar;
        public TerrenoCustom laguna;
        private Vector3 terrainCenter;
        private string sceneHeightmapPath;
        private string terrainTexturePath;
        private Bitmap HeightmapSize { get; set; }

        private string marHeightmapPath;
        private string marTexturePath;
        private string lagunaHeightmapPath;
        private string lagunaTexturePath;

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

        public TgcMesh fogata;
        private string fogataPath;
        private ParticleEmitter emisorFuego;

        private Palmera palmModel;
        private Roca rockModel;
        //private Planta plantModel;
        //private Arbol arbolModel;
        //private TgcMesh arbolFrutalModel;
        private Fruta frutaModel;
        private Pino pinoModel;
        //private TgcMesh palm2Model;
        private TgcMesh fogataModel;

        private Effect effectAgua;
        private float time;

        public bool activarFogata = false;
        public bool ubicacionFogataFija = false;

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

            marHeightmapPath = env.MediaDir + "Isla\\height_mar.jpg";
            marTexturePath = env.MediaDir + "Isla\\agua.jpg";

            lagunaHeightmapPath = env.MediaDir + "Isla\\height_mar.jpg";
            lagunaTexturePath = env.MediaDir + "Isla\\agua.jpg";

            fogataPath = env.MediaDir + "Fogata2\\Split+firewood-TgcScene.xml";
        }

        public void Init()
        {
            // Inicializo las Escalas
            SceneScaleY = 3 * 40f;
            SceneScaleXZ = 30 * 400f;

            //Cargar Heightmap y textura de la Escena
            HeightmapSize = new Bitmap(sceneHeightmapPath);

            terrainCenter = new Vector3(0, 0, 0);
            terrain = new TerrenoCustom();
            terrain.AlphaBlendEnable = true;
            terrain.loadHeightmap(sceneHeightmapPath, SceneScaleXZ, SceneScaleY, terrainCenter);
            terrain.loadTexture(terrainTexturePath);

            // Creo el agua
            effectAgua = TgcShaders.loadEffect(env.ShadersDir + "BasicShader.fx");

            mar = new TerrenoCustom();
            mar.AlphaBlendEnable = true;
            mar.loadHeightmap(marHeightmapPath,  9 * SceneScaleXZ, 2 * SceneScaleY, new Vector3(0, -200, 0));
            mar.loadTexture(marTexturePath);
            mar.Effect = effectAgua;
            mar.Technique = "RenderScene";

            laguna = new TerrenoCustom();
            laguna.AlphaBlendEnable = true;
            laguna.loadHeightmap(lagunaHeightmapPath, SceneScaleXZ/4, SceneScaleY/2, new Vector3(10, 10, 40));
            laguna.loadTexture(lagunaTexturePath);
            laguna.Effect = effectAgua;
            laguna.Technique = "RenderLaguna";


            // Creo el SkyBox
            CreateSkyBox();

            // La ubicacion de los Mesh es en coordenadas Originales del HeightMap (sin escalado) [-128,128]
            SceneMeshes = new List<TgcMesh>();
            Destroyables = new List<ObjetoEscena>();
            loader = new TgcSceneLoader();

            rockModel = new Roca(env);
            //CreateObjectsFromModel(rockModel.mesh, 100, new Vector3(0, 0, 0), new Vector3(0.9f, 0.9f, 0.9f), (HeightmapSize.Width / 3), new float[] { 30f, 35f, 40f, 45f });
            CreateObjectsFromModel(rockModel.mesh, 300, new Vector3(0, 0, 0), new Vector3(0.9f, 1.3f, 1.7f), (HeightmapSize.Width / 2) - 10, new float[] { 30f, 35f, 40f, 45f });

            palmModel = new Palmera(env);
            //CreateObjectsFromModel(palmModel.mesh, 50, new Vector3((HeightmapSize.Width / 5)*-1, 0, 0), new Vector3(0.5f, 0.5f, 0.5f), (HeightmapSize.Width / 3), new float[] { 60f, 65f, 70f, 75f });
            //CreateObjectsFromModel(palmModel.mesh, 50, new Vector3((HeightmapSize.Width / 6), 0, (HeightmapSize.Width / 6)), new Vector3(0.5f, 0.5f, 0.5f), (HeightmapSize.Width / 6) * 2, new float[] { 60f, 65f, 70f, 75f });
            CreateObjectsFromModel(palmModel.mesh, 300, new Vector3(0,0,0), new Vector3(0.5f, 0.7f, 0.9f), (HeightmapSize.Width / 2) - 10, new float[] { 60f, 65f, 70f, 75f });

            //arbolModel = new Arbol(env);
            //CreateObjectsFromModel(arbolModel.mesh, 30, new Vector3(10, 0, 10), new Vector3(0.8f, 0.8f, 0.8f), 20, new float[] { 50f, 55f, 60f, 65f });

            frutaModel = new Fruta(env);
            CreateObjectsFromModel(frutaModel.mesh, 150, new Vector3(0, 0, 0), new Vector3(0.8f, 0.8f, 0.8f), (HeightmapSize.Width / 2) - 10, new float[] { 2f, 2f, 2f, 2f });

            pinoModel = new Pino(env);
            //CreateObjectsFromModel(pinoModel.mesh, 100, new Vector3((HeightmapSize.Width / 4), 0, 0), new Vector3(0.8f, 0.8f, 0.8f), (HeightmapSize.Width / 4)-10, new float[] { 50, 55f, 60f, 65f });
            //CreateObjectsFromModel(pinoModel.mesh, 100, new Vector3(0, 0, (HeightmapSize.Width / 4)), new Vector3(0.8f, 0.8f, 0.8f), (HeightmapSize.Width / 4)-10, new float[] { 50, 55f, 60f, 65f });
            CreateObjectsFromModel(pinoModel.mesh, 100, new Vector3(0, 0,0), new Vector3(0.8f, 0.9f, 1.1f), (HeightmapSize.Width / 2) - 10, new float[] { 50, 55f, 60f, 65f });

            //plantModel = loader.loadSceneFromFile(plantMeshPath).Meshes[0];
            //CreateObjectsFromModel(plantModel, 70, new Vector3(75, 0, -75), new Vector3(0.8f, 0.8f, 0.8f), 75, new float[] { 50f, 55f, 60f, 65f });

            //palm2Model = loader.loadSceneFromFile(palm2MeshPath).Meshes[0];
            //CreateObjectsFromModel(palm2Model, 70, new Vector3(-20, 0, -50), new Vector3(0.8f, 0.8f, 0.8f), 80, new float[] { 10f, 15f, 20f, 25f });

            //arbolFrutalModel = loader.loadSceneFromFile(arbolFrutalMeshPath).Meshes[0];
            //CreateObjectsFromModel(arbolFrutalModel, 30, new Vector3(-75, 0, 75), new Vector3(0.8f, 0.8f, 0.8f), 50, new float[] { 50, 55f, 60f, 65f });

            //palm3Model = loader.loadSceneFromFile(palm3MeshPath).Meshes[0];
            //CreateObjectsFromModel(palm3Model, 70, new Vector3(-10, 0, -60), new Vector3(0.8f, 0.8f, 0.8f), 80, new float[] { 10f, 15f, 20f, 25f });


            var fogataModel = loader.loadSceneFromFile(fogataPath);
            fogata = fogataModel.Meshes[0];

            fogata.AutoTransformEnable = true;
            fogata.Scale = new Vector3(30f,30f,30f);
            fogata.Position = new Vector3(0, 0, 0);
            fogata.AlphaBlendEnable = true;
            fogata.Enabled = true;            

            // Inicializo el Fuego
            emisorFuego = new ParticleEmitter(env.MediaDir + "Fogata2\\fuego.png", 30);
            emisorFuego.Position = new Vector3(0, 0, 0);

            emisorFuego.MinSizeParticle = 5.5f;            
            emisorFuego.MaxSizeParticle = 10f;
            emisorFuego.ParticleTimeToLive = 1.5f;
            emisorFuego.CreationFrecuency = 0.25f;
            emisorFuego.Dispersion = 50;
            emisorFuego.Speed = new Vector3(50f, 50f*SceneScaleY, 50f);


            //Crear Quadtree: Defino el BoundinBox del Escenario
            triangle = new CustomVertex.PositionColored[5];
            triangle[0] = new CustomVertex.PositionColored((HeightmapSize.Width/2)* -1 * SceneScaleXZ, 0, 0, Color.Green.ToArgb());
            triangle[1] = new CustomVertex.PositionColored(0, 0, (HeightmapSize.Width / 2) * SceneScaleXZ, Color.Green.ToArgb());
            triangle[2] = new CustomVertex.PositionColored(0, (HeightmapSize.Width) * SceneScaleY, 0, Color.Green.ToArgb());

            triangle[3] = new CustomVertex.PositionColored((HeightmapSize.Width / 2) * SceneScaleXZ, 0, 0, Color.Green.ToArgb());
            triangle[4] = new CustomVertex.PositionColored(0, 0, (HeightmapSize.Width / 2) * -1 * SceneScaleXZ, Color.Green.ToArgb());

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
            if (!env.personaje.Muerto) { 

                // Para determinar el momento del día
                env.usoHorario += elapsedTime;

                // Para determinar el momento de la lluvia
                env.tiempoAcumLluvia += elapsedTime;

                //Actualizar Skybox (se genera efecto de paso del día)
                ActualizarSkyBox();

                // Actualizo el momento del día (dia o noche)
                if (env.usoHorario > 190) cambioHorario();

                
                if (env.tiempoAcumLluvia > 20) activarLluvia();
                if (env.tiempoAcumLluvia > 40) desactivarLluvia();


                // Detengo el sonido del Hacha si ya fue iniciado y supero el tiempo
                if (env.sonidoHacha){ env.tiempoAcumHacha += elapsedTime; }
                if (env.tiempoAcumHacha > 0.6) env.personaje.sonidoHacha(false);


                if (env.fogataEncendido)
                {
                    //Render de emisor
                    emisorFuego.Position = fogata.Position;
                }
            }
        }

        public void Render(float elapsedTime)
        {
            time += elapsedTime;
            effectAgua.SetValue("time", time);

            skyBoxGame[env.horaDelDia].render();
            terrain.render();
            mar.render();
            laguna.render();
            RenderSceneMeshes();
            quadtree.render(env.Frustum, false);

            if (activarFogata)
            {
                fogata.render();
            }


            if (env.fogataEncendido)
            {
                //IMPORTANTE PARA PERMITIR ESTE EFECTO.
                D3DDevice.Instance.ParticlesEnabled = true;
                D3DDevice.Instance.EnableParticles();

                //Render de emisor
                emisorFuego.render(elapsedTime);

                // Comienzo a contar el tiempo del fuego para determinar el fin del juego
                env.tiempoFogataEncendida += elapsedTime;

                // Determino la condicion de partido ganado
                if(env.tiempoFogataEncendida > 15){
                    env.partidoGanado = true;
                }
                
            }
                
        }

        public void Dispose()
        {
            // Se libera el Terreno
            terrain.dispose();
            mar.dispose();
            laguna.dispose();
            fogata.dispose();
            emisorFuego.dispose();


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
                    instance.Enabled = true;

                    // Lo guardo en una Lista de Objetos que están en el Escenario
                    SceneMeshes.Add(instance);
                }
            }
        }

        private void CreateSkyBox()
        {
            // Inicializo el SkyBox
            skyBoxCenter = new Vector3(0, 0, 0);
            var escalaSkyBox = 15;
            skyBoxSize = new Vector3((HeightmapSize.Width * escalaSkyBox) * SceneScaleXZ, (HeightmapSize.Width * escalaSkyBox) * SceneScaleXZ, (HeightmapSize.Width * escalaSkyBox) * SceneScaleXZ);
            skyBoxSkyEpsilon = HeightmapSize.Width * 15f;

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

        private void activarLluvia()
        {
            env.lloviendo = true;

            env.musica.selectionSound("Sonido\\lluvia_trueno.mp3");
            env.musica.startSound();

        }
        public void desactivarLluvia()
        {
            // Ingremento la cantidad de agua recolectada
            env.personaje.Inventario[1].cantidad += 2;

            env.tiempoAcumLluvia = 0;
            env.lloviendo = false;
            env.musica.selectionSound("Sonido\\ambiente1.mp3");
            env.musica.startSound();
        }


        // Las coordenas x,z son Originales (sin Escalado) y el z devuelto es Original también (sin Escalado)
        public float CalcularAlturaTerreno(float x, float z, bool personaje = false)
        {
            // Calculo las coordenadas en la Matriz de Heightmap
            var pos_i = x + (HeightmapSize.Width /2);
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


            var pi2 = pi - 1;
            var pj2 = pj - 1;
            if (pi2 < 0)
                pi2 = 0;
            if (pj2 < 0)
                pj2 = 0;

            // 2x2 percent closest filtering usual:
            var H0 = terrain.HeightmapData[pi, pj];
            var H1 = terrain.HeightmapData[pi1, pj];
            var H2 = terrain.HeightmapData[pi, pj1];
            var H3 = terrain.HeightmapData[pi1, pj1];

            var H4 = terrain.HeightmapData[pi2, pj];
            var H5 = terrain.HeightmapData[pi2, pj1];
            var H6 = terrain.HeightmapData[pi2, pj2];
            var H7 = terrain.HeightmapData[pi, pj2];
            var H8 = terrain.HeightmapData[pi1, pj2];


            var H = (H0 * (1 - fracc_i) + H1 * fracc_i) * (1 - fracc_j) +
                    (H2 * (1 - fracc_i) + H3 * fracc_i) * fracc_j;

            if (personaje)
            {
                H = FastMath.Max(FastMath.Max(FastMath.Max(H0, H1), FastMath.Max(H2, H3)), FastMath.Max(H4, H5)) + 1;
                //H = ((H0 + H1 + H2 + H3 + H4 + H5) / 6)+2;
            }


            return H;
        }

        private void RenderSceneMeshes()
        {
            //Analizar cada objeto contra el Frustum - con fuerza bruta
            totalMeshesRenderizados = 0;
            foreach (var mesh in SceneMeshes)
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

        public bool estaDentroTerreno()
        {
            // Fuerzo a que el Personaje este siempre sobre la Isla
            if (
                (FastMath.Abs(env.Camara.Position.X / SceneScaleXZ) < (HeightmapSize.Width / 2 - 1)) &&
                (FastMath.Abs(env.Camara.Position.Z / SceneScaleXZ) < (HeightmapSize.Width / 2 - 1)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}