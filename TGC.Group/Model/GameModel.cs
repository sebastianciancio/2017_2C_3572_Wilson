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
        public int horaDelDia;
        public bool presentacion = true;

        TgcTexture lluviaTexture;
        Texture renderTarget2D;
        Microsoft.DirectX.Direct3D.Effect effect;
        VertexBuffer screenQuadVB;
        Surface g_pDSShadow;     // Depth-stencil buffer for rendering to shadow map

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
            horaDelDia = 0; //0: dia, 1:tarde, 2:noche;

            terreno = new EscenarioGame.Escenario(this);
            terreno.Init();

            personaje = new Personaje(this);
            personaje.Init();

            hud = new HUD(this);
            hud.Init();

            musica = new Musica(this.MediaDir);
            musica.selectionSound();
            musica.startSound();

            InitCamera();


            // SkyBox: Se cambia el valor por defecto del farplane para evitar cliping de farplane.
            D3DDevice.Instance.Device.Transform.Projection =
                Matrix.PerspectiveFovLH(D3DDevice.Instance.FieldOfView,
                    D3DDevice.Instance.AspectRatio,
                    D3DDevice.Instance.ZNearPlaneDistance,
                    D3DDevice.Instance.ZFarPlaneDistance * 560f);

            /*
                        //Cargar shader con efectos de Post-Procesado
                        effect = TgcShaders.loadEffect(this.ShadersDir + "PostProcess.fx");

                        //Configurar Technique dentro del shader
                        effect.Technique = "RainTechnique";

                        //Cargar textura que se va a dibujar arriba de la escena del Render Target
                        lluviaTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, this.MediaDir + "Isla\\efecto_rain.png");


                        //Se crean 2 triangulos (o Quad) con las dimensiones de la pantalla con sus posiciones ya transformadas
                        // x = -1 es el extremo izquiedo de la pantalla, x = 1 es el extremo derecho
                        // Lo mismo para la Y con arriba y abajo
                        // la Z en 1 simpre
                        CustomVertex.PositionTextured[] screenQuadVertices = new CustomVertex.PositionTextured[]
                        {
                            new CustomVertex.PositionTextured( -1, 1, 1, 0,0),
                            new CustomVertex.PositionTextured(1,  1, 1, 1,0),
                            new CustomVertex.PositionTextured(-1, -1, 1, 0,1),
                            new CustomVertex.PositionTextured(1,-1, 1, 1,1)
                        };
                        //vertex buffer de los triangulos
                        screenQuadVB = new VertexBuffer(typeof(CustomVertex.PositionTextured),
                                4, D3DDevice.Instance.Device, Usage.Dynamic | Usage.WriteOnly,
                                    CustomVertex.PositionTextured.Format, Pool.Default);
                        screenQuadVB.SetData(screenQuadVertices, 0, LockFlags.None);

                        //Creamos un Render Targer sobre el cual se va a dibujar la pantalla

                        renderTarget2D = new Texture(D3DDevice.Instance.Device, D3DDevice.Instance.Device.PresentationParameters.BackBufferWidth
                                , D3DDevice.Instance.Device.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget,
                                    Format.X8R8G8B8, Pool.Default);
                        g_pDSShadow = D3DDevice.Instance.Device.CreateDepthStencilSurface(D3DDevice.Instance.Device.PresentationParameters.BackBufferWidth
                                , D3DDevice.Instance.Device.PresentationParameters.BackBufferHeight,
                                                                         DepthFormat.D24S8,
                                                                         MultiSampleType.None,
                                                                         0,
                                                                         true);
            */
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
            PreRender();

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
            }

            PostRender();
        }

        public override void Dispose()
        {
            terreno.Dispose();
            personaje.Dispose();
            hud.Dispose();
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

            DrawText.drawText("Mesh total: \n" + terreno.SceneMeshes.Count, 500, 20, Color.OrangeRed);
            DrawText.drawText("Mesh renderizados: \n" + terreno.totalMeshesRenderizados, 500, 100, Color.OrangeRed);

            DrawText.drawText("Camera position: \n" + Camara.Position, 0, 20, Color.OrangeRed);
            DrawText.drawText("Camera LookAt: \n" + Camara.LookAt, 0, 100, Color.OrangeRed);
            DrawText.drawText("Camera (Coordenada X Original): \n" + (int)(Camara.Position.X / terreno.SceneScaleXZ), 200, 20, Color.OrangeRed);
            DrawText.drawText("Camera (Coordenada Z Original): \n" + (int)(Camara.Position.Z / terreno.SceneScaleXZ), 200, 100, Color.OrangeRed);
            DrawText.drawText("Camera (Coordenada Y Terreno): \n" + terreno.CalcularAlturaTerreno((int)(Camara.Position.X / terreno.SceneScaleXZ), (int)(Camara.Position.Z / terreno.SceneScaleXZ)), 200, 180, Color.OrangeRed);


            /*
            DrawText.drawText("usoHorario: \n" + usoHorario, 200, 20, Color.OrangeRed);
            DrawText.drawText("Camera position: \n" + Camara.Position, 0, 20, Color.OrangeRed);
            DrawText.drawText("Camera LookAt: \n" + Camara.LookAt, 0, 100, Color.OrangeRed);
            DrawText.drawText("Camera (Coordenada X Original): \n" + FastMath.Abs(Camara.Position.X / terreno.SceneScaleXZ), 200, 20, Color.OrangeRed);
            DrawText.drawText("Camera (Coordenada Z Original): \n" + FastMath.Abs(Camara.Position.Z / terreno.SceneScaleXZ), 200, 100, Color.OrangeRed);
            DrawText.drawText("Camera (Coordenada Y Terreno): \n" + FastMath.Abs(terreno.CalcularAlturaTerreno((Camara.Position.X / terreno.SceneScaleXZ), (Camara.Position.Z / terreno.SceneScaleXZ))), 200, 180, Color.OrangeRed);
            DrawText.drawText("Posicion Personaje: \n" + personaje.Posicion, 0, 300, Color.OrangeRed);
            */

        }
    }
}