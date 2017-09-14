using Microsoft.DirectX;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Group.Model.Camara;
using TGC.Group.Model.EscenarioGame;
using TGC.Group.Model.Sprite;
using TGC.Group.Model.Character;

namespace TGC.Group.Model
{
    public class GameModel : TgcExample
    {
        // ***********************************************************
        // Parametros del Juego
        // ***********************************************************

        public Escenario terreno;
        private HUD hud;
        private Personaje personaje;

        // ***********************************************************

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public override void Init()
        {
            terreno = new Escenario(this);
            terreno.Init();

            personaje = new Personaje(this);
            personaje.Init();

            hud = new HUD(this);
            hud.Init();

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
            terreno.Update();
            personaje.Update(ElapsedTime, Input);
            Camara.UpdateCamera(ElapsedTime);
            hud.Update(ElapsedTime);
        }

        public override void Render()
        {
            PreRender();
            terreno.Render();
            personaje.Render(ElapsedTime);
            hud.Render();
            RenderHelpText();
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
            var posicionCamaraX = 0;
            var posicionCamaraZ = -90;
            var posicionCamaraY = terreno.CalcularAlturaTerreno(posicionCamaraX, posicionCamaraZ) + 1;

            var alturaOjos = 15f;

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
            DrawText.drawText("Camera (Coordenada X Original): \n" + ((Camara.Position.X / terreno.SceneScaleXZ) - (terreno.HeightmapSize.Width / 2)), 200, 20, Color.OrangeRed);
            DrawText.drawText("Camera (Coordenada Z Original): \n" + ((Camara.Position.Z / terreno.SceneScaleXZ) + (terreno.HeightmapSize.Width / 2)), 200, 100, Color.OrangeRed);
            DrawText.drawText("Posicion Personaje: \n" + personaje.personaje.Position, 0, 220, Color.OrangeRed);
            DrawText.drawText("Posicion Sphere: \n" + terreno.esferaColision.Position, 0, 300, Color.OrangeRed);
        }
    }
}