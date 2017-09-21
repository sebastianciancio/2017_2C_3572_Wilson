using Microsoft.DirectX;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Text;

namespace TGC.Group.Model.SpriteGame
{
    class HUD
    {
        private GameModel env;

        private Drawer2D drawer2D;
        private CustomSprite personaje;
        private CustomSprite backgroundHUD;
        private TgcText2D txtHambre;
        private TgcText2D txtSed;
        private TgcText2D txtCansancio;

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

            // Inicializo los puntos de vida
            txtCansancio = new TgcText2D();
            txtCansancio.Color = Color.Yellow;
            txtCansancio.Position = new Point(D3DDevice.Instance.Width - 120, D3DDevice.Instance.Height - 180);
            txtCansancio.Align = TgcText2D.TextAlign.RIGHT;
            txtCansancio.Size = new Size(100, 100);
            txtCansancio.changeFont(new Font("TimesNewRoman", 25, FontStyle.Bold ));

            txtHambre = new TgcText2D();
            txtHambre.Color = Color.Blue;
            txtHambre.Text = "Azul";
            txtHambre.Align = TgcText2D.TextAlign.RIGHT;
            txtHambre.Position = new Point(D3DDevice.Instance.Width - 120, D3DDevice.Instance.Height - 350);
            txtHambre.Size = new Size(100, 100);
            txtHambre.changeFont(new Font("TimesNewRoman", 25, FontStyle.Bold ));

            txtSed = new TgcText2D();
            txtSed.Color = Color.Red;
            txtSed.Text = "Rojo";
            txtSed.Align = TgcText2D.TextAlign.RIGHT;
            txtSed.Position = new Point(D3DDevice.Instance.Width - 120, D3DDevice.Instance.Height - 250);
            txtSed.Size = new Size(100, 100);
            txtSed.changeFont(new Font("TimesNewRoman", 25, FontStyle.Bold));

        }

        public void Init()
        {

        }

        public void Update(float ElapsedTime)
        {
            txtCansancio.Text = env.personaje.Cansancio.ToString();
            txtHambre.Text = env.personaje.Hambre.ToString();
            txtSed.Text = env.personaje.Sed.ToString();
        }

        public void Render()
        {
            // Vida del Personaje
            env.DrawText.drawText("Hambre: " + env.personaje.Hambre, D3DDevice.Instance.Width - 100, D3DDevice.Instance.Height - 110, Color.Yellow);
            env.DrawText.drawText("Sed: " + env.personaje.Sed, D3DDevice.Instance.Width - 100, D3DDevice.Instance.Height - 125, Color.Green);
            env.DrawText.drawText("Cansancio: " + env.personaje.Cansancio, D3DDevice.Instance.Width - 100, D3DDevice.Instance.Height - 140, Color.Blue);

            // Renderizo los puntos de vida
            txtCansancio.render();
            txtSed.render();
            txtCansancio.render();

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
            txtCansancio.Dispose();
            txtSed.Dispose();
            txtCansancio.Dispose();
        }
    }
}
