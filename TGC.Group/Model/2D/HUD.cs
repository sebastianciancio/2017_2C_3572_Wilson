using Microsoft.DirectX;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Text;

namespace TGC.Group.Model.SpriteGame
{
    public class HUD
    {
        private GameModel env;

        private Drawer2D drawer2D;
        private CustomSprite personaje;
        private CustomSprite backgroundHUD;
        private CustomSprite gameover;
        private CustomSprite vaso;
        private CustomSprite comida;
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

            //Crear Personaje 192 x 242
            personaje = new CustomSprite();
            personaje.Bitmap = new CustomBitmap(env.MediaDir + "\\HUD\\face1.png", D3DDevice.Instance.Device);
            textureSize = personaje.Bitmap.Size;
            personaje.Position = new Vector2(
                       D3DDevice.Instance.Width - (textureSize.Width * 0.3f) - 10,
                       D3DDevice.Instance.Height - (textureSize.Height * 0.3f) - 10);
            personaje.Scaling = new Vector2(0.3f, 0.3f);

            //Crear Vaso de Agua 512 x 512
            vaso = new CustomSprite();
            vaso.Bitmap = new CustomBitmap(env.MediaDir + "\\HUD\\agua.png", D3DDevice.Instance.Device);
            textureSize = vaso.Bitmap.Size;
            vaso.Position = new Vector2(
                       D3DDevice.Instance.Width - (textureSize.Width * 0.15f) - 10,
                       D3DDevice.Instance.Height - (textureSize.Height * 0.15f) - 250);
            vaso.Scaling = new Vector2(0.15f, 0.15f);

            //Crear Comida 512 x 512
            comida = new CustomSprite();
            comida.Bitmap = new CustomBitmap(env.MediaDir + "\\HUD\\food.png", D3DDevice.Instance.Device);
            textureSize = comida.Bitmap.Size;
            comida.Position = new Vector2(
                       D3DDevice.Instance.Width - (textureSize.Width * 0.15f) - 10,
                       D3DDevice.Instance.Height - (textureSize.Height * 0.15f) - 150);
            comida.Scaling = new Vector2(0.15f, 0.15f);

            // Inicializo los puntos de vida
            txtHambre = new TgcText2D();
            txtSed = new TgcText2D();
            txtCansancio = new TgcText2D();

            //Crear GameOver 549 x 245
            gameover = new CustomSprite();
            gameover.Bitmap = new CustomBitmap(env.MediaDir + "\\HUD\\game_over.png", D3DDevice.Instance.Device);
            textureSize = gameover.Bitmap.Size;
            gameover.Position = new Vector2(
                       D3DDevice.Instance.Width / 2 - (textureSize.Width / 2),
                       D3DDevice.Instance.Height / 2 - (textureSize.Height / 2));
            gameover.Scaling = new Vector2(1f, 1f);
        }


        public void InicializarTextos(TgcText2D instancia, Color color, TgcText2D.TextAlign Align, Point position, Size size, Font fuente)
        {
            instancia.Color = color;
            instancia.Align = Align;
            instancia.Position = position;
            instancia.Size = size;
            instancia.changeFont(fuente);
        }

        public void morirPersonaje()
        {
            drawer2D.BeginDrawSprite();
            drawer2D.DrawSprite(gameover);
            drawer2D.EndDrawSprite();
        }
        public void Init()
        {
            InicializarTextos(txtSed, Color.Blue, TgcText2D.TextAlign.RIGHT, new Point(D3DDevice.Instance.Width - 130, D3DDevice.Instance.Height - 300), new Size(100, 100), new Font("TimesNewRoman", 25, FontStyle.Bold));
            InicializarTextos(txtHambre, Color.Red, TgcText2D.TextAlign.RIGHT, new Point(D3DDevice.Instance.Width - 125, D3DDevice.Instance.Height - 200), new Size(100, 100), new Font("TimesNewRoman", 25, FontStyle.Bold));
            InicializarTextos(txtCansancio, Color.Yellow, TgcText2D.TextAlign.RIGHT, new Point(D3DDevice.Instance.Width - 130, D3DDevice.Instance.Height - 120), new Size(100, 100), new Font("TimesNewRoman", 25, FontStyle.Bold));
        }

        public void Update(float ElapsedTime)
        {
            txtCansancio.Text = env.personaje.Cansancio.ToString();
            txtHambre.Text = env.personaje.Hambre.ToString();
            txtSed.Text = env.personaje.Sed.ToString();
        }

        public void Render()
        {
            //Iniciar dibujado de todos los Sprites del HUD
            drawer2D.BeginDrawSprite();

            //Dibujar sprite (si hubiese mas, deberian ir todos aquí)
            drawer2D.DrawSprite(backgroundHUD);
            drawer2D.DrawSprite(personaje);
            drawer2D.DrawSprite(comida);
            drawer2D.DrawSprite(vaso);

            //Finalizar el dibujado de Sprites
            drawer2D.EndDrawSprite();

            // Renderizo los puntos de vida
            txtCansancio.render();
            txtSed.render();
            txtHambre.render();

        }

        public void Dispose()
        {
            personaje.Dispose();
            comida.Dispose();
            vaso.Dispose();
            backgroundHUD.Dispose();
            txtCansancio.Dispose();
            txtSed.Dispose();
            txtCansancio.Dispose();
        }
    }
}
