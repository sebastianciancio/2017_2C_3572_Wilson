using Microsoft.DirectX;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Group.Model.Camara;
using TGC.Group.Model.SpriteGame;
using TGC.Group.Model.Character;
using TGC.Group.Model.SoundsGame;
using TGC.Core.Shaders;
using TGC.Core.Textures;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Utils;

namespace TGC.Group.Model
{
    public class GameModel : TgcExample
    {
        // ***********************************************************
        // Parametros del Juego
        // ***********************************************************

        public EscenarioGame.Escenario terreno;
        public HUD hud;
        public Personaje personaje;
        public Musica musica;

        private Drawer2D drawer2D;
        private CustomSprite menuPresentacion;

        public float usoHorario;
        public float tiempoAcumLluvia;
        public int horaDelDia;
        public bool presentacion = true;
        public bool lloviendo = false;

        TgcTexture lluviaTexture;
        Microsoft.DirectX.Direct3D.Effect effect;
        private Surface depthStencil; // Depth-stencil buffer
        private Surface pOldRT;
        private Surface pOldDS;
        private Texture renderTarget2D;
        private VertexBuffer screenQuadVB;



        // ***********************************************************

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public override void Init()
        {
            // Defino que se muestre la presentacion
            presentacion = true;

            // Defino el Menu presentacion
            drawer2D = new Drawer2D();

            menuPresentacion = new CustomSprite();
            menuPresentacion.Bitmap = new CustomBitmap(this.MediaDir + "\\HUD\\presentacion.jpg", D3DDevice.Instance.Device);
            Size textureSize = menuPresentacion.Bitmap.Size;
            menuPresentacion.Position = new Vector2(0,0);
            menuPresentacion.Scaling = new Vector2(1.5f, 1.2f);

            // Para determinar que momento del día es
            usoHorario = 0;
            tiempoAcumLluvia = 0;
            horaDelDia = 0; //0: dia, 1:tarde, 2:noche;

            terreno = new EscenarioGame.Escenario(this);
            terreno.Init();

            personaje = new Personaje(this);
            personaje.Init();

            hud = new HUD(this);
            hud.Init();

            musica = new Musica(this.MediaDir);
            musica.selectionSound("Sonido\\ambiente1.mp3");
            musica.startSound();


            // Inicializacion de PostProcess con Render Target

            CustomVertex.PositionTextured[] screenQuadVertices =
            {
                new CustomVertex.PositionTextured(-1, 1, 1, 0, 0),
                new CustomVertex.PositionTextured(1, 1, 1, 1, 0),
                new CustomVertex.PositionTextured(-1, -1, 1, 0, 1),
                new CustomVertex.PositionTextured(1, -1, 1, 1, 1)
            };
            //vertex buffer de los triangulos
            screenQuadVB = new VertexBuffer(typeof(CustomVertex.PositionTextured),
                4, D3DDevice.Instance.Device, Usage.Dynamic | Usage.WriteOnly,
                CustomVertex.PositionTextured.Format, Pool.Default);
            screenQuadVB.SetData(screenQuadVertices, 0, LockFlags.None);

            //Creamos un Render Targer sobre el cual se va a dibujar la pantalla
            renderTarget2D = new Texture(D3DDevice.Instance.Device,
                D3DDevice.Instance.Device.PresentationParameters.BackBufferWidth
                , D3DDevice.Instance.Device.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget,
                Format.X8R8G8B8, Pool.Default);

            //Creamos un DepthStencil que debe ser compatible con nuestra definicion de renderTarget2D.
            depthStencil =
                D3DDevice.Instance.Device.CreateDepthStencilSurface(
                    D3DDevice.Instance.Device.PresentationParameters.BackBufferWidth,
                    D3DDevice.Instance.Device.PresentationParameters.BackBufferHeight,
                    DepthFormat.D24S8, MultiSampleType.None, 0, true);

            //Cargar shader con efectos de Post-Procesado
            effect = TgcShaders.loadEffect(ShadersDir + "PostProcess.fx");

            //Configurar Technique dentro del shader
            effect.Technique = "RainTechnique";

            //Cargar textura que se va a dibujar arriba de la escena del Render Target
            lluviaTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, this.MediaDir + "Isla\\efecto_rain.png");

