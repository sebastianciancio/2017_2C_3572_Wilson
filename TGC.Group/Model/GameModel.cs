
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
using TGC.Group.Model.Camara;
using System;
using System.Linq;

namespace TGC.Group.Model
{
    public class GameModel : TgcExample
    {
        // ***********************************************************
        // Parametros del Juego
        // ***********************************************************

        public Escenario terreno;

        // ***********************************************************

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public override void Init()
        {
            // Creo el Escenario
            terreno = new Escenario(this);
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

            terreno.Update();

        }

        public override void Render()
        {
            PreRender();

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

        private void InitCamera()
        {
            // Usar Coordenadas Originales del HeightMap [-256,256]
            var posicionCamaraX = 0;
            var posicionCamaraZ = 0;

            var cameraPosition = new Vector3(posicionCamaraX * sceneScaleXZ, (CalcularAlturaTerreno(posicionCamaraX, posicionCamaraZ) + 1 )* sceneScaleY, posicionCamaraZ * sceneScaleXZ);
            var cameraLookAt = new Vector3(0, 0, -1);
            var cameraMoveSpeed = 10f;
            var cameraJumpSpeed = 10f;

            // Creo la cámara y defino la Posición y LookAt
            Camara = new TgcFpsCamera(Input);
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


            PostRender();
        }

        public override void Dispose()
        {
            terreno.Dispose();
        }

    }
}