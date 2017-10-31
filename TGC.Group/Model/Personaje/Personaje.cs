using Microsoft.DirectX;
using System.Collections.Generic;
using TGC.Core.Input;
using TGC.Group.Model.Objetos;
using TGC.Core.BoundingVolumes;
using System.Drawing;
using TGC.Core.SceneLoader;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Utils;
using System.Windows.Forms;

namespace TGC.Group.Model.Character
{
    public class Personaje
    {
        private GameModel env;

        public List<ObjetoInventario> Inventario { get; set; }

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

        private TgcSceneLoader loader = new TgcSceneLoader();
        public TgcMesh hachaPersonaje;

        public Personaje(GameModel env)
        {
            this.env = env;
        }

        public void Init()
        {
            this.Posicion = new Vector3(0f, 0f, 0f);
            Inventario = new List<ObjetoInventario>();

            var fruta = new ObjetoInventario {
                objeto = new Fruta(),
                cantidad = 20
            };

            Inventario.Add(fruta);

            var agua = new ObjetoInventario {
                objeto = new Agua(),
                cantidad = 20
            };

            Inventario.Add(agua);

            var madera = new ObjetoInventario {
                objeto = new Madera(),
                cantidad = 0
            };

            Inventario.Add(madera);

            var piedra = new ObjetoInventario {
                objeto = new Piedra(),
                cantidad = 0
            };

            Inventario.Add(piedra);

            var encendedor = new ObjetoInventario {
                objeto = new Encendedor(),
                cantidad = 0
            };

            Inventario.Add(encendedor);

            // Creo la Espera que envuelve al personaje para detectar colisiones
            BoundingSphere = new TgcBoundingSphere(new Vector3(this.Posicion.X * env.terreno.SceneScaleXZ, env.terreno.CalcularAlturaTerreno(this.Posicion.X, this.Posicion.Z) * env.terreno.SceneScaleY + 10, this.Posicion.Z * env.terreno.SceneScaleXZ), 0.1f);
            BoundingSphere.setRenderColor(Color.Yellow);

            // Creo el Hacha del Personaje
            hachaPersonaje = loader.loadSceneFromFile(env.MediaDir + "hacha\\Hacha-TgcScene.xml").Meshes[0];
            hachaPersonaje.Position = new Vector3(12 * env.terreno.SceneScaleXZ, env.terreno.CalcularAlturaTerreno(12, 22) * env.terreno.SceneScaleY, 22 * env.terreno.SceneScaleXZ);
            hachaPersonaje.AlphaBlendEnable = true;
            hachaPersonaje.Scale = new Vector3(0.05f, 0.05f, 0.05f);
            hachaPersonaje.rotateY(FastMath.PI_HALF);
        }

        public void comer(int cantidad)
        {
            if (Inventario[0].cantidad > 0) {
                this.Hambre = Hambre + 15;
                Inventario[0].cantidad--;
            }
        }

        public void beber(int cantidad)
        {
            if (Inventario[1].cantidad > 0) {
                this.Sed = Sed + 15;
                Inventario[1].cantidad--;
            }
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

        public void actualizarControles(float elapsedTime)
        {
            if (env.Input.keyPressed(Key.D1)) {
                comer(1);
            }

            if (env.Input.keyPressed(Key.D2)) {
                beber(1);
            }
        }

        public void soltarObjeto()
        {
            //if(Inventario[algo].cantidad > 0)
            //env.terreno.SceneMeshes.Add( algo );
            //Inventario[algo].cantidad--;
        }

        public void guardarObjetoInventario(Objeto item)
        {
            if (item is Fruta) 
            {
                Inventario[0].cantidad++;
            }

            if (item is Agua) {
                Inventario[1].cantidad++;
            }

            if (item is Madera) {
                Inventario[2].cantidad++;
            }

            if (item is Piedra) {
                Inventario[3].cantidad++;
            }

            if (item is Encendedor) {
                Inventario[4].cantidad++;
            }
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

        public void sonidoHacha(bool activado)
        {
            if (activado)
            {
                //env.musica2.selectionSound("Sonido\\talar.mp3");
                //env.musica2.startSoundOnce();
            }else
            {
                //env.musica.selectionSound("Sonido\\ambiente1.mp3");
                //env.musica.startSound();
            }
        }
        public void desactivarLluvia()
        {
            env.tiempoAcumLluvia = 0;
            env.lloviendo = false;
            env.musica.selectionSound("Sonido\\ambiente1.mp3");
            env.musica.startSound();
        }

        public void Update(float ElapsedTime, TgcD3dInput Input)
        {
            // Actualizo la posicion del Hacha
            Posicion = env.Camara.Position;
            hachaPersonaje.Position = new Vector3(Posicion.X-1, Posicion.Y-0.8f, Posicion.Z);

            // Actualizo la Posicion de la Esfera de Colisión
            BoundingSphere.setCenter(Posicion);

            // Actualizo las Variables de Estado del Personaje
            actualizarEstado(ElapsedTime);

            actualizarControles(ElapsedTime);

            if (this.Muerto)
            {
                env.musica.playMp3(env.MediaDir + "Sonido\\game_over.mp3");
                env.musica.startSound();
            }
        }

        public void Render(float ElapsedTime)
        {
            if (this.Muerto)
            {
                env.terreno.desactivarLluvia();
                env.hud.morirPersonaje();
            }
            else
            {
                // Esfera para detectar las colisiones
               // BoundingSphere.render();
                //hachaPersonaje.render();
            }
        }

        public void Dispose()
        {
            BoundingSphere.dispose();
        }

    }
}
