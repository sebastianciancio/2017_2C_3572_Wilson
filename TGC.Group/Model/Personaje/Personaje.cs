using Microsoft.DirectX;
using System.Collections.Generic;
using TGC.Core.Input;
using TGC.Group.Model.Objetos;
using TGC.Core.BoundingVolumes;
using System.Drawing;

namespace TGC.Group.Model.Character
{
    public class Personaje
    {
        private GameModel env;

        private List<Objeto> Inventario { get; set; }

        public float MaxHambre { get; set; } = 100;
        public float MaxSed { get; set; } = 100;
        public float MaxCansancio { get; set; } = 100;

        public float Hambre { get; set; } = 100;
        public float Sed { get; set; } = 100;
        public float Cansancio { get; set; } = 100;
        public bool Muerto = false;

        private float tTranscurridoHambre = 0;
        private float tTranscurridoSed = 0;
        private float tTranscurridoCansancio = 0;

        public Vector3 Posicion;
        public TgcBoundingSphere BoundingSphere;

        public Personaje(GameModel env)
        {
            this.env = env;
        }

        public void Init()
        {
            this.Posicion = new Vector3(0f, 0f, 0f);
            Inventario = new List<Objeto>();

            // Creo la Espera que envuelve al personaje para detectar colisiones
            BoundingSphere = new TgcBoundingSphere(new Vector3(0 * env.terreno.SceneScaleXZ, env.terreno.CalcularAlturaTerreno(0, -90) * env.terreno.SceneScaleY + 20 * env.terreno.SceneScaleXZ, -90 * env.terreno.SceneScaleXZ), 0.01f * env.terreno.SceneScaleXZ);
            BoundingSphere.AlphaBlendEnable = true;
            BoundingSphere.setRenderColor(Color.Empty);
        }

        public void comer(int valor)
        {
            this.Hambre = Hambre + valor;
        }

        public void beber(int valor)
        {
            this.Sed = Sed + valor;
        }

        public void descansar(int valor)
        {
            this.Cansancio = Cansancio + valor;
        }

        public void deshidratarse(int valor)
        {
            this.Sed = Sed - valor;
        }

        public void pasarHambre(int valor)
        {
            this.Hambre = Hambre - valor;
        }

        public void caminar(int valor)
        {
            this.Cansancio = Cansancio - valor;
        }

        public void actualizarEstado(float elapsedTime)
        {
            if (!this.Muerto)
            {
                tTranscurridoHambre += elapsedTime;
                tTranscurridoSed += elapsedTime;
                tTranscurridoCansancio += elapsedTime;

                if (tTranscurridoHambre > 6)
                {
                    this.Hambre = Hambre - 2;
                    tTranscurridoHambre = 0;
                }
                if (tTranscurridoSed > 12)
                {
                    this.Sed = this.Sed - 2;
                    tTranscurridoSed = 0;
                }
                if (tTranscurridoCansancio > 30)
                {
                    this.Cansancio = this.Cansancio - 3;
                    tTranscurridoCansancio = 0;
                }

                if (this.Sed < 1 || this.Cansancio < 1 || this.Hambre < 1) this.Muerto = true;
            }
        }

        public void soltarObjeto()
        {
            //env.terreno.SceneMeshes.Add( algo );
        }

        public void guardarObjetoInventario(Objeto item)
        {
            this.Inventario.Add(item);
        }


        public void tomarObjeto()
        {
            /*
            world.objects.ForEach(crafteable => {
                if (crafteable.isNear(this))
                {
                    this.guardarObjetoInventario(crafteable);
                }
            });
            */
        }

        public void Update(float ElapsedTime, TgcD3dInput Input)
        {
            Posicion = env.Camara.Position;

            // Actualizo la Posicion de la Esfera de Colisión
            BoundingSphere.setCenter(Posicion);

            // Actualizo las Variables de Estado del Personaje
            actualizarEstado(ElapsedTime);
        }

        public void Render(float ElapsedTime)
        {
            if (this.Muerto)
            {
                env.musica.playMp3(env.MediaDir + "Sonido\\game_over.mp3");
                env.hud.morirPersonaje();

                this.Dispose();

            }else
            {
                // Esfera para detectar las colisiones
                BoundingSphere.render();
            }
        }

        public void Dispose()
        {
            BoundingSphere.dispose();
        }

    }
}
