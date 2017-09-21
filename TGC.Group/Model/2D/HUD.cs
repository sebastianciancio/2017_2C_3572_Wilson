using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX;
using TGC.Core.Geometry;
using TGC.Core.Textures;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Utils;
using TGC.Core.Example;
using TGC.Group.Model.Camara;
using TGC.Group.Model.EscenarioGame;
using TGC.Group.Model.SpriteGame;
using TGC.Group.Model.Character;
using TGC.Group.Model.SoundsGame;


namespace TGC.Group.Model.SpriteGame
{
    class HUD
    {
        private GameModel env;

        private Drawer2D drawer2D;
        private CustomSprite personaje;
        private CustomSprite backgroundHUD;

        public HUD(GameModel env)
        {
        	this.env = env;
            drawer2D = new Drawer2D();
            Size textureSize;

            //Creo el Background del HUD
            backgroundHUD = new CustomSprite();
            backgroundHUD.Bitmap = new CustomBitmap(env.MediaDir + "\\HUD\\background.png", D3DDevice.Instance.Device);
            textureSize = backgroundHUD.Bitmap.Size;
            backgroundHUD.Position = new Vector2(
                       D3DDevice.Instance.Width - (textureSize.Width * 6),
                       0);
            backgroundHUD.Scaling = new Vector2(6, D3DDevice.Instance.Height / 10);

            //Crear Personaje
            personaje = new CustomSprite();
            personaje.Bitmap = new CustomBitmap(env.MediaDir + "\\HUD\\face1.png", D3DDevice.Instance.Device);
            textureSize = personaje.Bitmap.Size;
            personaje.Position = new Vector2(
                       D3DDevice.Instance.Width - (textureSize.Width * 0.3f) - 10,
                       D3DDevice.Instance.Height - (textureSize.Height * 0.3f) - 10);
            personaje.Scaling = new Vector2(0.3f, 0.3f);
        }

        public void Init()
        {

        }

        public void Update(float ElapsedTime)
        {
        }

        public void Render()
        {
            // Vida del Personaje
            env.DrawText.drawText("100%", D3DDevice.Instance.Width - 50, D3DDevice.Instance.Height - 100, Color.OrangeRed);

            //Iniciar dibujado de todos los Sprites del HUD
            drawer2D.BeginDrawSprite();

            //Dibujar sprite (si hubiese mas, deberian ir todos aquí)
            drawer2D.DrawSprite(personaje);
            drawer2D.DrawSprite(backgroundHUD);

            //Finalizar el dibujado de Sprites
            drawer2D.EndDrawSprite();

        }

        public void Dispose()
        {
            personaje.Dispose();
            backgroundHUD.Dispose();
        }
    }
}