            // Inicializo la camara
            InitCamera();

            // SkyBox: Se cambia el valor por defecto del farplane para evitar cliping de farplane.
            D3DDevice.Instance.Device.Transform.Projection =
                Matrix.PerspectiveFovLH(D3DDevice.Instance.FieldOfView,
                    D3DDevice.Instance.AspectRatio,
                    D3DDevice.Instance.ZNearPlaneDistance,
                    D3DDevice.Instance.ZFarPlaneDistance * 560f);
        }

        public override void Update()
        {
            PreUpdate();

            if (!presentacion)
            {
                terreno.Update(ElapsedTime);
                personaje.Update(ElapsedTime, Input);
                Camara.UpdateCamera(ElapsedTime);
                hud.Update(ElapsedTime);
            }
        }

        public override void Render()
        {
            //PreRender();

            ClearTextures();


            //Cargamos el Render Targer al cual se va a dibujar la escena 3D. Antes nos guardamos el surface original
            //En vez de dibujar a la pantalla, dibujamos a un buffer auxiliar, nuestro Render Target.
            pOldRT = D3DDevice.Instance.Device.GetRenderTarget(0);
            pOldDS = D3DDevice.Instance.Device.DepthStencilSurface;
            var pSurf = renderTarget2D.GetSurfaceLevel(0);
            D3DDevice.Instance.Device.SetRenderTarget(0, pSurf);
            D3DDevice.Instance.Device.DepthStencilSurface = depthStencil;
            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            //Dibujamos la escena comun, pero en vez de a la pantalla al Render Target
            drawSceneToRenderTarget(D3DDevice.Instance.Device);

            //Liberar memoria de surface de Render Target
            pSurf.Dispose();

            //Si quisieramos ver que se dibujo, podemos guardar el resultado a una textura en un archivo para debugear su resultado (ojo, es lento)
            //TextureLoader.Save(this.ShadersDir + "render_target.bmp", ImageFileFormat.Bmp, renderTarget2D);

            //Ahora volvemos a restaurar el Render Target original (osea dibujar a la pantalla)
            D3DDevice.Instance.Device.SetRenderTarget(0, pOldRT);
            D3DDevice.Instance.Device.DepthStencilSurface = pOldDS;

            //Luego tomamos lo dibujado antes y lo combinamos con una textura con efecto de alarma
            drawPostProcess(D3DDevice.Instance.Device);

            // PostRender();
        }


        /// <summary>
        ///     Dibujamos toda la escena pero en vez de a la pantalla, la dibujamos al Render Target que se cargo antes.
        ///     Es como si dibujaramos a una textura auxiliar, que luego podemos utilizar.
        /// </summary>
        private void drawSceneToRenderTarget(Microsoft.DirectX.Direct3D.Device d3dDevice)
        {
            //Arrancamos el renderizado. Esto lo tenemos que hacer nosotros a mano porque estamos en modo CustomRenderEnabled = true
            d3dDevice.BeginScene();

            //Dibujamos todos los meshes del escenario

            // Si se muestra menu principal
            if (presentacion)
            {
                drawer2D.BeginDrawSprite();
                drawer2D.DrawSprite(menuPresentacion);
                drawer2D.EndDrawSprite();

                DrawText.changeFont((new System.Drawing.Font("TimesNewRoman", 35, FontStyle.Bold | FontStyle.Italic)));
                //DrawText.Size = new Size(300, 100);
                DrawText.drawText("Presione [ESPACIO] para COMENZAR", 150, 150, Color.OrangeRed);

                if (Input.keyDown(Key.Space))
                {
                    presentacion = false;
                }
            }
            else
            {
                if (Input.keyDown(Key.Escape))
                {
                    presentacion = true;
                }

                terreno.Render(ElapsedTime);
                hud.Render();
                RenderHelpText();
                personaje.Render(ElapsedTime);
                RenderFPS();
                RenderAxis();
            }

            //Terminamos manualmente el renderizado de esta escena. Esto manda todo a dibujar al GPU al Render Target que cargamos antes
            d3dDevice.EndScene();
        }

