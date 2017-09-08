

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
using TGC.Group.Model.EscenarioGame;

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
            terreno = new Escenario(this,Input,DrawText);
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

            // Renderizo el Escenario
            terreno.Render();

            PostRender();
        }

        public override void Dispose()
        {
            terreno.Dispose();
        }

    }
}