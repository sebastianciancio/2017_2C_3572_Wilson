using Microsoft.DirectX;
using System;
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
        private CustomSprite inventario0;
        private CustomSprite inventario1;
        private CustomSprite inventario2;
        private CustomSprite inventario3;
        private CustomSprite inventario4;
        private TgcText2D inventario0text;
        private TgcText2D inventario1text;
        private TgcText2D inventario2text;
        private TgcText2D inventario3text;
        private TgcText2D inventario4text;
        private TgcText2D txtHambre;
        private TgcText2D txtSed;
        private TgcText2D txtCansancio;

        private CustomSprite hacha;


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

            // Hacha
            hacha = new CustomSprite();
            hacha.Bitmap = new CustomBitmap(env.MediaDir + "\\hacha\\hacha.png", D3DDevice.Instance.Device);
            textureSize = hacha.Bitmap.Size;
            hacha.Position = new Vector2(
                       D3DDevice.Instance.Width/2,
                       D3DDevice.Instance.Height - textureSize.Height + 80);
            //hacha.Scaling = new Vector2(0.3f, 0.3f);


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

            //Crear Inventario0 512 x 512
            inventario0 = new CustomSprite();
            inventario0.Bitmap = new CustomBitmap(env.MediaDir + "\\HUD\\food.png", D3DDevice.Instance.Device);
            textureSize = inventario0.Bitmap.Size;
            inventario0.Position = new Vector2(
                       D3DDevice.Instance.Width - (textureSize.Width * 0.15f) - 10,
                       D3DDevice.Instance.Height - (textureSize.Height * 0.15f) - (D3DDevice.Instance.Height * 0.9f));
            inventario0.Scaling = new Vector2(0.15f, 0.15f);

            //Crear Inventario1 512 x 512
            inventario1 = new CustomSprite();
            inventario1.Bitmap = new CustomBitmap(env.MediaDir + "\\HUD\\agua.png", D3DDevice.Instance.Device);
            textureSize = inventario1.Bitmap.Size;
            inventario1.Position = new Vector2(
                       D3DDevice.Instance.Width - (textureSize.Width * 0.15f) - 10,
                       D3DDevice.Instance.Height - (textureSize.Height * 0.15f) - (D3DDevice.Instance.Height * 0.8f));
            inventario1.Scaling = new Vector2(0.15f, 0.15f);

            //Crear Inventario2 512 x 512
            inventario2 = new CustomSprite();
            inventario2.Bitmap = new CustomBitmap(env.MediaDir + "\\HUD\\wood.png", D3DDevice.Instance.Device);
            textureSize = inventario2.Bitmap.Size;
            inventario2.Position = new Vector2(
                       D3DDevice.Instance.Width - (textureSize.Width * 0.15f) - 10,
                       D3DDevice.Instance.Height - (textureSize.Height * 0.15f) - (D3DDevice.Instance.Height * 0.7f));
            inventario2.Scaling = new Vector2(0.15f, 0.15f);

            //Crear Inventario3 512 x 512
            inventario3 = new CustomSprite();
            inventario3.Bitmap = new CustomBitmap(env.MediaDir + "\\HUD\\stone.png", D3DDevice.Instance.Device);
            textureSize = inventario3.Bitmap.Size;
            inventario3.Position = new Vector2(
                       D3DDevice.Instance.Width - (textureSize.Width * 0.15f) - 10,
                       D3DDevice.Instance.Height - (textureSize.Height * 0.15f) - (D3DDevice.Instance.Height * 0.6f));
            inventario3.Scaling = new Vector2(0.15f, 0.15f);

            //Crear Inventario4 512 x 512
            inventario4 = new CustomSprite();
            inventario4.Bitmap = new CustomBitmap(env.MediaDir + "\\HUD\\fire.png", D3DDevice.Instance.Device);
            textureSize = inventario4.Bitmap.Size;
            inventario4.Position = new Vector2(
                       D3DDevice.Instance.Width - (textureSize.Width * 0.15f) - 10,
                       D3DDevice.Instance.Height - (textureSize.Height * 0.15f) - (D3DDevice.Instance.Height * 0.5f));
            inventario4.Scaling = new Vector2(0.15f, 0.15f);

            // Inicializo los puntos de vida
            txtHambre = new TgcText2D();
            txtSed = new TgcText2D();
            txtCansancio = new TgcText2D();

            inventario0text = new TgcText2D();
            inventario1text = new TgcText2D();
            inventario2text = new TgcText2D();
            inventario3text = new TgcText2D();
            inventario4text = new TgcText2D();

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

            InicializarTextos(inventario0text, Color.Black, TgcText2D.TextAlign.RIGHT, new Point(D3DDevice.Instance.Width - 60, Convert.ToInt32(D3DDevice.Instance.Height * 0.05f)), new Size(40, 40), new Font("TimesNewRoman", 25, FontStyle.Bold));
            InicializarTextos(inventario1text, Color.Black, TgcText2D.TextAlign.RIGHT, new Point(D3DDevice.Instance.Width - 60, Convert.ToInt32(D3DDevice.Instance.Height * 0.15f)), new Size(40, 40), new Font("TimesNewRoman", 25, FontStyle.Bold));
            InicializarTextos(inventario2text, Color.Black, TgcText2D.TextAlign.RIGHT, new Point(D3DDevice.Instance.Width - 60, Convert.ToInt32(D3DDevice.Instance.Height * 0.25f)), new Size(40, 40), new Font("TimesNewRoman", 25, FontStyle.Bold));
            InicializarTextos(inventario3text, Color.Black, TgcText2D.TextAlign.RIGHT, new Point(D3DDevice.Instance.Width - 60, Convert.ToInt32(D3DDevice.Instance.Height * 0.35f)), new Size(40, 40), new Font("TimesNewRoman", 25, FontStyle.Bold));
            InicializarTextos(inventario4text, Color.Black, TgcText2D.TextAlign.RIGHT, new Point(D3DDevice.Instance.Width - 60, Convert.ToInt32(D3DDevice.Instance.Height * 0.45f)), new Size(40, 40), new Font("TimesNewRoman", 25, FontStyle.Bold));
        }

        public void Update(float ElapsedTime)
        {
            txtCansancio.Text = env.personaje.Cansancio.ToString();
            txtHambre.Text = env.personaje.Hambre.ToString();
            txtSed.Text = env.personaje.Sed.ToString();

            inventario0text.Text = env.personaje.Inventario[0].cantidad.ToString();
            inventario1text.Text = env.personaje.Inventario[1].cantidad.ToString();
            inventario2text.Text = env.personaje.Inventario[2].cantidad.ToString();
            inventario3text.Text = env.personaje.Inventario[3].cantidad.ToString();
            inventario4text.Text = env.personaje.Inventario[4].cantidad.ToString();
        }

        public void Render()
        {
            if (!env.modoDios)
            {

                //Iniciar dibujado de todos los Sprites del HUD
                drawer2D.BeginDrawSprite();


                //Dibujar sprite (si hubiese mas, deberian ir todos aquí)
                drawer2D.DrawSprite(backgroundHUD);
                drawer2D.DrawSprite(personaje);
                drawer2D.DrawSprite(hacha);
                drawer2D.DrawSprite(comida);
                drawer2D.DrawSprite(vaso);
                drawer2D.DrawSprite(inventario0);
                drawer2D.DrawSprite(inventario1);
                drawer2D.DrawSprite(inventario2);
                drawer2D.DrawSprite(inventario3);
                drawer2D.DrawSprite(inventario4);

                //Finalizar el dibujado de Sprites
                drawer2D.EndDrawSprite();

                // Renderizo los puntos de vida
                txtCansancio.render();
                txtSed.render();
                txtHambre.render();

                inventario0text.render();
                inventario1text.render();
                inventario2text.render();
                inventario3text.render();
                inventario4text.render();
            }
        }

        public void Dispose()
        {
            personaje.Dispose();
            comida.Dispose();
            vaso.Dispose();
            hacha.Dispose();
            backgroundHUD.Dispose();
            txtCansancio.Dispose();
            txtSed.Dispose();
            txtCansancio.Dispose();
        }
    }
}