        /// <summary>
        ///     Se toma todo lo dibujado antes, que se guardo en una textura, y se le aplica un shader para distorsionar la imagen
        /// </summary>
        private void drawPostProcess(Microsoft.DirectX.Direct3D.Device d3dDevice)
        {
            //Arrancamos la escena
            d3dDevice.BeginScene();

            //Cargamos para renderizar el unico modelo que tenemos, un Quad que ocupa toda la pantalla, con la textura de todo lo dibujado antes
            d3dDevice.VertexFormat = CustomVertex.PositionTextured.Format;
            d3dDevice.SetStreamSource(0, screenQuadVB, 0);

            //Ver si el efecto de oscurecer esta activado, configurar Technique del shader segun corresponda

            effect.Technique = "RainTechnique";

            if (!presentacion && lloviendo)
            {
                effect.Technique = "RainTechnique";
            }
            else
            {
                effect.Technique = "DefaultTechnique";
            }

            //Cargamos parametros en el shader de Post-Procesado
            effect.SetValue("render_target2D", renderTarget2D);
            effect.SetValue("textura_alarma", lluviaTexture.D3dTexture);
            effect.SetValue("time", this.ElapsedTime);

            //Limiamos la pantalla y ejecutamos el render del shader
            d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            effect.Begin(FX.None);
            effect.BeginPass(0);
            d3dDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            effect.EndPass();
            effect.End();

            //Terminamos el renderizado de la escena
            d3dDevice.EndScene();
            d3dDevice.Present();
        }

        public override void Dispose()
        {
            terreno.Dispose();
            personaje.Dispose();
            hud.Dispose();


            effect.Dispose();
            screenQuadVB.Dispose();
            renderTarget2D.Dispose();
            depthStencil.Dispose();

        }

        private void InitCamera()
        {
            // Usar Coordenadas Originales del HeightMap [-32,32]
            var posicionCamaraX = 11;
            var posicionCamaraZ = 21;
            var posicionCamaraY = terreno.CalcularAlturaTerreno(posicionCamaraX, posicionCamaraZ);

            var alturaOjos = 0f;

            var cameraPosition = new Vector3(posicionCamaraX * terreno.SceneScaleXZ, posicionCamaraY * terreno.SceneScaleY + alturaOjos, posicionCamaraZ * terreno.SceneScaleXZ);
            var cameraLookAt = new Vector3(0, 0.5f, -1);
            var cameraMoveSpeed = 1000f;
            var cameraJumpSpeed = 10000f;

            // Creo la cámara y defino la Posición y LookAt
            Camara = new TgcFpsCamera(this,cameraPosition, cameraMoveSpeed, cameraJumpSpeed, Input);
            Camara.SetCamera(cameraPosition, cameraLookAt);
        }

        private void RenderHelpText()
        {
            DrawText.changeFont((new System.Drawing.Font("TimesNewRoman", 12)));
            DrawText.drawText("Mesh total: \n" + terreno.SceneMeshes.Count, 0, 20, Color.OrangeRed);
            DrawText.drawText("Mesh renderizados: \n" + terreno.totalMeshesRenderizados, 0, 100, Color.OrangeRed);
            //DrawText.drawText("usoHorario: \n" + (usoHorario/570)*24, 200, 20, Color.OrangeRed);
            
            /*
            DrawText.drawText("usoHorario: \n" + usoHorario, 200, 20, Color.OrangeRed);
            DrawText.drawText("Camera position: \n" + Camara.Position, 0, 20, Color.OrangeRed);
            DrawText.drawText("Camera LookAt: \n" + Camara.LookAt, 0, 100, Color.OrangeRed);
            DrawText.drawText("Camera (Coordenada X Original): \n" + (int)(Camara.Position.X / terreno.SceneScaleXZ), 200, 20, Color.OrangeRed);
            DrawText.drawText("Camera (Coordenada Z Original): \n" + (int)(Camara.Position.Z / terreno.SceneScaleXZ), 200, 100, Color.OrangeRed);
            DrawText.drawText("Camera (Coordenada Y Terreno): \n" + terreno.CalcularAlturaTerreno((int)(Camara.Position.X / terreno.SceneScaleXZ), (int)(Camara.Position.Z / terreno.SceneScaleXZ)), 200, 180, Color.OrangeRed);
            DrawText.drawText("Posicion Personaje: \n" + personaje.Posicion, 0, 300, Color.OrangeRed);
            */
        }
    }
}