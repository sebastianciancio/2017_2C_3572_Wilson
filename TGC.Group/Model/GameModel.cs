using Microsoft.DirectX;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Group.Model.Camara;
using TGC.Group.Model.SpriteGame;
using TGC.Group.Model.Character;
using TGC.Group.Model.SoundsGame;

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

        public float usoHorario;
        public int horaDelDia;

        // ***********************************************************

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public override void Init()
        {
            // Para determinar que momento del día es
            usoHorario = 0;
            horaDelDia = 1; //0: dia, 1:noche;

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
                    D3DDevice.Instance.ZFarPlaneDistance * 100f);

        }

        public override void Update()
        {
            PreUpdate();
            terreno.Update(ElapsedTime);
            personaje.Update(ElapsedTime, Input);
            Camara.UpdateCamera(ElapsedTime);
            hud.Update(ElapsedTime);
        }

        public override void Render()
        {
            PreRender();
            terreno.Render(ElapsedTime);
            hud.Render();
            RenderHelpText();
            personaje.Render(ElapsedTime);

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
            // Usar Coordenadas Originales del HeightMap [-256,256]
            var posicionCamaraX = 170;
            var posicionCamaraZ = 8;
            var posicionCamaraY = terreno.CalcularAlturaTerreno(posicionCamaraX, posicionCamaraZ);

            var alturaOjos = 0f;

            var cameraPosition = new Vector3(posicionCamaraX * terreno.SceneScaleXZ, posicionCamaraY * terreno.SceneScaleY + alturaOjos, posicionCamaraZ * terreno.SceneScaleXZ);
            var cameraLookAt = new Vector3(0, 0.5f, -1);
            var cameraMoveSpeed = 1000f;
            var cameraJumpSpeed = 1000f;

            // Creo la cámara y defino la Posición y LookAt
            Camara = new TgcFpsCamera(this,cameraPosition, cameraMoveSpeed, cameraJumpSpeed, Input);
            Camara.SetCamera(cameraPosition, cameraLookAt);
        }

        private void RenderHelpText()
        {
            DrawText.drawText("Camera position: \n" + Camara.Position, 0, 20, Color.OrangeRed);
            DrawText.drawText("Camera LookAt: \n" + Camara.LookAt, 0, 100, Color.OrangeRed);
            DrawText.drawText("Mesh count: \n" + terreno.SceneMeshes.Count, 0, 180, Color.OrangeRed);
            DrawText.drawText("Mesh renderizados: \n" + terreno.totalMeshesRenderizados, 0, 220, Color.OrangeRed);
/*
            DrawText.drawText("Camera (Coordenada X Original): \n" + FastMath.Abs(Camara.Position.X / terreno.SceneScaleXZ), 200, 20, Color.OrangeRed);
            DrawText.drawText("Camera (Coordenada Z Original): \n" + FastMath.Abs(Camara.Position.Z / terreno.SceneScaleXZ), 200, 100, Color.OrangeRed);
            DrawText.drawText("Camera (Coordenada Y Terreno): \n" + FastMath.Abs(terreno.CalcularAlturaTerreno((Camara.Position.X / terreno.SceneScaleXZ), (Camara.Position.Z / terreno.SceneScaleXZ))), 200, 180, Color.OrangeRed);
            DrawText.drawText("Posicion Personaje: \n" + personaje.Posicion, 0, 300, Color.OrangeRed);
*/

        }
    }
}